using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023.Scripts
{
    //
    // https://adventofcode.com/2023/day/9
    //
    public class Day09Part2Code : MonoBehaviour
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

        private const int MaxN = 99;

        private long[][] _pascalTriangle;

        private async void Start()
        {
            var file = inputFile switch
            {
                Input.Example2 => "day09/example2_in.txt",
                Input.Puzzle2 => "day09/puzzle2_in.txt",
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
            InitPascalTriangle();
            Debug.Log(string.Join('\n', _pascalTriangle.Select(row => string.Join(' ', row))));

            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync($"{Application.streamingAssetsPath}/{file}");
            }
            catch
            {
                return -1;
            }

            long sumOfExtrapolatedValues = 0;
            foreach (var line in lines)
            {
                Debug.Log($"Processing: {line}");
                var values = line.Split(" ").Select(long.Parse).ToList();
                var extrapolatedValue = ExtrapolateNextValue(values);
                Debug.Log($"NEXT: {extrapolatedValue}");
                sumOfExtrapolatedValues += extrapolatedValue;
            }

            return sumOfExtrapolatedValues;
        }

        private void InitPascalTriangle()
        {
            _pascalTriangle = new long[MaxN][];

            for (var row = 0; row < MaxN; row++)
            {
                _pascalTriangle[row] = new long [MaxN];

                for (var col = 0; col < MaxN; col++)
                {
                    if (col == 0 || col == row)
                    {
                        _pascalTriangle[row][col] = 1;
                    }
                    else if (col > row)
                    {
                        _pascalTriangle[row][col] = 0;
                    }
                    else
                    {
                        _pascalTriangle[row][col] = _pascalTriangle[row - 1][col - 1] + _pascalTriangle[row - 1][col];
                    }
                }
            }
        }

        private long ExtrapolateNextValue(IReadOnlyList<long> values)
        {
            var n = values.Count;
            long nextValue = 0;
            long sign = 1;

            for (var i = 0; i < n; i++)
            {
                // Debug.Log($"> {sign} x {values[i]} x {_pascalTriangle[n][i + 1]}");
                nextValue += sign * values[i] * _pascalTriangle[n][i + 1];
                sign = -sign;
            }

            return nextValue;
        }
    }
}