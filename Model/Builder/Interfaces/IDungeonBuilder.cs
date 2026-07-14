using PROJ.Enemies;
using PROJ.Tools.Classes;

namespace PROJ.Builder;
public interface IDungeonBuilder
{
    void CreateEmptyDungeon();
    void CreateFilledDungeon();
    void AddCorridors(int count);
    void AddRooms(int count);
    void AddCentralHall(int width, int height);
    void AddItems(int count, Func<Item>[]? items = null);
    void AddWeapons(int count, Func<Weapon>[]? weapons = null);
    void AddEnemies(int count, Func<Enemy>[]? enemies = null);
    Tile[,] GetDungeon();

}