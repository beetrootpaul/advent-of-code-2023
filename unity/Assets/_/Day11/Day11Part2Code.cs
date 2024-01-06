using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace aoc2023.Day11
{
    //
    // Day 11: Cosmic Expansion
    // https://adventofcode.com/2023/day/11
    //
    // part 1
    //
    internal class Day11Part2Code : MonoBehaviour
    {
        [Header("params")]
        [SerializeField]
        private InputData inputData;

        [Header("instrumentation")]
        [SerializeField]
        private TMP_Text? guiText;

        private void Start()
        {
            guiText.text = "...";

            var galaxies = Parse(inputData switch
            {
                InputData.Test => "day11/test_in.txt",
                InputData.Example2 => "day11/example2_2_in.txt",
                InputData.Example10 => "day11/example2_10_in.txt",
                InputData.Example100 => "day11/example2_100_in.txt",
                InputData.Example1000000 => "day11/example2_1000000_in.txt",
                InputData.Puzzle2 => "day11/puzzle2_2_in.txt",
                InputData.Puzzle1000000 => "day11/puzzle2_1000000_in.txt",
                _ => "NOT_SET"
            });

            Expand(galaxies, inputData switch
            {
                InputData.Test => 3,
                InputData.Example2 => 2,
                InputData.Example10 => 10,
                InputData.Example100 => 100,
                InputData.Example1000000 => 1000000,
                InputData.Puzzle2 => 2,
                InputData.Puzzle1000000 => 1000000,
                _ => 1000000
            });

            print("== original X --> expanded X ==");
            foreach (var g in galaxies)
            {
                print($"{g.OriginalX} --> {g.ExpandedX}");
            }
            print("== original Y / expanded XY==");
            foreach (var g in galaxies)
            {
                print($"{g.OriginalY} --> {g.ExpandedY}");
            }

            var sum = SumShortestPathsBetween(galaxies);

            print($"<color=green>sum = {sum}</color>");
            guiText.text = $"{sum}\nDONE";
        }

        private List<Galaxy> Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}")
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();

            var galaxies = new List<Galaxy>();
            for (var row = 0; row < lines.Count(); row++)
            {
                for (var col = 0; col < lines[row].Length; col++)
                {
                    if (lines[row][col] == '#')
                    {
                        galaxies.Add(new Galaxy()
                        {
                            OriginalX = col,
                            OriginalY = row,
                        });
                    }
                }
            }
            return galaxies;
        }

        private void Expand(List<Galaxy> galaxies, long multiplier)
        {
            galaxies.Sort((g1, g2) =>
                g1.OriginalX > g2.OriginalX
                    ? 1
                    : g1.OriginalX < g2.OriginalX
                        ? -1
                        : 0
            );
            var emptyCols = 0L;
            var prevX = -1L;
            foreach (var g in galaxies)
            {
                emptyCols += Math.Max(0, g.OriginalX - prevX - 1);
                g.ExpandedX = g.OriginalX + emptyCols * (multiplier - 1);
                prevX = g.OriginalX;
            }

            galaxies.Sort((g1, g2) =>
                g1.OriginalY > g2.OriginalY
                    ? 1
                    : g1.OriginalY < g2.OriginalY
                        ? -1
                        : 0
            );
            var emptyRows = 0L;
            var prevY = -1L;
            foreach (var g in galaxies)
            {
                emptyRows += Math.Max(0, g.OriginalY - prevY - 1);
                g.ExpandedY = g.OriginalY + emptyRows * (multiplier - 1);
                prevY = g.OriginalY;
            }
        }

        private long SumShortestPathsBetween(List<Galaxy> galaxies)
        {
            var sum = 0L;

            for (var i = 0; i < galaxies.Count; i++)
            {
                for (var j = i + 1; j < galaxies.Count; j++)
                {
                    sum += Galaxy.ShortestPathBetween(galaxies[i], galaxies[j]);
                }
            }

            return sum;
        }

        private enum InputData
        {
            Test,
            Example2,
            Example10,
            Example100,
            Example1000000,
            Puzzle2,
            Puzzle1000000,
        }

        private class Galaxy
        {
            internal long OriginalX;
            internal long OriginalY;
            internal long ExpandedX;
            internal long ExpandedY;

            public static long ShortestPathBetween(Galaxy g1, Galaxy g2)
            {
                return Math.Abs(g1.ExpandedX - g2.ExpandedX) + Math.Abs(g1.ExpandedY - g2.ExpandedY);
            }
        }
    }
}