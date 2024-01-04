using NUnit.Framework;

namespace aoc2023.Day07
{
    public class Day07Part2Tests
    {
        [Test]
        public void FiveOfAKind_WithoutJokers(
            [Values(
                "AAAAA",
                "KKKKK",
                "QQQQQ",
                "TTTTT",
                "99999",
                "88888",
                "77777",
                "66666",
                "55555",
                "44444",
                "33333",
                "22222"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.FiveOfAKind,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void FiveOfAKind_WithJokers(
            [Values(
                "AAJAA",
                "QQQJJ",
                "JJJ66",
                "JJ3JJ",
                "JJJJJ"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.FiveOfAKind,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void FourOfAKind_WithoutJokers(
            [Values(
                "AAAA2",
                "KKK3K",
                "QQ4QQ",
                "T5TTT",
                "69999"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.FourOfAKind,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void FourOfAKind_WithJokers(
            [Values(
                "JAAA2",
                "KJK3K",
                "QQ4QJ",
                "T5TJJ",
                "69JJJ"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.FourOfAKind,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void FullHouse_WithoutJokers(
            [Values(
                "AAA33",
                "KK222"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.FullHouse,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void FullHouse_WithJokers(
            [Values(
                "JAA33",
                "KK2J2"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.FullHouse,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void ThreeOfAKind_WithoutJokers(
            [Values(
                "AAA37",
                "9KKKQ"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.ThreeOfAKind,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void ThreeOfAKind_WithJokers(
            [Values(
                "AAJ37",
                "9JKJQ"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.ThreeOfAKind,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void TwoPair_WithoutJokers(
            [Values(
                "AA772",
                "33K55"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.TwoPair,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void OnePair_WithoutJokers(
            [Values(
                "AA654",
                "Q22TK"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.OnePair,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void OnePair_WithJokers(
            [Values(
                "JA654",
                "Q2JTK"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.OnePair,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }

        [Test]
        public void HighCard_WithoutJokers(
            [Values(
                "236KQ",
                "A3T78"
            )]
            string cards
        )
        {
            Assert.AreEqual(
                Day07Part2Code.HandType.HighCard,
                new Day07Part2Code.Hand(cards.ToCharArray()).Type
            );
        }
    }
}