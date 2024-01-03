using System.IO;
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
        private enum EInputData
        {
            Example,
            Puzzle
        }

        [Header("params")]
        [SerializeField]
        private EInputData inputData;

        [Header("instrumentation")]
        [SerializeField]
        private TMP_Text? guiText;

        private void Start()
        {
            guiText.text = "...";

            Parse(inputData switch
            {
                EInputData.Example => "day07/example1_in.txt",
                EInputData.Puzzle => "day07/puzzle1_in.txt",
                _ => "NOT_SET"
            });

            guiText.text = "DONE";
        }

        private void Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}");
            print(string.Join("\n", lines));
        }
    }
}