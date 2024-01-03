using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace aoc2023.day23
{
    //
    // https://adventofcode.com/2023/day/23
    //
    internal class Day23Part2Code : MonoBehaviour
    {
        private enum InputData
        {
            Example,
            NonSquare,
            Puzzle
        }

        [field: SerializeField] private InputData InputFile { get; set; }

        [field: SerializeField] private TextMeshProUGUI ResultText { get; set; }
        [field: SerializeField] private Tilemap ForestTilemap { get; set; }
        [field: SerializeField] private Tilemap PathTilemap { get; set; }
        [field: SerializeField] private TileBase PathTile { get; set; }
        [field: SerializeField] private TileBase PathMarkedTile { get; set; }
        [field: SerializeField] private TileBase ForestFullTile { get; set; }
        [field: SerializeField] private TileBase ForestTBTile { get; set; }
        [field: SerializeField] private TileBase ForestLRTile { get; set; }
        [field: SerializeField] private TileBase ForestTLTile { get; set; }
        [field: SerializeField] private TileBase ForestTRTile { get; set; }
        [field: SerializeField] private TileBase ForestBLTile { get; set; }
        [field: SerializeField] private TileBase ForestBRTile { get; set; }
        [field: SerializeField] private TileBase ForestTTile { get; set; }
        [field: SerializeField] private TileBase ForestBTile { get; set; }
        [field: SerializeField] private TileBase ForestLTile { get; set; }
        [field: SerializeField] private TileBase ForestRTile { get; set; }

        private Camera _camera;

        private char[][] _tiles = { };
        private int _rows = 1;
        private int _cols = 1;
        private Vector2Int _start = new(1, 0);
        private Vector2Int _end = new(1, 0);

        private PathsGraph _pathsGraph = new();
        private bool canHighlight;

        private int _yieldConstructionSteps;
        private int _yieldSearchSteps;

        private async void Start()
        {
            ResultText.text = "...";
            _camera = Camera.main;

            _yieldConstructionSteps = InputFile switch
            {
                InputData.Example => 5,
                InputData.NonSquare => 3,
                InputData.Puzzle => 50_000,
                _ => 10
            };
            _yieldSearchSteps = InputFile switch
            {
                InputData.Example => 1,
                InputData.NonSquare => 1,
                InputData.Puzzle => 25,
                _ => 1
            };

            Parse(InputFile switch
            {
                InputData.Example => "day23/example2_in.txt",
                InputData.NonSquare => "day23/non_square_in.txt",
                InputData.Puzzle => "day23/puzzle2_in.txt",
                _ => "NOT_SET"
            });

            ResultText.text = "constructing a graph ...";
            await ConstructPathsGraph();
            ResultText.text = "the graph is ready";
            canHighlight = true;

            canHighlight = false;
            _pathsGraph.MarkEverythingNotVisited();
            var counter = 0;
            var result = -1;
            foreach (var step in _pathsGraph.SearchForLongestPath(_start, _end))
            {
                counter++;

                result = Math.Max(result, step.maxLengthSoFar);

                if (step.reachedEnd)
                {
                    // print($"next length: max={step.maxLengthSoFar} last={step.recentLength}");
                    ResultText.text =
                        $"max: {step.maxLengthSoFar}\nlast: {step.recentLength}";
                }

                if (_yieldSearchSteps > 0 && counter % _yieldSearchSteps == 0)
                {
                    await UniTask.Delay(25);
                }
            }
            canHighlight = true;
            ResultText.text += "\nDONE";
        }

        private void Update()
        {
            DrawTiles();
            AdjustCameraToShowAllTiles();
            if (canHighlight)
            {
                HighlightGraphPortionUnderCursor();
            }
        }

        private void Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}");
            _tiles = lines
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .Select(line => line.ToCharArray())
                .ToArray();
            _rows = _tiles.Length;
            _cols = _tiles[0].Length;

            _start = new Vector2Int(1, 0);
            _end = new Vector2Int(_cols - 2, _rows - 1);

            print(string.Join('\n', _tiles.Select(tilesRow => string.Join("", tilesRow))));
        }

        private async UniTask ConstructPathsGraph()
        {
            _pathsGraph = new PathsGraph();

            // Just in case domain reloading is turned off for a preview mode entering 
            Joint.NextId = 1;
            Connection.NextId = 1;

            var counter = 0;

            var visited = new List<List<bool>>();
            for (var row = 0; row < _rows; row++)
            {
                var visitedRow = new List<bool>();
                for (var col = 0; col < _cols; col++)
                {
                    visitedRow.Add(false);
                }
                visited.Add(visitedRow);
            }

            var stack = new Stack<(Vector2Int prev, Vector2Int next)>();
            stack.Push((_start, _start));
            while (stack.Count > 0)
            {
                counter++;

                _pathsGraph.MarkEverythingNotVisited();

                var (prev, curr) = stack.Pop();
                if (visited[curr.y][curr.x])
                {
                    _pathsGraph.Connect(prev, curr);
                    continue;
                }

                // print($"QUEUE step: {curr}  {visited[curr.y][curr.x]}");

                var adjacent = AdjacentPathTilesOf(curr).ToList();

                var isJoint = curr == _start || curr == _end || adjacent.Count() > 2;
                if (isJoint)
                {
                    _pathsGraph.RecordJointAt(curr);
                }
                else
                {
                    _pathsGraph.RecordConnectionAt(curr);
                }

                _pathsGraph.Connect(prev, curr);
                _pathsGraph.MarkAdjacentJointsAndConnectionsVisitedAt(curr);

                visited[curr.y][curr.x] = true;
                foreach (var a in adjacent)
                {
                    stack.Push((curr, a));
                }

                if (_yieldConstructionSteps > 0 && counter % _yieldConstructionSteps == 0)
                {
                    print($"... graph construction still running (counter={counter})");
                    await UniTask.Delay(25);
                }
            }

            _pathsGraph.Log();
        }

        private IEnumerable<Vector2Int> AdjacentPathTilesOf(Vector2Int tile)
        {
            var adjacent = new List<Vector2Int>();
            if (tile.x > 0 && _tiles[tile.y][tile.x - 1] != '#')
            {
                adjacent.Add(new Vector2Int(tile.x - 1, tile.y));
            }
            if (tile.y > 0 && _tiles[tile.y - 1][tile.x] != '#')
            {
                adjacent.Add(new Vector2Int(tile.x, tile.y - 1));
            }
            if (tile.x < _cols - 1 && _tiles[tile.y][tile.x + 1] != '#')
            {
                adjacent.Add(new Vector2Int(tile.x + 1, tile.y));
            }
            if (tile.y < _rows - 1 && _tiles[tile.y + 1][tile.x] != '#')
            {
                adjacent.Add(new Vector2Int(tile.x, tile.y + 1));
            }
            return adjacent;
        }

        private void DrawTiles()
        {
            PathTilemap.ClearAllTiles();
            ForestTilemap.ClearAllTiles();

            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _cols; col++)
                {
                    var tilemapXy = new Vector3Int(col, _rows - row - 1);

                    PathTilemap.SetTile(tilemapXy,
                        _pathsGraph.IsVisited(new Vector2Int(col, row)) ? PathMarkedTile : PathTile);

                    if (_tiles[row][col] == '#')
                    {
                        ForestTilemap.SetTile(tilemapXy, ForestFullTile);
                    }
                    else
                    {
                        var t = row <= 0 || _tiles[row - 1][col] == '#';
                        var b = row >= _rows - 1 || _tiles[row + 1][col] == '#';
                        var l = col <= 0 || _tiles[row][col - 1] == '#';
                        var r = col >= _cols - 1 || _tiles[row][col + 1] == '#';
                        var mask = (t ? 0b1000 : 0) + (b ? 0b0100 : 0) + (l ? 0b0010 : 0) + (r ? 0b0001 : 0);
                        var forestTile = mask switch
                        {
                            0b0001 => ForestRTile,
                            0b0010 => ForestLTile,
                            0b0011 => ForestLRTile,
                            0b0100 => ForestBTile,
                            0b0101 => ForestBRTile,
                            0b0110 => ForestBLTile,
                            0b1000 => ForestTTile,
                            0b1001 => ForestTRTile,
                            0b1010 => ForestTLTile,
                            0b1100 => ForestTBTile,
                            _ => null
                        };
                        ForestTilemap.SetTile(tilemapXy, forestTile);
                    }
                }
            }
        }

        private void AdjustCameraToShowAllTiles()
        {
            var tilemapSize = PathTilemap.layoutGrid.cellSize * new Vector2(_cols, _rows);
            _camera.orthographicSize = (tilemapSize.x > tilemapSize.y * _camera.aspect)
                ? (tilemapSize.x * .5f / _camera.aspect)
                : (tilemapSize.y * .5f);
            _camera.transform.SetPositionAndRotation(
                new Vector3(tilemapSize.x * .5f, tilemapSize.y * .5f, _camera.transform.position.z),
                _camera.transform.rotation
            );
        }

        private void HighlightGraphPortionUnderCursor()
        {
            _pathsGraph.MarkEverythingNotVisited();

            var tileUnderCursor = PathTilemap.WorldToCell(_camera.ScreenToWorldPoint(Input.mousePosition));
            var x = tileUnderCursor.x;
            var y = _rows - tileUnderCursor.y - 1;
            if (x >= 0 && y >= 0 && x < _cols && y < _rows)
            {
                _pathsGraph.MarkAdjacentJointsAndConnectionsVisitedAt(new Vector2Int(x, y));
            }
        }
    }
}