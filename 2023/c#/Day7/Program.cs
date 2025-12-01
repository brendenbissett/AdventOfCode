using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Marshalling;
using Utilities;

internal class Program
{

    class Card
    {
        public string Label { get; }
        public int Value { get; }

        public int Strength { get; set; }

        public Card(string label)
        {
            this.Label = label;
            this.Value = GetValue(label);
        }

        private static int GetValue(string label)
        {
            switch (label)
            {
                case "A":
                    return 14;
                case "K":
                    return 13;
                case "Q":
                    return 12;
                case "J":
                    //return 11;  // Part 1
                    return 1;   // Part 2
                case "T":
                    return 10;
                default:
                    return int.Parse(label);
            }
        }   
    }

    class Hand : IComparable<Hand>
    {
        public Card[] Cards { get; }
        public int Bet { get; set; }

        public int Strength { get; set; }

        public Hand(Card[] cards, int bet)
        {
            this.Cards = cards;
            this.Bet = bet;
            this.Strength = GetStrength();
        }

        private int GetStrength()
        {
            // Label: A,  K,  Q,  J,  T,  9, 8, 7, 6, 5, 4, 3, 2
            // Value: 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 // Part 1
            // Value: 14, 13, 12, 1, 10, 9, 8, 7, 6, 5, 4, 3, 2 // Part 2

            // Hands
            // 7 - Five of a kind, where all five cards have the same label: AAAAA
            // 6 - Four of a kind, where four cards have the same label and one card has a different label: AA8AA
            // 5 - Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
            // 4 - Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
            // 3 - Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
            // 2 - One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
            // 1 - High card, where all cards' labels are distinct: 23456

            Dictionary<string, int> cardDict = new Dictionary<string, int>();
            foreach (var card in Cards)
            {
                if (cardDict.ContainsKey(card.Label))
                {
                    cardDict[card.Label]++;
                }
                else
                {
                    cardDict[card.Label] = 1;
                }
            }

            if (cardDict.Count == 1) return 7;

            // Part 2 (Need to consider if Jacks are present) ---
            if (cardDict.ContainsKey("J"))
            {
                int count = cardDict["J"];

                // Remove Jacks from the dictionary (Can throw the numbers if we don't)
                cardDict.Remove("J");

                // Add Jacks to the item with highest count
                // Doesn't really matter which card we add to, since we're only interested in the count to determine the strength
                var key = cardDict.Select(kvp => kvp.Key).OrderByDescending(k => cardDict[k]).First();

                // If Jacks are the majority, then they should still count in strength
                cardDict[key] += count;
            }

            // ---- Part 2

            if (cardDict.Count == 1) return 7;

            if (cardDict.Count == 2)
            {

                if (cardDict.ContainsValue(4))
                {
                    return 6;
                }
                else
                {
                    return 5;
                }
            }

            if (cardDict.Count == 3)
            {
                if (cardDict.ContainsValue(3))
                {
                    return 4;
                }
                else
                {
                    return 3;
                }
            }
            
            if (cardDict.Count == 4)
            {
                return 2;
            }

            return 1;
        }

        public int CompareTo(Hand? other)
        {
            // This hand is stronger
            if (other == null) return 1;   // Double check this

            if (this.Strength > other.Strength) return 1;
            if (this.Strength < other.Strength) return -1;
            if (this.Strength == other.Strength)
            {
                for (int a = 0; a < 5; a++)
                {
                    int cardA = this.Cards[a].Value;
                    int cardB = other.Cards[a].Value;

                    if (cardA > cardB)
                    {
                        return 1;
                    }
                    else if (cardA < cardB)
                    {
                        return -1;
                    }
                }
            }

            // Equal
            return 0;
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day7.txt");
        var lines = FileHandling.ReadInputFile(path);

        List<Hand> hands = new List<Hand>();

        foreach (var line in lines)
        {
            var parts = line.Trim().Split(" ");
            var cards = parts[0].ToCharArray().Select(c => new Card(c.ToString())).ToArray();
            var bet = int.Parse(parts[1]);

            hands.Add(new Hand(cards, bet));
        }

        // Part 1
        Console.WriteLine($"Part 1 - Hands to process: {hands.Count}");

        // Sort by strength
        hands.Sort();  // Implemented with IComparable

        // Calculate winnings
        int rank = 1;
        ulong winnings = 0;


        List<Hand> testHands = new List<Hand>();
        testHands.Add(new Hand(new Card[] { new Card("3"), new Card("2"), new Card("T"), new Card("3"), new Card("K") }, 765));
        testHands.Add(new Hand(new Card[] { new Card("T"), new Card("5"), new Card("5"), new Card("J"), new Card("5") }, 684));
        testHands.Add(new Hand(new Card[] { new Card("K"), new Card("K"), new Card("6"), new Card("7"), new Card("7") }, 28));
        testHands.Add(new Hand(new Card[] { new Card("K"), new Card("T"), new Card("J"), new Card("J"), new Card("T") }, 220));
        testHands.Add(new Hand(new Card[] { new Card("Q"), new Card("Q"), new Card("Q"), new Card("J"), new Card("A") }, 483));
        testHands.Sort();

        foreach (var hand in hands)
        {
            winnings += (ulong)(rank * hand.Bet);

            Console.WriteLine($"Hand: {string.Join(" ", hand.Cards.Select(c =>  $"{c.Label}({c.Value})"))} - Bet: {hand.Bet} - Strength: {hand.Strength} - RANK {rank}");
            rank++;
        }

        Console.WriteLine($"Part 1 - Winnings: {winnings}");

        // Part 1 - 248674263 -- Too low
        // Part 1 - 248958313 -- Too High
        // Part 1 - 248812215

        // Part 2 - 251097224 -- Too High
        // Part 2 - 249809252 -- Too Low
        // Part 2 - 250057090
    }
}