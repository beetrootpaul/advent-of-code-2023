using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023.day08
{
    //
    // https://adventofcode.com/2023/day/8
    //
    public class Day08Part1Code : MonoBehaviour
    {
        private enum Input
        {
            Example1A,
            Example1B,
            Puzzle1
        }

        [SerializeReference] private TextMeshProUGUI? resultText;

        [SerializeReference] private TextMeshProUGUI? progressText;

        [SerializeField] private Input inputFile;

        private bool _finished;

        private async void Start()
        {
            var file = inputFile switch
            {
                Input.Example1A => "day08/example1a_in.txt",
                Input.Example1B => "day08/example1b_in.txt",
                Input.Puzzle1 => "day08/puzzle1_in.txt",
                _ => "NOT_SET"
            };

            resultText.text = $"Calculating for:\n{file} ...";

            var steps = await CalculateFor(file);

            _finished = true;
            resultText.text = $"STEPS: {steps}";
            Debug.Log($"STEPS: {steps}");
        }

        private void Update()
        {
            if (_finished) return;
            var progress = (int)Math.Min(1 + Math.Floor(Time.time * 10), 9_999);
            progressText.text = "".PadRight(progress, '.');
        }

        private void OnApplicationQuit()
        {
            _finished = true;
        }

        private async Task<long> CalculateFor(string file)
        {
            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync($"{Application.streamingAssetsPath}/{file}");
            }
            catch
            {
                return -1;
            }

            var instructions = lines[0].ToCharArray();

            var lPaths = new Dictionary<string, string>();
            var rPaths = new Dictionary<string, string>();
            var current = "AAA";

            for (var i = 2; i < lines.Length; i++)
            {
                var tokens = lines[i].Split(' ');
                var src = tokens[0];
                var lDest = tokens[2].Substring(1, 3);
                var rDest = tokens[3].Substring(0, 3);

                Debug.Log($"{lDest} <-- {src} --> {rDest}");

                lPaths[src] = lDest;
                rPaths[src] = rDest;
            }

            var steps = 0;
            for (var i = 0L; i < 999_999_999_999L; i++)
            {
                if (current == "ZZZ")
                {
                    break;
                }

                current = instructions[i % instructions.Length] == 'L' ? lPaths[current] : rPaths[current];

                if (i % 1000 == 0)
                {
                    Debug.Log($"Processed step: {steps}");
                }

                steps++;
            }

            return steps;
        }
    }
}