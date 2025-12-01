namespace Utilities
{
    public class FileHandling
    {
        public static List<string> ReadInputFile(string path)
        {

            //string[] paths = {@"d:\archives", "2001", "media", "images"};
            //string fullPath = Path.Combine(paths);
            //Console.WriteLine(fullPath);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var lines = new List<string>();
            using (var reader = new StreamReader(path))
            {

                while (reader.Peek() >= 0)
                {
                    lines.Add(reader.ReadLine());
                }
            }

            return lines;
        }
    }
}
