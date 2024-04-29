using System.Numerics;

namespace Connect4Game
{
    public enum Player { Red, Yellow }

   
    public partial class Connect4Game : Form
    {
        Player currentPlayer = Player.Red;
        Player?[,] gameState = new Player?[6, 7]; // 6 rows, 7 columns
        

        public Connect4Game()
        {
            InitializeComponent();
        }

       
        private void Cell_Click(object sender, EventArgs e)
        {
            Button cell = (Button)sender;
            int column = tableLayoutPanel1.GetColumn(cell); // Get the column of the clicked cell
            
           

            int row = GetLowestEmptyRow(column); // Get the lowest empty row in the column
           
            if (row != -1)
            {
                // Place player's token in the cell
               
                cell.BackColor = currentPlayer == Player.Red ? Color.Red : Color.Yellow;
                // Update game state
                gameState[row, column] = currentPlayer;
                // Check for win or draw
                
                    // Switch player
                    currentPlayer = Player.Yellow;
                        if (HelperMethods.CheckForWin(gameState))
                        {
                            UpdateUI();
                            HandleWin("Congratulations, You are smarter than AI");
                        } else
                        {
                            MakeAIMove();
                        }
                        return;          
            }

        }


       
       
    
        // AI Methods
        /* private const int MaxDepth = 6; // Maximum search depth for the minimax algorithm

        private int Minimax(Player?[,] board, int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            if (depth == 0)
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
            // Update the UI to reflect the AI player's move
            gameState = MakeMove(gameState, bestMove, Player.Yellow);
           
            if (CheckForWin(gameState))
            {
                UpdateUI();
                HandleWin("Game over!");
                return;
            } else
            {
              
                currentPlayer = Player.Red;
                UpdateUI();
            };
        }*/

        // private IAIAlgorithm currentAIAlgorithm;

        /* private void SetAIAlgorithm(IAIAlgorithm algorithm)
        {
            currentAIAlgorithm = algorithm;
        } */

        private void MakeAIMove()
        {
            int bestMove = BFSAlgorithm.FindBestMove(gameState, Player.Yellow);

            // Apply the best move to the game state
            gameState = MakeMove(gameState, bestMove, Player.Yellow);

            if (HelperMethods.CheckForWin(gameState))
            {
                UpdateUI();
                HandleWin("Game over!");
                return;
            }
            else
            {

                currentPlayer = Player.Red;
                UpdateUI();
            };
        }

        //Helper methods
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

        private bool HandleWin(string message)
        {
            if (HelperMethods.CheckForWin(gameState))
            {
                // Disable the game board
                tableLayoutPanel1.Enabled = false;

                // Optionally, you can display a message indicating the winner
                MessageBox.Show(message, "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
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

        private void Reset_Game(object sender, EventArgs e)
        {
            gameState = new Player?[6, 7];

            // Enable the game board for the human player's turn
            tableLayoutPanel1.Enabled = true;
            foreach (Button button in tableLayoutPanel1.Controls.OfType<Button>())
            {
                button.BackColor = Color.LightGray; // Reset button colors to white (empty)
            }
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                control.Enabled = true;
            }


            // Optionally, switch the current player to the human player
            currentPlayer = Player.Red;
        }
    }
}
