using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect4Game
{
    public class HelperMethods
    {
        public static bool CheckForWin(Player?[,] board)
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

        public static Player?[,] MakeMove(Player?[,] board, int column, Player player)
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

        public static int GetLowestEmptyRow(int column, Player?[,] gameState)
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

        public static bool IsColumnFull(Player?[,] board, int column)
        {
            for (int row = 5; row >= 0; row--)
            {
                if (board[row, column] == null)
                    return false;
            }
            return true;
        }

        public static int CountConnectedTokens(Player?[,] board, Player player)
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


        public static int EvaluateBoard(Player?[,] board)
        {
            // Simple evaluation function that counts the number of connected tokens for each player
            int yellowScore = CountConnectedTokens(board, Player.Yellow);
            int redScore = CountConnectedTokens(board, Player.Red);
            return yellowScore - redScore;
        }


    }
}
