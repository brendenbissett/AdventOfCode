using Utilities;

internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day25.txt");

        List<string> lines = FileHandling.ReadInputFile(path);

        Console.WriteLine($"File Input Line Count: {lines.Count}");
    }
}