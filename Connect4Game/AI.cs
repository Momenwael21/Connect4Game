using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Connect4Game
{
    public class BFSAlgorithm
    {
        public static int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            Queue<(Player?[,], int, int)> queue = new Queue<(Player?[,], int, int)>();
            HashSet<string> visitedStates = new HashSet<string>();

            // Enqueue each valid starting move
            for (int col = 0; col < 7; col++)
            {
                if (!HelperMethods.IsColumnFull(board, col))
                {
                    Player?[,] newBoard = HelperMethods.MakeMove(board, col, currentPlayer);
                    string boardState = BoardToString(newBoard);

                    if (!visitedStates.Contains(boardState))
                    {
                        visitedStates.Add(boardState);
                        queue.Enqueue((newBoard, col, 0));
                    }
                }
            }

            while (queue.Count > 0)
            {
                (Player?[,] player, int currentCol, int depth) = queue.Dequeue();
                Player?[,] newBoard = HelperMethods.MakeMove(board, currentCol, currentPlayer);

                // Check if the current move leads to a win
                if (HelperMethods.CheckForWin(newBoard))
                {
                    return currentCol; // Return winning move
                }

                // Track visited state using a string representation of the board
                string boardState = BoardToString(newBoard);
                if (!visitedStates.Contains(boardState))
                {
                    visitedStates.Add(boardState);

                    // Explore adjacent moves (columns) if there is depth left to explore
                    for (int newCol = 0; newCol < 7; newCol++)
                    {
                        if (!HelperMethods.IsColumnFull(board, newCol))
                        {
                            queue.Enqueue((newBoard, newCol, depth + 1));
                        }
                    }
                }
            }

            // Fallback: Return any valid move if no winning move is found
            for (int col = 0; col < 7; col++)
            {
                if (!HelperMethods.IsColumnFull(board, col))
                    return col;
            }

            return -1; // No valid moves found (should not happen)
        }

        // Helper function to convert board to a string representation for state tracking
        private static string BoardToString(Player?[,] board)
        {
            // Convert the board state to a string for easy tracking in the visited states set
            var boardString = new System.Text.StringBuilder();
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    boardString.Append(board[row, col]?.ToString() ?? ".");
                }
            }
            return boardString.ToString();
        }
    }

    public class TupleComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y); // Compare costs
        }
    }

    public class UCSAlgorithm
    {
        public static int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            // Priority queue with a tuple comparer for state (cost, column) and priority as cost
            PriorityQueue<(Player?[,], int, int), int> pq = new PriorityQueue<(Player?[,], int, int), int>(new TupleComparer());

            // Set of visited states
            HashSet<(Player?[,], int)> visited = new HashSet<(Player?[,], int)>();

            // Enqueue the initial board state with zero cost
            pq.Enqueue((board, 0, 0), 0);
            visited.Add((board, 0));

            while (pq.Count > 0)
            {
                // Dequeue the state with the lowest cost
                var (currentBoard, currentCost, currentCol) = pq.Dequeue();

                // Check if the move leads to a win
                if (HelperMethods.CheckForWin(currentBoard))
                    return currentCol;

                // Explore adjacent columns (new moves)
                for (int newCol = 0; newCol < 7; newCol++)
                {
                    if (!HelperMethods.IsColumnFull(currentBoard, newCol))
                    {
                        // Make the move in the new column
                        Player?[,] newBoard = HelperMethods.MakeMove(currentBoard, newCol, currentPlayer);

                        // Create the new state
                        var newState = (newBoard, currentCost + 1, newCol);

                        // Check if the new state has been visited
                        if (!visited.Contains((newBoard, newCol)))
                        {
                            // Add the new state to the visited set
                            visited.Add((newBoard, newCol));

                            // Enqueue the new state with the updated cost
                            pq.Enqueue(newState, currentCost + 1);
                        }
                    }
                }
            }

            // Fallback: Return any valid move if no winning move is found
            for (int col = 0; col < 7; col++)
            {
                if (!HelperMethods.IsColumnFull(board, col))
                    return col;
            }

            return -1; // No valid moves found (should not happen)
        }
    }

    public class GreedySearchAlgorithm
    {
        public static int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            int bestMove = -1;
            int bestEval = int.MinValue;
            for (int col = 0; col < 7; col++)
            {
                if (HelperMethods.IsColumnFull(board, col))
                    continue;
                Player?[,] newBoard = HelperMethods.MakeMove(board, col, currentPlayer);
                int eval = HelperMethods.EvaluateBoard(newBoard);
                if (eval > bestEval)
                {
                    bestEval = eval;
                    bestMove = col;
                }
            }
            return bestMove;
        }
    }

    
}
