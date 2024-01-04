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
    // part 1
    //
    internal class Day07Part1Code : MonoBehaviour
    {
        private enum InputData
        {
            Example,
            Puzzle
        }

        private readonly struct Hand
        {
            private static readonly List<char> CardsFromWeakest = new(new[]
            {
                // '_' here is a fake card which occupies index 0, so we can start with 1 in reality
                'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2', '_'
            }.Reverse());

            internal readonly int Power;
            private readonly char[] _cards;

            public Hand(char[] cards)
            {
                _cards = cards;

                var counts = new Dictionary<char, int>();
                foreach (var c in cards)
                {
                    if (!counts.TryAdd(c, 1))
                    {
                        counts[c] += 1;
                    }
                }

                var type = HandType.HighCard;
                if (counts.Values.Any(count => count == 5))
                {
                    type = HandType.FiveOfAKind;
                }
                else if (counts.Values.Any(count => count == 4))
                {
                    type = HandType.FourOfAKind;
                }
                else if (counts.Values.Any(count => count == 3) && counts.Values.Any(count => count == 2))
                {
                    type = HandType.FullHouse;
                }
                else if (counts.Values.Any(count => count == 3))
                {
                    type = HandType.ThreeOfAKind;
                }
                else if (counts.Values.Count(count => count == 2) == 2)
                {
                    type = HandType.TwoPair;
                }
                else if (counts.Values.Any(count => count == 2))
                {
                    type = HandType.OnePair;
                }

                // Why +2?
                //   +1, because we add 1 to index (by placing fake card in CardsFromWeakest) to get the card's power
                //   +1, because better safe than sorry 😅
                var multiplier = CardsFromWeakest.Count + 2;

                Power = (int)type * multiplier * multiplier * multiplier * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[0]) * multiplier * multiplier * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[1]) * multiplier * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[2]) * multiplier * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[3]) * multiplier;
                Power += CardsFromWeakest.IndexOf(cards[4]);
            }

            public override string ToString() => _cards.ArrayToString();
        }

        private enum HandType
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
                InputData.Example => "day07/example1_in.txt",
                InputData.Puzzle => "day07/puzzle1_in.txt",
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