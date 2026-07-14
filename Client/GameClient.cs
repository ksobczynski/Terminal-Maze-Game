using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using PROJ.Communication.Network;
using PROJ.Communication.Snapshots;

namespace PROJ.Client;

public sealed class GameClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly int port;
    private readonly string host;

    public GameClient(string Host, int Port)
    {
        port = Port;
        host = Host;
    }

    public void Run()
    {
        using TcpClient client = new TcpClient(host, port);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)
        {
            AutoFlush = true
        };

        using CancellationTokenSource cancellation = new CancellationTokenSource();
        Task readTask = ReadSnapshotsAsync(reader, cancellation);

        while (!cancellation.IsCancellationRequested)
        {
            if (!Console.KeyAvailable)
            {
                Thread.Sleep(10);
                continue;
            }

            ConsoleKey? key = TryReadCommandKey();
            if (key == null)
                continue;

            ClientCommandMessage command = new ClientCommandMessage
            {
                Key = key.Value.ToString()
            };

            writer.WriteLine(JsonSerializer.Serialize(command, JsonOptions));
        }

        try
        {
            readTask.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static async Task ReadSnapshotsAsync(StreamReader reader, CancellationTokenSource cancellation) // TODO czemu cancellation token
    {
        while (!cancellation.IsCancellationRequested)
        {
            string? snapshotJson = await reader.ReadLineAsync();
            if (snapshotJson == null)
            {
                cancellation.Cancel();
                return;
            }

            GameSnapshotMessage? snapshot = RenderSnapshot(snapshotJson);
            if (snapshot?.GameEnded == true)
            {
                cancellation.Cancel();
                return;
            }
        }
    }

    private static ConsoleKey? TryReadCommandKey()
    {
        ConsoleKeyInfo key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Escape /* && IsTerminalEscapeSequence()*/)
            return null;

        return IsGameCommand(key.Key) ? key.Key : null;
    }

    private static bool IsTerminalEscapeSequence() // TODO Co tu sie dzieje
    {
        Thread.Sleep(25);

        if (!Console.KeyAvailable)
            return false;

        while (Console.KeyAvailable)
            Console.ReadKey(true);

        return true;
    }

    private static bool IsGameCommand(ConsoleKey key)
    {
        return key is ConsoleKey.W
            or ConsoleKey.A
            or ConsoleKey.S
            or ConsoleKey.D
            or ConsoleKey.E
            or ConsoleKey.B
            or ConsoleKey.Q
            or ConsoleKey.R
            or ConsoleKey.L
            or ConsoleKey.J
            or ConsoleKey.LeftArrow
            or ConsoleKey.RightArrow
            or ConsoleKey.UpArrow
            or ConsoleKey.DownArrow
            or ConsoleKey.Enter
            or ConsoleKey.D1
            or ConsoleKey.D2
            or ConsoleKey.D3
            or ConsoleKey.Escape;
    }

    private static GameSnapshotMessage? RenderSnapshot(string json)
    {
        GameSnapshotMessage? snapshot = JsonSerializer.Deserialize<GameSnapshotMessage>(json, JsonOptions);
        if (snapshot == null)
            return null;

        GameSnapshotRenderer.Render(snapshot);
        return snapshot;
    }
}
