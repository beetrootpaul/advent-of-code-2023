using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace aoc2023.day19
{
    internal struct Rating
    {
        public readonly int X;
        public readonly int M;
        public readonly int A;
        public readonly int S;

        public Rating(int x, int m, int a, int s)
        {
            X = x;
            M = m;
            A = a;
            S = s;
        }
    }

    internal struct Step
    {
        public readonly Predicate<Rating> Condition;
        public readonly bool IsAccepted;
        public readonly bool IsRejected;
        public readonly string NextWorkflowId;

        public Step(Predicate<Rating> condition, bool isAccepted, bool isRejected, string nextWorkflowId)
        {
            Condition = condition;
            IsAccepted = isAccepted;
            IsRejected = isRejected;
            NextWorkflowId = nextWorkflowId;
        }
    }

    //
    // https://adventofcode.com/2023/day/19
    //
    public class Day19Part1Code : MonoBehaviour
    {
        private enum Input
        {
            Example1,
            Puzzle1
        }

        [SerializeField] private TextMeshProUGUI? resultText;

        [SerializeField] private Input inputFile;

        private async void Start()
        {
            if (resultText == null) throw new Exception($"null {nameof(resultText)}");

            resultText.text = "parsing...";
            print("=== PARSE ===");
            var (workflows, ratings) = await Parse(inputFile switch
            {
                Input.Example1 => "day19/example1_in.txt",
                Input.Puzzle1 => "day19/puzzle1_in.txt",
                _ => "NOT_SET"
            });
            foreach (var workflow in workflows)
            {
                print($":: {workflow.Key} ::");
                foreach (var step in workflow.Value)
                {
                    print($"A:{step.IsAccepted} | R:{step.IsRejected} | nW:{step.NextWorkflowId}");
                }
            }
            foreach (var rating in ratings)
            {
                print($"RATING: x={rating.X},m={rating.M},a={rating.A},s={rating.S}");
            }

            resultText.text = "computing...";
            print("=== COMPUTE ===");
            List<Rating> acceptedRatings = new();
            foreach (var rating in ratings)
            {
                if (!await IsAccepted(rating, workflows, "in")) continue;
                acceptedRatings.Add(rating);
                print($"ACCEPTED: {rating.X}");
            }

            resultText.text = "preparing an answer...";
            print("=== SUM ===");
            var sum = acceptedRatings.Select(rating => rating.X + rating.M + rating.A + rating.S).Sum();
            print($"SUM: {sum}");
            resultText.text = $"{sum}";
        }

        private async Awaitable<(
            Dictionary<string, List<Step>>,
            List<Rating>
            )> Parse(string file)
        {
            Dictionary<string, List<Step>> workflows = new();
            List<Rating> ratings = new();

            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync($"{Application.streamingAssetsPath}/{file}");
            }
            catch
            {
                return (workflows, ratings);
            }

            var isReadingWorkflows = true;
            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    isReadingWorkflows = false;
                    continue;
                }

                if (isReadingWorkflows)
                {
                    var line2 = line.Split("}")[0];
                    var workflowId = line2.Split("{")[0];
                    var rules = line2.Split("{")[1];

                    var workflowSteps = new List<Step>();
                    workflows.Add(workflowId, workflowSteps);

                    foreach (var rule in rules.Split(","))
                    {
                        var isAccepted = false;
                        var isRejected = false;
                        var nextWorkflowId = "";
                        Predicate<Rating> condition = _ => true;

                        var tokens = rule.Split(":");
                        if (tokens.Length == 1)
                        {
                            switch (tokens[0])
                            {
                                case "A":
                                    isAccepted = true;
                                    break;
                                case "R":
                                    isRejected = true;
                                    break;
                                default:
                                    nextWorkflowId = tokens[0];
                                    break;
                            }
                        }
                        else
                        {
                            switch (tokens[1])
                            {
                                case "A":
                                    isAccepted = true;
                                    break;
                                case "R":
                                    isRejected = true;
                                    break;
                                default:
                                    nextWorkflowId = tokens[1];
                                    break;
                            }
                            if (tokens[0].Split("=").Length > 1)
                            {
                                var category = tokens[0].Split("=")[0];
                                var number = int.Parse(tokens[0].Split("=")[1]);
                                condition = rating =>
                                {
                                    return category switch
                                    {
                                        "x" => rating.X == number,
                                        "m" => rating.M == number,
                                        "a" => rating.A == number,
                                        "s" => rating.S == number,
                                        _ => false
                                    };
                                };
                            }
                            else if (tokens[0].Split("<").Length > 1)
                            {
                                var category = tokens[0].Split("<")[0];
                                var number = int.Parse(tokens[0].Split("<")[1]);
                                condition = rating =>
                                {
                                    return category switch
                                    {
                                        "x" => rating.X < number,
                                        "m" => rating.M < number,
                                        "a" => rating.A < number,
                                        "s" => rating.S < number,
                                        _ => false
                                    };
                                };
                            }
                            else if (tokens[0].Split(">").Length > 1)
                            {
                                var category = tokens[0].Split(">")[0];
                                var number = int.Parse(tokens[0].Split(">")[1]);
                                condition = rating =>
                                {
                                    return category switch
                                    {
                                        "x" => rating.X > number,
                                        "m" => rating.M > number,
                                        "a" => rating.A > number,
                                        "s" => rating.S > number,
                                        _ => false
                                    };
                                };
                            }
                        }

                        workflowSteps.Add(new Step(condition, isAccepted, isRejected, nextWorkflowId));
                    }
                }
                else
                {
                    var line2 = line.Split("{")[1].Split("}")[0];
                    var categoryRatings = line2.Split(",");
                    ratings.Add(new Rating(
                        int.Parse(categoryRatings[0].Split("=")[1]),
                        int.Parse(categoryRatings[1].Split("=")[1]),
                        int.Parse(categoryRatings[2].Split("=")[1]),
                        int.Parse(categoryRatings[3].Split("=")[1])
                    ));
                }
            }

            return (workflows, ratings);
        }

        private async Awaitable<bool> IsAccepted(Rating rating, Dictionary<string, List<Step>> workflows,
            string currentWorkflowId)
        {
            if (!workflows.TryGetValue(currentWorkflowId, out var steps)) return false;

            var matchingStep = steps.Find(step => step.Condition(rating));
            if (matchingStep.IsAccepted)
            {
                return true;
            }
            if (matchingStep.IsRejected)
            {
                return false;
            }
            return await IsAccepted(rating, workflows, matchingStep.NextWorkflowId);
        }
    }
}