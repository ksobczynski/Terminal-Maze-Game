using PROJ.Client;
using PROJ.Server;

namespace PROJ;

internal enum LaunchMode
{
    Local,
    Server,
    Client
}

internal sealed class LaunchOptions
{
    public LaunchMode Mode { get; init; } = LaunchMode.Local;
    public string Host { get; init; } = "127.0.0.1";
    public int Port { get; init; } = 5555;
}

static class Program
{
    static void Main(string[] args)
    {
        try
        {
            LaunchOptions options = args.Length == 0 ? PromptLaunchOptions() : ParseArgs(args);

            switch (options.Mode)
            {
                case LaunchMode.Server:
                    RunServer(options);
                    return;
                case LaunchMode.Client:
                    RunClient(options);
                    return;
                default:
                    RunLocal();
                    return;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            PrintUsage();
        }
    }

    private static LaunchOptions ParseArgs(string[] args)
    {
        if (args[0] == "--server")
        {
            int port = 5555;
            if (args.Length >= 2 && !int.TryParse(args[1], out port))
                throw new ArgumentException("Invalid server port.");

            return new LaunchOptions
            {
                Mode = LaunchMode.Server,
                Port = port
            };
        }

        if (args[0] == "--client")
        {
            string host = "127.0.0.1";
            int port = 5555;

            if (args.Length >= 2)
            {
                string[] parts = args[1].Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length == 0 || parts.Length > 2)
                    throw new ArgumentException("Invalid client endpoint. Expected ip:port.");

                host = parts[0];
                if (parts.Length == 2 && !int.TryParse(parts[1], out port))
                    throw new ArgumentException("Invalid client port.");
            }

            return new LaunchOptions
            {
                Mode = LaunchMode.Client,
                Host = host,
                Port = port
            };
        }

        if (args[0] == "--local")
            return new LaunchOptions();

        throw new ArgumentException("Unknown launch mode.");
    }

    private static LaunchOptions PromptLaunchOptions()
    {
        while (true)
        {
            Console.Write("Uruchomic jako (S)erwer czy (K)lient? [S/K/Lokalnie]: ");
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            Console.WriteLine();

            if (key == ConsoleKey.S)
            {
                Console.Write("Port serwera [5555]: ");
                string? value = Console.ReadLine();
                int port = 5555;
                if (!string.IsNullOrWhiteSpace(value) && !int.TryParse(value, out port))
                    throw new ArgumentException("Invalid server port.");

                return new LaunchOptions
                {
                    Mode = LaunchMode.Server,
                    Port = port
                };
            }

            if (key == ConsoleKey.K)
            {
                Console.Write("Adres klienta ip:port [127.0.0.1:5555]: ");
                string? value = Console.ReadLine();
                var (host, port) = ParseClientEndpoint(value);
                return new LaunchOptions
                {
                    Mode = LaunchMode.Client,
                    Host = host,
                    Port = port
                };
            }

            if (key == ConsoleKey.L)
                return new LaunchOptions();
        }
    }

    private static (string Host, int Port) ParseClientEndpoint(string? endpoint)
    {
        string host = "127.0.0.1";
        int port = 5555;

        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            string[] parts = endpoint.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0 || parts.Length > 2)
                throw new ArgumentException("Invalid client endpoint. Expected ip:port.");

            host = parts[0];
            if (parts.Length == 2 && !int.TryParse(parts[1], out port))
                throw new ArgumentException("Invalid client port.");
        }

        return (host, port);
    }

    private static void RunLocal()
    {
        Game game = new Game();
        game.Start();
    }

    private static void RunServer(LaunchOptions options)
    {
        GameServer server = new GameServer(options.Port);
        server.Run();
    }

    private static void RunClient(LaunchOptions options)
    {
        GameClient client = new GameClient(options.Host, options.Port);
        client.Run();
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  --server [port]");
        Console.WriteLine("  --client [ip:port]");
        Console.WriteLine("  --local");
    }
}
