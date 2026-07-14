using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using PROJ.Builder.Classes;
using PROJ.Communication.Network;
using PROJ.Communication.Snapshots;

namespace PROJ.Server;

public sealed class GameServer
{
    private const int MaxClients = 9;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly int port;
    private readonly object sync = new();
    private readonly SemaphoreSlim commandQueue = new(1, 1);
    private readonly Board board;
    private readonly List<ClientSession> sessions = new();

    public GameServer(int Port)
    {
        port = Port;
        (board, Player seedPlayer) = CreateGameWorld();
        board.RemovePlayer(seedPlayer);
    }

    public void Run()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server listening on port {port}");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            _ = HandleClientSafelyAsync(client);
        }
    }

    private async Task HandleClientSafelyAsync(TcpClient client)
    {
        ClientSession? session = null;

        try
        {
            using TcpClient currentClient = client;
            using NetworkStream stream = currentClient.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            session = await RegisterClientAsync(writer);
            if (session == null)
                return;

            while (true)
            {
                string? line = await reader.ReadLineAsync();
                if (line == null)
                    return;

                ConsoleKey? key = ParseCommandKey(line);
                if (key == null)
                    continue;

                await HandleCommandAsync(session, key.Value);

                if (session.Processor.GameEnded)
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client session ended with error: {ex.Message}");
        }
        finally
        {
            if (session != null)
                await UnregisterClientAsync(session);
        }
    }

    private async Task<ClientSession?> RegisterClientAsync(StreamWriter writer)
    {
        await commandQueue.WaitAsync();
        try
        {
            ClientSession? session;
            List<OutgoingSnapshot> snapshots;

            lock (sync)
            {
                if (sessions.Count >= MaxClients)
                {
                    session = null;
                    snapshots = new List<OutgoingSnapshot>
                    {
                        new(writer, CreateRejectedSnapshotJson("Server full"))
                    };
                }
                else
                {
                    int playerId = GetFreePlayerIdLocked();
                    Player player = new Player();
                    board.AddPlayer(player, playerId: playerId);
                    NetworkGameCommandProcessor processor = new NetworkGameCommandProcessor(board, player);
                    session = new ClientSession(playerId, player, processor, writer);
                    sessions.Add(session);
                    snapshots = CreateBroadcastSnapshotsLocked();
                    Console.WriteLine($"Player {playerId} connected");
                }
            }

            await SendSnapshotsAsync(snapshots);
            return session;
        }
        finally
        {
            commandQueue.Release();
        }
    }

    private async Task UnregisterClientAsync(ClientSession session)
    {
        await commandQueue.WaitAsync();
        try
        {
            List<OutgoingSnapshot> snapshots;
            lock (sync)
            {
                if (!sessions.Remove(session))
                    return;

                board.RemovePlayer(session.Player);
                snapshots = CreateBroadcastSnapshotsLocked();
                Console.WriteLine($"Player {session.PlayerId} disconnected");
            }

            await SendSnapshotsAsync(snapshots);
        }
        finally
        {
            commandQueue.Release();
        }
    }

    private async Task HandleCommandAsync(ClientSession session, ConsoleKey key)
    {
        await commandQueue.WaitAsync();
        try
        {
            List<OutgoingSnapshot> snapshots;
            lock (sync)
            {
                if (!sessions.Contains(session))
                    return;

                board.SetActivePlayer(session.Player);
                session.Processor.Handle(key);
                snapshots = CreateBroadcastSnapshotsLocked(session, session.Processor.LastEffects);
            }

            await SendSnapshotsAsync(snapshots);
        }
        finally
        {
            commandQueue.Release();
        }
    }

    private List<OutgoingSnapshot> CreateBroadcastSnapshotsLocked(
        ClientSession? actingSession = null,
        IReadOnlyList<GameOutput.RecordedTileEffect>? effects = null)
    {
        List<OutgoingSnapshot> snapshots = new();

        foreach (ClientSession session in sessions.ToList())
        {
            bool isActingSession = ReferenceEquals(session, actingSession);
            board.SetActivePlayer(session.Player);

            GameSnapshotMessage snapshot = GameSnapshotFactory.Create(
                board,
                session.Player,
                session.Processor.GameEnded,
                session.Processor.EndedGood,
                isActingSession ? session.Processor.LastActionError : null,
                isActingSession ? session.Processor.LastFightError : null,
                effects);

            snapshots.Add(new OutgoingSnapshot(session, JsonSerializer.Serialize(snapshot, JsonOptions)));
        }

        return snapshots;
    }

    private static async Task SendSnapshotsAsync(IReadOnlyList<OutgoingSnapshot> snapshots)
    {
        foreach (OutgoingSnapshot snapshot in snapshots)
        {
            await snapshot.SendAsync();
        }
    }

    private int GetFreePlayerIdLocked()
    {
        for (int id = 1; id <= MaxClients; id++)
        {
            if (sessions.All(session => session.PlayerId != id))
                return id;
        }

        throw new InvalidOperationException("No free player id.");
    }

    private static string CreateRejectedSnapshotJson(string message)
    {
        GameSnapshotMessage snapshot = new GameSnapshotMessage
        {
            Errors = new ErrorSnapshot
            {
                ActionError = message
            },
            GameEnded = true
        };

        return JsonSerializer.Serialize(snapshot, JsonOptions);
    }

    private static (Board Board, Player Player) CreateGameWorld()
    {
        Board board = new Board();
        Player player = new Player();
        player.AssignBoard(board);

        GameWorldBootstrapper bootstrapper = new GameWorldBootstrapper();
        bootstrapper.Populate(board, player, new NullPlayerMovesBuilder(), null);

        return (board, player);
    }

    private static ConsoleKey? ParseCommandKey(string line)
    {
        try
        {
            ClientCommandMessage? command = JsonSerializer.Deserialize<ClientCommandMessage>(line, JsonOptions);
            if (command != null && ConsoleKey.TryParse(command.Key, true, out ConsoleKey commandKey))
                return commandKey;
        }
        catch (JsonException)
        {
            // Raw command fallback keeps the server tolerant during manual testing.
        }

        if (ConsoleKey.TryParse(line, true, out ConsoleKey key))
            return key;

        if (line.Length == 1 && ConsoleKey.TryParse(line.ToUpperInvariant(), true, out ConsoleKey charKey))
            return charKey;

        return null;
    }

    private sealed class ClientSession
    {
        private readonly StreamWriter writer;
        private readonly SemaphoreSlim writeLock = new(1, 1);

        public ClientSession(int playerId, Player player, NetworkGameCommandProcessor processor, StreamWriter writer)
        {
            PlayerId = playerId;
            Player = player;
            Processor = processor;
            this.writer = writer;
        }

        public int PlayerId { get; }
        public Player Player { get; }
        public NetworkGameCommandProcessor Processor { get; }

        public async Task WriteAsync(string json)
        {
            await writeLock.WaitAsync();
            try
            {
                await writer.WriteLineAsync(json);
            }
            finally
            {
                writeLock.Release();
            }
        }
    }

    private sealed class OutgoingSnapshot
    {
        private readonly ClientSession? session;
        private readonly StreamWriter? writer;
        private readonly string json;

        public OutgoingSnapshot(ClientSession session, string json)
        {
            this.session = session;
            this.json = json;
        }

        public OutgoingSnapshot(StreamWriter writer, string json)
        {
            this.writer = writer;
            this.json = json;
        }

        public Task SendAsync()
        {
            if (session != null)
                return session.WriteAsync(json);

            return writer!.WriteLineAsync(json);
        }
    }
}
