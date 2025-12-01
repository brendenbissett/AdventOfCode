using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Utilities;

internal class Program
{
    enum VisualizationType
    {
        Ground,
        Pipe,
        Start,
        Water
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day10.txt");
        var lines = FileHandling.ReadInputFile(path);

        List<List<char>> map = new List<List<char>>();

        foreach (var line in lines)
        {
            map.Add(line.ToList<char>());
        }

        //Console.WriteLine($"Map Rows: {map.Count}");
        //Console.WriteLine($"Map Columns: {map[0].Count}");

        ulong furthest_steps = Part_1_Solution(map, 'S');
        Console.WriteLine($"Part_1 - Furthest steps: {furthest_steps}");  // 6856


        // Part 2
        ulong size_of_enclosed_area = Part_2_Solution(map, 'S');
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");  // 2287 -- Too High
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");  // -- 768 -- Too High
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");  // -- 766 -- Wrong
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");  // -- 751 -- Wrong
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");    // 748 -- Wrong
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");    // 732 -- Wrong
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}"); // 714 -- Wrong
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}"); // 712 -- Wrong
        //Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}"); // 711 -- Wrong
        Console.WriteLine($"Part_2 - Size of enclosed area: {size_of_enclosed_area}");

        // Manually counted based on the image jpeg generated.
        // Answer: 501

        // Testing
        Console.WriteLine();
        Console.WriteLine("-------------- TESTS --------------");
        Console.WriteLine();
        //Part_1_Test();
        //Part_2_Test_1();
        //Part_2_Test_2();
        //Part_2_Test_3();
    }

    /*
    | is a vertical pipe connecting north and south.
    - is a horizontal pipe connecting east and west.
    L is a 90-degree bend connecting north and east.
    J is a 90-degree bend connecting north and west.
    7 is a 90-degree bend connecting south and west.
    F is a 90-degree bend connecting south and east.
    . is ground; there is no pipe in this tile.
    S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has. 
    */

    private static ulong Part_1_Solution(List<List<char>> map, char start)
    {
        ulong result = 0;

        // Get starting position
        var start_row = map.FindIndex(x => x.Contains(start));
        var start_col = map[start_row].FindIndex(x => x == start);

        // We have 4 directions: N, E, S, W (Only 2 directions are valid at any given time, since they come back to the same starting position)
        foreach(char heading in new List<char> { 'N', 'E', 'S', 'W'})
        {
            (var journey, var points) = map_journey(map, start_row, start_col, heading);

            if (journey.Values.Count > 3)  // TODO: Confirm this magic number :)
            {
                if (journey.Values.Last() == 'S')
                {
                    result = Convert.ToUInt64(journey.Values.Count) - 1;
                    break; // Found the path.
                }   
            }
        }   
        
        return result / 2;
    }

    private static ulong Part_2_Solution(List<List<char>> map, char start)
    {

        // Get starting position
        var start_row = map.FindIndex(x => x.Contains(start));
        var start_col = map[start_row].FindIndex(x => x == start);

        Dictionary<string, char> our_journey = new Dictionary<string, char>();
        List<Point> our_points = new List<Point>();

        // We have 4 directions: N, E, S, W (Only 2 directions are valid at any given time, since they come back to the same starting position)
        foreach (char heading in new List<char> { 'N', 'E', 'S', 'W' })
        {
            (var journey, var points) = map_journey(map, start_row, start_col, heading);

            if (journey.Count > 3)  // TODO: Confirm this magic number :)
            {
                if (journey.Values.Last() == 'S')
                {
                    our_journey = journey;
                    our_points = points;
                    break; // Found the path.
                }
            }
        }

        // Steps
        remove_noise(map, our_journey);
        add_border_around_map(map, '.');
        flood_fill_map(map, new Point(1, 1), '.', '0');

        squeeze_through_pipes(map);
        squeeze_through_pipes(map);

        while (true)
        {
            var cells_flooded = check_for_missing_flood(map);
            if (cells_flooded == 0) break;
        }

        // Visualize the map
        visualize_map(map);
        create_map_image(map, our_points);

        // Count the ground left
        List<char> list = new List<char>() {};

         foreach (var row in map)
        {
            foreach (var col in row)
            {
                list.Add(col);
            }
        }

        return (UInt64)list.Where(x => x == '.').ToList().Count;// TODO - Still need to count
    }

    private static void remove_noise(List<List<char>> map, Dictionary<string, char> our_journey)
    {
        int max_row = map.Count;
        int max_col = map[0].Count;

        for (int row = 0; row < max_row; row++)
        {
            for (int col = 0; col < max_col; col++)
            {
                var key = $"{row}_{col}";
                var start_key = $"{row}_{col}_START";
                var end_key = $"{row}_{col}_END";

                if (our_journey.ContainsKey(key) || our_journey.ContainsKey(start_key) || our_journey.ContainsKey(end_key))
                {
                    char val = '#';
                    if (our_journey.ContainsKey(key))
                    {
                        val = our_journey[key];
                    }
                    else if (our_journey.ContainsKey(start_key))
                    {
                        val = our_journey[start_key];
                    }
                    else
                    {
                        val = our_journey[end_key];
                    }
                    map[row][col] = val;
                }
                else
                {
                    map[row][col] = '.';
                }
            }
        }
    }

    private static void visualize_map(List<List<char>> map)
    {
        Console.WriteLine();

        foreach (var row in map)
        {
            foreach (var col in row)
            {
                switch(col)
                {
                    case '0':
                        write_item_to_console(col, VisualizationType.Water);
                        break;

                    case '.':
                        //write_item_to_console(col, VisualizationType.Ground);
                        write_item_to_console('+', VisualizationType.Ground);
                        break;

                    case 'S':
                        write_item_to_console(col, VisualizationType.Start);
                        break;

                    case '|':
                    case '-':
                    case 'L':
                    case 'J':
                    case '7':
                    case 'F':
                        write_item_to_console(col, VisualizationType.Pipe);
                        break;

                    default:
                        Console.Write(col);
                        break;
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine();

    }

    private static void create_map_image(List<List<char>> map, List<Point> our_journey)
    {
        int width = map[0].Count;
        int height = map.Count;
        Bitmap image = new Bitmap(2560, 1440);

        int pixel_width = 2560 / width;
        int pixel_height = 1440 / height;


        Pen blue_pen = new Pen(Color.Blue, 1);
        Pen red_pen = new Pen(Color.PaleVioletRed, 3);
        Pen gray_pen = new Pen(Color.LightGray, 1);
        Pen green_pen = new Pen(Color.LimeGreen, 1);
        Pen white_pen = new Pen(Color.FloralWhite, 1);

        bool first_point = true;
        Point previous_point = new Point(0, 0);

        var graphics = Graphics.FromImage(image);

        graphics.Clear(Color.SlateGray);
        

        // Draw the journey
        Brush b_journey = new SolidBrush(Color.White);
        List<Point> scaled_points = new List<Point>();
        foreach (Point p in our_journey)
        {
            if (first_point)
            {
                previous_point = p;
                first_point = false;
                continue;
            }

            // Create points that define line.   
            Point point1 = new Point(previous_point.X * pixel_width, previous_point.Y * pixel_height);
            Point point2 = new Point(p.X * pixel_width, p.Y * pixel_height);

            scaled_points.Add(point1);
            scaled_points.Add(point2);

            // Draw line to screen.   
            graphics.DrawLine(white_pen, point1, point2);

            previous_point = p;
        }

        graphics.FillClosedCurve(b_journey, scaled_points.ToArray());
        //graphics.DrawClosedCurve(white_pen, scaled_points.ToArray(), 1, FillMode.Winding);
        
        

        // Draw the map
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                switch (map[row][col])
                {
                    case '0':
                        // Draw water on the image.
                        //Rectangle water = new Rectangle(col * pixel_width - (pixel_width / 2) - 5, row * pixel_height - (pixel_height / 2) - 5 , (pixel_width/2), (pixel_height / 2));
                        //graphics.DrawRectangle(blue_pen, water);
                        break;

                    case '.':
                        Rectangle ground = new Rectangle(col * pixel_width - (pixel_width / 2) - 5, row * pixel_height - (pixel_height / 2) -5, 1, 1);
                        graphics.DrawRectangle(red_pen, ground);
                        break;

                    case 'S':
                        //Rectangle starting_point = new Rectangle(col * pixel_width - (pixel_width / 2) - 5, row * pixel_height - (pixel_height / 2) - 5, 5, 5);
                        //graphics.DrawRectangle(green_pen, starting_point);
                        break;

                    case '|':
                    case '-':
                    case 'L':
                    case 'J':
                    case '7':
                    case 'F':
                        //RectangleF pipe = new RectangleF(col * pixel_width, row * pixel_height, pixel_width - 2.0f, pixel_height - 2.0f);
                        //graphics.DrawRectangle(gray_pen, pipe);
                        break;

                    default:
                        break;
                }
            }
        }

        // Flood the image
        //flood_fill_image(image, new Point(1, 1), Color.PaleVioletRed, Color.Blue);

        // Save the image to a file
        string filePath = "map.jpg";
        image.Save(filePath);

        // Dispose of the graphics object and image
        graphics.Dispose();
        image.Dispose();

        Console.WriteLine("Image generated and saved successfully.");
    }

    private static void add_border_around_map(List<List<char>> map, char character)
    {
        // Add border
        foreach (var row in map)
        {
            row.Insert(0, character);
            row.Add(character);
        }
        List<char> border = Enumerable.Repeat<char>(character, map[0].Count).ToList<char>();

        map.Insert(0, border);
        map.Add(border);
    }

    private static void flood_fill_map(List<List<char>> map, Point start_point, char target_char, char fill_char)
    {
        int max_x = map.Count;
        int max_y = map[0].Count;

        Stack<Point> pixels = new Stack<Point>();  // Helps to think in terms of "pixels"

        pixels.Push(start_point);

        while(pixels.Count > 0)
        {
            var point = pixels.Pop();

            // TODO: Check whether we should be looking at >= instead of > here
            if(point.X < max_x && point.X > 0 && point.Y < max_y && point.Y > 0)
            {
                // We are still in the map
                if (map[point.X][point.Y] == target_char)
                {
                    map[point.X][point.Y] = fill_char;

                    if (point.X > 0) pixels.Push(new Point(point.X - 1, point.Y));
                    if (point.X < max_x - 1) pixels.Push(new Point(point.X + 1, point.Y));
                    if (point.Y > 0) pixels.Push(new Point(point.X, point.Y - 1));
                    if (point.Y < max_y - 1) pixels.Push(new Point(point.X, point.Y + 1));
                }              

                // TODO: Should we worry about diagonals?
            }

        }

    }

    private static void flood_fill_image(Bitmap bmp, Point pt, Color target_color, Color replacement_color)
    {
        Stack<Point> pixels = new Stack<Point>();  // Helps to think in terms of "pixels"
        pixels.Push(pt);

        while (pixels.Count > 0)
        {
            var point = pixels.Pop();

            if (point.X < bmp.Width && point.X > 0 && point.Y < bmp.Height && point.Y > 0)
            {
                // We are still in the map
                if (bmp.GetPixel(point.X, point.Y) == target_color)
                {
                    bmp.SetPixel(point.X, point.Y, replacement_color);

                    if (point.X > 0) pixels.Push(new Point(point.X - 1, point.Y));
                    if (point.X < bmp.Width - 1) pixels.Push(new Point(point.X + 1, point.Y));
                    if (point.Y > 0) pixels.Push(new Point(point.X, point.Y - 1));
                    if (point.Y < bmp.Height - 1) pixels.Push(new Point(point.X, point.Y + 1));
                }

                // TODO: Should we worry about diagonals?
            }

        }

    }

    private static void squeeze_through_bottom_up(List<List<char>> map)
    {
        int max_rows = map.Count;
        int max_columns = map[0].Count;

        // BOTTOM UP
        for (int col = 0; col < max_columns; col++)
        {

            // Looking for a pipe that is not a dead end, and has water on both sides
            // 7F
            // 7|
            // ||
            // |F
            // JL

            List<char> chars = new List<char>();
            for (int row = 0; row < max_rows; row++)
            {
                chars.Add(map[row][col]);
            }

            char previous_char = '0';
            bool is_dead_end = true;

            for (int row = max_rows - 1; row >= 0; row--)
            {
                if (col + 1 >= max_columns) break; // Can't compare to the right

                // Compare bottom
                if (previous_char == '-') is_dead_end = true;
                if (previous_char == 'L') is_dead_end = true;
                if (previous_char == 'F') is_dead_end = true;
                if (previous_char == 'S') is_dead_end = true;

                // Consider current tile
                if (map[row][col] == 'F') is_dead_end = true;
                if (map[row][col] == 'L') is_dead_end = true;
                if (map[row][col] == '-') is_dead_end = true;

                // Negate by character next to us (right hand side)
                if (map[row][col] == '7' && map[row][col + 1] == 'J') is_dead_end = true;
                if (map[row][col] == 'J' && map[row][col + 1] == '7') is_dead_end = true;


                // Enable if required
                if (map[row][col] == '7' && map[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == '7' && map[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == '7' && map[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == '7' && map[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                if (map[row][col] == '|' && map[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == '|' && map[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == '|' && map[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == '|' && map[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                if (map[row][col] == 'J' && map[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == 'J' && map[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == 'J' && map[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (map[row][col] == 'J' && map[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                // Reset if we hit water again
                if (map[row][col] == '0') is_dead_end = false;

                // Change to water if needed
                if (map[row][col] == '.' && !is_dead_end)
                {
                    map[row][col] = '0'; // Change it :)
                }

                if (map[row][col + 1] == '.' && !is_dead_end)
                {
                    map[row][col + 1] = '0'; // Change it :)
                }

                previous_char = map[row][col];
            }
        }
    }

    private static void squeeze_through_pipes(List<List<char>> map) {

        List<List<char>> result = map;

        int max_rows = result.Count;
        int max_columns = result[0].Count;

        // BOTTOM UP
        for(int col = 0; col < max_columns; col++)
        {

            // Looking for a pipe that is not a dead end, and has water on both sides
            // 7F
            // 7|
            // ||
            // |F
            // JL

            List<char> chars = new List<char>();
            for (int row = 0; row < max_rows; row++)
            {
                chars.Add(result[row][col]);
            }

            char previous_char = '0';
            bool is_dead_end = true;

            for (int row = max_rows - 1; row >= 0; row--)
            {
                if (col + 1 >= max_columns) break; // Can't compare to the right

                // Compare bottom
                if (previous_char == '-') is_dead_end = true;
                if (previous_char == 'L') is_dead_end = true;
                if (previous_char == 'F') is_dead_end = true;
                if (previous_char == 'S') is_dead_end = true;

                // Consider current tile
                if (result[row][col] == 'F') is_dead_end = true;
                if (result[row][col] == 'L') is_dead_end = true;
                if (result[row][col] == '-') is_dead_end = true;

                // Negate by character next to us (right hand side)
                if (result[row][col] == '7' && result[row][col + 1] == 'J') is_dead_end = true;
                if (result[row][col] == 'J' && result[row][col + 1] == '7') is_dead_end = true;

                
                // Enable if required
                if (result[row][col] == '7' && result[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '7' && result[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '7' && result[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '7' && result[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == '|' && result[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '|' && result[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '|' && result[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '|' && result[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == 'J' && result[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                // Reset if we hit water again
                if (result[row][col] == '0') is_dead_end = false;

                // Change to water if needed
                if (result[row][col] == '.' && !is_dead_end)
                {
                    result[row][col] = '0'; // Change it :)
                }

                if (result[row][col+1] == '.' && !is_dead_end)
                {
                    result[row][col+1] = '0'; // Change it :)
                }

                previous_char = result[row][col];
            }
        }

        // TOP DOWN
        for (int col = 0; col < max_columns; col++)
        {
            List<char> chars = new List<char>();
            for (int row = 0; row < max_rows; row++)
            {
                chars.Add(result[row][col]);
            }

            // Looking for a pipe that is not a dead end, and has water on both sides
            // 7F
            // 7|
            // ||
            // |F
            // JL

            char previous_char = '0';
            bool is_dead_end = true;

            for (int row = 0; row < max_rows; row++)
            {
                if (col + 1 >= max_columns) break; // Can't compare to the right

                // Compare bottom
                if (previous_char == '-') is_dead_end = true;
                if (previous_char == 'F') is_dead_end = true;
                if (previous_char == 'L') is_dead_end = true;
                if (previous_char == 'S') is_dead_end = true;

                // consider current tile
                if (result[row][col] == 'F') is_dead_end = true;
                if (result[row][col] == 'L') is_dead_end = true;
                if (result[row][col] == '-') is_dead_end = true;

                // Negate
                if (result[row][col] == '7' && result[row][col + 1] == 'J') is_dead_end = true;
                if (result[row][col] == 'J' && result[row][col + 1] == '7') is_dead_end = true;

                // Enable if required
                if (result[row][col] == '7' && result[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '7' && result[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '7' && result[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '7' && result[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == '|' && result[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '|' && result[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '|' && result[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '|' && result[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == 'J' && result[row][col + 1] == 'L' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row][col + 1] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row][col + 1] == '|' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row][col + 1] == '0' && !is_dead_end) is_dead_end = false;


                // Reset if we hit water again
                if (result[row][col] == '0') is_dead_end = false;

                // Change to water if needed
                if (result[row][col] == '.' && !is_dead_end)
                {
                    result[row][col] = '0'; // Change it :)
                }

                if (result[row][col + 1] == '.' && !is_dead_end)
                {
                    result[row][col + 1] = '0'; // Change it :)
                }

                previous_char = result[row][col];
            }
        }


        // LEFT TO RIGHT
        for (int row = 0; row < max_rows; row++)
        {
            List<char> chars = new List<char>();
            for (int col = 0; col < max_columns; col++)
            {
                chars.Add(result[row][col]);
            }

            // Looking for a pipe that is not a dead end, and has water on both sides
            // L    J   -
            // F    7   -

            char previous_char = '0';
            bool is_dead_end = true;

            for (int col = 0; col < max_columns; col++)
            {
                if (row + 1 >= max_rows) break; // Can't compare to the bottom

                // Compare left
                if (previous_char == '|') is_dead_end = true;
                if (previous_char == '7') is_dead_end = true;
                if (previous_char == 'F') is_dead_end = true;
                if (previous_char == 'S') is_dead_end = true;

                // Negate
                if (result[row][col] == 'L' && result[row + 1][col] == 'L') is_dead_end = true;
                if (result[row][col] == 'L' && result[row + 1][col] == 'J') is_dead_end = true;
                if (result[row][col] == 'L' && result[row + 1][col] == '|') is_dead_end = true;

                if (result[row][col] == '-' && result[row + 1][col] == 'J') is_dead_end = true;
                if (result[row][col] == '-' && result[row + 1][col] == 'L') is_dead_end = true;
                if (result[row][col] == '-' && result[row + 1][col] == '|') is_dead_end = true;


                // Compare to the bottom
                if (result[row][col] == 'L' && result[row + 1][col] == '7' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'L' && result[row + 1][col] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'L' && result[row + 1][col] == '-' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'L' && result[row + 1][col] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == 'J' && result[row + 1][col] == '7' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row + 1][col] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row + 1][col] == '-' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row + 1][col] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == '-' && result[row + 1][col] == '-' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '-' && result[row + 1][col] == '7' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '-' && result[row + 1][col] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '-' && result[row + 1][col] == '0' && !is_dead_end) is_dead_end = false;

                // Reset if we hit water again
                if (result[row][col] == '0') is_dead_end = false;

                // Change to water if needed
                if (result[row][col] == '.' && !is_dead_end)
                {
                    result[row][col] = '0'; // Change it :)
                }

                if (result[row + 1][col] == '.' && !is_dead_end)
                {
                    result[row + 1][col] = '0'; // Change it :)
                }

                previous_char = result[row][col];
            }

        }

        // RIGHT TO LEFT
        for (int row = 0; row < max_rows; row++)
        {
            List<char> chars = new List<char>();
            for (int col = max_rows - 1; col >= 0; col--)
            {
                chars.Add(result[row][col]);
            }

            // Looking for a pipe that is not a dead end, and has water on both sides
            // L    J   -
            // F    7   -

            char previous_char = '0';
            bool is_dead_end = true;

            for (int col = max_rows - 1; col >= 0; col--)
            {
                if (row + 1 >= max_rows) break; // Can't compare to the bottom

                // Compare right
                if (previous_char == '|') is_dead_end = true;
                if (previous_char == '7') is_dead_end = true;
                if (previous_char == 'F') is_dead_end = true;
                if (previous_char == 'S') is_dead_end = true;

                // Negate (Compar to next character in row)
                if (result[row][col] == 'L' && result[row + 1][col] == '|') is_dead_end = true;
                if (result[row][col] == 'L' && result[row + 1][col] == 'J') is_dead_end = true;
                if (result[row][col] == 'L' && result[row + 1][col] == 'L') is_dead_end = true;

                if (result[row][col] == 'J' && result[row + 1][col] == 'L') is_dead_end = true;
                if (result[row][col] == 'J' && result[row + 1][col] == '|') is_dead_end = true;
                if (result[row][col] == 'J' && result[row + 1][col] == 'J') is_dead_end = true;

                if (result[row][col] == '-' && result[row + 1][col] == 'J') is_dead_end = true;
                if (result[row][col] == '-' && result[row + 1][col] == 'L') is_dead_end = true;
                if (result[row][col] == '-' && result[row + 1][col] == '|') is_dead_end = true;


                // Compare to the bottom
                if (result[row][col] == 'L' && result[row + 1][col] == '7' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'L' && result[row + 1][col] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'L' && result[row + 1][col] == '-' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'L' && result[row + 1][col] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == 'J' && result[row + 1][col] == '7' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row + 1][col] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row + 1][col] == '-' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == 'J' && result[row + 1][col] == '0' && !is_dead_end) is_dead_end = false;

                if (result[row][col] == '-' && result[row + 1][col] == '-' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '-' && result[row + 1][col] == '7' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '-' && result[row + 1][col] == 'F' && !is_dead_end) is_dead_end = false;
                if (result[row][col] == '-' && result[row + 1][col] == '0' && !is_dead_end) is_dead_end = false;

                // Reset if we hit water again
                if (result[row][col] == '0') is_dead_end = false;

                // Change to water if needed
                if (result[row][col] == '.' && !is_dead_end)
                {
                    result[row][col] = '0'; // Change it :)
                }

                if (result[row + 1][col] == '.' && !is_dead_end)
                {
                    result[row + 1][col] = '0'; // Change it :)
                }

                previous_char = result[row][col];
            }

        }

    }

    private static int check_for_missing_flood(List<List<char>> map)
    {
        int cells_flooded = 0;

        // Start flooding
        int max_rows = map.Count;
        int max_columns = map[0].Count;

        // LEFT TO RIGHT
        for (int col = 0; col < max_columns; col++)
        {
            for (int row = 0; row < max_rows; row++)
            {
                //if (map[row][col] == '#') {
                if (map[row][col] != '.')
                {
                    continue; // Skip
                }

                List<(int x, int y)> surrounding_coordinates = get_surrounding_coordinates(max_rows, max_columns, row, col);

                foreach (var coordinate in surrounding_coordinates)
                {
                    if (map[coordinate.x][coordinate.y] == '0')
                    {
                        map[row][col] = '0';
                        cells_flooded++;
                        break;
                    }
                }
            }

        }

        return cells_flooded;
    }

    private static void write_item_to_console(char character, VisualizationType type)
    {
        var backup_color = System.Console.ForegroundColor;

        switch (type)
        {
            case VisualizationType.Ground:
                //Console.ForegroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case VisualizationType.Pipe:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case VisualizationType.Start:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case VisualizationType.Water:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
        }

        Console.Write(character);

        Console.ForegroundColor = backup_color;
    }

    private static List<(int x, int y)> get_surrounding_coordinates(int max_rows, int max_cols, int row, int col)
    {
        List<(int x, int y)> coordinates = new List<(int x, int y)>();
       
        // Left-top
        if (row > 0 && col > 0)
        {
            coordinates.Add((row - 1, col - 1));
        }

        // Top
        if (row > 0)
        {
            coordinates.Add((row - 1, col));
        }

        // Right-top
        if (row > 0 && col < max_cols - 1)
        {
            coordinates.Add((row - 1, col + 1));
        }

        // Right
        if (col < max_cols - 1)
        {
            coordinates.Add((row, col + 1));
        }

        // Right-bottom
        if (row < max_rows - 1 && col < max_cols - 1)
        {
            coordinates.Add((row + 1, col + 1));
        }

        // Bottom
        if (row < max_rows - 1)
        {
            coordinates.Add((row + 1, col));
        }

        // Left-bottom
        if (row < max_rows - 1 && col > 0)
        {
            coordinates.Add((row + 1, col - 1));
        }

        // Left
        if (col > 0)
        {
            coordinates.Add((row, col - 1));
        }

        return coordinates;
    }
   
    // Returns list of characters followed, and the list of coordinates
    private static (Dictionary<string, char>, List<Point>) map_journey(List<List<char>> map, int startRowIndex, int startColumnIndex, char heading)
    {
        Dictionary<string, char> journey = new Dictionary<string, char>();

        List<Point> points = new List<Point>();

        bool end = false;
        int max_row = map.Count;
        int max_col = map[0].Count;

        var source_character = map[startRowIndex][startColumnIndex];
        journey.Add($"{startRowIndex}_{startColumnIndex}_START", source_character);
        points.Add(new Point(startColumnIndex, startRowIndex));

        int current_row = startRowIndex;
        int current_col = startColumnIndex;

        while (!end)
        {
            // Figure out the next step on the map
            switch (heading)
            {
                case 'N':
                    current_row--;
                    break;
                case 'E':
                    current_col++;
                    break;
                case 'S':
                    current_row++;
                    break;
                case 'W':
                    current_col--;
                    break;
            }

            // Validate if we are still in the map
            if (current_row < 0 || current_row >= max_row || current_col < 0 || current_col >= max_col)
            {
                end = true;  // Out of bounds
                break;
            }

            // Check if current position is reachable from previous position
            if (!is_current_reachable_from_previous(map[current_row][current_col], get_source_direction_based_on_heading(heading), source_character))
            {
                end = true; // Not reachable
                break;
            }

            // Add current to the list of steps
            var journey_key = (source_character == 'S') ? $"{current_row}_{current_col}_END" : $"{current_row}_{current_col}";
            journey.Add(journey_key, map[current_row][current_col]);
            points.Add(new Point(current_col, current_row));
            source_character = map[current_row][current_col];

            // Check if we are back at the start
            if (source_character == 'S')
            {
                end = true;
                break;
            }

            // Get the next heading
            heading = get_next_heading(map[current_row][current_col], get_source_direction_based_on_heading(heading));
        }

        return (journey, points);
    }

    private static bool is_current_reachable_from_previous(char current, char source_direction, char source_character)
    {
        string key = $"{current}_from_{source_direction}";

        // For this to work, the key needs to be the current pipe, combined with the direction previous pipe was coming from
        Dictionary<string, List<char>> rules = new Dictionary<string, List<char>>()
        {
            {"|_from_S", new List<char> {'S', '|', 'L', 'J' } },
            {"|_from_N", new List<char> {'S', '|', '7', 'F' } },
            {"|_from_E", new List<char> {} },
            {"|_from_W", new List<char> {} },

            {"-_from_S", new List<char> {} },
            {"-_from_N", new List<char> {} },
            {"-_from_E", new List<char> {'S', '-', 'J', '7'} },
            {"-_from_W", new List<char> {'S', '-', 'L', 'F'} },

            {"L_from_S", new List<char> {} },
            {"L_from_N", new List<char> {'S', '|', 'F', '7'} },
            {"L_from_E", new List<char> {'S', '-', 'J', '7'} },
            {"L_from_W", new List<char> {} },


            {"J_from_S", new List<char> {} },
            {"J_from_N", new List<char> {'S', '|', '7', 'F' } },
            {"J_from_E", new List<char> {} },
            {"J_from_W", new List<char> {'S', '-', 'F', 'L'} },

            {"7_from_S", new List<char> {'S', '|', 'J', 'L'} },
            {"7_from_N", new List<char> {} },
            {"7_from_E", new List<char> {} },
            {"7_from_W", new List<char> {'S', '-', 'L', 'F'} },

            {"F_from_S", new List<char> {'S', '|', 'J', 'L'} },
            {"F_from_N", new List<char> {} },
            {"F_from_E", new List<char> {'S', '-', 'J', '7'} },
            {"F_from_W", new List<char> {} },

            {"._from_S", new List<char> {} },
            {"._from_N", new List<char> {} },
            {"._from_E", new List<char> {} },
            {"._from_W", new List<char> {} },

            {"S_from_S", new List<char> {'|', 'J', 'L'} },
            {"S_from_N", new List<char> {'|', '7', 'F'} },
            {"S_from_E", new List<char> {'-', 'J', '7'} },
            {"S_from_W", new List<char> {'-', 'F', 'L'} },

        };

        return rules[key].Contains(source_character);
    }

    private static char get_source_direction_based_on_heading(char heading)
    {
        char source_heading = 'X';
        switch (heading)
        {
            case 'N':
                source_heading = 'S';
                break;
            case 'E':
                source_heading = 'W';
                break;
            case 'S':
                source_heading = 'N';
                break;
            case 'W':
                source_heading = 'E';
                break;
        }

        return source_heading;
    }

    private static char get_next_heading(char pipe, char source_direction)
    {
        // Going to assume the the source direction has already been validated at this point.
        char next_heading = 'X'; 

        switch(pipe)
        {
            case '|': 
                next_heading = (source_direction == 'N') ? 'S' : 'N';
                break;
            case '-':
                next_heading = (source_direction == 'E') ? 'W' : 'E';
                break;
            case 'L':
                next_heading = (source_direction == 'N') ? 'E' : 'N';
                break;
            case 'J': 
                next_heading = (source_direction == 'N') ? 'W' : 'N';
                break;
            case '7':
                next_heading = (source_direction == 'W') ? 'S' : 'W';
                break;
            case 'F':
                next_heading = (source_direction == 'E') ? 'S' : 'E';
                break;
            case '.':
                next_heading = 'X'; // This is a dead end. Leads nowhere.
                break;
            case 'S': 
                next_heading = 'X'; // This is the starting point. IE, you are back at the start, where you began.
                break;
        }

        return next_heading;
    }


    // ------------------ TESTS ------------------

    private static void Part_1_Test()
    {
        List<List<char>> map = new List<List<char>>()
        {
            new List<char> {'.', '.', 'F', '7', '.' },
            new List<char> {'.', 'F', 'J', '|', '.' },
            new List<char> {'S', 'J', '.', 'L', '7' },
            new List<char> {'|', 'F', '-', '-', 'J' },
            new List<char> {'L', 'J', '.', '.', '.' },
        };

        ulong furthest_steps = Part_1_Solution(map, 'S');

        Console.WriteLine($"Part_1_Test() - Furthest steps: {furthest_steps}");
        Console.WriteLine();
    }

    private static void Part_2_Test_1()
    {
        List<List<char>> map = new List<List<char>>()
        {
            "..........".ToCharArray().ToList<char>(),
            ".S------7.".ToCharArray().ToList<char>(),
            ".|F----7|.".ToCharArray().ToList<char>(),
            ".||OOOO||.".ToCharArray().ToList<char>(),
            ".||OOOO||.".ToCharArray().ToList<char>(),
            ".|L-7F-J|.".ToCharArray().ToList<char>(),
            ".|II||II|.".ToCharArray().ToList<char>(),
            ".L--JL--J.".ToCharArray().ToList<char>(),
            "..........".ToCharArray().ToList<char>(),
        };

        ulong size_of_enclosed_area = Part_2_Solution(map, 'S');

        Console.WriteLine($"Part_2_Test_1() - Size of enclosed area: {size_of_enclosed_area}");
        Console.WriteLine();
    }

    private static void Part_2_Test_2()
    {
        List<List<char>> map = new List<List<char>>()
        {
            ".F----7F7F7F7F-7....".ToCharArray().ToList<char>(),
            ".|F--7||||||||FJ....".ToCharArray().ToList<char>(),
            ".||.FJ||||||||L7....".ToCharArray().ToList<char>(),
            "FJL7L7LJLJ||LJ.L-7..".ToCharArray().ToList<char>(),
            "L--J.L7...LJS7F-7L7.".ToCharArray().ToList<char>(),
            "....F-J..F7FJ|L7L7L7".ToCharArray().ToList<char>(),
            "....L7.F7||L7|.L7L7|".ToCharArray().ToList<char>(),
            ".....|FJLJ|FJ|F7|.LJ".ToCharArray().ToList<char>(),
            "....FJL-7.||.||||...".ToCharArray().ToList<char>(),
            "....L---J.LJ.LJLJ...".ToCharArray().ToList<char>(),
        };

        ulong size_of_enclosed_area = Part_2_Solution(map, 'S');

        Console.WriteLine($"Part_2_Test_2() - Size of enclosed area: {size_of_enclosed_area}");
        Console.WriteLine();
    }

    private static void Part_2_Test_3()
    {
        List<List<char>> map = new List<List<char>>()
        {
            "FF7FSF7F7F7F7F7F---7".ToCharArray().ToList<char>(),
            "L|LJ||||||||||||F--J".ToCharArray().ToList<char>(),
            "FL-7LJLJ||||||LJL-77".ToCharArray().ToList<char>(),
            "F--JF--7||LJLJ7F7FJ-".ToCharArray().ToList<char>(),
            "L---JF-JLJ.||-FJLJJ7".ToCharArray().ToList<char>(),
            "|F|F-JF---7F7-L7L|7|".ToCharArray().ToList<char>(),
            "|FFJF7L7F-JF7|JL---7".ToCharArray().ToList<char>(),
            "7-L-JL7||F7|L7F-7F7|".ToCharArray().ToList<char>(),
            "L.L7LFJ|||||FJL7||LJ".ToCharArray().ToList<char>(),
            "L7JLJL-JLJLJL--JLJ.L".ToCharArray().ToList<char>(),
        };

        ulong size_of_enclosed_area = Part_2_Solution(map, 'S');

        Console.WriteLine($"Part_2_Test_3() - Size of enclosed area: {size_of_enclosed_area}");
        Console.WriteLine();
    }

}