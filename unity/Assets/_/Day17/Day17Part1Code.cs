using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace aoc2023.Day17
{
    //
    // Day 17: Clumsy Crucible
    // https://adventofcode.com/2023/day/17
    //
    // part 1
    //
    internal class Day17Part1Code : MonoBehaviour
    {
        [Header("params")]
        [SerializeField]
        private InputData inputData;

        [Header("debug")]
        [SerializeField]
        private Vector2Int debugXy;

        [Header("instrumentation")]
        [SerializeField]
        private TMP_Text? guiText;

        private Vector2Int _size;
        private int[][] _heatLossMap;

        private int[][][][] _heatLossCalculation;

        private void Start()
        {
            guiText.text = "...";

            Parse(inputData switch
            {
                InputData.Example => "day17/example1_in.txt",
                InputData.Puzzle => "day17/puzzle1_in.txt",
                _ => "NOT_SET"
            });

            CalculateMinHeatLoss();

            var goalY = _size.y - 1;
            var goalX = _size.x - 1;
            var result = new List<int>(new[]
            {
                _heatLossCalculation[goalY][goalX][(int)Direction.Right].Min(),
                _heatLossCalculation[goalY][goalX][(int)Direction.Down].Min(),
                _heatLossCalculation[goalY][goalX][(int)Direction.Left].Min(),
                _heatLossCalculation[goalY][goalX][(int)Direction.Up].Min(),
            }).Min();

            print("<color=yellow>debug</color>");
            print($"R2 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Right][2]}");
            print($"R1 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Right][1]}");
            print($"R0 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Right][0]}");
            print($"D2 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Down][2]}");
            print($"D1 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Down][1]}");
            print($"D0 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Down][0]}");
            print($"L2 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Left][2]}");
            print($"L1 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Left][1]}");
            print($"L0 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Left][0]}");
            print($"U2 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Up][2]}");
            print($"U1 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Up][1]}");
            print($"U0 {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Up][0]}");

            print($"<color=green>result = {result}</color>");
            guiText.text = $"{result}\nDONE";
        }

        private void Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}")
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();

            _size = new Vector2Int(lines[0].Length, lines.Count);
            _heatLossMap = new int[_size.y][];
            _heatLossCalculation = new int[_size.y][][][];

            for (var y = 0; y < _size.y; y++)
            {
                _heatLossMap[y] = new int[_size.x];
                _heatLossCalculation[y] = new int[_size.x][][];

                for (var x = 0; x < _size.x; x++)
                {
                    _heatLossMap[y][x] = int.Parse(lines[y][x].ToString());
                    _heatLossCalculation[y][x] = new int[4][];
                    _heatLossCalculation[y][x][(int)Direction.Right] =
                        new[] { int.MaxValue, int.MaxValue, int.MaxValue };
                    _heatLossCalculation[y][x][(int)Direction.Down] =
                        new[] { int.MaxValue, int.MaxValue, int.MaxValue };
                    _heatLossCalculation[y][x][(int)Direction.Left] =
                        new[] { int.MaxValue, int.MaxValue, int.MaxValue };
                    _heatLossCalculation[y][x][(int)Direction.Up] =
                        new[] { int.MaxValue, int.MaxValue, int.MaxValue };
                }
            }
        }

        private void CalculateMinHeatLoss()
        {
            var q = new Queue<(int y, int x, int lossSoFar, Direction direction, int stepsLeft)>();

            q.Enqueue((0, 1, 0, Direction.Right, 2));
            q.Enqueue((1, 0, 0, Direction.Down, 2));

            while (q.Count > 0)
            {
                var (y, x, lossSoFar, direction, stepsLeft) = q.Dequeue();
                lossSoFar += _heatLossMap[y][x];

                if (_heatLossCalculation[y][x][(int)direction][stepsLeft] <= lossSoFar)
                {
                    continue;
                }
                _heatLossCalculation[y][x][(int)direction][stepsLeft] = lossSoFar;

                if (x == _size.x - 1 && y == _size.y - 1)
                {
                    continue;
                }

                if (stepsLeft > 0)
                {
                    switch (direction)
                    {
                        case Direction.Right when x + 1 < _size.x:
                            q.Enqueue((y, x + 1, lossSoFar, Direction.Right, stepsLeft - 1));
                            break;
                        case Direction.Down when y + 1 < _size.y:
                            q.Enqueue((y + 1, x, lossSoFar, Direction.Down, stepsLeft - 1));
                            break;
                        case Direction.Left when x - 1 >= 0:
                            q.Enqueue((y, x - 1, lossSoFar, Direction.Left, stepsLeft - 1));
                            break;
                        case Direction.Up when y - 1 >= 0:
                            q.Enqueue((y - 1, x, lossSoFar, Direction.Up, stepsLeft - 1));
                            break;
                    }
                }
                if (direction != Direction.Right && direction != Direction.Left && x + 1 < _size.x)
                {
                    q.Enqueue((y, x + 1, lossSoFar, Direction.Right, 2));
                }
                if (direction != Direction.Down && direction != Direction.Up && y + 1 < _size.y)
                {
                    q.Enqueue((y + 1, x, lossSoFar, Direction.Down, 2));
                }
                if (direction != Direction.Right && direction != Direction.Left && x - 1 >= 0)
                {
                    q.Enqueue((y, x - 1, lossSoFar, Direction.Left, 2));
                }
                if (direction != Direction.Down && direction != Direction.Up && y - 1 >= 0)
                {
                    q.Enqueue((y - 1, x, lossSoFar, Direction.Up, 2));
                }
            }
        }

        private enum InputData
        {
            Example,
            Puzzle,
        }

        private enum Direction
        {
            Right,
            Down,
            Left,
            Up,
        }
    }
}