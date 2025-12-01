using System.Numerics;
using Utilities;

internal class Program
{

    public class Race
    {
        public int Time { get; set; }
        public int Distance { get; set; }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day6.txt");
        var lines = FileHandling.ReadInputFile(path);

        if (lines.Count != 2) throw new Exception("Invalid input file");

        // Process the lines
        List<int> times = lines[0].Split(":")[1].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList().Select(x => int.Parse(x.Trim())).ToList();
        List<int> distances = lines[1].Split(":")[1].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList().Select(x => int.Parse(x.Trim())).ToList();

        // Build up list of races
        List<Race> races = new List<Race>();
        for(int i = 0; i< times.Count; i++)
        {
            races.Add(new Race() { Time = times[i], Distance = distances[i] });
        }

        // Test example provided
        TestExample_Part1();

        // Process races
        List<int> possibilities = new List<int>();
        int speedIncrement = 1;
        foreach(Race race in races)
        {
            possibilities.Add(CountHowManyWaysToBeatRecord(race.Time, race.Distance, speedIncrement));
        }

        var mult = possibilities.Aggregate((x, y) => x * y);

        // Results
        Console.WriteLine($"Number of races to process: {races.Count}");
        Console.WriteLine($"Possibilities multiplied: {mult}");

        // Part 2
        TestExample_Part2();
        ulong result = CountHowManyWaysToBeatRecord_Part2(42686985, 284100511221341, 1);
        Console.WriteLine($"Part 2: {result}");

        // Part 1 - 440000
        // Part 2 - 26187338
    }

    private static int CountHowManyWaysToBeatRecord(int time, int record, int speedIncrement)
    {
        int qty = 0;

        for(int pressed = 1; pressed < time; pressed++)
        {
            // Calculate how fast we would be moving
            int speed = pressed * speedIncrement;
            int timeLeft = time - pressed;

            int distance = CalculateDistance(speed, timeLeft);

            if (distance > record)
            {
                qty++;
            }
        }

        return qty;
    }

    private static ulong CountHowManyWaysToBeatRecord_Part2(ulong time, ulong record, ulong speedIncrement)
    {
        ulong qty = 0;

        for (ulong pressed = 1; pressed < time; pressed++)
        {
            // Calculate how fast we would be moving
            ulong speed = pressed * speedIncrement;
            ulong timeLeft = time - pressed;

            ulong distance = CalculateDistance(speed, timeLeft);

            if (distance > record)
            {
                qty++;
            }
        }

        return qty;
    }

    static T CalculateDistance<T>(T speed, T time) where T : INumber<T>
    {
        return speed * time;
    }

    private static void TestExample_Part1()
    {
        int example_1 = CountHowManyWaysToBeatRecord(7, 9, 1);
        int example_2 = CountHowManyWaysToBeatRecord(15, 40, 1);
        int example_3 = CountHowManyWaysToBeatRecord(30, 200, 1);
        Console.WriteLine($"Example For Part 1: {example_1} * {example_2} * {example_3} = {example_1 * example_2 * example_3}");
    }

    private static void TestExample_Part2()
    {
        ulong result = CountHowManyWaysToBeatRecord_Part2(71530, 940200, 1);
        
        Console.WriteLine($"Example For Part 2: {result}");
    }


}