using System;
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
    // part 2
    //
    internal class Day17Part2Code : MonoBehaviour
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

        private const int MinStepsForward = 4;
        private const int MaxStepsForward = 10;

        private void Start()
        {
            guiText.text = "...";

            Parse(inputData switch
            {
                InputData.ExampleA => "day17/example2a_in.txt",
                InputData.ExampleB => "day17/example2b_in.txt",
                InputData.Puzzle => "day17/puzzle2_in.txt",
                _ => "NOT_SET"
            });

            CalculateMinHeatLoss();

            var goalY = _size.y - 1;
            var goalX = _size.x - 1;
            var result = new List<int>(new[]
            {
                // `.SkipLast(MinStepsForward - 1)`, because we have to ignore cases where the crucible was unable to stop at the end (minimum of 4 steps forward required)
                _heatLossCalculation[goalY][goalX][(int)Direction.Right].SkipLast(MinStepsForward - 1).Min(),
                _heatLossCalculation[goalY][goalX][(int)Direction.Down].SkipLast(MinStepsForward - 1).Min(),
                _heatLossCalculation[goalY][goalX][(int)Direction.Left].SkipLast(MinStepsForward - 1).Min(),
                _heatLossCalculation[goalY][goalX][(int)Direction.Up].SkipLast(MinStepsForward - 1).Min(),
            }).Min();

            print("<color=yellow>debug</color>");
            for (var stepsLeft = 0; stepsLeft < MaxStepsForward; stepsLeft++)
            {
                print($"for steps left = {stepsLeft}");
                print($"R{stepsLeft} {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Right][stepsLeft]}");
                print($"D{stepsLeft} {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Down][stepsLeft]}");
                print($"L{stepsLeft} {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Left][stepsLeft]}");
                print($"U{stepsLeft} {_heatLossCalculation[debugXy.y][debugXy.x][(int)Direction.Up][stepsLeft]}");
            }

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
                        Enumerable.Range(0, MaxStepsForward).Select(_ => int.MaxValue).ToArray();
                    _heatLossCalculation[y][x][(int)Direction.Down] =
                        Enumerable.Range(0, MaxStepsForward).Select(_ => int.MaxValue).ToArray();
                    _heatLossCalculation[y][x][(int)Direction.Left] =
                        Enumerable.Range(0, MaxStepsForward).Select(_ => int.MaxValue).ToArray();
                    _heatLossCalculation[y][x][(int)Direction.Up] =
                        Enumerable.Range(0, MaxStepsForward).Select(_ => int.MaxValue).ToArray();
                }
            }
        }

        private void CalculateMinHeatLoss()
        {
            var q = new Queue<(int y, int x, int lossSoFar, Direction direction, int stepsLeft)>();

            q.Enqueue((0, 1, 0, Direction.Right, MaxStepsForward - 1));
            q.Enqueue((1, 0, 0, Direction.Down, MaxStepsForward - 1));

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

                // move in the same direction before the steps limit is reached
                if (stepsLeft > 0)
                {
                    switch (direction)
                    {
                        case Direction.Right when x + 1 < _size.x:
                            q.Enqueue((y, x + 1, lossSoFar, direction, stepsLeft - 1));
                            break;
                        case Direction.Down when y + 1 < _size.y:
                            q.Enqueue((y + 1, x, lossSoFar, direction, stepsLeft - 1));
                            break;
                        case Direction.Left when x - 1 >= 0:
                            q.Enqueue((y, x - 1, lossSoFar, direction, stepsLeft - 1));
                            break;
                        case Direction.Up when y - 1 >= 0:
                            q.Enqueue((y - 1, x, lossSoFar, direction, stepsLeft - 1));
                            break;
                    }
                }

                // first 4 steps *have* to go in the same direction
                if (MaxStepsForward - stepsLeft < MinStepsForward)
                {
                    continue;
                }

                // after first 4 steps it's possible to turn (but not go backwards)
                if (direction != Direction.Right && direction != Direction.Left && x + 1 < _size.x)
                {
                    q.Enqueue((y, x + 1, lossSoFar, Direction.Right, MaxStepsForward - 1));
                }
                if (direction != Direction.Down && direction != Direction.Up && y + 1 < _size.y)
                {
                    q.Enqueue((y + 1, x, lossSoFar, Direction.Down, MaxStepsForward - 1));
                }
                if (direction != Direction.Right && direction != Direction.Left && x - 1 >= 0)
                {
                    q.Enqueue((y, x - 1, lossSoFar, Direction.Left, MaxStepsForward - 1));
                }
                if (direction != Direction.Down && direction != Direction.Up && y - 1 >= 0)
                {
                    q.Enqueue((y - 1, x, lossSoFar, Direction.Up, MaxStepsForward - 1));
                }
            }
        }

        private enum InputData
        {
            ExampleA,
            ExampleB,
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