using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using Utilities;

internal class Program
{
    class JourneyStart
    {
        public Tuple<int,int> Start { get; set; }
        public Tuple<int, int> Current { get; set; }

        public char Direction { get; set; }
        public bool Complete { get; set; }

        public Dictionary<string, int> Journey { get; set; }

        public JourneyStart(Tuple<int, int> start, char direction)
        {
            this.Start = start;
            this.Current = start;
            this.Direction = direction;
            this.Complete = false;
            this.Journey = new Dictionary<string, int>();
            this.Journey.Add($"{start.Item1},{start.Item2}", 1);
        }

        public void Complete_Journey()
        {
            this.Complete = true;
        }

        public void Move_To(int row, int column, char direction)
        {
            this.Current = new Tuple<int, int>(row, column);
            this.Direction = direction;

            // Need to make sure we are not looping back on ourselves
            var key = $"{row},{column}";
            if (this.Journey.ContainsKey(key))
            {
                this.Journey[key]++;
            }
            else
            {
                this.Journey.Add(key, 1);
            }
            
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day16.txt");

        List<string> lines = FileHandling.ReadInputFile(path);

        Console.WriteLine($"File Input Line Count: {lines.Count}");
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("PART - 1");

        char[,] grid = WorkWithArrays.ConvertListTo2DChar_StaggeredArray(lines);
        ulong energized_part_1 = Solve_Part_1(grid);
        Console.WriteLine($"Part 1: {energized_part_1}");  // 7939 -- Correct

        ulong energized_part_2 = Solve_Part_2(grid);
        Console.WriteLine($"Part 2: {energized_part_2}");  // 8318 -- Correct

        Console.WriteLine("-----------------------------------");
        Console.WriteLine("Tests");
        Part_1_Test();  // 46 -- Correct
        Part_2_Test();  // 51 -- Correct
    }

    private static ulong Solve_Part_2(char[,] grid)
    {
        ulong answer = 0;

        int max_x = grid.GetLength(0) - 1;
        int max_y = grid.GetLength(1) - 1;

        // Left to Right
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            var starting_point = new Tuple<int, int>(i, 0);
            var journey = Map_Journey(starting_point, 'R', grid);

            if ((ulong)journey.Count > answer)
            {
                answer = (ulong)journey.Count;
            }
        }

        // Right to Left
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            var starting_point = new Tuple<int, int>(i, max_y - 1);
            var journey = Map_Journey(starting_point, 'L', grid);

            if ((ulong)journey.Count > answer)
            {
                answer = (ulong)journey.Count;
            }
        }

        // Top to Bottom
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            var starting_point = new Tuple<int, int>(0, i);
            var journey = Map_Journey(starting_point, 'D', grid);

            if ((ulong)journey.Count > answer)
            {
                answer = (ulong)journey.Count;
            }
        }

        // Bottom to Top
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            var starting_point = new Tuple<int, int>(max_x - 1, i);
            var journey = Map_Journey(starting_point, 'U', grid);

            if ((ulong)journey.Count > answer)
            {
                answer = (ulong)journey.Count;
            }
        }

        return answer;
    }

    private static ulong Solve_Part_1(char[,] grid)
    {

        Dictionary<string, ulong> journey = Map_Journey(new Tuple<int, int> (0, 0), 'R', grid);

        return (ulong)journey.Count;
    }

    private static Dictionary<string, ulong> Map_Journey(Tuple<int, int> start, char direction, char[,] grid)
    {
        Dictionary<string, ulong> journal = new Dictionary<string, ulong>();  // Key "{row},{column}"
        Dictionary<string, int> splits_Reached = new Dictionary<string, int>(); // Key "{row},{column}_{direction}"

        int max_x = grid.GetLength(0) - 1;
        int max_y = grid.GetLength(1) - 1;

        bool journey_complete = false;

        Stack<JourneyStart> adventures = new Stack<JourneyStart>();
        adventures.Push(new JourneyStart(start, direction));

        JourneyStart current_adventure = adventures.Pop();

        while(!journey_complete)
        {
            // Get next adventure if current is complete
            if (current_adventure.Complete)
            {
                if(adventures.Count == 0)
                {
                    journey_complete = true;
                    continue;
                }
                else
                {
                    current_adventure = adventures.Pop();
                }
            }

            var current = grid[current_adventure.Current.Item1, current_adventure.Current.Item2];

            string key = $"{current_adventure.Current.Item1},{current_adventure.Current.Item2}";
            if (journal.ContainsKey(key))
            {
                if(current_adventure.Start != current_adventure.Current)
                {
                    journal[key]++;  // Only increment if we are not at the start
                }
            }
            else
            {
                journal.Add(key, 1);
            }


            if (current_adventure.Complete)
            {
                continue;
            }

            switch (current)
            {
                case '|':

                    switch (current_adventure.Direction)
                    {
                        case 'U':
                            // Make sure we haven't hit the top of the grid
                            if (current_adventure.Current.Item1 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1-1, current_adventure.Current.Item2, 'U');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'D':
                            // Make sure we haven't hit the bottom of the grid
                            if (current_adventure.Current.Item1 < max_y)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1+1, current_adventure.Current.Item2, 'D');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'L':
                        case 'R':

                            var split_key = $"{current_adventure.Direction}_{current_adventure.Current.Item1},{current_adventure.Current.Item2}_U";

                            if (splits_Reached.ContainsKey(split_key))
                            {
                                current_adventure.Complete_Journey();
                                continue;
                            }
                            splits_Reached.Add(split_key, 1);

                            // Need to split the beam (add a new adventure)
                            adventures.Push(new JourneyStart(current_adventure.Current, 'U'));

                            // Continue down
                            // Make sure we haven't hit the bottom of the grid
                            if (current_adventure.Current.Item1 < max_y)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 + 1, current_adventure.Current.Item2, 'D');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;
                        default:
                            break;
                    }

                    break
                        ;
                case '-':
                    switch(current_adventure.Direction)
                    {
                        case 'L':
                            // Make sure we haven't hit the left of the grid
                            if (current_adventure.Current.Item2 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2-1, 'L');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'R':
                            // Make sure we haven't hit the right of the grid
                            if (current_adventure.Current.Item2 < max_x)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2+1, 'R');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'U':
                        case 'D':

                            var split_key = $"{current_adventure.Direction}_{current_adventure.Current.Item1},{current_adventure.Current.Item2}_R";

                            if (splits_Reached.ContainsKey(split_key))
                            {
                                current_adventure.Complete_Journey();
                                continue;
                            }
                            splits_Reached.Add(split_key, 1);

                            // Need to split the beam (add a new adventure)
                            adventures.Push(new JourneyStart(current_adventure.Current, 'L'));

                            // Continue right
                            // Make sure we haven't hit the right of the grid
                            if (current_adventure.Current.Item2 < max_x)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 + 1, 'R');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case '/':
                    switch(current_adventure.Direction)
                    {
                        case 'L':
                            // Make sure we haven't hit the bottom of the grid
                            if (current_adventure.Current.Item1 < max_y)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 + 1, current_adventure.Current.Item2, 'D');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'R':
                            // Make sure we haven't hit the top of the grid
                            if (current_adventure.Current.Item1 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 - 1, current_adventure.Current.Item2, 'U');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'U':
                            // Make sure we haven't hit the right of the grid
                            if (current_adventure.Current.Item2 < max_x)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 + 1, 'R');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'D':
                            // Make sure we haven't hit the left of the grid
                            if (current_adventure.Current.Item2 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 - 1, 'L');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case '\\':
                    switch (current_adventure.Direction)
                    {
                        case 'L':
                            // Make sure we haven't hit the top of the grid
                            if (current_adventure.Current.Item1 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 - 1, current_adventure.Current.Item2, 'U');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'R':
                            // Make sure we haven't hit the bottom of the grid
                            if (current_adventure.Current.Item1 < max_y)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 + 1, current_adventure.Current.Item2, 'D');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'U':
                            // Make sure we haven't hit the left of the grid
                            if (current_adventure.Current.Item2 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 - 1, 'L');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'D':
                            // Make sure we haven't hit the right of the grid
                            if (current_adventure.Current.Item2 < max_x)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 + 1, 'R');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case '.':
                    // Keep going in the same direction
                    switch (current_adventure.Direction)
                    {
                        case 'U':
                            // Make sure we haven't hit the top of the grid
                            if (current_adventure.Current.Item1 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 - 1, current_adventure.Current.Item2, 'U');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'D':
                            // Make sure we haven't hit the bottom of the grid
                            if (current_adventure.Current.Item1 < max_y)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1 + 1, current_adventure.Current.Item2, 'D');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'L':
                            // Make sure we haven't hit the left of the grid
                            if (current_adventure.Current.Item2 > 0)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 - 1, 'L');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        case 'R':
                            // Make sure we haven't hit the right of the grid
                            if (current_adventure.Current.Item2 < max_x)
                            {
                                current_adventure.Move_To(current_adventure.Current.Item1, current_adventure.Current.Item2 + 1, 'R');
                            }
                            else
                            {
                                current_adventure.Complete_Journey();
                            }
                            break;

                        default:
                            break;
                    }   
                    break;

                default:
                    break;
            }
        }

        return journal;
    }

    private static void Part_1_Test()
    {
        List<string> input = new List<string>
        {
            @".|...\....",
            @"|.-.\.....",
            @".....|-...",
            @"........|.",
            @"..........",
            @".........\",
            @"..../.\\..",
            @".-.-/..|..",
            @".|....-|.\",
            @"..//.|....",
        };

        char[,] grid = WorkWithArrays.ConvertListTo2DChar_StaggeredArray(input);
        ulong energized = Solve_Part_1(grid);
        Console.WriteLine($"Part 1 Test: {energized}");
    }

    private static void Part_2_Test()
    {
        List<string> input = new List<string>
        {
            @".|...\....",
            @"|.-.\.....",
            @".....|-...",
            @"........|.",
            @"..........",
            @".........\",
            @"..../.\\..",
            @".-.-/..|..",
            @".|....-|.\",
            @"..//.|....",
        };

        char[,] grid = WorkWithArrays.ConvertListTo2DChar_StaggeredArray(input);
        ulong energized = Solve_Part_2(grid);
        Console.WriteLine($"Part 1 Test: {energized}");
    }

    private static void print_grid(char[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            Console.WriteLine(string.Join("", grid.GetRow(i).ToArray()));
        }
    }

    private static void visualize_journey(Dictionary<string, ulong> journey, char[,] grid)
    {
        char[,] journey_grid = new char[grid.GetLength(0), grid.GetLength(1)];

        for (int i = 0; i < journey_grid.GetLength(0); i++)
        {
            for (int j = 0; j < journey_grid.GetLength(1); j++)
            {
                journey_grid[i, j] = '.';
            }
        }

        foreach (var item in journey)
        {
            var row = int.Parse(item.Key.Split(',')[0]);
            var column = int.Parse(item.Key.Split(',')[1]);

            journey_grid[row, column] = '#';
        }

        print_grid(journey_grid);
    }
}