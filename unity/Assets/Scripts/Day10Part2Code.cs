using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023
{
    //
    // https://adventofcode.com/2023/day/10
    //
    public class Day10Part2Code : MonoBehaviour
    {
        private enum Input
        {
            Example2A,
            Example2B,
            Example2C,
            Example2D,
            Puzzle2
        }

        [SerializeReference]
        private TextMeshProUGUI resultText;

        [SerializeReference]
        private TextMeshProUGUI progressText;

        [SerializeField]
        private Input inputFile;

        private bool _finished;

        private const int MaxN = 150;

        private StringBuilder[] _sketch;
        private StringBuilder[] _markers;

        private async void Start()
        {
            var file = inputFile switch
            {
                Input.Example2A => "day10/example2a_in.txt",
                Input.Example2B => "day10/example2b_in.txt",
                Input.Example2C => "day10/example2c_in.txt",
                // TODO: For this input my solution marks 1 tile (near S) wrong and gives an answer 11 instead of 10.
                Input.Example2D => "day10/example2d_in.txt",
                Input.Puzzle2 => "day10/puzzle1_in.txt",
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
            _markers = new StringBuilder[MaxN];
            for (var row = 0; row < MaxN; row++)
            {
                _sketch[row] = new StringBuilder((row >= 1 && row <= lines.Length ? lines[row - 1] : "")
                    .PadRight(MaxN - 1, '.')
                    .PadLeft(MaxN, '.'));
                _markers[row] = new StringBuilder("".PadRight(MaxN, '.'));
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
            _markers[start.y][start.x] = 'x';

            //
            // 4. Find 1st pipe tile, move start there
            //
            var first = start;
            if (_sketch[start.y - 1][start.x] == '7' || _sketch[start.y - 1][start.x] == '|' ||
                _sketch[start.y - 1][start.x] == 'F')
            {
                first.y -= 1;
            }
            else if (_sketch[start.y + 1][start.x] == 'J' || _sketch[start.y + 1][start.x] == '|' ||
                     _sketch[start.y + 1][start.x] == 'L')
            {
                first.y += 1;
            }
            else if (_sketch[start.y][start.x - 1] == 'L' || _sketch[start.y][start.x - 1] == '-' ||
                     _sketch[start.y][start.x - 1] == 'F')
            {
                first.x -= 1;
            }
            else if (_sketch[start.y][start.x + 1] == 'J' || _sketch[start.y][start.x + 1] == '-' ||
                     _sketch[start.y][start.x + 1] == '7')
            {
                first.x += 1;
            }
            Debug.Log($"FIRST tile at ({start.x},{start.y})");

            await UnblockAndCheckIfCancelled();

            //
            // 5. Follow the pipe and mark its tiles as well as its neighbour A/B areas
            //
            await MarkPipe(first, first - start);
            Debug.Log(string.Join("\n", _markers.Select(m => m.ToString())));

            //
            // 6. Expand A/B areas' markers
            //
            await ExpandAreas();
            Debug.Log(string.Join("\n", _markers.Select(m => m.ToString())));

            //
            // 7. Detect whether A or B is the "inside" area
            //
            var insideAreaMarker = RecognizeInsideArea();
            Debug.Log($"INSIDE = {insideAreaMarker}");

            //
            // 8. Count "inside" area tiles
            //
            return CountTiles(insideAreaMarker);
        }

        private async Task MarkPipe(Vector2Int startTile, Vector2Int movement)
        {
            var tile = startTile;

            var t = _sketch[tile.y][tile.x];
            if (movement.y < 0 || (movement.y == 0 && t is '7' or 'F'))
            {
                if (_markers[tile.y][tile.x - 1] != 'x' && _markers[tile.y][tile.x - 1] != 'A' &&
                    _markers[tile.y][tile.x - 1] != 'B')
                {
                    _markers[tile.y][tile.x - 1] = 'A';
                }
                if (_markers[tile.y][tile.x + 1] != 'x' && _markers[tile.y][tile.x + 1] != 'A' &&
                    _markers[tile.y][tile.x + 1] != 'B')
                {
                    _markers[tile.y][tile.x + 1] = 'B';
                }
            }
            if (movement.y > 0 || (movement.y == 0 && t is 'J' or 'L'))
            {
                if (_markers[tile.y][tile.x - 1] != 'x' && _markers[tile.y][tile.x - 1] != 'A' &&
                    _markers[tile.y][tile.x - 1] != 'B')
                {
                    _markers[tile.y][tile.x - 1] = 'B';
                }
                if (_markers[tile.y][tile.x + 1] != 'x' && _markers[tile.y][tile.x + 1] != 'A' &&
                    _markers[tile.y][tile.x + 1] != 'B')
                {
                    _markers[tile.y][tile.x + 1] = 'A';
                }
            }
            if (movement.x < 0 || (movement.x == 0 && t is 'L' or 'F'))
            {
                if (_markers[tile.y - 1][tile.x] != 'x' && _markers[tile.y - 1][tile.x] != 'A' &&
                    _markers[tile.y - 1][tile.x] != 'B')
                {
                    _markers[tile.y - 1][tile.x] = 'B';
                }
                if (_markers[tile.y + 1][tile.x] != 'x' && _markers[tile.y + 1][tile.x] != 'A' &&
                    _markers[tile.y + 1][tile.x] != 'B')
                {
                    _markers[tile.y + 1][tile.x] = 'A';
                }
            }
            if (movement.x > 0 || (movement.x == 0 && t is 'J' or '7'))
            {
                if (_markers[tile.y - 1][tile.x] != 'x' && _markers[tile.y - 1][tile.x] != 'A' &&
                    _markers[tile.y - 1][tile.x] != 'B')
                {
                    _markers[tile.y - 1][tile.x] = 'A';
                }
                if (_markers[tile.y + 1][tile.x] != 'x' && _markers[tile.y + 1][tile.x] != 'A' &&
                    _markers[tile.y + 1][tile.x] != 'B')
                {
                    _markers[tile.y + 1][tile.x] = 'B';
                }
            }

            var i = 0;
            const int limit = 99_999;
            while (i < limit && _markers[tile.y][tile.x] != 'x')
            {
                t = _sketch[tile.y][tile.x];
                Debug.Log($"NEXT tile '{t}' at ({tile.x},{tile.y})");

                _markers[tile.y][tile.x] = 'x';

                var dirN = t is '|' or 'J' or 'L';
                var dirS = t is '|' or '7' or 'F';
                var dirW = t is '-' or '7' or 'J';
                var dirE = t is '-' or 'F' or 'L';

                var nextTile = tile;
                if (dirN && _markers[tile.y - 1][tile.x] != 'x' &&
                    (_sketch[tile.y - 1][tile.x] == '7' || _sketch[tile.y - 1][tile.x] == '|' ||
                     _sketch[tile.y - 1][tile.x] == 'F'))
                {
                    nextTile.y -= 1;
                }
                else if (dirS && _markers[tile.y + 1][tile.x] != 'x' &&
                         (_sketch[tile.y + 1][tile.x] == 'J' || _sketch[tile.y + 1][tile.x] == '|' ||
                          _sketch[tile.y + 1][tile.x] == 'L'))
                {
                    nextTile.y += 1;
                }
                else if (dirW && _markers[tile.y][tile.x - 1] != 'x' &&
                         (_sketch[tile.y][tile.x - 1] == 'L' || _sketch[tile.y][tile.x - 1] == '-' ||
                          _sketch[tile.y][tile.x - 1] == 'F'))
                {
                    nextTile.x -= 1;
                }
                else if (dirE && _markers[tile.y][tile.x + 1] != 'x' &&
                         (_sketch[tile.y][tile.x + 1] == 'J' || _sketch[tile.y][tile.x + 1] == '-' ||
                          _sketch[tile.y][tile.x + 1] == '7'))
                {
                    nextTile.x += 1;
                }
                movement = nextTile - tile;

                if (movement.y < 0 || (movement.y == 0 && t is '7' or 'F'))
                {
                    if (_markers[tile.y][tile.x - 1] != 'x' && _markers[tile.y][tile.x - 1] != 'A' &&
                        _markers[tile.y][tile.x - 1] != 'B')
                    {
                        _markers[tile.y][tile.x - 1] = 'A';
                    }
                    if (_markers[tile.y][tile.x + 1] != 'x' && _markers[tile.y][tile.x + 1] != 'A' &&
                        _markers[tile.y][tile.x + 1] != 'B')
                    {
                        _markers[tile.y][tile.x + 1] = 'B';
                    }
                }
                if (movement.y > 0 || (movement.y == 0 && t is 'J' or 'L'))
                {
                    if (_markers[tile.y][tile.x - 1] != 'x' && _markers[tile.y][tile.x - 1] != 'A' &&
                        _markers[tile.y][tile.x - 1] != 'B')
                    {
                        _markers[tile.y][tile.x - 1] = 'B';
                    }
                    if (_markers[tile.y][tile.x + 1] != 'x' && _markers[tile.y][tile.x + 1] != 'A' &&
                        _markers[tile.y][tile.x + 1] != 'B')
                    {
                        _markers[tile.y][tile.x + 1] = 'A';
                    }
                }
                if (movement.x < 0 || (movement.x == 0 && t is 'L' or 'F'))
                {
                    if (_markers[tile.y - 1][tile.x] != 'x' && _markers[tile.y - 1][tile.x] != 'A' &&
                        _markers[tile.y - 1][tile.x] != 'B')
                    {
                        _markers[tile.y - 1][tile.x] = 'B';
                    }
                    if (_markers[tile.y + 1][tile.x] != 'x' && _markers[tile.y + 1][tile.x] != 'A' &&
                        _markers[tile.y + 1][tile.x] != 'B')
                    {
                        _markers[tile.y + 1][tile.x] = 'A';
                    }
                }
                if (movement.x > 0 || (movement.x == 0 && t is 'J' or '7'))
                {
                    if (_markers[tile.y - 1][tile.x] != 'x' && _markers[tile.y - 1][tile.x] != 'A' &&
                        _markers[tile.y - 1][tile.x] != 'B')
                    {
                        _markers[tile.y - 1][tile.x] = 'A';
                    }
                    if (_markers[tile.y + 1][tile.x] != 'x' && _markers[tile.y + 1][tile.x] != 'A' &&
                        _markers[tile.y + 1][tile.x] != 'B')
                    {
                        _markers[tile.y + 1][tile.x] = 'B';
                    }
                }

                i++;
                tile = nextTile;
            }

            if (i >= limit)
            {
                Debug.LogWarning($"Reached limit of {limit} iterations");
            }

            if (i % 100 == 0)
            {
                await UnblockAndCheckIfCancelled();
            }
        }

        private async Task ExpandAreas()
        {
            var qA = new Queue<Vector2Int>();
            var qB = new Queue<Vector2Int>();
            for (var row = 0; row < MaxN; row++)
            {
                for (var col = 0; col < MaxN; col++)
                {
                    switch (_markers[row][col])
                    {
                        case 'A':
                            qA.Enqueue(new Vector2Int(col, row));
                            break;
                        case 'B':
                            qB.Enqueue(new Vector2Int(col, row));
                            break;
                    }
                }
            }

            var i = 0;

            while (qA.Count > 0)
            {
                var a = qA.Dequeue();
                if (a.y > 0 && _markers[a.y - 1][a.x] == '.')
                {
                    _markers[a.y - 1][a.x] = 'A';
                    qA.Enqueue(new Vector2Int(a.x, a.y - 1));
                }
                if (a.y < MaxN - 1 && _markers[a.y + 1][a.x] == '.')
                {
                    _markers[a.y + 1][a.x] = 'A';
                    qA.Enqueue(new Vector2Int(a.x, a.y + 1));
                }
                if (a.x > 0 && _markers[a.y][a.x - 1] == '.')
                {
                    _markers[a.y][a.x - 1] = 'A';
                    qA.Enqueue(new Vector2Int(a.x - 1, a.y));
                }
                if (a.x < MaxN - 1 && _markers[a.y][a.x + 1] == '.')
                {
                    _markers[a.y][a.x + 1] = 'A';
                    qA.Enqueue(new Vector2Int(a.x + 1, a.y));
                }

                if (++i % 100 == 0)
                {
                    await UnblockAndCheckIfCancelled();
                }
            }

            while (qB.Count > 0)
            {
                var a = qB.Dequeue();
                if (a.y > 0 && _markers[a.y - 1][a.x] == '.')
                {
                    _markers[a.y - 1][a.x] = 'B';
                    qB.Enqueue(new Vector2Int(a.x, a.y - 1));
                }
                if (a.y < MaxN - 1 && _markers[a.y + 1][a.x] == '.')
                {
                    _markers[a.y + 1][a.x] = 'B';
                    qB.Enqueue(new Vector2Int(a.x, a.y + 1));
                }
                if (a.x > 0 && _markers[a.y][a.x - 1] == '.')
                {
                    _markers[a.y][a.x - 1] = 'B';
                    qB.Enqueue(new Vector2Int(a.x - 1, a.y));
                }
                if (a.x < MaxN - 1 && _markers[a.y][a.x + 1] == '.')
                {
                    _markers[a.y][a.x + 1] = 'B';
                    qB.Enqueue(new Vector2Int(a.x + 1, a.y));
                }

                if (++i % 100 == 0)
                {
                    await UnblockAndCheckIfCancelled();
                }
            }
        }

        private char RecognizeInsideArea()
        {
            for (var row = 0; row < MaxN; row++)
            {
                for (var col = 0; col < MaxN; col++)
                {
                    if (row != 0 && col != 0 && row != MaxN - 1 && col != MaxN - 1) continue;
                    switch (_markers[row][col])
                    {
                        case 'A':
                            return 'B';
                        case 'B':
                            return 'A';
                    }
                }
            }
            return '?';
        }

        private int CountTiles(char marker)
        {
            var sum = 0;

            for (var row = 0; row < MaxN; row++)
            {
                for (var col = 0; col < MaxN; col++)
                {
                    if (_markers[row][col] == marker)
                    {
                        sum += 1;
                    }
                }
            }

            return sum;
        }

        private async Task UnblockAndCheckIfCancelled()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            // unblock the execution by yielding
            await Task.Yield();
        }
    }
}