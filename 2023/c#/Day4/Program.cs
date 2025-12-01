using System.Collections;
using Utilities;

internal class Program
{
    class Card
    {
        public string Name { get; set; }
        public List<int> WinningNumbers { get; set; }
        public List<int> NumbersOnCard { get; set; }
        public List<int> MatchingNumbers { get; set; }

        public int Score { get; set; }

        public Card(string name, List<int> winningNumbers, List<int> numbersOnCard)
        {
            this.Name = name.Trim();
            this.WinningNumbers = winningNumbers;
            this.NumbersOnCard = numbersOnCard;
            this.MatchingNumbers = new List<int>();
            this.Score = 0;

            CalculateScore();
        }

        public void CalculateScore()
        {
            int score = 0;

            //foreach (int number in NumbersOnCard.Select(x => x).Distinct())
            foreach (int number in NumbersOnCard)
            {
                if (WinningNumbers.Contains(number))
                {
                    this.MatchingNumbers.Add(number);

                    if (score == 0)
                    {
                        score = 1;
                        continue;
                    }

                    score *= 2;
                }
            }
            this.Score = score;
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day4.txt");

        var lines = FileHandling.ReadInputFile(path);

        List<Card> cards = new List<Card>();
        foreach (string line in lines)
        {
            // Get the card name
            string name = line.Split(':')[0].Trim();

            // Get the winning numbers
            string winningNumbers = line.Split(':')[1].Trim().Split('|')[0].Trim();
            winningNumbers = winningNumbers.Replace("  ", " ");

            // Get the numbers on the card
            string cardNumbers = line.Split(':')[1].Trim().Split('|')[1].Trim();
            cardNumbers = cardNumbers.Replace("  ", " ");

            Card c = new Card(
                name,
                winningNumbers.Split(' ').Select(x => int.Parse(x)).ToList(),
                cardNumbers.Split(' ').Select(x => int.Parse(x)).ToList());
            
            cards.Add(c);
        }

        // Calculate the total
        int total = 0;
        foreach(Card c in cards)
        {
            //Console.WriteLine($"{c.Name} - {c.Score} - {string.Join(", ", c.WinningNumbers)} | {string.Join(", ", c.NumbersOnCard)} === {string.Join(", ", c.MatchingNumbers)}");
            total += c.Score;
        }
        Console.WriteLine($"Total score: {total}");  
        // Part 1 : 26914


        // -- Part 2 --
        CalculateNumberOfCardsWon(cards);  
        // Part 2 : 416 -- Too Low
        // Part 2 : 13080971

    }

    private static void CalculateNumberOfCardsWon(List<Card> original)
    {
        Dictionary<string, List<Card>> copies = new Dictionary<string, List<Card>>();
        Dictionary<string, List<Card>> cardsAddedBy = new Dictionary<string, List<Card>>();

        int max = original.Count;

        for(int c = 0; c < max; c++)
        {
            Card card = original[c];

            List<Card> cardsToAdd = new List<Card>();

            for (int i = 1; i <= card.MatchingNumbers.Count; i++)
            {
                if (c + i >= max) break; // TODO: Should log this as an error

                cardsToAdd.Add(original[c + i]);
                if (!cardsAddedBy.ContainsKey(card.Name))
                {
                    cardsAddedBy.Add(card.Name, new List<Card>());
                }
                cardsAddedBy[card.Name].Add(original[c + i]);

                if (!copies.ContainsKey(original[c + i].Name))
                {
                    copies.Add(original[c + i].Name, new List<Card>());
                }

                copies[original[c + i].Name].Add(original[c + i]); // Add next card(s)
            }

            // Check if there are copied cards to process
            if (cardsToAdd.Count == 0) continue;

            if (copies.ContainsKey(card.Name))
            {
                int numberOfCopies = copies[card.Name].Count;

                for(int i = 0; i < numberOfCopies; i++)
                {
                    foreach (Card lazyCard in cardsToAdd) // Already identified the cards to add above
                    {
                        // Just add the cards again
                        copies[lazyCard.Name].Add(lazyCard);
                    }
                }
            }
        }   

        int qtyOriginalCards = original.Count;

        int qtyCopyCards = 0;
        foreach(string key in copies.Keys)
        {
            qtyCopyCards += copies[key].Count;
        }

        Console.WriteLine($"Copied Cards for Card 4 == {copies["Card   4"].Count}"); 
        /*
        int counter = 1;
        foreach (Card card in original)
        {

            if (cardsAddedBy.ContainsKey(card.Name))
            {
                Console.WriteLine($"{counter++} - Original - {card.Name} ({card.MatchingNumbers.Count}) - [ {string.Join(", ", cardsAddedBy[card.Name].Select(x => x.Name))} ]");
            } else
            {
                Console.WriteLine($"{counter++} - Original - {card.Name} ({card.MatchingNumbers.Count}) - [ ]");
            }

            if (copies.ContainsKey(card.Name))
            {
                foreach (Card copy in copies[card.Name])
                {
                    if (cardsAddedBy.ContainsKey(card.Name))
                    {
                        Console.WriteLine($"{counter++} - C - {copy.Name} - [ {string.Join(", ", cardsAddedBy[card.Name].Select(x => x.Name))} ]");
                    } else
                    {
                        Console.WriteLine($"{counter++} - C - {copy.Name} - [ ]");
                    }
                }
            }
        }
        */


        Console.WriteLine($"Number of cards won: {qtyOriginalCards + qtyCopyCards} (O: {qtyOriginalCards}, C: {qtyCopyCards})");
    }
}