using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023.Scripts
{
    //
    // https://adventofcode.com/2023/day/10
    //
    public class Day10Part1Code : MonoBehaviour
    {
        private enum Input
        {
            Test01,
            Example1A,
            Example1B,
            Puzzle1
        }

        [SerializeReference]
        private TextMeshProUGUI? resultText;

        [SerializeReference]
        private TextMeshProUGUI? progressText;

        [SerializeField]
        private Input inputFile;

        private bool _finished;

        private const int MaxN = 150;

        private StringBuilder[] _sketch;

        private async void Start()
        {
            var file = inputFile switch
            {
                Input.Test01 => "day10/test_01_in.txt",
                Input.Example1A => "day10/example1a_in.txt",
                Input.Example1B => "day10/example1b_in.txt",
                Input.Puzzle1 => "day10/puzzle1_in.txt",
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
            //
            // 1. Read the input
            //
            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync($"{Application.streamingAssetsPath}/{file}");
            }
            catch
            {
                return -1;
            }

            //
            // 2. Parse and normalize the sketch
            //
            _sketch = new StringBuilder[MaxN];
            for (var row = 0; row < MaxN; row++)
            {
                _sketch[row] = new StringBuilder((row >= 1 && row <= lines.Length ? lines[row - 1] : "")
                    .PadRight(MaxN - 1, '.')
                    .PadLeft(MaxN, '.'));
            }
            Debug.Log(string.Join("\n", _sketch.Select(s => s.ToString())));

            //
            // 3. Find "S"
            //
            var start = new Vector2Int(-1, -1);
            for (var row = 0; row < MaxN; row++)
            {
                for (var col = 0; col < MaxN; col++)
                {
                    if (_sketch[row][col] == 'S')
                    {
                        start = new Vector2Int(col, row);
                    }
                }
            }
            Debug.Log($"Found S at ({start.x},{start.y})");

            //
            // 4. Find 1st pipe tile, move start there
            //
            if (_sketch[start.y - 1][start.x] == '7' || _sketch[start.y - 1][start.x] == '|' ||
                _sketch[start.y - 1][start.x] == 'F')
            {
                start.y -= 1;
            }
            else if (_sketch[start.y + 1][start.x] == 'J' || _sketch[start.y + 1][start.x] == '|' ||
                     _sketch[start.y + 1][start.x] == 'L')
            {
                start.y += 1;
            }
            else if (_sketch[start.y][start.x - 1] == 'L' || _sketch[start.y][start.x - 1] == '-' ||
                     _sketch[start.y][start.x - 1] == 'F')
            {
                start.x -= 1;
            }
            else if (_sketch[start.y][start.x + 1] == 'J' || _sketch[start.y][start.x + 1] == '-' ||
                     _sketch[start.y][start.x + 1] == '7')
            {
                start.x += 1;
            }
            Debug.Log($"FIRST tile at ({start.x},{start.y})");

            //
            // 5. Follow the pipe to measure its length
            //
            var pipeLength = await MeasurePipe(start);
            Debug.Log(string.Join("\n", _sketch.Select(s => s.ToString())));
            Debug.Log($"Measured length: {pipeLength}");

            //
            // 6. Calculate the farthest tile
            //
            var farthestTile = pipeLength / 2;

            return farthestTile;
        }

        private async Task<int> MeasurePipe(Vector2Int startTile)
        {
            var tile = startTile;
            var length = 1;
            const int limit = 999_999;
            var i = 0;
            while (i < limit && _sketch[tile.y][tile.x] != 'x')
            {
                var t = _sketch[tile.y][tile.x];
                Debug.Log($"NEXT tile '{t}' at ({tile.x},{tile.y})");

                _sketch[tile.y][tile.x] = 'x';

                var dirN = t is '|' or 'J' or 'L';
                var dirS = t is '|' or '7' or 'F';
                var dirW = t is '-' or '7' or 'J';
                var dirE = t is '-' or 'F' or 'L';

                if (dirN && (_sketch[tile.y - 1][tile.x] == '7' || _sketch[tile.y - 1][tile.x] == '|' ||
                             _sketch[tile.y - 1][tile.x] == 'F'))
                {
                    tile.y -= 1;
                }
                else if (dirS && (_sketch[tile.y + 1][tile.x] == 'J' || _sketch[tile.y + 1][tile.x] == '|' ||
                                  _sketch[tile.y + 1][tile.x] == 'L'))
                {
                    tile.y += 1;
                }
                else if (dirW && (_sketch[tile.y][tile.x - 1] == 'L' || _sketch[tile.y][tile.x - 1] == '-' ||
                                  _sketch[tile.y][tile.x - 1] == 'F'))
                {
                    tile.x -= 1;
                }
                else if (dirE && (_sketch[tile.y][tile.x + 1] == 'J' || _sketch[tile.y][tile.x + 1] == '-' ||
                                  _sketch[tile.y][tile.x + 1] == '7'))
                {
                    tile.x += 1;
                }

                length += 1;
                i++;
            }

            if (i >= limit)
            {
                Debug.LogWarning($"Reached limit of {limit} iterations");
            }

            return length;
        }
    }
}