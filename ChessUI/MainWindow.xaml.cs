using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ChessLogic;

namespace ChessUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Image[,] pieceImages = new Image[8, 8];
    private readonly Rectangle[,] highlights = new Rectangle[8, 8];
    private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();
    private readonly int ENGINE_DEPTH = 4;
    private readonly bool DEBUG_MODE = false; // Used for testing chess engine on different positions
    private readonly string DEBUG_FEN = "1B2q1B1/2n1kPR1/R1b2n1Q/2p1r3/8/3Q2B1/4p3/4K3 w - - 0 1";
    //private readonly string DEBUG_FEN = "kbK5/pp6/1P6/8/8/8/8/R7 w - - 0 1";
    private GameState gameState;
    private Position selectedPosition = null;
    private Player userPerspective = Player.White;
    private OpponentType opponentType = OpponentType.Freestyle;

    public MainWindow()
    {
        InitializeComponent();
        InitializeBoard();
        ShowStartMenu();
        gameState = new GameState(Player.White, Chessboard.Initial());
        DrawBoard(gameState.Chessboard);
        SetCursor(gameState.CurrentPlayer);
    }

    private void InitializeBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Image image = new Image();
                pieceImages[row, col] = image;
                PieceGrid.Children.Add(image);

                Rectangle highlight = new Rectangle();
                highlights[row, col] = highlight;
                HighlightGrid.Children.Add(highlight);
            }
        }
    }

    private void DrawBoard(Chessboard chessboard)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                int displayRow = userPerspective == Player.Black ? BlackDisplayOffset(row) : row;
                int displayCol = userPerspective == Player.Black ? BlackDisplayOffset(col) : col;

                Piece piece = chessboard[row, col];
                pieceImages[displayRow, displayCol].Source = Images.GetImage(piece);

            }
        }
    }

    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMenuOnScreen()) return;
        if (opponentType != OpponentType.Freestyle && IsBotTurn()) return;

        Point point = e.GetPosition(BoardGrid);
        Position pos = ToSquarePosition(point);

        if (selectedPosition == null)
        {
            OnFromPositionSelected(pos);
        }
        else
        {
            OnToPositionSelected(pos);
        }
    }

    private void OnFromPositionSelected(Position from)
    {
        IEnumerable<Move> moves = gameState.LegalMovesForPiece(from);

        if (moves.Any())
        {
            selectedPosition = from;
            CacheMoves(moves);
            ShowHighlights();
        }
    }

    private void OnToPositionSelected(Position pos)
    {
        selectedPosition = null;
        HideHighlights();

        if (moveCache.TryGetValue(pos, out Move move))
        {
            if (move.Type == MoveType.PawnPromotion)
            {
                HandlePromotion(move.FromPosition, move.ToPosition);
            }
            else 
            {
                HandleMove(move);
            }                
        }
    }

    private void HandlePromotion(Position from, Position to)
    {
        pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
        pieceImages[from.Row, from.Column].Source = null;

        PromotionMenu promotionMenu = new PromotionMenu(gameState.CurrentPlayer);
        MenuContainer.Content = promotionMenu;

        promotionMenu.PieceSelected += type =>
        {
            MenuContainer.Content = null;
            Move promotionMove = new PawnPromotion(from, to, type);
            HandleMove(promotionMove);
        };
    }

    private void HandleMove(Move move)
    {
        gameState.MakeMove(move);
        DrawBoard(gameState.Chessboard);
        SetCursor(gameState.CurrentPlayer);

        if (gameState.IsGameOver())
        {
            ShowGameOver();
            return;
        }

        // If not freestyle, let bot make move
        if (opponentType == OpponentType.Random && IsBotTurn()) HandleRandomBotMove();
        if (opponentType == OpponentType.Engine && IsBotTurn()) HandleEngineBotMove();
    }

    private async void HandleRandomBotMove()
    {
        await Task.Delay(500);
        RandomBot.MakeRandomMove(gameState);
        DrawBoard(gameState.Chessboard);
        SetCursor(gameState.CurrentPlayer);
        if (gameState.IsGameOver())
        {
            ShowGameOver();
        }
    }

    private async void HandleEngineBotMove()
    {
        await Task.Delay(100);
        ChessEngineBot.MakeEngineMove(gameState, ENGINE_DEPTH);
        DrawBoard(gameState.Chessboard);
        SetCursor(gameState.CurrentPlayer);
        if (gameState.IsGameOver())
        {
            ShowGameOver();
        }
    }

    private Position ToSquarePosition(Point point)
    {
        double squareSize = BoardGrid.ActualHeight / 8;
        int row = (int)(point.Y / squareSize);
        int col = (int)(point.X / squareSize);

        if (userPerspective == Player.Black)
        {
            row = BlackDisplayOffset(row);
            col = BlackDisplayOffset(col);
        }

        return new Position(row, col);
    }

    private void CacheMoves(IEnumerable<Move> moves)
    {
        moveCache.Clear();

        foreach (Move move in moves)
        {
            moveCache[move.ToPosition] = move;
        }
    }

    private void ShowHighlights()
    {
        Color color = Color.FromArgb(150, 125, 255, 125);

        foreach (Position to in moveCache.Keys)
        {
            int displayRow = (userPerspective == Player.Black) ? BlackDisplayOffset(to.Row) : to.Row;
            int displayCol = (userPerspective == Player.Black) ? BlackDisplayOffset(to.Column) : to.Column;
            highlights[displayRow, displayCol].Fill = new SolidColorBrush(color);
        }
    }

    private void HideHighlights()
    {
        foreach (Position to in moveCache.Keys)
        {
            int displayRow = (userPerspective == Player.Black) ? BlackDisplayOffset(to.Row) : to.Row;
            int displayCol = (userPerspective == Player.Black) ? BlackDisplayOffset(to.Column) : to.Column;
            highlights[displayRow, displayCol].Fill = Brushes.Transparent;
        }
    }

    private void SetCursor(Player player)
    {
        if (player == Player.White)
        {
            Cursor = Cursors.WhiteCursor;
        }
        else
        {
            Cursor = Cursors.BlackCursor;
        }
    }

    private bool IsMenuOnScreen()
    {
        return MenuContainer.Content != null;
    }

    private void ShowGameOver()
    {
        GameOverMenu gameOverMenu = new GameOverMenu(gameState);
        MenuContainer.Content = gameOverMenu;
        gameOverMenu.OptionSelected += option =>
        {
            if (option == Option.Restart)
            {
                MenuContainer.Content = null;
                RestartGame();
            }
            if (option == Option.Exit)
            {
                Application.Current.Shutdown();
            }
        };
    }

    private void StartGame()
    {
        MenuContainer.Content = null;
        RestartGame();
    }

    private void RestartGame()
    {
        selectedPosition = null;
        HideHighlights();
        moveCache.Clear();

        if (DEBUG_MODE) gameState = FenString.GameStateFromFen(DEBUG_FEN);
        else gameState = new GameState(Player.White, Chessboard.Initial());

        DrawBoard(gameState.Chessboard);
        SetCursor(gameState.CurrentPlayer);
        if (opponentType == OpponentType.Random && IsBotTurn()) HandleRandomBotMove();
        if (opponentType == OpponentType.Engine && IsBotTurn()) HandleEngineBotMove();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (!IsMenuOnScreen() && e.Key == Key.Escape)
        {
            ShowPauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        PauseMenu pauseMenu = new PauseMenu();
        MenuContainer.Content = pauseMenu;
        pauseMenu.OptionSelected += option =>
        {
            MenuContainer.Content = null;

            if (option == Option.Restart)
            {
                RestartGame();
            }
            if (option == Option.Menu)
            {
                ShowStartMenu();
            }
        };
    }

    private int BlackDisplayOffset(int rowOrCol)
    {
        return 7 - rowOrCol;
    }

    private void ShowStartMenu()
    {
        RestartGame();
        StartMenu startMenu = new StartMenu();
        startMenu.PlayerButton.Content = userPerspective;
        startMenu.OpponentSelect.SelectedValue = opponentType;
        MenuContainer.Content = startMenu;
        startMenu.OptionSelected += option =>
        {
            // Player Color Option
            if (option == Option.Black) ChangeUserPerspective(Player.Black);
            if (option == Option.White) ChangeUserPerspective(Player.White);
            // Opponent Type Option
            if (option == Option.Freestyle) opponentType = OpponentType.Freestyle;
            if (option == Option.Random) opponentType = OpponentType.Random;
            if (option == Option.Engine) opponentType = OpponentType.Engine;
            // Start/Exit Option
            if (option == Option.Start) StartGame();
            if (option == Option.Exit) Application.Current.Shutdown();
        };
    }

    private void ChangeUserPerspective(Player player)
    {
        userPerspective = player;
        DrawBoard(gameState.Chessboard);
    }

    private bool IsBotTurn()
    {
        return gameState.CurrentPlayer != userPerspective;
    }
}