using System.Collections;
using aoc2023.Scripts;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace aoc2023.Tests
{
    public class Day19Part1Tests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Day19Part1TestsSimplePasses()
        {
            // Use the Assert class to test conditions
            // GameObject.InstantiateGameObjects(Day19Part1Code);
            Assert.AreEqual(Adder.Add(11,34), 45);
            
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Day19Part1TestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            
            Assert.AreEqual(Adder.Add(11,34), 46);
        }
    }
}
