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
            var bricks = Day22Steps.Step1_Parse("day22/example.txt");
            foreach (var brick in bricks.Values)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new(x1: 1, z1: 8, x2: 1, z2: 9),
                    new(x1: 0, z1: 6, x2: 2, z2: 6),
                    new(x1: 2, z1: 5, x2: 2, z2: 5),
                    new(x1: 0, z1: 4, x2: 0, z2: 4),
                    new(x1: 0, z1: 3, x2: 2, z2: 3),
                    new(x1: 0, z1: 2, x2: 2, z2: 2),
                    new(x1: 1, z1: 1, x2: 1, z2: 1),
                }.OrderBy(b => b.XyzMin.z).Select(b => (b.XyzMin, b.XyzMax)),
                bricks.Values.OrderBy(b => b.XyzMin.z).Select(b => (b.XyzMin, b.XyzMax))
            );
        }

        [Test]
        public void Step2_SettleFall()
        {
            // given
            var bricks = new Brick[]
            {
                new(id: 41, x1: 100, z1: 19, x2: 119, z2: 19),
                //
                new(id: 33, x1: 111, z1: 14, x2: 111, z2: 15),
                new(id: 32, x1: 100, z1: 13, x2: 100, z2: 15),
                new(id: 31, x1: 100, z1: 8, x2: 111, z2: 10),
                //
                new(id: 22, x1: 110, z1: 7, x2: 111, z2: 7),
                new(id: 21, x1: 109, z1: 3, x2: 110, z2: 4),
                new(id: 20, x1: 100, z1: 3, x2: 100, z2: 5),
                //
                new(id: 2, x1: 110, z1: 2, x2: 119, z2: 2),
                new(id: 1, x1: 100, z1: 2, x2: 109, z2: 2)
            }.ToDictionary(b => b.Id);

            // when
            Day22Steps.Step2_SettleFall(bricks);
            foreach (var brick in bricks.Values)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new(id: 41, x1: 100, z1: 11, x2: 119, z2: 11)
                        { SupportingBricks = { 32 }, SupportedBricks = { } },
                    //
                    new(id: 33, x1: 111, z1: 8, x2: 111, z2: 9)
                        { SupportingBricks = { 31 }, SupportedBricks = { } },
                    new(id: 32, x1: 100, z1: 8, x2: 100, z2: 10)
                        { SupportingBricks = { 31 }, SupportedBricks = { 41 } },
                    new(id: 31, x1: 100, z1: 5, x2: 111, z2: 7)
                        { SupportingBricks = { 20, 22 }, SupportedBricks = { 32, 33 } },
                    //
                    new(id: 22, x1: 110, z1: 4, x2: 111, z2: 4)
                        { SupportingBricks = { 21 }, SupportedBricks = { 31 } },
                    new(id: 21, x1: 109, z1: 2, x2: 110, z2: 3)
                        { SupportingBricks = { 1, 2 }, SupportedBricks = { 22 } },
                    new(id: 20, x1: 100, z1: 2, x2: 100, z2: 4)
                        { SupportingBricks = { 1 }, SupportedBricks = { 31 } },
                    //
                    new(id: 2, x1: 110, z1: 1, x2: 119, z2: 1)
                        { SupportingBricks = { 0 }, SupportedBricks = { 21 } },
                    new(id: 1, x1: 100, z1: 1, x2: 109, z2: 1)
                        { SupportingBricks = { 0 }, SupportedBricks = { 20, 21 } },
                }.OrderBy(b => b.XyzMin.z).Select(b => b.Serialized()),
                bricks.Values.OrderBy(b => b.XyzMin.z).Select(b => b.Serialized())
            );
        }

        [Test]
        public void Step3_MarkSafeToDisintegrate()
        {
            // given
            var bricks = new Brick[]
            {
                new(id: 3, x1: 100, z1: 4, x2: 102, z2: 4) { SupportingBricks = { 2 } },
                new(id: 2, x1: 100, z1: 2, x2: 102, z2: 3) { SupportingBricks = { 1 }, SupportedBricks = { 3 } },
                new(id: 1, x1: 100, z1: 1, x2: 102, z2: 1) { SupportedBricks = { 2 } },
            }.ToDictionary(b => b.Id);

            // when
            Day22Steps.Step3_MarkSafeToDisintegrate(bricks);
            foreach (var brick in bricks.Values)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new(id: 3, x1: 100, z1: 4, x2: 102, z2: 4, safeToDisintegrate: true) { SupportingBricks = { 2 } },
                    new(id: 2, x1: 100, z1: 2, x2: 102, z2: 3) { SupportingBricks = { 1 }, SupportedBricks = { 3 } },
                    new(id: 1, x1: 100, z1: 1, x2: 102, z2: 1) { SupportedBricks = { 2 } }
                }.OrderBy(b => b.XyzMin.z).Select(b => b.Serialized()),
                bricks.Values.OrderBy(b => b.XyzMin.z).Select(b => b.Serialized())
            );
        }

        [Test]
        public void Part1_ExampleFull()
        {
            // given
            var expectedResult =
                int.Parse(File.ReadAllLines($"{streamingAssetsPath}/day22/example_result.txt")[0]);

            // when
            var bricks = Day22Steps.Step1_Parse("day22/example.txt");
            Day22Steps.Step2_SettleFall(bricks);
            Day22Steps.Step3_MarkSafeToDisintegrate(bricks);
            var result = Day22Steps.Step4_CountSafeToDisintegrate(bricks);

            // 
            Assert.AreEqual(expectedResult, result);
        }
    }
}