using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using Utilities;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input.txt");

        var lines = FileHandling.ReadInputFile(path);

        //var part1_total = Part1(lines);
        //Console.WriteLine($"Part 1 Total: {part1_total}");
        // Part 1 : 54644


        var part2_total = Part2(lines);
        Console.WriteLine($"Part 2 Total: {part2_total}");

        // Part 2 : 53355 - too high
        // Part 2 : 53348

        //ManualTests();
    }

    private static int Part1(List<string> lines)
    {
        var results = new List<int>();

        foreach (string line in lines)
        {
            var number = GetCombinedFirstAndLastNumber(line);

            Console.WriteLine($"Part 1: {number} - {line}");
            results.Add(number);
        }

        var total = results.Sum();

        return total;
    }

    private static int Part2(List<string> lines)
    {
        var results = new List<int>();

        foreach (string line in lines)
        {
            var number = GetCombinedFirstAndLastNumber_Part2(line.ToLower());

            Console.WriteLine($"Part 2: {number} - {line}");
            results.Add(number);
        }

        var total = results.Sum();

        return total;
    }

    private static int GetCombinedFirstAndLastNumber(string line)
    {
        List<int> numbers = new List<int>();

        foreach (char c in line)
        {
            if (char.IsDigit(c))
            {
                numbers.Add(Int32.Parse(c.ToString()));
            }
        }

        return Int32.Parse($"{numbers.First()}{numbers.Last()}");
    }

    private static int GetCombinedFirstAndLastNumber_Part2(string line)
    {
        List<int> numbers = new List<int>();
        string tempNumber = "";

        Dictionary<string, int> checklist = new Dictionary<string, int>()
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 }
        };  

        foreach (char c in line)
        {
            tempNumber += c;

            if (char.IsDigit(c))
            {
                numbers.Add(Int32.Parse(c.ToString()));
                tempNumber = ""; // Reset
                continue;
            }

            if (checklist.Keys.Contains(tempNumber))
            {
                numbers.Add(checklist[tempNumber]);
                tempNumber = "" + c; // Reset, but include current character, might be start of new number
                continue;
            }

            if (!IsPotentialTextNumber(checklist, tempNumber))
            {
                // Before resetting, drop first character and check if word might be the start of a new number
                if (tempNumber.Length > 1)
                {
                    tempNumber = tempNumber.Remove(0, 1);
                    if (IsPotentialTextNumber(checklist, tempNumber))
                    {
                        continue;  // Don't reset, we have potential start of new number
                    }
                }

                tempNumber = "" + c; // Reset, but include current character, might be start of new number
            }
        }

        return Int32.Parse($"{numbers.First()}{numbers.Last()}");
    }

    private static bool IsPotentialTextNumber(Dictionary<string, int> dict, string val)
    {
        foreach (var key in dict.Keys)
        {
            if (key.StartsWith(val))
            {
                return true;
            }
        }

        return false;
    }


    private static void ManualTests()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { "1abc2", 12 },
            { "pqr3stu8vwx", 38 },
            { "a1b2c3d4e5f", 15 },
            { "treb7uchet", 77 },
            { "two1nine", 29 },
            { "eightwothree", 83 },
            { "abcone2threexyz", 13 },
            { "xtwone3four", 24 },
            { "4nineeightseven2", 42 },
            { "zoneight234", 14 },
            { "7pqrstsixteen", 76 },
            { "foursixtwoninevtzzgntnlg6oneightbxp", 48 } // Sneaky little bugger this
        };  

        foreach (var key in dict.Keys)
        {
            var results = GetCombinedFirstAndLastNumber_Part2(key);
            Console.WriteLine($"{key} - {results} - {dict[key]}");
        }
    }
}