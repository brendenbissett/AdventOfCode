using Superpower.Parsers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day12.txt");
        var lines = FileHandling.ReadInputFile(path);


        // Springs are arranged in rows
        // Every springs is shows as:
        //   . = operational
        //   # = damaged
        //   ? = unknown
        ulong part_1 = solve(lines);
        Console.WriteLine($"Part 1 - Answer : {part_1}");  // 7361 -- Correct
        part_1_test_input();
        part_2_test_input();
    }

    private static ulong solve_part_2(List<string> input)
    {
        ulong total_arrangements = 0;

        foreach (var line in input)
        {
            var data = line.Split(" ");
            var arrangement = data[0].Trim();
            var contigous_damaged_springs = data[1].Trim().Split(",").Select(x => int.Parse(x)).ToList();

            total_arrangements += get_arrangements_part_2(arrangement, contigous_damaged_springs);
        }

        return total_arrangements;
    }

    private static ulong get_arrangements_part_2(string original, List<int> contigous_damaged_springs)
    {
        /// Not to self. (Used the last ? index as way to determine where to start building the string)

        List<string> permutations = new List<string>();

        // Build regex
        string pattern = get_regex_pattern(contigous_damaged_springs);
        Regex rx = new Regex(pattern);

        // Calculate the possible permutations based on the string.
        // Build from left (Comparing to the original string)
        // If that fails, build from right
        // Once you have a permutation, check if it matches the regex.
        // Run through algorithm to get all permutations that still meet regex and original string

        StringBuilder seed = new StringBuilder();
        int dots = 0;
        for(int i = 0; i < original.Length; i++) {

            if(i >= 1) {
                // Ensure that new string matches the previous character.
                if(seed[i-1] != original[i-1]) {
                    
                }
            }

            if(original[i] == '.') {
                dots++;
                seed.Append('.');
                continue;
            } 
        }
        
        while(true) {
            // Validate string with regex
            if (rx.IsMatch(seed.ToString()) && permutation_matches_original(original, seed.ToString()))
            {
                break;
            }
            
            if (original[0] == '.') seed.Append(".");
            
            foreach(int d in contigous_damaged_springs) {
                seed.Append(string.Concat(Enumerable.Repeat("#", d)));
                if(seed.Length < original.Length) seed.Append('.');;
            }
            
            if(seed.Length < original.Length) {
                seed.Append(string.Concat(Enumerable.Repeat(".", original.Length - seed.Length)));
            }

            Console.WriteLine($"Original: {original} ==> Seed: {seed.ToString()}");

            if (original[0] == '.')
            {
                seed.Append("#");
            }
            else
            {
                seed.Append(".");
            }


        }

       
/*
?###???????? 3,2,1
.###.##.#...
.###.##..#..
.###.##...#.
.###.##....#
.###..##.#..
.###..##..#.
.###..##...#
.###...##.#.
.###...##..#
.###....##.#
*/


        return (ulong)permutations.Count;
    }



    private static ulong solve(List<string> input)
    {
        ulong total_arrangements = 0;

        foreach(var line in input)
        {
            var data = line.Split(" ");
            var arrangement = data[0].Trim();
            var contigous_damaged_springs = data[1].Trim().Split(",").Select(x => int.Parse(x)).ToList();

            total_arrangements += get_arrangements(arrangement, contigous_damaged_springs);
        }

        return total_arrangements;
    }

    private static ulong get_arrangements(string data, List<int> contigous_damaged_springs)
    {
        ulong arrangements = 0;

        // Decide how to format possible combinations of "." and "#" in length of string
        int total_damaged_springs = contigous_damaged_springs.Sum();

        string damaged = string.Concat(Enumerable.Repeat("#", total_damaged_springs));
        string operational = string.Concat(Enumerable.Repeat(".", data.Length - total_damaged_springs));

        string seed_string = $"{damaged}{operational}";

        // Build regex
        string pattern = get_regex_pattern(contigous_damaged_springs);
        Regex rx = new Regex(pattern);

        List<string> permutations = GeneratePermutations(seed_string, rx);

        //Console.WriteLine($"Seed string: {seed_string}");

        foreach (var option in permutations)
        {
            bool is_a_match = rx.IsMatch(option);
            if (is_a_match)
            {
                // Confirm if '.' and '#' is in right location based on original data
                if (permutation_matches_original(data, option))
                {
                    //Console.WriteLine($"{pattern} \t {option} \t {is_a_match}");
                    arrangements++;   
                }
            }
        }

        // Use the contigous_damaged_springs as regex to filter our possible combinations that meet that regex.
        // Count the number of possible combinations that meet the regex.

        return arrangements;
    }

    private static bool permutation_matches_original(string original, string permutation)
    {
        if (original.Length != permutation.Length) return false;

        for (int i = 0; i < original.Length; i++)
        {
            char o = original[i];
            char p = permutation[i];
            
            if (o == '?') continue;
            if (o != p) return false;
        }
        
        return true;
    }

    private static string get_regex_pattern(List<int> damaged_springs)
    {
        // ^\.*#{1}\.+#{1}\.+#{3}\.*$   <<-- This is gold!

        StringBuilder sb = new StringBuilder("^\\.*");

        for (int i = 0; i < damaged_springs.Count; i++)
        {
            if (i == damaged_springs.Count - 1)
            {
                sb.Append("#{" + damaged_springs[i] + "}\\.*");
            }
            else
            {
                sb.Append("#{" + damaged_springs[i] + "}\\.+");   
            }
        }

        sb.Append("$");
        
        return sb.ToString();
    }

    private static List<string> GeneratePermutations(string input, Regex rx)
    {
        var array = input.ToCharArray().OrderBy(c => c).ToArray();
        Span<char> spanInput = array.AsSpan();

        var result = new List<string>() { new string(spanInput) };
        while (NextPermutation(spanInput))
        {
            if (rx.IsMatch(new string(spanInput)))
            {
                result.Add(new string(spanInput));
            }
        }

        return result;
    }

    private static bool NextPermutation(Span<char> input)
    {
        int i = input.Length - 2;
        while (i >= 0 && input[i] >= input[i + 1])
        {
            i--;
        }
        if (i < 0)
            return false;
        int j = input.Length - 1;
        while (input[j] <= input[i])
        {
            j--;
        }
        (input[i], input[j]) = (input[j], input[i]);
        Reverse(input, i + 1);
        return true;
    }

    private static void Reverse(Span<char> input, int start)
    {
        int i = start;
        int j = input.Length - 1;
        while (i < j)
        {
            (input[i], input[j]) = (input[j], input[i]);
            i++;
            j--;
        }
    }

    private static void unfold_input(List<string> input)
    {
        for (int r = 0; r < input.Count; r++)
        {
            var data = input[r].Split(" ");
            
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            sb1.Append(data[0]);
            sb2.Append(data[1]);
            for (int x=0; x < 4; x++)
            {
                sb1.Append("?" + data[0]);
                sb2.Append("," + data[1]);
            }

            input[r] = $"{sb1.ToString()} {sb2.ToString()}";
        }
    }

    private static void part_1_test_input()
    {
        List<string> input = new List<string>()
        {
            "???.### 1,1,3",
            ".??..??...?##. 1,1,3",
            "?#?#?#?#?#?#?#? 1,3,1,6",
            "????.#...#... 4,1,1",
            "????.######..#####. 1,6,5",
            "?###???????? 3,2,1",
        };

        ulong total_arrangements = solve(input);

        Console.WriteLine($"part_1_test_input - {total_arrangements}"); // Expect 21
    }
    
    private static void part_2_test_input()
    {
        List<string> input = new List<string>()
        {
            "???.### 1,1,3",
            ".??..??...?##. 1,1,3",
            "?#?#?#?#?#?#?#? 1,3,1,6",
            "????.#...#... 4,1,1",
            "????.######..#####. 1,6,5",
            "?###???????? 3,2,1",
        };

        unfold_input(input);
        ulong total_arrangements = solve_part_2(input);
  
        Console.WriteLine($"part_2_test_input - {total_arrangements}"); // Expect 525152
    }
}