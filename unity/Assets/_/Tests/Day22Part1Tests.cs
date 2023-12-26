using System.Collections.Generic;
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
            foreach (var brick in bricks)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new(z1: 8, z2: 9),
                    new(z1: 6, z2: 6),
                    new(z1: 5, z2: 5),
                    new(z1: 4, z2: 4),
                    new(z1: 3, z2: 3),
                    new(z1: 2, z2: 2),
                    new(z1: 1, z2: 1),
                }.OrderBy(b => b.XyzMin.z).Select(b => b.Serialized()),
                bricks.Values.OrderBy(b => b.XyzMin.z).Select(b => b.Serialized())
            );
        }

        [Test]
        public void Step2_SettleFall()
        {
            // given
            var bricks = new Brick[]
            {
                new(id: 5, z1: 13, z2: 15),
                new(id: 4, z1: 8, z2: 10),
                new(id: 3, z1: 7, z2: 7),
                new(id: 2, z1: 3, z2: 4),
                new(id: 1, z1: 2, z2: 2),
            }.ToDictionary(b => b.Id);

            // when
            Day22Steps.Step2_SettleFall(bricks);
            foreach (var brick in bricks)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new(id: 5, z1: 8, z2: 10) { SupportingBricks = { 4 } },
                    new(id: 4, z1: 5, z2: 7) { SupportingBricks = { 3 }, SupportedBricks = { 5 } },
                    new(id: 3, z1: 4, z2: 4) { SupportingBricks = { 2 }, SupportedBricks = { 4 } },
                    new(id: 2, z1: 2, z2: 3) { SupportingBricks = { 1 }, SupportedBricks = { 3 } },
                    new(id: 1, z1: 1, z2: 1) { SupportedBricks = { 2 } },
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
                new(id: 3, z1: 4, z2: 4) { SupportingBricks = { 2 } },
                new(id: 2, z1: 2, z2: 3) { SupportingBricks = { 1 }, SupportedBricks = { 3 } },
                new(id: 1, z1: 1, z2: 1) { SupportedBricks = { 2 } },
            }.ToDictionary(b => b.Id);

            // when
            Day22Steps.Step3_MarkSafeToDisintegrate(bricks);
            foreach (var brick in bricks)
            {
                Debug.Log(brick);
            }

            // then
            Assert.AreEqual(new Brick[]
                {
                    new(id: 3, z1: 4, z2: 4, safeToDisintegrate: true) { SupportingBricks = { 2 } },
                    new(id: 2, z1: 2, z2: 3) { SupportingBricks = { 1 }, SupportedBricks = { 3 } },
                    new(id: 1, z1: 1, z2: 1) { SupportedBricks = { 2 } }
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