using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using Utilities;
using CommunityToolkit.HighPerformance;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day14.txt");

        char[,] grid = WorkWithArrays.ConvertListTo2DChar_StaggeredArray(FileHandling.ReadInputFile(path));

        // Part 1
        // ---------------------------------------
        //roll_rocks_directional(grid, 'N');

        //ulong load = calculate_the_load(grid);
        //Console.WriteLine($"part_1_answer = {load}"); // 106997 --> Correct Answer!


        // Part 2
        // ---------------------------------------
        /*
        ulong cycles = 1000000000;
        ulong update_every_n_cycles = 1000000;

        for (ulong i = 0; i < cycles; i++)
        {
            // Full Cycle
            roll_rocks_directional(grid, 'N');
            roll_rocks_directional(grid, 'W');
            roll_rocks_directional(grid, 'S');
            roll_rocks_directional(grid, 'E');

            if (i % update_every_n_cycles == 0)
            {
                Console.WriteLine($"{((double)i / (double)cycles) * 100}% {i} / {cycles} cycles complete");
            }
        }

        ulong part_2_load = calculate_the_load(grid);
        Console.WriteLine($"part_2_test: Load = {part_2_load}");
        */

        // Testing
        //part_1_test();
        part_2_test();
    }

    private static void roll_rocks_directional(char[,] grid, int direction)
    {
        // Directions
        // 1. North
        // 2. East
        // 3. South
        // 4. West

        Span2D<char> span = grid;

        if(direction == 'N')
        {
            for(int i = 0; i < span.Width; i++) 
            {

                var items = span.GetColumn(i);

                int total_rolling_rocks = items.ToArray().Count(c => c == 'O');
               

                int rolling_rocks = 0;
                Queue<int> free_slots = new Queue<int>();

                for (int j = 0; j < items.Length; j++)
                {
                    if (rolling_rocks >= total_rolling_rocks) break; // No rolling rocks left

                    switch (items[j])
                    {
                        case 'O':
                            if (free_slots.Count > 0)  // If not slots, then the rolling rock is already in the right spot
                            {
                                // Move the rock to the free slot
                                int free_slot = free_slots.Dequeue();
                                items[free_slot] = 'O';
                                items[j] = '.';
                                free_slots.Enqueue(j);
                            }
                            rolling_rocks++;
                            break; // Can just keep going
                        case '.':
                            free_slots.Enqueue(j);  // Keep track of free index
                            break;
                        case '#':
                            free_slots.Clear(); // Reset. We hit an immovable rock.
                            break;
                        default:
                            break;

                    }
                }
            }

        } 

        if(direction == 'E'){
            for(int i = 0; i < span.Height; i++) 
            {

                var items = span.GetRow(i);

                int total_rolling_rocks = items.ToArray().Count(c => c == 'O');
                int rolling_rocks = 0;
                Queue<int> free_slots = new Queue<int>();

                for (int j = items.Length - 1; j >= 0; j--)
                {
                    if (rolling_rocks >= total_rolling_rocks) break; // No rolling rocks left

                    switch (items[j])
                    {
                        case 'O':
                            if (free_slots.Count > 0)  // If not slots, then the rolling rock is already in the right spot
                            {
                                // Move the rock to the free slot
                                int free_slot = free_slots.Dequeue();
                                items[free_slot] = 'O';
                                items[j] = '.';
                                free_slots.Enqueue(j);
                            }
                            rolling_rocks++;
                            break; // Can just keep going
                        case '.':
                            free_slots.Enqueue(j);  // Keep track of free index
                            break;
                        case '#':
                            free_slots.Clear(); // Reset. We hit an immovable rock.
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        if(direction == 'S') {
            for(int i = 0; i < span.Width; i++) 
            {

                var items = span.GetColumn(i);

                int total_rolling_rocks = items.ToArray().Count(c => c == 'O');
                int rolling_rocks = 0;
                Queue<int> free_slots = new Queue<int>();

                for (int j = items.Length - 1; j >= 0; j--)
                {
                    if (rolling_rocks >= total_rolling_rocks) break; // No rolling rocks left

                    switch (items[j])
                    {
                        case 'O':
                            if (free_slots.Count > 0)  // If not slots, then the rolling rock is already in the right spot
                            {
                                // Move the rock to the free slot
                                int free_slot = free_slots.Dequeue();
                                items[free_slot] = 'O';
                                items[j] = '.';
                                free_slots.Enqueue(j);
                            }
                            rolling_rocks++;
                            break; // Can just keep going
                        case '.':
                            free_slots.Enqueue(j);  // Keep track of free index
                            break;
                        case '#':
                            free_slots.Clear(); // Reset. We hit an immovable rock.
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        if(direction == 'W') {
            for(int i = 0; i < span.Height; i++) 
            {

                var items = span.GetRow(i);

                int total_rolling_rocks = items.ToArray().Count(c => c == 'O');
                int rolling_rocks = 0;
                Queue<int> free_slots = new Queue<int>();

                for (int j = 0; j < items.Length; j++)
                {
                    if (rolling_rocks >= total_rolling_rocks) break; // No rolling rocks left

                    switch (items[j])
                    {
                        case 'O':
                            if (free_slots.Count > 0)  // If not slots, then the rolling rock is already in the right spot
                            {
                                // Move the rock to the free slot
                                int free_slot = free_slots.Dequeue();
                                items[free_slot] = 'O';
                                items[j] = '.';
                                free_slots.Enqueue(j);
                            }
                            rolling_rocks++;
                            break; // Can just keep going
                        case '.':
                            free_slots.Enqueue(j);  // Keep track of free index
                            break;
                        case '#':
                            free_slots.Clear(); // Reset. We hit an immovable rock.
                            break;
                        default:
                            break;

                    }
                }
            }
        }


    }

    private static void RollRocksDirectional_Parallel(char[,] grid, int direction)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        Parallel.For(0, (direction == 'N' || direction == 'S') ? cols : rows, i =>
        {
            int totalRollingRocks;
            Queue<int> freeSlots;

            char[] items;
            if (direction == 'N' || direction == 'S')
            {
                items = new char[rows];
                for (int row = 0; row < rows; row++)
                    items[row] = (direction == 'N') ? grid[row, i] : grid[row, i];
            }
            else // East or West
            {
                items = new char[cols];
                for (int col = 0; col < cols; col++)
                    items[col] = (direction == 'E') ? grid[i, col] : grid[i, col];
            }

            totalRollingRocks = items.Count(c => c == 'O');
            freeSlots = new Queue<int>();

            int rollingRocks = 0;

            for (int j = 0; j < items.Length; j++)
            {
                if (rollingRocks >= totalRollingRocks) break; // No rolling rocks left

                switch (items[j])
                {
                    case 'O':
                        if (freeSlots.Count > 0)  // If not slots, then the rolling rock is already in the right spot
                        {
                            // Move the rock to the free slot
                            int freeSlot = freeSlots.Dequeue();
                            if (direction == 'N' || direction == 'S')
                                grid[freeSlot, i] = 'O';
                            else
                                grid[i, freeSlot] = 'O';

                            if (direction == 'N' || direction == 'E')
                                grid[j, i] = '.';
                            else
                                grid[i, j] = '.';

                            freeSlots.Enqueue(j);
                        }
                        rollingRocks++;
                        break;
                    case '.':
                        freeSlots.Enqueue(j);  // Keep track of free index
                        break;
                    case '#':
                        freeSlots.Clear(); // Reset. We hit an immovable rock.
                        break;
                    default:
                        break;
                }
            }
        });
    }


    private static ulong calculate_the_load(char[,] grid) {
            
            ulong total_load = 0;
    
            Span2D<char> span = grid;
    
            for (int i = 0; i < span.Width; i++)
            {
                var items = span.GetColumn(i);
    
                int total_rolling_rocks = items.ToArray().Count(c => c == 'O');
                int rolling_rocks = 0;
    
                for (int j = 0; j < items.Length; j++)
                {
                    if (rolling_rocks >= total_rolling_rocks) break; // No rolling rocks left
    
                    if (items[j] == 'O')
                    {
                        total_load += (ulong)(span.Width - j);
                        rolling_rocks++;
                    }
                }
            }
    
            return total_load;
    }

    private static void part_1_test()
    {
        List<string> input = new List<string>()
        {
            "O....#....",
            "O.OO#....#",
            ".....##...",
            "OO.#O....O",
            ".O.....O#.",
            "O.#..O.#.#",
            "..O..#O..O",
            ".......O..",
            "#....###..",
            "#OO..#...."
        };

        char[,] grid = WorkWithArrays.ConvertListTo2DChar_StaggeredArray(input);

        roll_rocks_directional(grid, 'N');

        ulong load = calculate_the_load(grid);
        Console.WriteLine($"part_1_test: Load = {load}");
    }

    private static void part_2_test()
    {
        List<string> input = new List<string>()
        {
            "O....#....",
            "O.OO#....#",
            ".....##...",
            "OO.#O....O",
            ".O.....O#.",
            "O.#..O.#.#",
            "..O..#O..O",
            ".......O..",
            "#....###..",
            "#OO..#...."
        };

        char[,] grid = WorkWithArrays.ConvertListTo2DChar_StaggeredArray(input);

        ulong cycles = 1000000000;
        ulong update_every_n_cycles = 1000000;
        ulong percent_base = cycles / update_every_n_cycles;

        for(ulong i = 0; i < cycles; i++)
        {
            // Full Cycle
            Parallel.Invoke(
                () => RollRocksDirectional_Parallel(grid, 'N'),
                () => RollRocksDirectional_Parallel(grid, 'W'),
                () => RollRocksDirectional_Parallel(grid, 'S'),
                () => RollRocksDirectional_Parallel(grid, 'E')
            );

            if (i % update_every_n_cycles == 0) {
                Console.WriteLine($"{((double)i / (double)cycles) * 100}% {i} / {cycles} cycles complete");
            }
        }
        
        ulong load = calculate_the_load(grid);
        Console.WriteLine($"part_2_test: Load = {load}");
    }

    private static void print_grid(char[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            Console.WriteLine(string.Join("", grid.GetRow(i).ToArray()));
        }
    }
}