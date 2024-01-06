using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private enum InputData
        {
            Example2,
            Example10,
            Example100,
            Example1000000,
            Puzzle2,
            Puzzle1000000,
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

            var data = Parse(inputData switch
            {
                InputData.Example2 => "day11/example2_2_in.txt",
                InputData.Example10 => "day11/example2_10_in.txt",
                InputData.Example100 => "day11/example2_100_in.txt",
                InputData.Example1000000 => "day11/example2_1000000_in.txt",
                InputData.Puzzle2 => "day11/puzzle2_2_in.txt",
                InputData.Puzzle1000000 => "day11/puzzle2_1000000_in.txt",
                _ => "NOT_SET"
            });

            // TODO: implement the actual solution
            var result = data.Count;

            print($"<color=green>result = {result}</color>");
            guiText.text = $"{result}\nDONE";
        }

        private List<string> Parse(string inputFilePath)
        {
            return File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}")
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();
        }
    }
}