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
using ChessLogic.Pieces;

namespace ChessGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8,8];
        private readonly Rectangle[,] highlights = new Rectangle[8,8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();

            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
        }

        private void InitializeBoard()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++) 
                {
                    Image image = new Image();
                    pieceImages[i,j] = image;
                    PieceGrid.Children.Add(image);

                    Rectangle highlight = new Rectangle();
                    highlights[i, j] = highlight;
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }

        private void DrawBoard(Board board)
        {
            for (int i = 0; i < 8;i++)
            {
                for(int  k = 0; k < 8; k++)
                {
                    Piece piece = board[i, k];
                    pieceImages[i,k].Source = Images.GetImage(piece);
                }
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMenuOnScreen())
            {
                return;
            }

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);

            if(selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int col = (int)(point.X / squareSize);
            return new Position(row, col);
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighLights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighLights();

            if(moveCache.TryGetValue(pos, out Move move))
            {
                if(move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPos, move.ToPos);
                }
                else
                {
                    HandleMove(move);
                }
            }
        }

        private void HandlePromotion(Position fromPos, Position toPos)
        {
            pieceImages[toPos.Row, toPos.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[fromPos.Row, fromPos.Column].Source = null;

            PromotionMenu promotionMenu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = promotionMenu;

            promotionMenu.PieceSelected += type =>
            {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(fromPos, toPos, type);
                HandleMove(promMove);
            };
        }

        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board);

            if (gameState.IsGameOver())
            {
                ShowGameOver();
            }
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();

            foreach(Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        private void ShowHighLights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125);
            foreach(Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighLights()
        {
            foreach(Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
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
                else
                {
                    Application.Current.Shutdown();
                }
            };
        }

        private void RestartGame()
        {
            selectedPos = null;
            HideHighLights();
            moveCache.Clear();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(!IsMenuOnScreen() && e.Key == Key.Escape)
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

                if(option == Option.Restart)
                {
                    RestartGame();
                }
            };
        }
    }
}