using System.Numerics;

namespace Connect4Game
{
    public partial class Connect4Game : Form
    {
        enum Player { Red, Yellow }
        Player currentPlayer = Player.Red;
        Player?[,] gameState = new Player?[6, 7]; // 6 rows, 7 columns
        private int GetLowestEmptyRow(int column)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (gameState[row, column] == null)
                {
                    return row; // Return the lowest empty row
                }
            }
            return -1; // Column is full
        }


        public Connect4Game()
        {
            InitializeComponent();
            InitializeGameBoard();
        }

        private void InitializeGameBoard()
        {
            // Code to dynamically create the game board grid
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            Button cell = (Button)sender;
            int column = tableLayoutPanel1.GetColumn(cell); // Get the column of the clicked cell
            
            // Only allow the current player to make moves
            if ((currentPlayer == Player.Red && cell.BackColor == Color.Red) ||
                (currentPlayer == Player.Yellow && cell.BackColor == Color.Yellow))
            {
                
                // Player can't make a move in a column that is already full
                return;
            }

            int row = GetLowestEmptyRow(column); // Get the lowest empty row in the column
           
            if (row != -1)
            {
                // Place player's token in the cell
               
                cell.BackColor = currentPlayer == Player.Red ? Color.Red : Color.Yellow;
                // Update game state
                gameState[row, column] = currentPlayer;
                // Check for win or draw
                if (CheckForWin(gameState))
                {
                    MessageBox.Show("Winner");
                }
                else
                {
                    // Switch player
                    currentPlayer = currentPlayer == Player.Red ? Player.Yellow : Player.Red;

                    // Make AI move if it's the AI's turn
                    if (currentPlayer == Player.Yellow)
                    {
                        MakeAIMove();
                    }  else
                    {
                        Console.WriteLine("Human Role");
                    }
                }
            }
        }

        private bool CheckForWin(Player?[,] board)
        {
            // Check horizontally
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] != null &&
                        board[row, col] == board[row, col + 1] &&
                        board[row, col] == board[row, col + 2] &&
                        board[row, col] == board[row, col + 3])
                    {
                        return true; // Four consecutive tokens in a row
                    }
                }
            }

            // Check vertically
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (board[row, col] != null &&
                        board[row, col] == board[row + 1, col] &&
                        board[row, col] == board[row + 2, col] &&
                        board[row, col] == board[row + 3, col])
                    {
                        return true; // Four consecutive tokens in a column
                    }
                }
            }

            // Check diagonally (bottom-left to top-right)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[row, col] != null &&
                        board[row, col] == board[row + 1, col + 1] &&
                        board[row, col] == board[row + 2, col + 2] &&
                        board[row, col] == board[row + 3, col + 3])
                    {

                        return true; // Four consecutive tokens in a diagonal
                    }
                }
            }

            // Check diagonally (bottom-right to top-left)
            for (int row = 0; row < 3; row++)
            {
                for (int col = 3; col < 7; col++)
                {
                    if (board[row, col] != null &&
                        board[row, col] == board[row + 1, col - 1] &&
                        board[row, col] == board[row + 2, col - 2] &&
                        board[row, col] == board[row + 3, col - 3])
                    {
                        return true; // Four consecutive tokens in a diagonal
                    }
                }
            }

            return false; // No win condition found
        }


        private const int MaxDepth = 6; // Maximum search depth for the minimax algorithm

        private int Minimax(Player?[,] board, int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            if (depth == 0 || !CheckForWin(board) )
            {
                return EvaluateBoard(board);
            }

            if (isMaximizingPlayer)
            {
                int maxEval = int.MinValue;
                for (int col = 0; col < 7; col++)
                {
                    if (IsColumnFull(board, col))
                        continue;

                    Player?[,] newBoard = MakeMove(board, col, Player.Yellow);
                    int eval = Minimax(newBoard, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int col = 0; col < 7; col++)
                {
                    if (IsColumnFull(board, col))
                        continue;

                    Player?[,] newBoard = MakeMove(board, col, Player.Red);
                    int eval = Minimax(newBoard, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        private int EvaluateBoard(Player?[,] board)
        {
            // Simple evaluation function that counts the number of connected tokens for each player
            int yellowScore = CountConnectedTokens(board, Player.Yellow);
            int redScore = CountConnectedTokens(board, Player.Red);
            return yellowScore - redScore;
        }

        private int CountConnectedTokens(Player?[,] board, Player player)
        {
            int score = 0;
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (board[row, col] == player)
                    {
                        // Check horizontal
                        if (col <= 3)
                        {
                            bool connected = true;
                            for (int k = 0; k < 4; k++)
                            {
                                if (board[row, col + k] != player)
                                {
                                    connected = false;
                                    break;
                                }
                            }
                            if (connected)
                                score++;
                        }
                        // Check vertical
                        if (row <= 2)
                        {
                            bool connected = true;
                            for (int k = 0; k < 4; k++)
                            {
                                if (board[row + k, col] != player)
                                {
                                    connected = false;
                                    break;
                                }
                            }
                            if (connected)
                                score++;
                        }
                        // Check diagonal (bottom-left to top-right)
                        if (col >= 3 && row <= 2)
                        {
                            bool connected = true;
                            for (int k = 0; k < 4; k++)
                            {
                                if (board[row + k, col - k] != player)
                                {
                                    connected = false;
                                    break;
                                }
                            }
                            if (connected)
                                score++;
                        }
                        // Check diagonal (bottom-right to top-left)
                        if (col <= 3 && row <= 2)
                        {
                            bool connected = true;
                            for (int k = 0; k < 4; k++)
                            {
                                if (board[row + k, col + k] != player)
                                {
                                    connected = false;
                                    break;
                                }
                            }
                            if (connected)
                                score++;
                        }
                    }
                }
            }
            return score;
        }

        private bool IsColumnFull(Player?[,] board, int column)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (board[row, column] == null)
                    return false;
            }
            return true;
        }

        private Player?[,] MakeMove(Player?[,] board, int column, Player player)
        {
            Player?[,] newBoard = (Player?[,])board.Clone();
            for (int row = 5; row >= 0; row--)
            {
                if (newBoard[row, column] == null)
                {
                    newBoard[row, column] = player;
                    return newBoard;
                }
            }
            return newBoard;
        }

        private void UpdateUI()
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (tableLayoutPanel1.GetControlFromPosition(col, row) is Button cellButton)
                    {
                        Player? player = gameState[row, col];
                        if (player == Player.Red)
                        {
                            cellButton.BackColor = Color.Red; // Set color for Red player's token
                        }
                        else if (player == Player.Yellow)
                        {
                            cellButton.BackColor = Color.Yellow; // Set color for Yellow player's token
                        }
                        else
                        {
                            cellButton.BackColor = Color.LightGray; // Empty cell
                        }
                    }
                }
            }

            // Re-enable all buttons if it's the human player's turn
            if (currentPlayer == Player.Red)
            {
                foreach (Control control in tableLayoutPanel1.Controls)
                {
                    control.Enabled = control.BackColor == Color.LightGray; // Enable empty cells
                }
            }
            else
            {
                foreach (Control control in tableLayoutPanel1.Controls)
                {
                    control.Enabled = false; // Disable all buttons during AI's turn
                }
            }
        }

        private void MakeAIMove()
        {
            int bestMove = -1;
            int bestEval = int.MinValue;
            for (int col = 0; col < 7; col++)
            {
                if (IsColumnFull(gameState, col))
                    continue;

                Player?[,] newBoard = MakeMove(gameState, col, Player.Yellow);
                int eval = Minimax(newBoard, MaxDepth, int.MinValue, int.MaxValue, false);
                if (eval > bestEval)
                {
                    bestEval = eval;
                    bestMove = col;
                }
            }
            // Apply the best move to the game state
            gameState = MakeMove(gameState, bestMove, Player.Yellow);
            currentPlayer = currentPlayer == Player.Red ? Player.Yellow : Player.Red;
            // Update the UI to reflect the AI player's move
            UpdateUI();
        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
