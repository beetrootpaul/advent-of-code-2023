using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace aoc2023
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

            var bricks = Day22Steps.Step1_Parse(inputFile switch
            {
                Input.Example => "day22/example.txt",
                Input.Puzzle => "day22/puzzle.txt",
                _ => "NOT_SET"
            });
            // TODO: render it

            Day22Steps.Step2_SettleFall(bricks);
            // TODO: render it

            Day22Steps.Step3_MarkSafeToDisintegrate(bricks);
            // TODO: render it

            var result = Day22Steps.Step4_CountSafeToDisintegrate(bricks);

            resultText.text = $"{result}";
        }
    }

    internal static class Day22Steps
    {
        public static IDictionary<int, Brick> Step1_Parse(string inputFilePath)
        {
            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}");
            var bricks = new Dictionary<int, Brick>();
            foreach (var line in lines)
            {
                var xyzText1 = line.Split('~')[0];
                var xyzText2 = line.Split('~')[1];
                var nextBrick = new Brick(
                    z1: int.Parse(xyzText1.Split(',')[2]),
                    z2: int.Parse(xyzText2.Split(',')[2])
                );
                bricks.Add(nextBrick.Id, nextBrick);
            }
            return bricks;
        }

        public static void Step2_SettleFall(IDictionary<int, Brick> bricks)
        {
            var highestBrickSoFar = Brick.GroundId;
            foreach (var brick in bricks.Values.OrderBy(b => b.XyzMin.z))
            {
                if (highestBrickSoFar == Brick.GroundId)
                {
                    brick.FallToGround();
                }
                else
                {
                    brick.FallTo(bricks[highestBrickSoFar]);
                }
                highestBrickSoFar = brick.Id;
            }
        }

        // TODO: honor XY
        public static void Step3_MarkSafeToDisintegrate(IDictionary<int, Brick> bricks)
        {
            foreach (var brick in bricks.Values)
            {
                brick.SafeToDisintegrate = brick.SupportedBricks.All(topBrickId =>
                    bricks[topBrickId].SupportingBricks.Count(siblingBrickId =>
                        siblingBrickId != brick.Id
                    ) > 0
                );
            }
        }

        public static int Step4_CountSafeToDisintegrate(IDictionary<int, Brick> bricks)
        {
            return bricks.Values.Count(b => b.SafeToDisintegrate);
        }
    }

    internal class Brick
    {
        public static int GroundId = 0;
        private static int _nextId = 1;

        public int Id { get; }
        public Vector3Int XyzMin { get; private set; }
        public Vector3Int XyzMax { get; private set; }
        public bool SafeToDisintegrate { get; internal set; }

        internal ICollection<int> SupportingBricks = new List<int>();
        internal ICollection<int> SupportedBricks = new List<int>();

        public Brick(int z1, int z2, bool safeToDisintegrate = false, int id = -1)
        {
            Id = id > -1 ? id : _nextId++;
            XyzMin = new Vector3Int(0, 0, Math.Min(z1, z2));
            XyzMax = new Vector3Int(0, 0, Math.Max(z1, z2));
            SafeToDisintegrate = safeToDisintegrate;
        }

        private Vector3Int Size => XyzMax - XyzMin + Vector3Int.one;

        // TODO: make it mutate the brick itself? 
        public void FallToGround()
        {
            const int newZMin = 1;
            XyzMax = new Vector3Int(XyzMax.x, XyzMax.y, newZMin + Size.z - 1);
            XyzMin = new Vector3Int(XyzMin.x, XyzMin.y, newZMin);
        }

        public void FallTo(Brick lowerBrick)
        {
            var newZMin = lowerBrick.XyzMax.z + 1;
            XyzMax = new Vector3Int(XyzMax.x, XyzMax.y, newZMin + Size.z - 1);
            XyzMin = new Vector3Int(XyzMin.x, XyzMin.y, newZMin);

            SupportingBricks.Add(lowerBrick.Id);
            lowerBrick.SupportedBricks.Add(Id);
        }

        public string Serialized()
        {
            var supporting = $"|{string.Join(",", SupportingBricks)}| -> ";
            var coords = $"[ ? , ? , {XyzMin.z}{(Size.z > 1 ? $"_{XyzMax.z}" : "")} ]";
            var attributes = $"{(SafeToDisintegrate ? " (X)" : "")}";
            var supported = $" -> |{string.Join(",", SupportedBricks)}|";
            return $"{supporting}{coords}{attributes}{supported}";
        }

        public override string ToString() => Serialized();
    };
}