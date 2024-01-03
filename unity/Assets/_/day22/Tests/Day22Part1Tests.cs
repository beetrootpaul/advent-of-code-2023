using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static UnityEngine.Application;

namespace aoc2023.Day22
{
    public class Day22Part1Tests
    {
        [Test]
        public void Step1_Parse()
        {
            // when
            var bricksCoords = Day22Steps.Step1_Parse("day22/example.txt");
            foreach (var brick in bricksCoords)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new() { XyzMin = new Vector3Int(1, 1, 8), XyzMax = new Vector3Int(1, 1, 9) },
                    new() { XyzMin = new Vector3Int(0, 1, 6), XyzMax = new Vector3Int(2, 1, 6) },
                    new() { XyzMin = new Vector3Int(2, 0, 5), XyzMax = new Vector3Int(2, 2, 5) },
                    new() { XyzMin = new Vector3Int(0, 0, 4), XyzMax = new Vector3Int(0, 2, 4) },
                    new() { XyzMin = new Vector3Int(0, 2, 3), XyzMax = new Vector3Int(2, 2, 3) },
                    new() { XyzMin = new Vector3Int(0, 0, 2), XyzMax = new Vector3Int(2, 0, 2) },
                    new() { XyzMin = new Vector3Int(1, 0, 1), XyzMax = new Vector3Int(1, 2, 1) },
                }.OrderBy(b => b.XyzMin.z).Select(b => (xyzMin: b.XyzMin, xyzMax: b.XyzMax)),
                bricksCoords.OrderBy(brickCoords => Math.Min(brickCoords.Item1.z, brickCoords.Item2.z))
            );
        }

        [Test]
        public void Part1_ExampleFull()
        {
            // given
            var expectedResult =
                int.Parse(File.ReadAllLines($"{streamingAssetsPath}/day22/example_result.txt")[0]);

            // when
            var bricksCoords = Day22Steps.Step1_Parse("day22/example.txt");
            var bricks = bricksCoords.Select(brickCoords =>
            {
                var brick = new GameObject().AddComponent<Brick>();
                brick.XyzMin = brickCoords.Item1;
                brick.XyzMax = brickCoords.Item2;
                return brick;
            }).ToDictionary(b => b.Id);
            Day22Steps.Step2_SettleFall(bricks);
            Day22Steps.Step3_MarkSafeToDisintegrate(bricks);
            var result = Day22Steps.Step4_CountSafeToDisintegrate(bricks);

            // 
            Assert.AreEqual(expectedResult, result);
        }
    }
}