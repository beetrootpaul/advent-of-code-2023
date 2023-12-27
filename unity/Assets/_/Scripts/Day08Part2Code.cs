using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023
{
    //
    // https://adventofcode.com/2023/day/8
    //
    public class Day08Part2Code : MonoBehaviour
    {
        private enum Input
        {
            Example2,
            Puzzle2
        }

        [SerializeReference]
        private TextMeshProUGUI? resultText;

        [SerializeReference]
        private TextMeshProUGUI? progressText;

        [SerializeField]
        private Input inputFile;

        private bool _finished;
        private readonly CancellationTokenSource _destroyCancellationTokenSource = new();

        private async void Start()
        {
            var file = inputFile switch
            {
                Input.Example2 => "day08/example2_in.txt",
                Input.Puzzle2 => "day08/puzzle2_in.txt",
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
            _destroyCancellationTokenSource.Cancel();
            _destroyCancellationTokenSource.Dispose();
        }

        private async Task<long> CalculateFor(string file)
        {
            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync(
                    $"{Application.streamingAssetsPath}/{file}",
                    _destroyCancellationTokenSource.Token
                );
            }
            catch
            {
                return -1;
            }

            var instructions = lines[0].ToCharArray();

            var lPaths = new Dictionary<string, string>();
            var rPaths = new Dictionary<string, string>();
            var current = new List<string>();

            for (var i = 2; i < lines.Length; i++)
            {
                var tokens = lines[i].Split(' ');
                var src = tokens[0];
                var lDest = tokens[2].Substring(1, 3);
                var rDest = tokens[3].Substring(0, 3);

                lPaths[src] = lDest;
                rPaths[src] = rDest;
                if (src[2] == 'A')
                {
                    current.Add(src);
                }
            }

            var steps = 0;
            for (var i = 0L; i < 999_999_999_999L; i++)
            {
                if (current.All(c => c[2] == 'Z'))
                {
                    break;
                }

                for (var j = 0; j < current.Count; j++)
                {
                    current[j] = instructions[i % instructions.Length] == 'L' ? lPaths[current[j]] : rPaths[current[j]];
                }

                if (i % 1000 == 0)
                {
                    Debug.Log($"Processed step: {steps}");
                    await UnblockAndCheckIfCancelled();
                }

                steps++;
            }

            return steps;
        }

        private async Task UnblockAndCheckIfCancelled()
        {
            _destroyCancellationTokenSource.Token.ThrowIfCancellationRequested();

            // unblock the execution by yielding
            await Task.Yield();
        }
    }
}