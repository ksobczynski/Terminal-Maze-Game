using PROJ.Builder.Classes;
using PROJ.Enemies;
using PROJ.Tools.Classes;
using PROJ.Tools.Classes.Items;
using PROJ.Tools.Classes.Weapons;

namespace PROJ.Themes;

public class SeaThemeFactory : IDungeonThemeFactory
{
    public Func<Enemy>[] CreateEnemyPool(Board board)
    {
        return new Func<Enemy>[]
        {
            () => new MediumEnemy(name: "Shark", vis: 'ᗦ',b:board),
            () => new BigEnemy(name: "Whale", vis: 'ᘯ',b:board),
            () => new SmallEnemy(name: "Sailor", vis:'&',b:board),
        };
    }

    public Func<Item>[] CreateItemPool(Player p)
    {
        return new Func<Item>[]
        {
            () => new MediumItem(p, name:"Captain's Log", vis:'⎈'),
            () => new SmallItem(p, name:"Compass", vis: '⌖'),
            () => new CoolItem(p, name: "ShellAmulet",vis:'◔' )
        };
    }

    public Func<Weapon>[] CreateWeaponPool(Player p)
    {
        return new Func<Weapon>[]
        {
            () => new MediumWeapon(p, name: "Fishing Rod", vis:'Ͽ'),
            () => new SmallMagicWeapon(p, name: "Mast", vis:'╫'),
            () => new TwoHandedWeapon(p, name:"Moby Dick's Fang", vis:'☽')
        };
    }

    public void Build(DungeonBuilder db, Player p, Board board)
    {
        db.CreateEmptyDungeon();
        db.AddItems(20, CreateItemPool(p)); // TODO kolorki zmien jak motyw
        db.AddWeapons(20, CreateWeaponPool(p));
        db.AddArtifact(new SmallMagicWeapon(p, "Magic Compass", '*'));
        db.AddEnemies(5, CreateEnemyPool(board));
        // d;
        // db.AddCorridors(20);
    }

    public string Name => "Sea Theme";
    public string IntroMessage => "Welcome to the depths of the sea!";
}