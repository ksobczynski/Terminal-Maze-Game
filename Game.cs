using PROJ.Builder.Classes;
using PROJ.Configuration;
using PROJ.Handlers;

namespace PROJ;

public class Game
{
    private Board _board;
    private Player _player;
    
    //Boxes
    private ActionBox _actionBox;
    private VitalsBox _vitalsBox;
    private WealthBox _wealthBox;
    private EquipmentBox _eqBox;
    private LeftHandBox _leftHandBox;
    private RightHandBox _rightHandBox;
    private AboveActionErrorSpace _errSpace;
    private AboveFightErrorSpace _fightErrSpace;
    private PlayerMovesBox _pmBox;
    private FightBox _fightBox;
    private GameController _controller;
    private readonly GameWorldBootstrapper _worldBootstrapper = new();

    
    
    public Game()
    {
        _actionBox = new ActionBox();
        _fightBox = new FightBox();
        _pmBox = new PlayerMovesBox();
        _board = new Board();
        _player = new Player();
        _player.AssignBoard(_board);

        _vitalsBox = new VitalsBox(_player);
        _wealthBox = new WealthBox(_player);
        _eqBox = new EquipmentBox(_player);
        _leftHandBox = new LeftHandBox(_player);
        _rightHandBox = new RightHandBox(_player);
        _errSpace = new AboveActionErrorSpace();
        _fightErrSpace = new AboveFightErrorSpace();

        var gameView = GameView.GetInstance();
        gameView.ActionBox = _actionBox;
        gameView.VitalsBox = _vitalsBox;
        gameView.WealthBox = _wealthBox;
        gameView.EquipmentBox = _eqBox;
        gameView.LeftHandBox = _leftHandBox;
        gameView.RightHandBox = _rightHandBox;
        gameView.AboveActionErrorSpace = _errSpace;
        gameView.AboveFightErrorSpace = _fightErrSpace;
        gameView.PlayerMovesBox = _pmBox;
        gameView.FightBox = _fightBox;
        
        
    }
    public void Start()
    {
        var config = Configurator.Instance.Configure();
        Console.Clear();

        var themeSettings = Configurator.Instance.ConfigureTheme();
        
        
        Console.CursorVisible = false;
        _worldBootstrapper.Populate(_board, _player, new PlayerMovesBuilder(_pmBox), themeSettings, config.PlayerName);
        BoardView.Display(_board.CreateSnapshotResult());
        _actionBox.DisplayFrame();
        _fightBox.DisplayFrame();
        
        _vitalsBox.DisplayFrame();
        _vitalsBox.DisplayVitals();
        
        _wealthBox.DisplayFrame();
        _wealthBox.DisplayGoods();
        
        _eqBox.DisplayFrame();
        _eqBox.DisplayItems();
        
        _leftHandBox.DisplayFrame();
        _leftHandBox.DisplayHand();
        
        _rightHandBox.DisplayFrame();
        _rightHandBox.DisplayHand();
        _controller = new GameController(_board, _player, _errSpace);
        _controller.Run();
        
    }
    
    

    
}
