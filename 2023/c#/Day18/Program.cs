using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Utilities;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day18.txt");

        List<string> lines = FileHandling.ReadInputFile(path);

        Console.WriteLine($"File Input Line Count: {lines.Count}");

        // Part 1
        Solve_Part_1(lines);

        Console.WriteLine();

        // Testing
        Part_1_Test();
        Part_2_Test();
    }

    private static ulong Solve_Part_1(List<string> input) {

        char[,] grid = GetGrid(input);

        //PrintGrid(grid);
        //Console.WriteLine();
        flood_fill_map(grid, new Point(253, 151), '.', '#' );
        //PrintGrid(grid);

        ulong count = CountCubicMetres(grid);
        //Console.WriteLine($"Part 1: {count}");  // 5864 -- Too Low
        Console.WriteLine($"Part 1: {count}");  // 108909 -- Correct!

        return 0;
    }

    private static char[,] GetGrid(List<string> input) {

        int currentX = 0;
        int currentY = 0;
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        foreach(string line in input) {

            var parts = line.Split(' ');
            string direction = parts[0].Trim();
            int distance = int.Parse(parts[1].Trim());
            string color = parts[2].Trim().Replace("(", "").Replace(")", "");

            switch (direction) {
                case "R":
                    currentX += distance;
                    break;
                case "L":
                    currentX -= distance;
                    break;
                case "U":
                    currentY -= distance;
                    break;
                case "D":
                    currentY += distance;
                    break;
            }
            
            minX = Math.Min(minX, currentX);
            minY = Math.Min(minY, currentY);

            maxX = Math.Max(maxX, currentX);
            maxY = Math.Max(maxY, currentY);
        }

        Console.WriteLine($"MinX: {minX}, MaxX: {maxX}, MinY: {minY}, MaxY: {maxY}");
        
        int width = Math.Abs(minX) + Math.Abs(maxX) + 1; 
        int height = Math.Abs(minY) + Math.Abs(maxY) + 1;

        Console.WriteLine($"Width: {width}, Height: {height}");

        char[,] grid = new char[width, height];

        for (int y = 0; y < height; y++) {
            string line = input[y];
            for (int x = 0; x < width; x++) {
                grid[x, y] = '.';
            }
        }

        MapPath(grid, Math.Abs(minX), Math.Abs(minY), input);

        return grid;
    }

    private static void PrintGrid(char[,] grid) {

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int y = 0; y < height; y++) {
            string line = "";
            for (int x = 0; x < width; x++) {
                line += grid[x, y];
            }
            Console.WriteLine(line);
        }
    }

    private static void MapPath(char[,] grid, int startX, int startY, List<string> input) {

        int currentX = startX;
        int currentY = startY;
        char dig = '#';

        // Dig the first hole
        grid[currentX, currentY] = dig;

        foreach (string line in input) {

            var parts = line.Split(' ');
            string direction = parts[0].Trim();
            int distance = int.Parse(parts[1].Trim());
            string color = parts[2].Trim().Replace("(", "").Replace(")", "");

            switch (direction) {
                case "R":
                    for (int i = 0; i < distance; i++) {
                        currentX++;
                        grid[currentX, currentY] = dig;
                    }
                    break;
                case "L":
                    for (int i = 0; i < distance; i++) {
                        currentX--;
                        grid[currentX, currentY] = dig;
                    }
                    break;
                case "U":
                    for (int i = 0; i < distance; i++) {
                        currentY--;
                        grid[currentX, currentY] = dig;
                    }
                    break;
                case "D":
                    for (int i = 0; i < distance; i++) {
                        currentY++;
                        grid[currentX, currentY] = dig;
                    }
                    break;
            }
        }
    }

    private static void flood_fill_map(char[,] map, Point start_point, char target_char, char fill_char)
    {
        int max_x = map.GetLength(1);
        int max_y = map.GetLength(0);

        Stack<Point> pixels = new Stack<Point>();  // Helps to think in terms of "pixels"

        pixels.Push(start_point);

        while(pixels.Count > 0)
        {
            var point = pixels.Pop();

            if(point.Y == 6 && point.X == 3) {
                //Console.WriteLine($"Point: {point.X}, {point.Y}");
            }

            // TODO: Check whether we should be looking at >= instead of > here
            if(point.X < max_x && point.X > 0 && point.Y < max_y && point.Y > 0)
            {
                // We are still in the map
                if (map[point.Y, point.X] == target_char)
                {
                    map[point.Y, point.X] = fill_char;

                    if (point.X > 0) pixels.Push(new Point(point.X - 1, point.Y));
                    if (point.X < max_x - 1) pixels.Push(new Point(point.X + 1, point.Y));
                    if (point.Y > 0) pixels.Push(new Point(point.X, point.Y - 1));
                    if (point.Y < max_y - 1) pixels.Push(new Point(point.X, point.Y + 1));
                }              

                // TODO: Should we worry about diagonals?
            }

        }
    }

    private static ulong CountCubicMetres(char[,] map) {
        ulong count = 0;

        int max_x = map.GetLength(1);
        int max_y = map.GetLength(0);

        for (int y = 0; y < max_y; y++) {
            for (int x = 0; x < max_x; x++) {
                if (map[y, x] == '#') {
                    count++;
                }
            }
        }

        return count;
    }

    private static void Part_1_Test() {
        List<string> input = new List<string>() {
            "R 6 (#70c710)",
            "D 5 (#0dc571)",
            "L 2 (#5713f0)",
            "D 2 (#d2c081)",
            "R 2 (#59c680)",
            "D 2 (#411b91)",
            "L 5 (#8ceee2)",
            "U 2 (#caa173)",
            "L 1 (#1b58a2)",
            "U 2 (#caa171)",
            "R 2 (#7807d2)",
            "U 3 (#a77fa3)",
            "L 2 (#015232)",
            "U 2 (#7a21e3)",
        };

        char[,] grid = GetGrid(input);
        //PrintGrid(grid);
        Console.WriteLine();
        flood_fill_map(grid, new Point(1, 1), '.', '#' );
        //PrintGrid(grid);

        ulong count = CountCubicMetres(grid);
        Console.WriteLine($"Count: {count}");

    }

    private static void Part_2_Test() {
        List<string> input = new List<string>() {
            "R 6 (#70c710)",
            "D 5 (#0dc571)",
            "L 2 (#5713f0)",
            "D 2 (#d2c081)",
            "R 2 (#59c680)",
            "D 2 (#411b91)",
            "L 5 (#8ceee2)",
            "U 2 (#caa173)",
            "L 1 (#1b58a2)",
            "U 2 (#caa171)",
            "R 2 (#7807d2)",
            "U 3 (#a77fa3)",
            "L 2 (#015232)",
            "U 2 (#7a21e3)",
        };

        List<string> corrected_input = CorrectInput(input);

        char[,] grid = GetGrid(corrected_input);
        //PrintGrid(grid);
        Console.WriteLine();
        flood_fill_map(grid, new Point(1, 1), '.', '#' );
        //PrintGrid(grid);

        ulong count = CountCubicMetres(grid);
        Console.WriteLine($"Part 2 Count: {count}");

    }

    private static List<string> CorrectInput(List<string> input) {
        List<string> corrected_input = new List<string>();

        foreach (string line in input) {
            var parts = line.Split(' ');

            string color = parts[2].Trim().Replace("(", "").Replace(")", "").Replace("#", "");

            string coded_distance = color.Substring(0, 5);
            string coded_direction  = color.Substring(5, 1);

            int distance = int.Parse(coded_distance, System.Globalization.NumberStyles.HexNumber);
            string direction = "";
            switch (coded_direction) {
                case "0":
                    direction = "R";
                    break;
                case "1":
                    direction = "D";
                    break;
                case "2":
                    direction = "L";
                    break;
                case "3":
                    direction = "U";
                    break;
            }
            corrected_input.Add($"{direction} {distance} {color}");
        }

        return corrected_input;
    }
}