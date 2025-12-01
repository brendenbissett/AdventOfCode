using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Utilities;

internal class Program
{
    class Pattern
    {
        public int Id { get; set; }

        public List<string> Rows { get; set; }
        public List<string> Columns { get; set; }

        public int HorizontalMirrorIndex { get; set; }
        public int HorizontalMirrorCount { get; set; }
        public int VerticalMirrorIndex { get; set; }
        public int VerticalMirrorCount { get; set; }

        public Pattern(int Id, List<string> rows)
        {
            this.Id = Id;
            this.Rows = new List<string>(rows);
            this.Columns = createColumns();

            var horizontal = GetHorizontalMirrorCount();
            this.HorizontalMirrorCount = horizontal.count;
            this.HorizontalMirrorIndex = horizontal.index;

            var vertical = GetVerticalMirrorCount();
            this.VerticalMirrorCount = vertical.count;
            this.VerticalMirrorIndex = vertical.index;

            //this.DuplicateRowCount = this.Rows.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList().Count();
            //this.DuplicateColumnCount = this.Columns.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList().Count();

        }

        private List<string> createColumns()
        {
            List<string> columns = new List<string>();

            for (int c = 0; c < Rows[0].Length; c++)
            {
                StringBuilder sb = new StringBuilder();

                for (int r = 0; r < Rows.Count; r++)
                {
                    sb.Append(Rows[r][c]);
                }

                columns.Add(sb.ToString());
            }

            return columns;
        }

        public bool IsSplitHorizontally
        {
            get
            {
                return this.HorizontalMirrorCount >= this.VerticalMirrorCount;
            }
        }

        public bool IsSplitVertically
        {
            get
            {
                return this.VerticalMirrorCount > this.HorizontalMirrorCount;
            }
        }

        public int GetHorizontalValue()
        {
            if (this.HorizontalMirrorCount == 0) return 0;

            return (this.HorizontalMirrorIndex + 1) * 100;
        }

        public int GetVerticalValue()
        {
            if (this.VerticalMirrorCount == 0) return 0;

            return this.VerticalMirrorIndex + 1;
        }

        private (int count, int index) GetHorizontalMirrorCount()
        {

            // <index, mirrorCount>
            List<int> possibleMirrorIndexes = new List<int>();

            // Step 1 - Find the center row
            for (int i = 0; i < Rows.Count - 1; i++)
            {
                if (Rows[i] == Rows[i + 1])
                {
                    // Found it // TODO: We are assuming this is the one
                    possibleMirrorIndexes.Add(i);
                }
            }

            // Step 2 - Find the number if we start on index
            int returnIndex = 0;
            int returnCount = 0;
            foreach (int mirrorIndex in possibleMirrorIndexes)
            {
                int localCount = 0;
                int i = 0;
                int j = mirrorIndex;
                for (i = mirrorIndex, j = mirrorIndex + 1; i >= 0; i--, j++)
                {
                    if (j >= Rows.Count) break; // Reached end of the list
                    if (Rows[i] != Rows[j])
                    {
                        localCount = 0;
                        Console.WriteLine($"Pattern {Id} - Horizontal Mirror: {i}, {j}");
                        break; // No longer mirrored
                    }

                    localCount++;
                }

                if(i <= 0 || j >= Rows.Count)
                {
                    if (localCount > returnCount)
                    {
                        returnCount = localCount;
                        returnIndex = mirrorIndex;
                    }
                }

                
            }

            return (returnCount, returnIndex);
        }

        private (int count, int index) GetVerticalMirrorCount()
        {
            // <index, mirrorCount>
            List<int> possibleMirrorIndexes = new List<int>();

            // Step 1 - Find the center row
            for (int i = 0; i < Columns.Count - 1 ; i++)
            {
                if (Columns[i] == Columns[i + 1])
                {
                    // Found it // TODO: We are assuming this is the one
                    possibleMirrorIndexes.Add(i);
                }
            }

            // Step 2 - Find the number if we start on index
            int returnIndex = 0;
            int returnCount = 0;
            foreach (int mirrorIndex in possibleMirrorIndexes)
            {
                int localCount = 0;
                int i = 0;
                int j = mirrorIndex;
                for (i = mirrorIndex, j = mirrorIndex + 1; i >= 0; i--, j++)
                {
                    if (j >= Columns.Count) break; // Reached end of the list
                    if (Columns[i] != Columns[j])
                    {
                        localCount = 0;
                        Console.WriteLine($"Pattern {Id} - Vertical Mirror: {i}, {j}");
                        break; ; // No longer mirrored
                    }

                    localCount++;
                }

                if (i <= 0 || j >= Columns.Count)
                {
                    if (localCount > returnCount)
                    {
                        returnCount = localCount;
                        returnIndex = mirrorIndex;
                    }
                }

            }

            return (returnCount, returnIndex);
        }

        public void PrintToConsole()
        {
            Console.WriteLine($"Pattern {Id}:");

            foreach(string row in Rows)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine();
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day13.txt");
        var lines = FileHandling.ReadInputFile(path);

        Console.WriteLine($"Day 13 - Input file: {lines.Count} rows.");

        List<Pattern> list = new List<Pattern>();
        int id = 1;

        List<string> patterns = new List<string>();

        foreach (string line in lines)
        {
            if (line == string.Empty)
            {
                // Add the pattern
                list.Add(new Pattern(id++, patterns));
                patterns.Clear();
                continue;
            }

            patterns.Add(line);
        }
        // Add the last pattern
        list.Add(new Pattern(id++, patterns));

        // Print to console
        foreach(var p in list)
        {
            //p.PrintToConsole();
        }   

        // Part 1
        //ulong total = solve_part_1(list);
        //Console.WriteLine($"Part_1: {total}"); // 19773 -- Too Low
        //Console.WriteLine($"Part_1: {total}");  // 33273 -- Too Low
        //Console.WriteLine($"Part_1: {total}");    // 27747  -- Too Low
                                                  // 27693 -- Nope
                                                  // 28270 -- Nope
                                                  // 30219 -- Nope
                                                  // 33356 - Yes!  (Needed to ensure that any "mirror" was a full mirror, not just a partial mirror)


        // Testing

        Part_1_Test_1();
    }

    private static ulong solve_part_1(List<Pattern> list)
    {
        // Identify if horizontal or vertical mirror

        ulong total = 0;
        ulong total_both_h_and_v = 0;

        foreach(Pattern p in list)
        {
            //string v_h = p.IsSplitHorizontally ? "H" : "V";
            //Console.WriteLine($"{p.Id}\tPattern: {v_h}\tH Val ({p.GetHorizontalValue()})\tcount:{p.HorizontalMirrorCount}-index_{p.HorizontalMirrorIndex}\t|\tV Val ({p.GetVerticalValue()})\tcount:{p.VerticalMirrorCount}-index_{p.VerticalMirrorIndex}");

            if(p.HorizontalMirrorCount == p.VerticalMirrorCount)
            {
                Console.WriteLine($"WARNING: Pattern {p.Id} is has same vertical and horizontal mirror counts... What to do?");
                continue;
            }

            if(p.IsSplitHorizontally)
            {
                total += (ulong)p.GetHorizontalValue();

            } else
            {
                total += (ulong)p.GetVerticalValue();
            }

        }

        return total;
    }

    private static void Part_1_Test_1()
    {
        List<Pattern> list = new List<Pattern>();
        int id = 1;

        var p1 = new Pattern(id++, new List<string>
        {
            "#.##..##.",
            "..#.##.#.",
            "##......#",
            "##......#",
            "..#.##.#.",
            "..##..##.",
            "#.#.##.#.",
        });

        var p2 = new Pattern(id++, new List<string>
        {
            "#...##..#",
            "#....#..#",
            "..##..###",
            "#####.##.",
            "#####.##.",
            "..##..###",
            "#....#..#",
        });

        list.Add(p1);
        list.Add(p2);

        ulong total = solve_part_1(list);
        Console.WriteLine($"Part_1_Test_1: {total}");
    }

}