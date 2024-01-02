using System.IO;
using TMPro;
using UnityEngine;

namespace aoc2023.Day23
{
    //
    // https://adventofcode.com/2023/day/23
    //
    public class Day23Part2Code : MonoBehaviour
    {
        private enum Input
        {
            Example,
            Puzzle
        }

        [field: SerializeField] private TextMeshProUGUI ResultText { get; set; }
        [field: SerializeField] private Input InputFile { get; set; }

        private void Start()
        {
            ResultText.text = "...";

            Parse(InputFile switch
            {
                Input.Example => "day23/example2_in.txt",
                Input.Puzzle => "day23/puzzle2_in.txt",
                _ => "NOT_SET"
            });

            ResultText.text = "DONE";
        }

        private static void Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}");
            print(string.Join('\n', lines));
        }
    }
}