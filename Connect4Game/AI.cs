using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Connect4Game
{
    public interface IAIAlgorithm
    {
        int FindBestMove(Player?[,] board, Player currentPlayer);
    }

    private interface MaxDepth = 6;

    public class BFSAlgorithm : IAIAlgorithm
    {
        public int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            Queue<int> queue = new Queue<int>();
            int[] moves = new int[7];
            queue.Enqueue(0);
            while (queue.Count > 0)
            {
                int col = queue.Dequeue();
                Player?[,] newBoard = HelperMethods.MakeMove(board, col, currentPlayer);
                if (HelperMethods.CheckForWin(newBoard))
                    return col;
                for (int newCol = 0; newCol < 7; newCol++)
                {
                    if (!HelperMethods.IsColumnFull(board, newCol))
                        queue.Enqueue(newCol);
                }
            }
            // Return any valid move (fallback)
            for (int col = 0; col < 7; col++)
            {
                if (!HelperMethods.IsColumnFull(board, col))
                    return col;
            }
            return -1;
        }
    }

    public class DepthLimitedSearchAlgorithm : IAIAlgorithm
    {
        // Parameters
        private const int DepthLimit = 4; // The depth limit for the search

        public int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            int bestMove = -1;
            int bestUtility = int.MinValue;

            // Perform depth-limited search for each column
            for (int col = 0; col < 7; col++)
            {
                if (IsColumnFull(board, col))
                    continue;

                // Make a move in the current column
                Player?[,] newBoard = MakeMove(board, col, currentPlayer);

                // Perform DLS from the new board state
                int utility = DepthLimitedSearch(newBoard, currentPlayer, DepthLimit - 1, false);

                // Track the best move based on utility
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestMove = col;
                }
            }

            return bestMove;
        }

        private int DepthLimitedSearch(Player?[,] board, Player currentPlayer, int depth, bool isMaximizingPlayer)
        {
            if (depth == 0)
            {
                // At the depth limit, evaluate the board
                return EvaluateBoard(board, currentPlayer);
            }

            int bestUtility = isMaximizingPlayer ? int.MinValue : int.MaxValue;

            for (int col = 0; col < 7; col++)
            {
                if (IsColumnFull(board, col))
                    continue;

                // Make a move in the current column
                Player?[,] newBoard = MakeMove(board, col, currentPlayer);

                // Switch player for the next move
                Player nextPlayer = isMaximizingPlayer ? currentPlayer. : currentPlayer;

                // Recursively perform DLS from the new board state
                int utility = DepthLimitedSearch(newBoard, nextPlayer, depth - 1, !isMaximizingPlayer);

                // Update the best utility based on whether maximizing or minimizing
                if (isMaximizingPlayer)
                {
                    bestUtility = Math.Max(bestUtility, utility);
                }
                else
                {
                    bestUtility = Math.Min(bestUtility, utility);
                }
            }

            return bestUtility;
        }

        private bool IsColumnFull(Player?[,] board, int col)
        {
            // Implement logic to check if a column is full
            return board[0, col] != null;
        }

        private Player?[,] MakeMove(Player?[,] board, int column, Player player)
        {
            // Implement logic to make a move and return the new board state
            Player?[,] newBoard = (Player?[,])board.Clone();

            for (int row = 5; row >= 0; row--)
            {
                if (newBoard[row, column] == null)
                {
                    newBoard[row, column] = player;
                    break;
                }
            }

            return newBoard;
        }

        private int EvaluateBoard(Player?[,] board, Player currentPlayer)
        {
            // Implement your evaluation function here
            // Calculate the utility score based on the current game state
            return 0; // Placeholder: replace with your utility calculation logic
        }
    }

    public class UCSAlgorithm : IAIAlgorithm
    {
        public int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            PriorityQueue<(int, int)> pq = new PriorityQueue<(int, int)>();
            pq.Enqueue((0, 0)); // (cost, column)
            while (pq.Count > 0)
            {
                (int cost, int col) = pq.Dequeue();
                Player?[,] newBoard = HelperMethods.MakeMove(board, col, currentPlayer);
                if (HelperMethods.CheckForWin(newBoard))
                    return col;
                for (int newCol = 0; newCol < 7; newCol++)
                {
                    if (!HelperMethods.IsColumnFull(board, newCol))
                        pq.Enqueue((cost + 1, newCol));
                }
            }
            // Return any valid move (fallback)
            for (int col = 0; col < 7; col++)
            {
                if (!HelperMethods.IsColumnFull(board, col))
                    return col;
            }
            return -1;
        }
    }

    public class GreedySearchAlgorithm : IAIAlgorithm
    {
        public int FindBestMove(Player?[,] board, Player currentPlayer)
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

    public class GeneticSearchAlgorithm : IAIAlgorithm
    {
        // Parameters
        private const int PopulationSize = 20; // Size of the population
        private const int NumGenerations = 100; // Number of generations to evolve
        private const double MutationRate = 0.1; // Mutation rate (10%)
        private const double EliteRate = 0.2; // Percentage of the population to keep as elite individuals

        public int FindBestMove(Player?[,] board, Player currentPlayer)
        {
            // Initialize population
            List<int> population = InitializePopulation(PopulationSize);

            // Evolve the population
            for (int generation = 0; generation < NumGenerations; generation++)
            {
                // Evaluate the fitness of the population
                List<(int move, int fitness)> fitnessList = EvaluatePopulation(population, board, currentPlayer);

                // Select parents based on fitness
                List<int> parents = SelectParents(fitnessList);

                // Generate a new population
                List<int> newPopulation = GenerateNewPopulation(parents);

                // Mutate the new population
                MutatePopulation(newPopulation);

                // Replace old population with the new one
                population = newPopulation;
            }

            // Evaluate the final population to find the best move
            List<(int move, int fitness)> finalFitnessList = EvaluatePopulation(population, board, currentPlayer);
            (int bestMove, _) = finalFitnessList.OrderByDescending(f => f.fitness).First();

            return bestMove;
        }

        private List<int> InitializePopulation(int size)
        {
            List<int> population = new List<int>();
            Random random = new Random();

            for (int i = 0; i < size; i++)
            {
                // Generate a random move (column) for each individual
                int randomMove = random.Next(0, 7);
                population.Add(randomMove);
            }

            return population;
        }

        private List<(int move, int fitness)> EvaluatePopulation(List<int> population, Player?[,] board, Player currentPlayer)
        {
            List<(int move, int fitness)> fitnessList = new List<(int move, int fitness)>();

            foreach (int move in population)
            {
                if (!HelperMethods.IsColumnFull(board, move))
                {
                    // Make a move
                    Player?[,] newBoard = HelperMethods.MakeMove(board, move, currentPlayer);

                    // Calculate fitness (you can use your existing evaluation function)
                    int fitness = EvaluateBoard(newBoard, currentPlayer);

                    fitnessList.Add((move, fitness));
                }
                else
                {
                    // If the move is invalid (column full), assign a low fitness value
                    fitnessList.Add((move, int.MinValue));
                }
            }

            return fitnessList;
        }

        private List<int> SelectParents(List<(int move, int fitness)> fitnessList)
        {
            // Sort the fitness list in descending order by fitness
            fitnessList = fitnessList.OrderByDescending(f => f.fitness).ToList();

            // Keep a percentage of elite individuals
            int eliteCount = (int)(EliteRate * PopulationSize);
            List<int> parents = fitnessList.Take(eliteCount).Select(f => f.move).ToList();

            // Use roulette wheel selection to fill the remaining parents
            int remainingCount = PopulationSize - eliteCount;
            int totalFitness = fitnessList.Sum(f => f.fitness);
            Random random = new Random();

            for (int i = 0; i < remainingCount; i++)
            {
                int selectionPoint = random.Next(0, totalFitness);
                int currentSum = 0;

                foreach ((int move, int fitness) in fitnessList)
                {
                    currentSum += fitness;
                    if (currentSum >= selectionPoint)
                    {
                        parents.Add(move);
                        break;
                    }
                }
            }

            return parents;
        }

        private List<int> GenerateNewPopulation(List<int> parents)
        {
            List<int> newPopulation = new List<int>();
            Random random = new Random();

            // Use crossover to generate new individuals
            for (int i = 0; i < parents.Count - 1; i += 2)
            {
                int parent1 = parents[i];
                int parent2 = parents[i + 1];

                // Combine moves from the two parents
                int childMove = (parent1 + parent2) / 2; // Simple crossover (average of parents)

                newPopulation.Add(childMove);
            }

            return newPopulation;
        }

        private void MutatePopulation(List<int> population)
        {
            Random random = new Random();

            // Mutate each individual in the population based on the mutation rate
            for (int i = 0; i < population.Count; i++)
            {
                if (random.NextDouble() < MutationRate)
                {
                    // Introduce a random mutation in the move (column)
                    population[i] = random.Next(0, 7);
                }
            }
        }

        private int EvaluateBoard(Player?[,] board, Player currentPlayer)
        {
            // Implement your evaluation function here
            // Calculate the fitness score based on the current game state
            return 0; // Placeholder: replace with your fitness calculation logic
        }
    }
}
