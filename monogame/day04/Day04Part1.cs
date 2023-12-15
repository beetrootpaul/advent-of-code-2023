using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace monogame.day04;

//
// https://adventofcode.com/2023/day/4
//
public class Day04Part1 : Game
{
    // ReSharper disable once NotAccessedField.Local
    private GraphicsDeviceManager _graphics;

    public Day04Part1()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        // const string inputFile = "example1_in.txt";
        const string inputFile = "puzzle1_in.txt";
        
        var pileOfScratchcardsWorth = 0;

        foreach (var nextLine in File.ReadAllLines($"day04/{inputFile}"))
        {
            Console.WriteLine($"NEXT: {nextLine}");

            var gameNumbersText = nextLine.Split(':')[1];
            var winningNumbersText = gameNumbersText.Split("|")[0];
            var chosenNumbersText = gameNumbersText.Split("|")[1];

            var winningNumbers = new HashSet<int>();
            foreach (var t in winningNumbersText.Split(" "))
            {
                if (int.TryParse(t, out var n))
                {
                    winningNumbers.Add(n);
                }
            }

            var chosenNumbers = new HashSet<int>();
            foreach (var t in chosenNumbersText.Split(" "))
            {
                if (int.TryParse(t, out var n))
                {
                    chosenNumbers.Add(n);
                }
            }

            var matches = winningNumbers.Intersect(chosenNumbers);

            // floor removes 0.5 which is obtained for matches of size 0
            var cardWorth = (int)Math.Floor(Math.Pow(2, matches.ToArray().Length - 1));
            Console.WriteLine($"This card is worth: {cardWorth}");

            pileOfScratchcardsWorth += cardWorth;
        }


        Console.WriteLine($"\nRESULT: {pileOfScratchcardsWorth}");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }
}