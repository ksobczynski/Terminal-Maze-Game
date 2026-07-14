using PROJ.Enemies.Species.Interfaces;

namespace PROJ.Enemies.Death_Reaction_Strategies;

public class ScaredStrategy : IDeathReactionStrategy
{
    public void ReactToDeath(Enemy e)
    {
        e.ModifyDamage(-10);
        e.ModifyArmor(-10);
        GameOutput.SpecificBlink(e.ObjBoard?.CreateTileBlinkResult(e.X, e.Y, ConsoleColor.Green));
        // e.Blink(ConsoleColor.Green);
        // e.ModifyHealth(-5);
    }
}
