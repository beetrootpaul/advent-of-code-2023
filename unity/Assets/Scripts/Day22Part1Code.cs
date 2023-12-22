using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace aoc2023.Scripts
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
        private Input inputFile;

        private void Start()
        {
            if (resultText == null) throw new Exception($"null {nameof(resultText)}");
            resultText.text = "...";

            var snapshot = Day22Steps.Step1_Parse(inputFile switch
            {
                Input.Example => "day22/example.txt",
                Input.Puzzle => "day22/puzzle.txt",
                _ => "NOT_SET"
            });
            // TODO: render the snapshot

            var settled = Day22Steps.Step2_SettleFall(snapshot);
            // TODO: render the snapshot

            resultText.text = $"{settled}";
        }
    }

    internal static class Day22Steps
    {
        public static SortedSet<Brick> Step1_Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}");
            var snapshot = new SortedSet<Brick>(new ByBrickMinZ());
            foreach (var line in lines)
            {
                var xyzText1 = line.Split('~')[0];
                var xyzText2 = line.Split('~')[1];
                snapshot.Add(new Brick(
                    z1: int.Parse(xyzText1.Split(',')[2]),
                    z2: int.Parse(xyzText2.Split(',')[2])
                ));
            }

            return snapshot;
        }

        public static SortedSet<Brick> Step2_SettleFall(SortedSet<Brick> snapshot)
        {
            var settled = new SortedSet<Brick>(new ByBrickMinZ());

            // TODO: honor XY
            var highestZSoFar = 0;
            foreach (var brick in snapshot)
            {
                var fallenBrick = brick.FallToZ(highestZSoFar + 1);
                settled.Add(fallenBrick);
                highestZSoFar = fallenBrick.XyzMax.z;
            }

            return settled;
        }
    }

    internal readonly struct Brick
    {
        public Vector3Int XyzMin { get; }

        public Vector3Int XyzMax { get; }

        private Vector3Int Size => XyzMax - XyzMin + Vector3Int.one;

        public Brick(int z1, int z2)
        {
            XyzMin = new Vector3Int(0, 0, Math.Min(z1, z2));
            XyzMax = new Vector3Int(0, 0, Math.Max(z1, z2));
        }

        public Brick FallToZ(int newZMin)
        {
            return new Brick(z1: newZMin, z2: newZMin + Size.z - 1);
        }

        public override string ToString()
        {
            return $"[ ? , ? , {XyzMin.z}{(Size.z > 1 ? $"_{XyzMax.z}" : "")} ]";
        }
    };

    internal class ByBrickMinZ : IComparer<Brick>
    {
        public int Compare(Brick b1, Brick b2)
        {
            if (b1.XyzMin.z != b2.XyzMin.z) return b1.XyzMin.z - b2.XyzMin.z;
            return 0;
        }
    }
}