using PROJ.Enemies;
using PROJ.Communication.Results;
using PROJ.Fight.Classes.Visitors;
using PROJ.Fight.Handlers;
using PROJ.Fight.Visitors;
using PROJ.GameConstansts;
using PROJ.Handlers;
using PROJ.Handlers.Enums;
using PROJ.Logging.Classes;
using PROJ.Tools.Classes;
using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Fight;

public class FightMenu
{
    private Player _player;
    private Enemy _enemy;
    private int? _attack;
    private char? _hand;
    public FightMenu(Player p, Enemy e)
    {
        _player = p;
        _enemy = e;
    }

    public int? CurrentAttack => _attack;
    public char? CurrentHand => _hand;

    public FightLoopEndResult StartFight()
    {
        var efh = new ExitFightHandler(this);
        var sah = new SelectAttackHandler(this);
        var shh = new SelectHandHandler(this);
        var ah = new AttackHandler(this);

        efh.SetNext(sah);
        sah.SetNext(shh);
        shh.SetNext(ah);
        while (true)
        {
            if (_enemy.Dead())
                return new FightLoopEndResult(true, false, false);
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            var res = efh.Handle(key);
            if (res == HandleResult.ExitGame)
            {
                if (_player.Dead())
                    return new FightLoopEndResult(false, false, true);
                return new FightLoopEndResult(false, true, false);
            }
        }
    }

    public FightAttackSelectionResult SetAttack(int which)
    {
        _attack = which;
        return new FightAttackSelectionResult(which);
    }

    public FightHandSelectionResult SetHand(char c)
    {
        _hand = c;
        return new FightHandSelectionResult(c, CreatePlayerFightResult());
    }

    public FightExitResult ExitFight()
    {
        return new FightExitResult();
    }

    public FightTurnResult SimulateAttack()
    {
        if(_hand == null)
        {
            _hand = 'L';
        }

        if (_attack == null)
        {
            _attack = 1;
        }

        AttackVisitor? visitor = null;
        if (_attack == 1) // TODO na enuma
        {
            visitor = new NormalAttackVisitor(_player);
        }
        else if (_attack == 2)
        {
            visitor = new SneakyAttackVisitor(_player);
        }
        else if (_attack == 3)
        {
            visitor = new MagicAttackVisitor(_player);
        }

        if (visitor == null)
        {
            return new FightTurnResult(false, (int)_attack, (char)_hand, CreateEnemyViewResult(), CreatePlayerFightResult(), errorMessage: "UNKNOWN ERROR!");
        }

        Tool? w = null;

        if (_hand == 'L')
        {
            w = _player.LeftHand;
        }
        else if (_hand == 'R')
        {
            w = _player.RightHand;
        }

        if (w == null)
        {
            return new FightTurnResult(false, (int)_attack, (char)_hand, CreateEnemyViewResult(), CreatePlayerFightResult(), errorMessage: "Cannot Attack with bare hands!");
        }

        var result = w.Accept(visitor);
        var logger = Logger.GetInstance;
        _enemy.Hit(result.DamageToEnemy);
        
        logger.Log($"{_player.Name} Hit {_enemy.Name} with {result.DamageToEnemy} damage");
        Thread.Sleep(50);
        if (_enemy.Dead())
        {
            logger.Log($"{_player.Name} Killed {_enemy.Name}");
            _enemy.DeleteYourself();
            return new FightTurnResult(true, (int)_attack, (char)_hand, CreateEnemyViewResult(), CreatePlayerFightResult(), updateEnemyVitals: true, enemyDead: true);
        }
        _enemy.Attack(_player, result.PlayerDefense);
        logger.Log($"{_enemy.Name} Attacked {_player.Name} with {int.Max(_enemy.Damage - result.PlayerDefense,0)} damage");

        Thread.Sleep(50);
        if (_player.Dead())
        {
            logger.Log($"{_enemy.Name} Killed {_player.Name}");
            return new FightTurnResult(true, (int)_attack, (char)_hand, CreateEnemyViewResult(), CreatePlayerFightResult(), updateEnemyVitals: true, playerDead: true, exitFightMode: true);
        }

        return new FightTurnResult(true, (int)_attack, (char)_hand, CreateEnemyViewResult(), CreatePlayerFightResult(), updateEnemyVitals: true, refreshPlayerVitals: true);
    }

    private EnemyViewResult CreateEnemyViewResult()
    {
        return new EnemyViewResult(_enemy.Name, _enemy.Description, _enemy.Fightable, _enemy.Visual, _enemy.Health, _enemy.Armor);
    }

    private PlayerFightViewResult CreatePlayerFightResult()
    {
        return new PlayerFightViewResult(
            _player.LeftHand != null ? _player.LeftHand.Visual : GameConstants.EmptyHandSymbol,
            _player.RightHand != null ? _player.RightHand.Visual : GameConstants.EmptyHandSymbol);
    }
}
