using PROJ.Enemies.Species.Interfaces;

namespace PROJ.Enemies.Death_Reaction_Strategies;

public class AngryStrategy:  IDeathReactionStrategy
{
    public void ReactToDeath(Enemy e)
    {
        e.ModifyDamage(5);
        GameOutput.SpecificBlink(e.ObjBoard?.CreateTileBlinkResult(e.X, e.Y, ConsoleColor.Red));
        // e.Blink(ConsoleColor.Red);
    }
}
