using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using static UnityEngine.Application;

namespace aoc2023.Day22
{
    public class Day22Part1Tests
    {
        [Test]
        public void Step1_Parse()
        {
            // when
            var snapshot = Day22Steps.Step1_Parse("day22/example.txt");

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
            }.Reverse(), snapshot);
        }

        [Test]
        public void Step2_SettleFall()
        {
            // given
            var snapshot = new SortedSet<Brick>(new Brick[]
            {
                new(z1: 12, z2: 14),
                new(z1: 7, z2: 9),
                new(z1: 6, z2: 6),
                new(z1: 2, z2: 3),
                new(z1: 1, z2: 1),
            }, new ByBrickMinZ());

            // when
            var settled = Day22Steps.Step2_SettleFall(snapshot);

            // then
            Assert.AreEqual(new Brick[]
            {
                new(z1: 8, z2: 10),
                new(z1: 5, z2: 7),
                new(z1: 4, z2: 4),
                new(z1: 2, z2: 3),
                new(z1: 1, z2: 1),
            }.Reverse(), settled);
        }

        [Test]
        public void Part1_ExampleFull()
        {
            // given
            var expectedResult =
                int.Parse(File.ReadAllLines($"{streamingAssetsPath}/day22/example_result.txt")[0]);

            // when
            var result = Day22Steps.Step1_Parse("day22/example.txt");

            // then
            Assert.AreEqual(expectedResult + 12, result);
        }
    }
}