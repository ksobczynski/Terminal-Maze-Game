using PROJ.Boxes;
using PROJ.Communication.Results;
using PROJ.GameConstansts;

namespace PROJ;

public class FightBox : Box
{
    
    public override void DisplayFrame()
    {
        // ┌ ┐ └ ┘ ─ │
        Console.SetCursorPosition(GameConstants.FightBoxLeft, GameConstants.FightBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.FightBoxRight, GameConstants.FightBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.FightBoxLeft, GameConstants.FightBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.FightBoxRight, GameConstants.FightBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.FightBoxBottom - GameConstants.FightBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.FightBoxLeft, GameConstants.FightBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.FightBoxRight, GameConstants.FightBoxTop + i);
            Console.Write('│');
        }
        for (int i = 1; i < GameConstants.FightBoxRight - GameConstants.FightBoxLeft ; i++)
        {
            Console.SetCursorPosition(GameConstants.FightBoxLeft + i, GameConstants.FightBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.FightBoxLeft + i, GameConstants.FightBoxBottom);
            Console.Write('─');
        }

        string tmp = " FightBox ";
        Console.SetCursorPosition((GameConstants.FightBoxRight + GameConstants.FightBoxLeft) / 2 - tmp.Length/2, GameConstants.FightBoxTop);
        Console.Write(tmp);
    }

    public void AfterMoveAssesment(EnemyViewResult? enemy)
    {
        if (enemy == null)
        {
            ClearInside();
            return;
        }
        
        
        DisplayConcreteEnemy(enemy);
    }
    private void ClearInside()
    {
        int innerWidth = GameConstants.FightBoxRight - GameConstants.FightBoxLeft - 1;

        for (int y = GameConstants.FightBoxTop + 1; y < GameConstants.FightBoxBottom; y++)
        {
            Console.SetCursorPosition(GameConstants.FightBoxLeft + 1, y);
            Console.Write(new string(' ', innerWidth));
        }
    }
    
    public void DisplayConcreteEnemy(EnemyViewResult e)
    {
        ClearInside(); // TODO needed???
        string nameStr = "You're Near: ";
        nameStr += e.Name;
        
        Console.SetCursorPosition((GameConstants.FightBoxLeft + GameConstants.FightBoxRight - nameStr.Length)/2, GameConstants.FightBoxWritingPointName);
        Console.Write(nameStr);
        
        Console.SetCursorPosition((GameConstants.FightBoxLeft + GameConstants.FightBoxRight - e.Description.Length)/2, GameConstants.FightBoxWritingPointDesc);
        Console.Write(e.Description);
        if (e.Fightable)
        {
            string pickupStr = "Press 'Enter' to fight";
            Console.SetCursorPosition((GameConstants.FightBoxLeft + GameConstants.FightBoxRight - pickupStr.Length)/2, GameConstants.FightBoxWritingPointPickup);
            Console.Write(pickupStr);
        }
    }

    public void FightMode(PlayerFightViewResult p, EnemyViewResult e)
    {
        ClearInside();
        
        Console.SetCursorPosition(GameConstants.FightBoxPlayerLeftHand, GameConstants.FightBoxPlayerIconTop);
        Console.Write(p.LeftHandVisual);
        
        Console.SetCursorPosition(GameConstants.FightBoxPlayerIconLeft, GameConstants.FightBoxPlayerIconTop);
        Console.Write(GameConstants.PlayerSymbol);
        
        Console.SetCursorPosition(GameConstants.FightBoxPlayerRightHand, GameConstants.FightBoxPlayerIconTop);
        Console.Write(p.RightHandVisual);

        TypeNormal(null);
        TypeSneaky(null);
        TypeMagic(null);
        
        Console.SetCursorPosition(GameConstants.FightBoxEnemyPositionLeft,GameConstants.FightBoxEnemyPositionTop);
        Console.Write(e.Visual);
        
        Console.SetCursorPosition(GameConstants.FightBoxEnemyVitalsLeft, GameConstants.FightBoxEnemyVitalsTop);
        Console.Write("Health: " + e.Health);
        
        Console.SetCursorPosition(GameConstants.FightBoxEnemyVitalsLeft, GameConstants.FightBoxEnemyVitalsTop + 1);
        Console.Write("Armor: " + e.Armor);
        
    }

    public void HighlightAttack(int which)
    {
        ClearPreviousAttack();
        if (which == 1)
            TypeNormal(ConsoleColor.Green);
        else if (which == 2)
            TypeSneaky(ConsoleColor.Green);
        else if (which == 3)
            TypeMagic(ConsoleColor.Green);
    }

    private void TypeNormal(ConsoleColor? color)
    {
        Console.SetCursorPosition(GameConstants.FightBoxAttacksLeft, GameConstants.FightBoxAttacksTop + 2);
        if(color!=null) 
            Console.ForegroundColor = (ConsoleColor)color;
        Console.Write("1. Normal Attack");
        Console.ResetColor();
    }
    
    private void TypeSneaky(ConsoleColor? color)
    {
        Console.SetCursorPosition(GameConstants.FightBoxAttacksLeft, GameConstants.FightBoxAttacksTop + 3);
        if(color!=null) 
            Console.ForegroundColor = (ConsoleColor)color;
        Console.Write("2. Sneaky Attack");
        Console.ResetColor();
    }
    
    private void TypeMagic(ConsoleColor? color)
    {
        Console.SetCursorPosition(GameConstants.FightBoxAttacksLeft, GameConstants.FightBoxAttacksTop + 4);
        if(color!=null) 
            Console.ForegroundColor = (ConsoleColor)color;
        Console.Write("3. Magic Attack");
        Console.ResetColor();
    }

    private void ClearPreviousAttack()
    {
        TypeNormal(null);
        TypeSneaky(null);
        TypeMagic(null);
    }


    private void TypeLeftHand(ConsoleColor? color, PlayerFightViewResult p)
    {
        Console.SetCursorPosition(GameConstants.FightBoxPlayerLeftHand, GameConstants.FightBoxPlayerIconTop);
        if(color!=null) 
            Console.ForegroundColor = (ConsoleColor)color;
        Console.Write(p.LeftHandVisual);
        Console.ResetColor();
    }
    private void TypeRightHand(ConsoleColor? color, PlayerFightViewResult p)
    {
        Console.SetCursorPosition(GameConstants.FightBoxPlayerRightHand, GameConstants.FightBoxPlayerIconTop);
        if(color!=null) 
            Console.ForegroundColor = (ConsoleColor)color;
        Console.Write(p.RightHandVisual);
        Console.ResetColor();
    }

    private void ClearPreviousHand(PlayerFightViewResult p)
    {
        TypeLeftHand(null,p);
        TypeRightHand(null,p);
    }
    public void HighlightHand(char hand, PlayerFightViewResult p)
    {
        ClearPreviousHand(p);
        if(hand == 'L')
            TypeLeftHand(ConsoleColor.Green,p);
        else if(hand == 'R')
            TypeRightHand(ConsoleColor.Green,p);
    }

    public void UpdateEnemyVitals(EnemyViewResult e)
    {
        Console.SetCursorPosition(GameConstants.FightBoxEnemyVitalsLeft, GameConstants.FightBoxEnemyVitalsTop);
        Console.Write("           ");
        Console.SetCursorPosition(GameConstants.FightBoxEnemyVitalsLeft, GameConstants.FightBoxEnemyVitalsTop);
        Console.Write("Health: " + e.Health);
        
        Console.SetCursorPosition(GameConstants.FightBoxEnemyVitalsLeft, GameConstants.FightBoxEnemyVitalsTop + 1);
        Console.Write("           ");
        Console.SetCursorPosition(GameConstants.FightBoxEnemyVitalsLeft, GameConstants.FightBoxEnemyVitalsTop + 1);
        
        Console.Write("Armor: " + e.Armor);
    }

    public void DeadEnemyDisplay(EnemyViewResult e)
    {
        ClearInside();
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition((GameConstants.FightBoxLeft + GameConstants.FightBoxRight)/2, (GameConstants.FightBoxTop + GameConstants.FightBoxBottom)/2);
        Console.Write($"You Killed {e.Name}");
        Console.ForegroundColor = color;
        Task.Run(async () =>
        {
            await Task.Delay(3000);
            ClearInside();
        });
    }
    
    public void Render(EnemyViewResult? enemy)
    {
        AfterMoveAssesment(enemy);
    }

    public void Clear()
    {
        ClearInside();
    }
    
}
