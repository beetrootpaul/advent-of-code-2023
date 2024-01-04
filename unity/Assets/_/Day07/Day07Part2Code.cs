using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace aoc2023.Day07
{
    //
    // Day 7: Camel Cards
    // https://adventofcode.com/2023/day/7
    //
    // part 2
    //
    internal class Day07Part2Code : MonoBehaviour
    {
        private enum InputData
        {
            Example,
            Puzzle
        }

        internal readonly struct Hand
        {
            private static readonly List<char> CardsFromWeakest = new(new[]
            {
                // '_' here is a fake card which occupies index 0, so we can start with 1 in reality
                'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J', '_'
            }.Reverse());

            internal readonly int Power;
            internal readonly HandType Type;
            private readonly char[] _cards;

            public Hand(char[] cards)
            {
                _cards = cards;

                var counts = new Dictionary<char, int>();
                var jokerCount = 0;
                foreach (var c in cards)
                {
                    if (c == 'J')
                    {
                        jokerCount += 1;
                    }
                    else if (!counts.TryAdd(c, 1))
                    {
                        counts[c] += 1;
                    }
                }

                // Some condition for weaker types are omitted here, because they would be
                //   already caught by the higher type conditions, e.g. 299AJ would never
                //   be "two pair" (99+AA) it it can be a stronger "three of a kind" (999).
                Type = HandType.HighCard;
                if (
                    counts.Values.Any(count => count == 5) ||
                    (counts.Values.Any(count => count == 4) && jokerCount == 1) ||
                    (counts.Values.Any(count => count == 3) && jokerCount == 2) ||
                    (counts.Values.Any(count => count == 2) && jokerCount == 3) ||
                    (counts.Values.Any(count => count == 1) && jokerCount == 4) ||
                    jokerCount == 5
                )
                {
                    Type = HandType.FiveOfAKind;
                }
                else if (
                    counts.Values.Any(count => count == 4) ||
                    (counts.Values.Any(count => count == 3) && jokerCount == 1) ||
                    (counts.Values.Any(count => count == 2) && jokerCount == 2) ||
                    (counts.Values.Any(count => count == 1) && jokerCount == 3) ||
                    jokerCount == 4
                )
                {
                    Type = HandType.FourOfAKind;
                }
                else if (
                    (counts.Values.Any(count => count == 3) && counts.Values.Any(count => count == 2)) ||
                    (counts.Values.Count(count => count == 2) == 2 && jokerCount == 1)
                )
                {
                    Type = HandType.FullHouse;
                }
                else if (
                    counts.Values.Any(count => count == 3) ||
                    (counts.Values.Any(count => count == 2) && jokerCount == 1) ||
                    (counts.Values.Any(count => count == 1) && jokerCount == 2)
                )
                {
                    Type = HandType.ThreeOfAKind;
                }
                else if (
                    counts.Values.Count(count => count == 2) == 2
                )
                {
                    Type = HandType.TwoPair;
                }
                else if (
                    counts.Values.Any(count => count == 2) ||
                    (counts.Values.Any(count => count == 1) && jokerCount == 1)
                )
                {
                    Type = HandType.OnePair;
                }

                // Why +2?
                //   +1, because we add 1 to index (by placing fake card in CardsFromWeakest) to get the card's power
                //   +1, because better safe than sorry 😅
                var multiplier = CardsFromWeakest.Count + 2;

                Power = (int)Type * multiplier * multiplier * multiplier * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[0]) * multiplier * multiplier * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[1]) * multiplier * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[2]) * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[3]) * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[4]);
            }

            public override string ToString() => _cards.ArrayToString();
        }

        internal enum HandType
        {
            FiveOfAKind = 7,
            FourOfAKind = 6,
            FullHouse = 5,
            ThreeOfAKind = 4,
            TwoPair = 3,
            OnePair = 2,
            HighCard = 1
        }

        [Header("params")]
        [SerializeField]
        private InputData inputData;

        [Header("instrumentation")]
        [SerializeField]
        private TMP_Text? guiText;

        private void Start()
        {
            guiText.text = "...";

            var entries = Parse(inputData switch
            {
                InputData.Example => "day07/example2_in.txt",
                InputData.Puzzle => "day07/puzzle2_in.txt",
                _ => "NOT_SET"
            });
            print("_ input _\n" + string.Join("\n", entries.Select(entry => $"{entry.hand} [{entry.hand.Power}]")));

            entries.Sort((entry1, entry2) => entry1.hand.Power - entry2.hand.Power);
            print("_ sorted _\n" + string.Join("\n", entries.Select(entry => $"{entry.hand} [{entry.hand.Power}]")));

            var totalWinnings = 0L;
            var rank = 1L;
            foreach (var (hand, bid) in entries)
            {
                var winning = rank * bid;
                print($"{hand} : {bid} x #{rank} = {winning}");
                totalWinnings += winning;
                rank += 1L;
            }

            print($"<color=green>totalWinnings = {totalWinnings}</color>");
            guiText.text = $"{totalWinnings}\nDONE";
        }

        private List<(Hand hand, int bid)> Parse(string inputFilePath)
        {
            return File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}")
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .Select(line => (
                    new Hand(line.Split(" ")[0].ToCharArray()),
                    int.Parse(line.Split(" ")[1])
                ))
                .ToList();
        }
    }
}