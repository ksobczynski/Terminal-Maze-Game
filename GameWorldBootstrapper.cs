using PROJ.Builder;
using PROJ.Builder.Classes;
using PROJ.Enemies;
using PROJ.Themes;

namespace PROJ;

public sealed class GameWorldBootstrapper
{
    public void Populate(Board board, Player player, IPlayerMovesBuilder playerMovesBuilder, IDungeonThemeFactory? factory,
        string? playerName = null)
    {
        DungeonBuilder builder = new DungeonBuilder(board, player, playerMovesBuilder);

        if (factory == null)
        {
            MixGenerate(builder);
            builder.AddCentralHall(10, 10);
        }
        else
        {
            factory.Build(builder, player, board);
        }

        PickUpSoundBus.GetInstance.Init(board);

        Tile[,] tiles = builder.GetDungeon();
        board.InsertEnemies(GetEnemies(tiles));
        board.AddPlayer(player, playerName);
        board.Init(tiles);
    }

    private static void MixGenerate(DungeonBuilder builder)
    {
        builder.CreateEmptyDungeon();
        builder.AddCentralHall(10, 7);
        builder.AddRooms(10);
        builder.AddCorridors(10);
        builder.AddWeapons(20);
        builder.AddItems(10);
        builder.AddEnemies(5);
    }

    private static List<Enemy> GetEnemies(Tile[,] tiles)
    {
        List<Enemy> enemies = new();
        foreach (Tile tile in tiles)
        {
            Enemy? enemy = tile.TryGetEnemy();
            if (enemy != null)
                enemies.Add(enemy);
        }

        return enemies;
    }
}
