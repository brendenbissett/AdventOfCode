using System.Xml.Linq;
using Utilities;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day8.txt");
        var lines = FileHandling.ReadInputFile(path);

        String instructions = string.Empty;
        Dictionary<string, (string, string)> map = new Dictionary<string, (string, string)>();

        var is_first_line = true;

        foreach (var line in lines)
        {
            if(String.IsNullOrEmpty(line.Trim())) continue;
            
            if (is_first_line)
            {
                instructions = line;
                is_first_line = false;
                continue;
            }

            var split = line.Split("=");

            var key = split[0].Trim();
            var values = split[1].Trim().Replace("(", "").Replace(")", "").Split(",");

            map.Add(key, (values[0].Trim(), values[1].Trim()));
        }

        // We have everything we need now
        int step_count = countSteps_part_1(instructions, map, "AAA", "ZZZ");

        // Part 1
        Console.WriteLine($"Part 1 - Step count: {step_count}"); // 16409

        // Tests
        Test_Part_1_Simple();
        Test_Part_1_Loop();
        Test_Part2();

        // Part 2
        long part2_count = countSteps_Part_2(instructions, map, 'A', 'Z');
        Console.WriteLine($"Part 2 - Step count: {part2_count}");  // 11795205644011

    }

    private static int countSteps_part_1(string instructions, Dictionary<string, (string, string)> map, string startKey, string endKey)
    {
        int counter = 0;
        var key = startKey;

        var steps = instructions.ToCharArray();
        for (int i = 0; i < steps.Length; i++)
        {
            // Get next location
            key = (steps[i] == 'L') ? map[key].Item1 : map[key].Item2;
            counter++;

            if (key == endKey)
            {
                break; // Solved
            }

            // Reset if you reach end of the instructions
            if (i + 1 >= steps.Length)
            {
                i = -1;
                continue;
            }
        }

        return counter;
    }
    private static long countSteps_Part_2(string instructions, Dictionary<string, (string, string)> map, char startKeyEndsWith, char endKeyEndsWith)
    {
        var steps = instructions.ToCharArray();
        List<string> current_nodes = map.Keys.Where((k, v) => k.EndsWith(startKeyEndsWith)).ToList();


        List<int> stepCounts = new List<int>();

        foreach(string node in current_nodes)
        {
            // Only look at one node
            string start_node = node;
            string current_node = start_node;
            string next_node = start_node;
            int counter = 0;

            Dictionary<string, int> journey = new Dictionary<string, int>();

            for (int i = 0; i < steps.Length; i++)
            {
                current_node = next_node;
                next_node = (steps[i] == 'L') ? map[next_node].Item1 : map[next_node].Item2;

                if (journey.ContainsKey(next_node))
                {
                    journey[next_node]++;
                }
                else
                {
                    journey.Add(next_node, 1);
                }

                counter++;

                if (next_node.EndsWith(endKeyEndsWith)) break;


                // Reset if you reach end of the instructions
                if (i + 1 >= steps.Length)
                {
                    i = -1;
                    continue;
                }
            }

            //Console.WriteLine($"Found Target Node (START: {node}): {next_node} in {counter} steps");
            stepCounts.Add(counter);
        }

        //Console.WriteLine($"Number of items to calculate LCM for: {stepCounts.Count}");
        long lcm = Calculations.lcm_of_array_elements(stepCounts.ToArray());
        //Console.WriteLine($"LC = {lcm}");

        return lcm;
    }


    private static void Test_Part_1_Simple()
    {
        String instructions = "RL";
        Dictionary<string, (string, string)> map = new Dictionary<string, (string, string)>();

        map.Add("AAA", ("BBB", "CCC"));
        map.Add("BBB", ("DDD", "EEE"));
        map.Add("CCC", ("ZZZ", "GGG"));
        map.Add("DDD", ("DDD", "DDD"));
        map.Add("EEE", ("EEE", "EEE"));
        map.Add("GGG", ("GGG", "GGG"));
        map.Add("ZZZ", ("ZZZ", "ZZZ"));

        int count = countSteps_part_1(instructions, map, "AAA", "ZZZ");

        Console.WriteLine($"Test Part 1 - Step count: {count}");
    }

    private static void Test_Part_1_Loop()
    {
        String instructions = "LLR";
        Dictionary<string, (string, string)> map = new Dictionary<string, (string, string)>();

        map.Add("AAA", ("BBB", "BBB"));
        map.Add("BBB", ("AAA", "ZZZ"));
        map.Add("ZZZ", ("ZZZ", "ZZZ"));

        int count = countSteps_part_1(instructions, map, "AAA", "ZZZ");

        Console.WriteLine($"Test Part 1 (Loop) - Step count: {count}");
    }

    private static void Test_Part2()
    {
        String instructions = "LR";
        Dictionary<string, (string, string)> map = new Dictionary<string, (string, string)>();

        map.Add("11A", ("11B", "XXX"));
        map.Add("11B", ("XXX", "11Z"));
        map.Add("11Z", ("11B", "XXX"));
        map.Add("22A", ("22B", "XXX"));
        map.Add("22B", ("22C", "22C"));
        map.Add("22C", ("22Z", "22Z"));
        map.Add("22Z", ("22B", "22B"));
        map.Add("XXX", ("XXX", "XXX"));

        long count = countSteps_Part_2(instructions, map, 'A', 'Z');
        Console.WriteLine($"Test Part 2 (LCD) - Step count: {count}");
    }
}