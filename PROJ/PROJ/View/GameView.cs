using PROJ.Communication.Results;

namespace PROJ;

public class GameView
{
    private static readonly GameView _instance = new();

    private ActionBox? _actionBox;
    private VitalsBox? _vitalsBox;
    private WealthBox? _wealthBox;
    private EquipmentBox? _equipmentBox;
    private LeftHandBox? _leftHandBox;
    private RightHandBox? _rightHandBox;
    private AboveActionErrorSpace? _aboveActionErrorSpace;
    private AboveFightErrorSpace? _aboveFightErrorSpace;
    private PlayerMovesBox? _playerMovesBox;
    private FightBox? _fightBox;

    private GameView()
    {
    }

    public static GameView GetInstance()
    {
        return _instance;
    }

    public ActionBox? ActionBox
    {
        get => _actionBox;
        set => _actionBox = value;
    }

    public VitalsBox? VitalsBox
    {
        get => _vitalsBox;
        set => _vitalsBox = value;
    }

    public WealthBox? WealthBox
    {
        get => _wealthBox;
        set => _wealthBox = value;
    }

    public EquipmentBox? EquipmentBox
    {
        get => _equipmentBox;
        set => _equipmentBox = value;
    }

    public LeftHandBox? LeftHandBox
    {
        get => _leftHandBox;
        set => _leftHandBox = value;
    }

    public RightHandBox? RightHandBox
    {
        get => _rightHandBox;
        set => _rightHandBox = value;
    }

    public AboveActionErrorSpace? AboveActionErrorSpace
    {
        get => _aboveActionErrorSpace;
        set => _aboveActionErrorSpace = value;
    }

    public AboveFightErrorSpace? AboveFightErrorSpace
    {
        get => _aboveFightErrorSpace;
        set => _aboveFightErrorSpace = value;
    }

    public PlayerMovesBox? PlayerMovesBox
    {
        get => _playerMovesBox;
        set => _playerMovesBox = value;
    }

    public FightBox? FightBox
    {
        get => _fightBox;
        set => _fightBox = value;
    }

    public void Apply(BackpackModeSwitchResult result)
    {
        _equipmentBox?.UpdatePointer(result);
    }

    public void Apply(BackpackPointerMoveResult result)
    {
        if (!result.IsSuccess)
            return;
        _equipmentBox?.RenderPointer(result);
    }

    public void Apply(BackpackHandChangeResult result)
    {
        if (!result.IsSuccess)
            return;

        if (result.RefreshEquipment && _equipmentBox != null)
        {
            if (result.LeavePointer)
            {
                _equipmentBox.DisplayItemsLeavePointer();
            }
            else
            {
                _equipmentBox.ClearPointer(result.PointerIdx);
                _equipmentBox.DisplayItems();
                if (result.ResetPointerToTop)
                    _equipmentBox.PointerInit();
            }
        }

        if (result.RefreshLeftHand)
            _leftHandBox?.DisplayHand();

        if (result.RefreshRightHand)
            _rightHandBox?.DisplayHand();
    }

    public void Apply(BackpackDropResult result)
    {
        if (!result.IsSuccess || _equipmentBox == null)
            return;

        _equipmentBox.ClearPointer(result.BackpackIdx);
        _equipmentBox.DisplayItems();
        _equipmentBox.PointerInit();

        if (result.TileChangeResult != null)
            BoardView.RenderTile(result.TileChangeResult);
    }

    public void Apply(List<MoveResult> results)
    {
        if (results.Count == 0)
            return;

        if (results[0]._success)
            _actionBox?.Render(results[0].ActionBoxResult);

        foreach (var moveResult in results)
        {
            if (moveResult._success)
                BoardView.MoveRender(moveResult);

            if (moveResult.RefreshPlayerVitals)
                _vitalsBox?.DisplayVitals();

            if (moveResult.PlayerDead)
                Apply(new GameEndResult(false));
        }

        _fightBox?.Render(results[0].NearbyEnemy);
    }

    public void Apply(FightStartResult result)
    {
        if (!result.IsSuccess)
        {
            if (result.ErrorMessage != null)
                _aboveActionErrorSpace?.DisplayErr(result.ErrorMessage);
            return;
        }

        if (result.Enemy != null && result.Player != null)
            _fightBox?.FightMode(result.Player, result.Enemy);
    }

    public void Apply(FightAttackSelectionResult result)
    {
        _fightBox?.HighlightAttack(result.Attack);
    }

    public void Apply(FightHandSelectionResult result)
    {
        _fightBox?.HighlightHand(result.Hand, result.Player);
    }

    public void Apply(FightExitResult result)
    {
        _fightBox?.Clear();
    }

    public void Apply(FightTurnResult result)
    {
        _fightBox?.HighlightHand(result.Hand, result.Player);
        _fightBox?.HighlightAttack(result.Attack);

        if (result.ErrorMessage != null)
        {
            _aboveFightErrorSpace?.DisplayErr(result.ErrorMessage);
            return;
        }

        if (result.UpdateEnemyVitals)
            _fightBox?.UpdateEnemyVitals(result.Enemy);

        if (result.EnemyDead)
            _fightBox?.DeadEnemyDisplay(result.Enemy);

        if (result.RefreshPlayerVitals)
            _vitalsBox?.DisplayVitals();

        if (result.PlayerDead)
            Apply(new GameEndResult(false));
    }

    public void Apply(FightLoopEndResult result)
    {
        if (result.TileChangeResult != null)
            BoardView.RenderTile(result.TileChangeResult);
    }

    public void Apply(SeekResult result)
    {
        _actionBox?.Render(result.ActionBoxResult);
    }

    public void Apply(PickUpResult result)
    {
        if (result.IsSuccess)
        {
            if (result.TileChangeResult != null)
            {
                BoardView.RenderTile(result.TileChangeResult);
                _actionBox?.Render(result.ActionBoxResult);
                _wealthBox?.DisplayGoods();
                _equipmentBox?.DisplayItems();
            }

            if (result.Result != null)
                BoardView.SendWave(result.Result);

            return;
        }

        if (result.Errormsg != null && _actionBox != null && _aboveActionErrorSpace != null)
            _actionBox.ErrorDisplay(_aboveActionErrorSpace, result.Errormsg);
    }

    public void Apply(GameEndResult result)
    {
        if (result.EndedGood)
            EndGood();
        else
            EndBad();
    }

    public static void EndGood()
    {
        Console.Clear();
        Console.WriteLine("Game Ended!");
        // Environment.Exit(0);
    }
    
    public static void EndBad() // TODO Lepiej to zrob
    {
        var endScreen = new EndScreen();
        endScreen.EndGame();
        Thread.Sleep(3000);
        Console.Clear();
        // Environment.Exit(0);
    }
}
