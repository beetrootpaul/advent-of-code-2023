using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace aoc2023
{
    //
    // https://adventofcode.com/2023/day/11
    //
    public class Day11Part1Code : MonoBehaviour
    {
        private enum Input
        {
            Example1,
            Puzzle1
        }

        private enum State
        {
            Init,
            InitDone,
            MarkEmptyRowsAndColsEnter,
            MarkEmptyRowsAndCols,
            MarkEmptyRowsAndColsDone,
            ExpandEmptyRowsAndColsEnter,
            ExpandEmptyRowsAndCols,
            ExpandEmptyRowsAndColsDone,
            CalculateShortestPathsEnter,
            CalculateShortestPaths,
            CalculateShortestPathsDone
        }

        [SerializeField]
        private TextMeshProUGUI? resultText;

        [FormerlySerializedAs("tileEmptyPrefab")]
        [SerializeField]
        private Tile? tilePrefab;
        [SerializeField]
        private Tile? tileStarPrefab;
        [SerializeField]
        private Button? nextButton;
        [SerializeField]
        private Transform? parentForInstantiatedTiles;

        [SerializeField]
        private Input inputFile;

        private Camera? _camera;

        private List<StringBuilder> _imageData = new();
        private readonly List<List<Tile>> _tiles = new();
        private Vector2Int _size;
        private readonly ICollection<int> _emptyRows = new HashSet<int>();
        private readonly ICollection<int> _emptyCols = new HashSet<int>();

        private State _state = State.Init;


        private async void Start()
        {
            _camera = Camera.main;
            resultText.text = "";

            await InitFor(inputFile switch
            {
                Input.Example1 => "day11/example1_in.txt",
                Input.Puzzle1 => "day11/puzzle1_in.txt",
                _ => "NOT_SET"
            });

            await InstantiateTiles();
            _state = State.InitDone;
        }

        private void OnEnable()
        {
            nextButton.onClick.AddListener(ProceedToNextState);
        }

        private void OnDisable()
        {
            nextButton.onClick.RemoveListener(ProceedToNextState);
        }

        private void ProceedToNextState()
        {
            _state = _state switch
            {
                State.InitDone => State.MarkEmptyRowsAndColsEnter,
                State.MarkEmptyRowsAndColsDone => State.ExpandEmptyRowsAndColsEnter,
                State.ExpandEmptyRowsAndColsDone => State.CalculateShortestPathsEnter,
                _ => _state
            };
        }

        private async void Update()
        {
            switch (_state)
            {
                case State.MarkEmptyRowsAndColsEnter:
                    _state = State.MarkEmptyRowsAndCols;
                    MarkEmptyRowsAndCols();
                    _state = State.MarkEmptyRowsAndColsDone;
                    break;
                case State.ExpandEmptyRowsAndColsEnter:
                    _state = State.ExpandEmptyRowsAndCols;
                    await ExpandEmptyRowsAndCols();
                    _state = State.ExpandEmptyRowsAndColsDone;
                    break;
                case State.CalculateShortestPathsEnter:
                    _state = State.CalculateShortestPaths;
                    var shortestPaths = await CalculateShortestPaths();
                    var sum = shortestPaths.Sum();
                    resultText.text = $"SUM: {sum}";
                    _state = State.CalculateShortestPathsDone;
                    break;
            }

            if (UnityEngine.Input.GetKeyDown("return"))
            {
                ProceedToNextState();
            }
        }

        private async UniTask InitFor(string file)
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

            _imageData = new List<StringBuilder>();
            _size = Vector2Int.zero;
            foreach (var line in lines)
            {
                if (!line.Contains("#"))
                {
                    _emptyRows.Add(_size.y);
                }
                _size.y++;
                _size.x = line.Length;
                _imageData.Add(new StringBuilder(line));
            }
            print(string.Join("\n", _imageData.Select(s => s.ToString())));

            for (var col = 0; col < _size.x; col++)
            {
                var isColEmpty = true;
                for (var row = 0; row < _size.y; row++)
                {
                    isColEmpty = isColEmpty && _imageData[row][col] != '#';
                }
                if (isColEmpty)
                {
                    _emptyCols.Add(col);
                }
            }
        }

        private async UniTask InstantiateTiles()
        {
            var scale = new Vector2(
                Math.Min(7f / _size.y, 7f / _size.x),
                Math.Min(7f / _size.y, 7f / _size.x)
            );
            var w = .9f / _camera.aspect;
            var h = .9f;
            for (var row = 0; row < _size.y; row++)
            {
                _tiles.Add(new List<Tile>());
                for (var col = 0; col < _size.x; col++)
                {
                    // This line is no longer needed since we moved from Task to 3rd party UniTask:
                    // destroyCancellationToken.ThrowIfCancellationRequested();
                    
                    var tile = Instantiate(
                        _imageData[row][col] == '#' ? tileStarPrefab : tilePrefab,
                        _camera.ViewportToWorldPoint(
                            new Vector3(
                                (1f - w) / 2f + (col + .5f) / _size.x * w,
                                1f - (1f - h) / 2f - (row + .5f) / _size.y * h,
                                1
                            )
                        ),
                        Quaternion.identity,
                        parentForInstantiatedTiles
                    );
                    tile.transform.localScale = scale;
                    _tiles[row].Add(tile);

                    tile.Appear(500);

                    // await UniTask.Delay(TimeSpan.FromSeconds(0.02));
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        }

        private void MarkEmptyRowsAndCols()
        {
            foreach (var row in _emptyRows)
            {
                for (var col = 0; col < _size.x; col++)
                {
                    _tiles[row][col].IndicateEmpty();
                }
            }
            foreach (var col in _emptyCols)
            {
                for (var row = 0; row < _size.y; row++)
                {
                    _tiles[row][col].IndicateEmpty();
                }
            }
        }

        private async UniTask ExpandEmptyRowsAndCols()
        {
            for (var col = _size.x - 1; col >= 0; col--)
            {
                if (!_emptyCols.Contains(col)) continue;
                _size.x++;
                for (var row = 0; row < _size.y; row++)
                {
                    _imageData[row].Insert(col, '.');
                }
            }
            for (var row = _size.y - 1; row >= 0; row--)
            {
                if (!_emptyRows.Contains(row)) continue;
                _size.y++;
                _imageData.Insert(row, new StringBuilder("".PadRight(_size.x, '.')));
            }
            print(string.Join("\n", _imageData.Select(s => s.ToString())));

            var prevTiles = new List<Tile>(_tiles.SelectMany(tilesRow => tilesRow));
            foreach (var tile in prevTiles)
            {
                tile.Disappear(250);
            }

            await Task.Delay(250);

            await InstantiateTiles();
        }

        private async UniTask<List<int>> CalculateShortestPaths()
        {
            var shortestPaths = new List<int>();

            for (var row = 0; row < _size.y; row++)
            {
                for (var col = 0; col < _size.x; col++)
                {
                    if (_imageData[row][col] != '#') continue;
                    destroyCancellationToken.ThrowIfCancellationRequested();
                    for (var row2 = row; row2 < _size.y; row2++)
                    {
                        for (var col2 = 0; col2 < _size.x; col2++)
                        {
                            if (row2 == row && col2 <= col) continue;
                            if (_imageData[row2][col2] == '#')
                            {
                                shortestPaths.Add(Math.Abs(row2 - row) + Math.Abs(col2 - col));
                            }
                        }
                    }
                    await UniTask.Yield();
                }
            }

            return shortestPaths;
        }
    }
}