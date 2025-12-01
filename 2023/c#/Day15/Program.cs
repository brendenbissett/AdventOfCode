using System.Reflection;
using Utilities;

internal class Program
{
    struct Lens
    {
        public string Label;
        public int Value;
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day15.txt");

        List<string> lines = FileHandling.ReadInputFile(path);

        // Part 1 
        ulong part_1_result = Solve_Part_1(lines[0].Split(','));
        Console.WriteLine($"part_1_result: {part_1_result}");  // 506269 -- Correct result!

        // Part 2
        ulong part_2_result = Solve_Part_2(lines[0].Split(','));
        Console.WriteLine($"part_2_result: {part_2_result}");  // 264021 -- Correct result!

        // Testing
        Test_Part_1_1();
        Test_Part_1_2();
        Test_Part_2();
    }

    private static ulong Solve_Part_1(string[] lines)
    {
        ulong result = 0;

        foreach (string line in lines)
        {
            result += Hash(line);
        }

        return result;
    }

    private static ulong Solve_Part_2(string[] lines)
    {
        ulong result = 0;

        Dictionary<ulong, List<Lens>> boxes = new Dictionary<ulong, List<Lens>>();

        // Make sure all the boxes exist
        for (int b = 0; b < 256; b++)
        {
            boxes.Add((ulong)b, new List<Lens>());
        }


        foreach (string line in lines)
        {
            char operation = '_';
            string label = string.Empty;
            if (line.Contains('=')) {
                label = line.Split('=')[0];
                operation = '=';
            } else {
                label = line.Split('-')[0];
                operation = '-';
            }
            int value = (line.Contains('=') ? int.Parse(line.Split('=')[1]) : 0);
            ulong key = Hash(label);

            if (!boxes.ContainsKey(key) && operation == '=')
            {
                boxes.Add(key, new List<Lens>() { new Lens() { Label = label, Value = value } });
            }
            else if (boxes.ContainsKey(key) && operation == '=')
            {
                bool found = false;
                for (int i = 0; i < boxes[key].Count; i++)
                {
                    if (boxes[key][i].Label == label)
                    {
                        boxes[key][i] = new Lens() { Label = label, Value = value };
                        found = true;
                        break;
                    }
                }
                if (!found) boxes[key].Add(new Lens() { Label = label, Value = value });
            }
            else if (boxes.ContainsKey(key) && operation == '-')
            {
                foreach (Lens lens in boxes[key])
                {
                    if (lens.Label == label)
                    {
                        boxes[key].Remove(lens);
                        break;
                    }
                }
            }
        }

        // Add them up
        foreach (var box in boxes)
        {
            ulong boxNumber = box.Key + 1;
            for(int l = 0; l < box.Value.Count; l++)
            {
                result += boxNumber * (ulong)(l+1) * (ulong)box.Value[l].Value;
            }
        }   

        return result;
    }

    private static ulong Hash(string input)
    {
        ulong currenValue = 0;

        for (int i = 0; i < input.Length; i++)
        {
            var ascii = (ulong)input[i];

            currenValue += ascii;

            currenValue = (currenValue * 17) % 256;
        }

        //Console.WriteLine($"Hash({input}) = {currenValue}");

        return currenValue;
    }

    private static void Test_Part_1_1()
    {
        string input = "HASH";

        ulong result = Solve_Part_1(input.Split(','));

        Console.WriteLine($"Test_Part_1_1 - Result: {result}");
    }

    private static void Test_Part_1_2()
    {
        string input = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";

        ulong result = Solve_Part_1(input.Split(','));

        Console.WriteLine($"Test_Part_1_2 - Result: {result}");
    }

    private static void Test_Part_2()
    {
        string input = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";

        ulong result = Solve_Part_2(input.Split(','));

        Console.WriteLine($"Test_Part_2 - Result: {result}");
    }
}