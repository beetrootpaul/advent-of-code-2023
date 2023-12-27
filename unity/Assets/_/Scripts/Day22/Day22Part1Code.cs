using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace aoc2023.Day22
{
    //
    // https://adventofcode.com/2023/day/22
    //
    public class Day22Part1Code : MonoBehaviour
    {
        private enum Input
        {
            Example,
            Puzzle
        }

        [SerializeField]
        private TextMeshProUGUI? resultText;
        [SerializeField]
        private GameObject? brickPrefab;
        [SerializeField]
        private GameObject? parentForInstantiatedTiles;

        [SerializeField]
        private Input inputFile;

        private async void Start()
        {
            if (resultText == null) throw new Exception($"null {nameof(resultText)}");
            resultText.text = "...";

            var bricksCoords = Day22Steps.Step1_Parse(inputFile switch
            {
                Input.Example => "day22/example.txt",
                Input.Puzzle => "day22/puzzle.txt",
                _ => "NOT_SET"
            });
            var bricks = bricksCoords.Select(brickCoords =>
            {
                var brick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity)!.GetComponent<Brick>();
                if (parentForInstantiatedTiles != null)
                {
                    brick.transform.parent = parentForInstantiatedTiles.transform;
                }
                brick.xyzMin = Vector3Int.Min(brickCoords.Item1, brickCoords.Item2);
                brick.xyzMax = Vector3Int.Max(brickCoords.Item1, brickCoords.Item2);
                return brick;
            }).ToDictionary(b => b.Id);

            var maxZ = bricks.Values.Select(b => b.xyzMax.z).Max();
            var frustumHeight = (maxZ + 4) * 1.1f;
            var distance = frustumHeight * .5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            Debug.Log(distance);
            Camera.main.transform.SetPositionAndRotation(
                new Vector3(
                    Camera.main.transform.position.x,
                    distance,
                    Camera.main.transform.position.z - 2f + 0.9f * frustumHeight / 2f
                ),
                Camera.main.transform.rotation
            );

            await UniTask.Delay(2000);
            Day22Steps.Step2_SettleFall(bricks);

            await UniTask.Delay(2000);
            Day22Steps.Step3_MarkSafeToDisintegrate(bricks);

            var result = Day22Steps.Step4_CountSafeToDisintegrate(bricks);
            resultText.text = $"{result}";
        }
    }

    internal static class Day22Steps
    {
        public static IEnumerable<(Vector3Int, Vector3Int)> Step1_Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}");
            var bricks = new List<(Vector3Int, Vector3Int)>();
            foreach (var line in lines)
            {
                var xyzText1 = line.Split('~')[0];
                var xyzText2 = line.Split('~')[1];
                var xyz1 = new Vector3Int(
                    int.Parse(xyzText1.Split(',')[0]),
                    int.Parse(xyzText1.Split(',')[1]),
                    int.Parse(xyzText1.Split(',')[2])
                );
                var xyz2 = new Vector3Int(
                    int.Parse(xyzText2.Split(',')[0]),
                    int.Parse(xyzText2.Split(',')[1]),
                    int.Parse(xyzText2.Split(',')[2])
                );
                bricks.Add((xyz1, xyz2));
            }
            return bricks;
        }

        public static void Step2_SettleFall(IDictionary<int, Brick> bricks)
        {
            var maxX = bricks.Values.Select(b => b.xyzMax.x).Max();
            var maxY = bricks.Values.Select(b => b.xyzMax.y).Max();
            var highestBricksSoFar = Enumerable.Range(0, maxX + 1).Select(_ =>
                Enumerable.Range(0, maxY + 1).Select(_ => Brick.Ground).ToArray()
            ).ToArray();
            foreach (var current in bricks.Values.OrderBy(b => b.xyzMin.z))
            {
                var localMaxHeight = 0;
                for (var x = current.xyzMin.x; x <= current.xyzMax.x; x++)
                {
                    for (var y = current.xyzMin.y; y <= current.xyzMax.y; y++)
                    {
                        localMaxHeight = Math.Max(localMaxHeight, highestBricksSoFar[x][y].xyzMax.z);
                    }
                }
                current.FallTo(localMaxHeight + 1);
                for (var x = current.xyzMin.x; x <= current.xyzMax.x; x++)
                {
                    for (var y = current.xyzMin.y; y <= current.xyzMax.y; y++)
                    {
                        if (highestBricksSoFar[x][y].xyzMax.z == localMaxHeight)
                        {
                            current.ConnectAsSupporting(highestBricksSoFar[x][y]);
                        }
                        highestBricksSoFar[x][y] = current;
                    }
                }
            }
        }

        public static void Step3_MarkSafeToDisintegrate(IDictionary<int, Brick> bricks)
        {
            foreach (var brick in bricks.Values)
            {
                brick.safeToDisintegrate = brick.SupportedBricks.All(topBrickId =>
                    bricks[topBrickId].SupportingBricks.Count(siblingBrickId =>
                        siblingBrickId != brick.Id
                    ) > 0
                );
            }
        }

        public static int Step4_CountSafeToDisintegrate(IDictionary<int, Brick> bricks)
        {
            return bricks.Values.Count(b => b.safeToDisintegrate);
        }
    }
}