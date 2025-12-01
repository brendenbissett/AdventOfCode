using Utilities;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day9.txt");
        var lines = FileHandling.ReadInputFile(path);

        var input = lines.Select(x => x.Trim().Split(" ").Select(y => int.Parse(y)).ToList()).ToList();


        // Part 1
        int part1_total = 0;
        foreach (var record in input)
        {
            part1_total += get_next_in_sequence(record);
        }

        Console.WriteLine($"get_next_in_sequence() : {part1_total}"); // 1904165718

        // Part 2
        int part2_total = 0;
        foreach (var record in input)
        {
            part2_total += get_prev_in_sequence(record);
        }

        Console.WriteLine($"get_prev_in_sequence() : {part2_total}"); // 964


        // Testing
        //part_1_test();
        //part_2_test();

    }

    private static List<List<int>> build_list_to_zero(List<int> input)
    {
        List<List<int>> tree = new List<List<int>>();

        tree.Add(input);

        // Build up tree till we get to zero
        // -----------------------------------------------------
        bool keep_processing = true;
        int index = 0;
        while (keep_processing)
        {
            List<int> bottom_row = new List<int>();
            bool contains_non_zero = false;

            for (int s = 0; s < tree[index].Count; s++)
            {
                if (s + 1 == tree[index].Count)
                {
                    break; // We are at the last element for new bottom row
                }

                int diff = tree[index][s + 1] - tree[index][s];

                bottom_row.Add(diff);

                // Make we can use this to see whether we have reached zero
                if (diff != 0) contains_non_zero = true;
            }

            // Add to the tree
            tree.Add(bottom_row);

            // Test whether we have reached zero
            if (!contains_non_zero)
            {
                // We have reached zero
                keep_processing = false;
                break;
            }

            index++;
        }

        // Return result
        return tree;
    }

    private static int get_next_in_sequence(List<int> input)
    {
        var tree = build_list_to_zero(input);

        // Reverse trees to get next in sequence
        // -----------------------------------------------------
        for(int x = tree.Count - 2; x >= 0; x--)  // -2 since we don't want to increase the last row's value.
        {
            int add_value = tree[x+1].Last();
            int last_value_in_row = tree[x].Last();
            tree[x].Add(last_value_in_row + add_value);
        }

        // Return result
        return tree[0].Last();
    }

    private static int get_prev_in_sequence(List<int> input)
    {
        var tree = build_list_to_zero(input);

        // Reverse trees to get next in sequence
        // -----------------------------------------------------
        for (int x = tree.Count - 2; x >= 0; x--)  // -2 since we don't want to increase the last row's value.
        {
            int subtract_value = tree[x + 1].First();
            int first_value_in_row = tree[x].First();
            tree[x].Insert(0, first_value_in_row - subtract_value);
        }

        // Return result
        return tree[0].First();
    }

    private static void part_1_test()
    {
        List<List<int>> input = new List<List<int>>()
        {
            new List<int> { 0, 3, 6, 9 ,12, 15 },
            new List<int> { 1, 3, 6, 10, 15, 21 },
            new List<int> { 10, 13, 16, 21, 30, 45 },
        };

        int total = 0;
        foreach(var test in input)
        {
            int result = get_next_in_sequence(test);

            Console.WriteLine($"Input: {string.Join(",", test)}");
            Console.WriteLine($"Next in sequence: {result}");

            total += result;        
        }

        Console.WriteLine($"part_1_test() : {total}");        
    }

    private static void part_2_test()
    {
        List<List<int>> input = new List<List<int>>()
        {
            new List<int> { 0, 3, 6, 9 ,12, 15 },
            new List<int> { 1, 3, 6, 10, 15, 21 },
            new List<int> { 10, 13, 16, 21, 30, 45 },
        };

        int total = 0;
        foreach (var test in input)
        {
            int result = get_prev_in_sequence(test);

            Console.WriteLine($"Input: {string.Join(",", test)}");
            Console.WriteLine($"Previous in sequence: {result}");

            total += result;
        }

        Console.WriteLine($"part_2_test() : {total}");
    }
}