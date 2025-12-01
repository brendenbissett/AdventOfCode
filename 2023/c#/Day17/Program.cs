using System.Collections;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Utilities;
using static System.Reflection.Metadata.BlobBuilder;

internal class Program
{

    #nullable enable

    class Node
    {
        public int row;
        public int column;
        public int heatLoss;
        public bool visited;
        public ulong distance; // Distance from start node
        public Node from; // Previous node in the path

        public Node(int row, int column, int heatLoss, ulong distance, Node from)
        {
            this.row = row;
            this.column = column;
            this.heatLoss = heatLoss;
            this.distance = distance;
            this.visited = false;
            this.from = from;
        }

    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day17.txt");

        List<string> lines = FileHandling.ReadInputFile(path);

        var graph = WorkWithArrays.ConvertListTo2DInt_StaggeredArray(lines);

        Console.WriteLine($"File Input Line Count: {lines.Count}");

        // Part 1
        //---------------------------
        /*
        int maxRow = graph.GetLength(0);
        int maxColumn = graph.GetLength(1);

        var startNode = new Tuple<int, int>(0, 0);
        var targetNode = new Tuple<int, int>(maxRow-1, maxColumn-1);

        ulong distance = Solve_Part_1(graph, startNode, targetNode);

        Console.WriteLine($"Part 1: {distance}"); // 1075 -- Too High
        */

        // Part 2 - TODO

        // Tests
        //---------------------------
        Part_1_Test();
    }

    private static Node[,] ConvertToNodeArray(int[,] graph)
    {
        int maxRow = graph.GetLength(0);
        int maxColumn = graph.GetLength(1);

        Node[,] result = new Node[maxRow, maxColumn];

        for (int row = 0; row < maxRow; row++)
        {
            for (int column = 0; column < maxColumn; column++)
            {
                int heatLoss = graph[row, column];
                result[row, column] = new Node(row, column, heatLoss, 0, null);
            }
        }

        return result;
    }

    private static ulong Solve_Part_1(int[,] graph, Tuple<int, int> startNode, Tuple<int, int> targetNode)
    {
        // Start Top Left, End Bottom Right
        // Don't incur the heat loss penalty for starting position. (Will incur if you re-visit the starting position)
        // Each block has a heat loss penalty
        // You can move at most three blocks in a single direction before you must turn 90 degrees, either left or right.
        // You cannot reverse direction, IE, you can only turn left, right , or continue straight.
        // Return the least amount of heat loss that can be incurred to reach the bottom right corner.

        // Current research shows a couple options: (https://www.graphable.ai/blog/pathfinding-algorithms/#:~:text=Pathfinding%20algorithms%20are%20a%20crucial,efficient%20path%20between%20two%20points.)
        // 1. Dijkstra's Algorithm  -- Probably the best option, since we have heat loss penalties, and we need to find the shortest, least loss path.
        // 2. A* Algorithm

        // Dijkstra's Algorithm (BUT we need to consider that we are not allowed to move more than 3 blocks in a single direction before turning)

        // 1. Create a list of all vertices, with their heat loss penalty, and their distance from the start node.
        // 2. Set the distance of the start node to 0, and all other nodes to infinity.
        // 3. Set the start node as the current node.
        // 4. For the current node, consider all of its unvisited neighbors and calculate their tentative distances through the current node.
        // 5. When done considering all of the neighbors of the current node, mark the current node as visited and remove it from the unvisited set.
        // 6. If the destination node has been marked visited, then stop. The algorithm has finished.
    
        int maxRow = graph.GetLength(0);
        int maxColumn = graph.GetLength(1);

        var grid = ConvertToNodeArray(graph);

        List<Node> backlog = new List<Node>();

        // Add starting node to the backlog for processing
        backlog.Add(grid[startNode.Item1, startNode.Item2]);

        // Keep going until we've processed all nodes or we've reached the target node
        while (backlog.Count > 0)
        {
            // Need to ensure the queue is sorted by distance from the start node
            backlog.Sort((x, y) => x.distance.CompareTo(y.distance)); 
            //backlog = backlog.OrderBy(v => v.heatLoss).ThenBy(v => GetDistanceToTarget(v, targetNode)).ToList();
            //backlog = backlog.OrderBy(v => v.distance).ToList();
        

            // Get the next node to process
            Node current = backlog[0];

            // Get the neighbors of the current node
            List<string> neighbors = GetOrderedNeighbors(grid, current, maxRow, maxColumn, targetNode);

            if (current.row == 3 && current.column == 10)
            {
                Console.WriteLine("DEBUG");
            }

            // Add the neighbors to the queue
            foreach (var neighbor in neighbors)
            {
                // Get the neighbor node
                var n = grid[int.Parse(neighbor.Split(',')[0]), int.Parse(neighbor.Split(',')[1])];

                // Will traversing this node cause us to exceed the max number of blocks in a single direction?
                if (WillExceedMaxBlocks(current, n))
                {
                    // Yes, so discard this neigbour
                    continue;
                }

                if(n.visited)
                {
                    // Already visited, Consider whether we need to update the distance if we've found a shorter path
                    if (n.distance >= current.distance + (ulong)n.heatLoss)
                    {
                        n.from = current;
                        n.distance = current.distance + (ulong)n.heatLoss;
                        backlog.Add(n);
                    }
                    continue;
                }

                n.visited = true;
                n.from = current;
                n.distance = current.distance + (ulong)n.heatLoss;

                // Have we hit our target?
                if (n.row == targetNode.Item1 && n.column == targetNode.Item2)
                {
                    Console.WriteLine($"Found target node: {n.row},{n.column}");
                    VisualizePath(n, grid);
                    // We've reached the target node, so return the distance
                    return n.distance;
                }

                // Add the neighbor to the backlog for processing
                backlog.Add(n);
            }

            // Done with current, so discard
            backlog.Remove(current);
        }

        return 0;
    }

    private static bool WillExceedMaxBlocks(Node current, Node neighbor)
    {
        // Will moving to this neighbor cause us to exceed the max number of blocks in a single direction?
        if(current.from == null) return false;
        if(current.from.from == null) return false;
        if(current.from.from.from == null) return false;
        if(current.from.from.from.from == null) return false; // TODO???? 

        // Is current node on the same row as the neighbor?
        if(neighbor.row == current.row)
        {
            // 1
            if(current.row == current.from.row)
            {
                // 2
                if(current.from.row == current.from.from.row) 
                {    
                    // 3
                    if(current.from.from.row == current.from.from.from.row) 
                    {       
                        // 4
                        return true;
                    }
                }
            }
        }
        else if(neighbor.column == current.column)
        {
            // 1
            if (current.column == current.from.column)
            {
                // 2
                if (current.from.column == current.from.from.column)
                {
                    // 3
                    if(current.from.from.column == current.from.from.from.column) 
                    {
                        // 4
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static void VisualizePath(Node node, Node[,] grid) {
        int maxRow = grid.GetLength(0);
        int maxColumn = grid.GetLength(1);

        // Walk the path
        Dictionary<string, Node> path = new Dictionary<string, Node>();
        while(node.from != null) {
            path.Add($"{node.row},{node.column}", node);
            node = node.from;
        }

        // Print the grid
        for (int row = 0; row < maxRow; row++)
        {
            for (int column = 0; column < maxColumn; column++)
            {

                var key = $"{row},{column}";

                if (path.ContainsKey(key))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    //Console.Write("#");
                    Console.Write(grid[row, column].heatLoss);
                    Console.ResetColor();
                }
                else {
                    Console.Write(grid[row, column].heatLoss);
                }
            }
            Console.WriteLine();
        }
    }

    private static List<string> GetOrderedNeighbors(Node[,] graph, Node current, int maxRow, int maxColumn, Tuple<int, int> targetNode)
    {
        List<Node> neighbors = new List<Node>();
        int fromRow = current.from?.row ?? -100;
        int fromColumn = current.from?.column ?? -100;

        // Check up
        if (current.row > 0)
        {
            // Ignore if I came from this node
            if (!(fromRow == current.row - 1 && fromColumn == current.column))
            {
                neighbors.Add(graph[current.row - 1, current.column]);
            }
        }

        // Check down
        if (current.row < maxRow - 1)
        {
            // Ignore if I came from this node
            if (!(fromRow == current.row + 1 && fromColumn == current.column))
            {
                neighbors.Add(graph[current.row + 1, current.column]);
            }
        }

        // Check left
        if (current.column > 0)
        {
            // Ignore if I came from this node
            if (!(fromRow == current.row && fromColumn == current.column - 1))
            {
                neighbors.Add(graph[current.row, current.column - 1]);
            }
        }

        // Check right
        if (current.column < maxColumn - 1)
        {
            // Ignore if I came from this node
            if (!(fromRow == current.row && fromColumn == current.column + 1))
            {
                neighbors.Add(graph[current.row, current.column + 1]);
            }

        }

        // Need to sort the neigbours by heat loss, and distance to target.
        //neighbors.Sort((x, y) => x.heatLoss.CompareTo(y.heatLoss)); 
        if (current.row == 3 && current.column == 10)
        {
            Console.WriteLine("DEBUG");
        }

        var sorted = neighbors.OrderBy(v => v.heatLoss).ThenBy(v => GetDistanceToTarget(v, targetNode)).ToList();
        
        return sorted.Select(v => $"{v.row},{v.column}").ToList();
    }

    private static ulong GetDistanceToTarget(Node node, Tuple<int, int> targetNode)
    {
        return (ulong)(Math.Abs(node.row - targetNode.Item1) + Math.Abs(node.column - targetNode.Item2));
    }

    private static void Part_1_Test()
    {
        List<string> input = new List<string>()
        {
            "2413432311323",
            "3215453535623",
            "3255245654254",
            "3446585845452",
            "4546657867536",
            "1438598798454",
            "4457876987766",
            "3637877979653",
            "4654967986887",
            "4564679986453",
            "1224686865563",
            "2546548887735",
            "4322674655533",
        };

        var graph = WorkWithArrays.ConvertListTo2DInt_StaggeredArray(input);

        int maxRow = graph.GetLength(0);
        int maxColumn = graph.GetLength(1);

        var startNode = new Tuple<int, int>(0, 0);
        var targetNode = new Tuple<int, int>(maxRow-1, maxColumn-1);

        ulong distance = Solve_Part_1(graph, startNode, targetNode);

        Console.WriteLine($"Test 1: {distance}"); 
    }
}