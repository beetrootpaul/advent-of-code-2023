using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023
{
    //
    // https://adventofcode.com/2023/day/19
    //
    public class Day19Part1Code : MonoBehaviour
    {
        private enum Input
        {
            Example1,
            Puzzle1
        }

        [SerializeField]
        private TextMeshProUGUI resultText;

        [SerializeField]
        private Input inputFile;

        private async void Start()
        {
            resultText.text = "...";

            await ComputeFor(inputFile switch
            {
                Input.Example1 => "day19/example1_in.txt",
                Input.Puzzle1 => "day19/puzzle1_in.txt",
                _ => "NOT_SET"
            });
        }

        private async UniTask ComputeFor(string file)
        {
            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync($"{Application.streamingAssetsPath}/{file}");
            }
            catch
            {
                return;
            }

            print(string.Join("\n", lines));
        }
    }
}