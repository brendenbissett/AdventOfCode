
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Utilities;


internal class Program
{
    class PartNumber
    {
        public int partNumber { get; set; }
        public List<string> adjacentCoordinates { get; set; }

    }


    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day3.txt");

        var lines = FileHandling.ReadInputFile(path);

        // Step 1 - Run through scematic and create a coordinate map of the symbols

        string[] arrLines = lines.ToArray();
        Dictionary<string, char> symbolMap = new Dictionary<string, char>();

        for (int r = 0; r < arrLines.Length; r++)
        {

            // Build Map of Symbols
            for (int c = 0; c < arrLines[r].Length; c++)
            {
                if (arrLines[r][c] == '.') continue;
                if (char.IsDigit(arrLines[r][c])) continue;

                // Assume this is now a symbol
                var key = $"{r},{c}";
                symbolMap.Add(key, arrLines[r][c]);
            }
        }   

        foreach(string key in symbolMap.Keys)
        {
            Console.WriteLine($"{key} --> {symbolMap[key]}");
        }
        

        // Step 2 - Go through the schematic again, and add up part numbers that are next to any of the coordinates
        List<PartNumber> potentialParts = new List<PartNumber>();
        for (int r = 0; r < arrLines.Length; r++)
        {
            // Build up list of potential part numbers
            string partNumber = "";
            int startColumn = -1;

            for (int c = 0; c < arrLines[r].Length; c++)
            {
                if (!char.IsDigit(arrLines[r][c]) || (c + 1 >= arrLines[r].Length))
                {
                    if (c + 1 >= arrLines[r].Length && char.IsDigit(arrLines[r][c]))
                    {
                        partNumber += arrLines[r][c]; // This is the last character in the line, so add it to the part number
                    }

                    if (partNumber.Length > 0)
                    {
                        // We have a part number
                        PartNumber pn = new PartNumber();
                        pn.partNumber = int.Parse(partNumber);

                        pn.adjacentCoordinates = [
                            .. CalculateAdjacentCoordinates(r, startColumn, partNumber.Length, arrLines.Length, arrLines[r].Length)
                        ];

                        potentialParts.Add(pn);

                        // Reset
                        partNumber = "";
                        startColumn = -1;
                    }   

                    continue;
                }

                partNumber += arrLines[r][c];

                if (startColumn == -1)
                {
                    startColumn = c;
                }
                
            }
        }


        // Could do this earlier, but go through parts and add up the parts that are next to any of the symbol coordinates
        int total = 0;
        foreach (PartNumber pn in potentialParts)
        {
            //Console.WriteLine($"Part Number: {pn.partNumber}");
            foreach (string coord in pn.adjacentCoordinates)
            {
                //Console.WriteLine($"  {coord}");
                if (symbolMap.ContainsKey(coord))
                {
                    total += pn.partNumber;
                    break;
                }
            }
        }

        Console.WriteLine($"Total: {total}"); 

        // Part 1 : 512794

        // ---------------- Part 2 ----------------
        Dictionary<string, List<int>> gearCountMap = new Dictionary<string, List<int>>();

        foreach (string key in symbolMap.Keys)
        {
            if (symbolMap[key] == '*')
            {
                gearCountMap.Add(key, new List<int>());
            }
        }

        foreach (PartNumber pn in potentialParts)
        {
            //Console.WriteLine($"Part Number: {pn.partNumber}");
            foreach (string coord in pn.adjacentCoordinates)
            {
                //Console.WriteLine($"  {coord}");
                if (gearCountMap.ContainsKey(coord))
                {
                    gearCountMap[coord].Add(pn.partNumber);
                    break;
                }
            }
        }

        int totalGearRatio = 0;
        foreach (string key in gearCountMap.Keys)
        {
            if (gearCountMap[key].Count == 2) // Has to be exactly 2
            {
                totalGearRatio += gearCountMap[key][0] * gearCountMap[key][1];
            }
        }

        Console.WriteLine($"Gear Ratio Total: {totalGearRatio}");    
        // Part 2 : 67779080

    }

    private static List<string> CalculateAdjacentCoordinates(int row, int col, int partNumberLength, int maxRows, int maxCols)
    {
        List<string> result = new List<string>();

        // Top Left
        if (row > 0 && col > 0)
        {
            result.Add($"{row - 1},{col - 1}");
        }

        // Top
        if (row > 0)
        {
            for (int i = 0; i < partNumberLength; i++)
            {
                result.Add($"{row - 1},{col + i}");
            }
        }

        // Top Right
        if (row > 0 && (col + partNumberLength - 1) < maxCols)
        {
            result.Add($"{row - 1},{col + partNumberLength}");
        }

        // Right
        if ((col + partNumberLength - 1) < maxCols)
        {
            result.Add($"{row},{col + partNumberLength}");
        }

        // Bottom Right
        if (row < maxRows && (col + partNumberLength - 1) < maxCols)
        {
            result.Add($"{row + 1},{col + partNumberLength}");
        }

        // Bottom
        if (row < maxRows)
        {
            for (int i = 0; i < partNumberLength; i++)
            {
                result.Add($"{row + 1},{col + i}");
            }
        }

        // Bottom Left
        if (row < maxRows && col > 0)
        {
            result.Add($"{row + 1},{col - 1}");
        }

        // Left
        if (col > 0)
        {
            result.Add($"{row},{col - 1}");
        }

        return result;
    }
}