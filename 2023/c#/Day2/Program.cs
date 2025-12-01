using Utilities;
using System.Linq;

class Hand
{
    public string color { get; set; }
    public int count { get; set; }

    public Hand(string color, int count)
    {
        this.color = color;
        this.count = count;
    }
}

class Set
{
    public List<Hand> hands { get; set; }

    public Set()
    {
        this.hands = new List<Hand>();
    }
}

class Game
{
    public int id { get; set; }
    public List<Set> sets { get; set; }
    public bool IsValid { get; set; }

    public int power { get; set; }

    public Game(int id)
    {
        this.id = id;
        this.sets = new List<Set>();
        this.IsValid = true;
    }
    public void CalculatePower()
    {
        Dictionary<string, int> map = new Dictionary<string, int>();

        foreach (Set set in this.sets)
        {
            foreach (Hand hand in set.hands)
            {
                if (!map.ContainsKey(hand.color))
                {
                    map[hand.color] = hand.count;
                    continue;
                }

                if (map[hand.color] < hand.count)
                {
                    map[hand.color] = hand.count;
                }
            }
        }

        int power = 0;
        foreach (var item in map)
        {
            if (power == 0)
            {
                power = item.Value;
                continue;
            }
            power *= item.Value;
        }

        this.power = power;
    }

    public void ValidateGame(Dictionary<string, int> ruleSet)
    {
        foreach (Set set in this.sets)
        {
            foreach (Hand hand in set.hands)
            {
                if (hand.count > ruleSet[hand.color])
                {
                    this.IsValid = false;
                    break;
                }
            }
        }
    }
}


internal class Program
{
    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day2.txt");

        var lines = FileHandling.ReadInputFile(path);
        var games = GetGames(lines);

        // Game configuration
        Dictionary<string, int> RuleSet = new Dictionary<string, int>()
        {
            { "red", 12 },
            { "green", 13 },
            { "blue", 14 },
        };

        // Validate the games
        foreach (Game game in games)
        {
            game.ValidateGame(RuleSet);
        }

        var gameQuery = from game in games
                        where game.IsValid
                        select game;

        var idSum = gameQuery.Sum(g => g.id);
        Console.WriteLine($"Sum of valid game ids: {idSum}");

        // Part 1 : 2776

        // ---- Part 2 ----

        var part2Query = from game in games
                        select game;

        foreach (Game game in part2Query)
        {
            game.CalculatePower();
            Console.WriteLine($"Game {game.id} | Sets: {game.sets.Count} | Is Valid: {game.IsValid} | Power: {game.power}");
        }

        var powerSum = part2Query.Sum(g => g.power);
        Console.WriteLine($"Sum of all games power: {powerSum}");

        // Part 1 : 16965 - Too Low // Was only looking at valid games, but should have been looking at all games
        // Part 1 : 68638

    }

    private static List<Game> GetGames(List<string> lines)
    {
        List<Game> games = new List<Game>();

        foreach (string line in lines)
        {
            // Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green

            string[] parts = line.Split(':');

            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid line: " + line);
                continue; // Skip this line, it's not valid
            }

            int id = int.Parse(parts[0].Trim().Split(' ')[1]);  // Retrieve the game id

            Game game = new Game(id);

            // Retrieve the sets
            // 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            string[] setPairs = parts[1].Trim().Split(';');

            foreach (string sets in setPairs)
            {
                Set set = new Set();

                // 3 blue, 4 red
                foreach (var h in sets.Trim().Split(','))
                {
                    // 3 blue
                    var components = h.Trim().Split(' ');

                    Hand hand = new (components[1], int.Parse(components[0]));
                    
                    set.hands.Add(hand);                    
                }

                game.sets.Add(set);
            }

            games.Add(game);
        }

        return games;
    }   

}