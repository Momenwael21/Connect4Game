using System.Numerics;

namespace Connect4Game
{
    public partial class Connect4Game : Form
    {
        enum Player { Red, Yellow }
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

            int row = GetLowestEmptyRow(column); // Get the lowest empty row in the column

            if (row != -1)
            {
                // Place player's token in the cell
                cell.BackColor = currentPlayer == Player.Red ? Color.Red : Color.Yellow;
                // Update game state
                gameState[row, column] = currentPlayer;
                // Check for win or draw
                if (CheckForWin(row, column) || CheckForDraw())
                {
                    // Handle win or draw
                }
                else
                {
                    // Switch player
                    currentPlayer = currentPlayer == Player.Red ? Player.Yellow : Player.Red;
                }
            }
        }

        private bool CheckForWin(int row, int column)
        {
            // Implement logic to check for a win
            // Check horizontally, vertically, and diagonally
            return true;
        }

        private bool CheckForDraw()
        {
            // Implement logic to check for a draw
            // Check if the grid is completely filled
            return true;
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
