using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private enum Input
        {
            Example,
            NonSquare,
            Puzzle
        }

        [field: SerializeField] private Input InputFile { get; set; }

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

        private char[][] _tiles = { };
        private int _rows = 1;
        private int _cols = 1;
        private Vector2Int _start = new(1, 0);
        private Vector2Int _end = new(1, 0);

        private PathsGraph _pathsGraph = new();

        private Camera _camera;

        private void Start()
        {
            ResultText.text = "...";
            _camera = Camera.main;

            Parse(InputFile switch
            {
                Input.Example => "day23/example2_in.txt",
                Input.NonSquare => "day23/non_square_in.txt",
                Input.Puzzle => "day23/puzzle2_in.txt",
                _ => "NOT_SET"
            });

            ConstructPathsGraph();

            foreach (var step in _pathsGraph.SearchForLongestPath(_start, _end))
            {
                ResultText.text = $"max: {step.maxLengthSoFar}\nlast: {step.recentLength}";
            }
            ResultText.text += "\nDONE";
        }

        private void Update()
        {
            DrawTiles();
            AdjustCameraToShowAllTiles();
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

        private void ConstructPathsGraph()
        {
            _pathsGraph = new PathsGraph();

            // Just in case domain reloading is turned off for a preview mode entering 
            Connection.NextId = 1;

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

            var queue = new Queue<(Vector2Int prev, Vector2Int next)>();
            queue.Enqueue((_start, _start));
            while (queue.Count > 0)
            {
                var (prev, curr) = queue.Dequeue();
                if (visited[curr.y][curr.x])
                {
                    continue;
                }

                print($"QUEUE step: {curr}  {visited[curr.y][curr.x]}");

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

                visited[curr.y][curr.x] = true;
                foreach (var a in adjacent)
                {
                    queue.Enqueue((curr, a));
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
    }
}