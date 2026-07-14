using PROJ.Builder.Classes;
using PROJ.Enemies;
using PROJ.Tools.Classes;

namespace PROJ.Themes;

public interface IDungeonThemeFactory
{
    string Name { get; }
    string IntroMessage { get; }

    Func<Item>[] CreateItemPool(Player p);
    Func<Weapon>[] CreateWeaponPool(Player p);
    Func<Enemy>[] CreateEnemyPool(Board b);
    void Build(DungeonBuilder db, Player p, Board board);
}