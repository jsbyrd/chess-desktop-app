using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
    private GameState gameState;
    private Position selectedPosition = null;

    public MainWindow()
    {
        InitializeComponent();
        InitializeBoard();
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
                Piece piece = chessboard[row, col];
                pieceImages[row, col].Source = Images.GetImage(piece);

            }
        }
    }

    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMenuOnScreen()) return;

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
        }
    }

    private Position ToSquarePosition(Point point)
    {
        double squareSize = BoardGrid.ActualHeight / 8;
        int row = (int)(point.Y / squareSize);
        int col = (int)(point.X / squareSize);
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
            highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
        }
    }

    private void HideHighlights()
    {
        foreach (Position to in moveCache.Keys)
        {
            highlights[to.Row, to.Column].Fill = Brushes.Transparent;
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

    private void RestartGame()
    {
        selectedPosition = null;
        HideHighlights();
        moveCache.Clear();
        gameState = new GameState(Player.White, Chessboard.Initial());
        DrawBoard(gameState.Chessboard);
        SetCursor(gameState.CurrentPlayer);
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
        };
    }
}