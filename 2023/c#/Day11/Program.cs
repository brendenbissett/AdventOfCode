using Superpower;
using System.Collections.Generic;
using System.Diagnostics;
using Utilities;

internal class Program
{
    enum GalaxyObjectType
    {
        Space,
        Galaxy
    }

    class Point
    {
        public int x { get; set; }
        public int y { get; set; }

        public GalaxyObjectType type { get; set; }

        public Point(int x, int y, GalaxyObjectType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day11.txt");
        var lines = FileHandling.ReadInputFile(path);

        // Data
        List<List<char>> map = new List<List<char>>();

        foreach (var line in lines)
        {
            map.Add(line.ToArray().ToList());
        }

        // Part 1 
        List<Point> part_1_objects = Get_Points_In_Galaxy(map, 1);
        ulong part_1_result = Part_1_Solution(part_1_objects);
        //Console.WriteLine($"Part 1: {part_1_result}"); // 20155700 -- Too High
        Console.WriteLine($"Part 1: {part_1_result}"); // 10077850
        Console.WriteLine();


        // Part 2 
        List<Point> part_2_objects = Get_Points_In_Galaxy(map, 1000000);
        ulong part_2_result = Part_1_Solution(part_2_objects);
        Console.WriteLine($"Part 2: {part_2_result}");  // 504715068438
        Console.WriteLine();

        // Tests
        Part_1_Test();
        Part_2_Test_1();
        Part_2_Test_2();
    }

    private static List<Point> Get_Points_In_Galaxy(List<List<char>> map, int empty_space_multiple)
    {

        (List<int> empty_rows, List<int> empty_columns) = get_indexes_to_expand(map); // Part 2

        List<Point> objects = new List<Point>();

        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Count; x++)
            {
                if (map[y][x] != '.')
                {
                    int new_x = x;
                    int new_y = y;

                    if (empty_space_multiple != 1)
                    {
                        var col_count = empty_columns.Where(c => c < x).Count();
                        var row_count = empty_rows.Where(r => r < y).Count();

                        new_x = x + ((col_count * empty_space_multiple) - col_count);
                        new_y = y + ((row_count * empty_space_multiple) - row_count);
                    }

                    objects.Add(new Point(new_x, new_y, GalaxyObjectType.Galaxy));
                }

            }
        }

        return objects;
    }

    private static ulong Part_1_Solution(List<Point> objects)
    {

        // Get possible pairs
        List<Tuple<Point, Point>> pairs = new List<Tuple<Point, Point>>();

        for (int I = 0; I < objects.Count; ++I)
            for (int j = I + 1; j < objects.Count; ++j)
                pairs.Add(new Tuple<Point, Point>(objects[I], objects[j]));


        // Iterate through each possible combination and calculate the distance between each point.
        ulong total_distance = 0;
        //Console.WriteLine($"Possible Pairs: {pairs.Count}");

        foreach (var combination in pairs)
        {
            total_distance += (ulong)get_shortest_steps(combination.Item1.x, combination.Item1.y, combination.Item2.x, combination.Item2.y);
        }

        // Return the total distance for each combination.
        return total_distance;
    }

    private static (List<int> rows, List<int> columns) get_indexes_to_expand(List<List<char>> map)
    {
        // Expand rows
        List<int> empty_row_indexes = new List<int>();
        for (int i = 0; i < map.Count; i++)
        {
            if (map[i].Where(x => x != '.').Count() == 0)
            {
                // Row is only space, so expand it.
                empty_row_indexes.Add(i);
            }
        }

        // Expand columns
        List<int> empty_column_indexes = new List<int>();
        for (int c = 0; c < map[0].Count; c++)
        {
            List<char> column = new List<char>();
            for(int r = 0; r < map.Count; r++)
            {
                column.Add(map[r][c]); 
            }

            if (column.Where(x => x != '.').Count() == 0)
            {
                // Column is only space, so expand it.
                empty_column_indexes.Add(c);
            }
        }

        return (empty_row_indexes, empty_column_indexes);
    }

    private static int get_shortest_steps(int x, int y, int destination_x, int destination_y)
    {
        int x_distance = Math.Abs(x - destination_x);
        int y_distance = Math.Abs(y - destination_y);

        return x_distance + y_distance;
    }

    private static void Part_1_Test()
    {
        List<List<char>> input = new List<List<char>> {
            "....#........".ToArray().ToList(),
            ".........#...".ToArray().ToList(),
            "#............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            "........#....".ToArray().ToList(),
            ".#...........".ToArray().ToList(),
            "............#".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".........#...".ToArray().ToList(),
            "#....#.......".ToArray().ToList(),
        };

        var objects = Get_Points_In_Galaxy(input, 1);

        ulong total_distance = Part_1_Solution(objects);
        Console.WriteLine($"Part 1 Test: {total_distance}");
        Console.WriteLine();
    }

    private static void Part_2_Test_1()
    {
        List<List<char>> input = new List<List<char>> {
            "....#........".ToArray().ToList(),
            ".........#...".ToArray().ToList(),
            "#............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            "........#....".ToArray().ToList(),
            ".#...........".ToArray().ToList(),
            "............#".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".........#...".ToArray().ToList(),
            "#....#.......".ToArray().ToList(),
        };

        var objects = Get_Points_In_Galaxy(input, 5); // NB: The advent calendar says expand by 10, would give sum of 1030, but should only expand by 5 to get that result.

        ulong total_distance = Part_1_Solution(objects);
        Console.WriteLine($"Part 2 Test (1): {total_distance}");
        Console.WriteLine();
    }

    private static void Part_2_Test_2()
    {
        List<List<char>> input = new List<List<char>> {
            "....#........".ToArray().ToList(),
            ".........#...".ToArray().ToList(),
            "#............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            "........#....".ToArray().ToList(),
            ".#...........".ToArray().ToList(),
            "............#".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".............".ToArray().ToList(),
            ".........#...".ToArray().ToList(),
            "#....#.......".ToArray().ToList(),
        };

        var objects = Get_Points_In_Galaxy(input, 50);  // NB: The advent calendar says expand by 100, would give sum of 8410, but should only expand by 50 to get that result.

        ulong total_distance = Part_1_Solution(objects);
        Console.WriteLine($"Part 2 Test (2): {total_distance}");
        Console.WriteLine();
    }
}