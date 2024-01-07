using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace aoc2023.Day19
{
    //
    // Day 19: Aplenty
    // https://adventofcode.com/2023/day/19
    //
    // part 2
    //
    public partial class Day19Part2Code : MonoBehaviour
    {
        [Header("params")]
        [SerializeField]
        private InputData inputData;

        [Header("GUI")]
        [SerializeField]
        private TMP_Text? guiText;

        private void Start()
        {
            guiText.text = "...";

            print("<color=yellow>=== PARSE ===</color>");
            var workflows = Parse(inputData switch
            {
                InputData.Example => "day19/example2_in.txt",
                InputData.Puzzle => "day19/puzzle2_in.txt",
                _ => "NOT_SET"
            });
            foreach (var workflow in workflows)
            {
                print($":: {workflow.Key} ::");
                foreach (var step in workflow.Value)
                {
                    print(step);
                }
            }

            print("<color=yellow>=== COMPUTE ===</color>");
            var ranges = ComputePartRangesFor(workflows)
                .ToList();

            print("<color=yellow>=== RANGES ===</color>");
            foreach (var range in ranges)
            {
                var sb = new StringBuilder();
                sb.Append("<color=");
                sb.Append(range.Destination switch
                {
                    "A" => "green",
                    "R" => "red",
                    _ => "yellow",
                });
                sb.Append(">");
                sb.Append(range);
                sb.Append($" ({range.Combinations()})");
                sb.Append("</color>");
                print(sb.ToString());
            }

            var acceptedPartRanges = ranges.Where(partRange => partRange.Destination == "A");
            var result = acceptedPartRanges.Select(apr => apr.Combinations()).Sum();

            print($"<color=green>result = {result}</color>");
            guiText.text = $"{result}\nDONE";
        }

        private Dictionary<string, List<WorkflowStep>> Parse(string inputFilePath)
        {
            Dictionary<string, List<WorkflowStep>> workflows = new();

            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}")
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && line[0] != '{')
                .ToList();

            foreach (var line in lines)
            {
                var workflowId = line.Split("{")[0];
                var rules = line.Split("{")[1].Split("}")[0];

                var workflowSteps = new List<WorkflowStep>();
                workflows.Add(workflowId, workflowSteps);

                foreach (var rule in rules.Split(","))
                {
                    var step = new WorkflowStep();

                    var tokens = rule.Split(":");
                    if (tokens.Length == 1)
                    {
                        step.Destination = tokens[0];
                    }
                    else
                    {
                        step.Destination = tokens[1];
                        var separator = tokens[0].Contains('<') ? "<" : ">";
                        step.ConditionCategory = tokens[0].Split(separator)[0] switch
                        {
                            "x" => Category.X,
                            "m" => Category.M,
                            "a" => Category.A,
                            "s" => Category.S,
                            _ => Category.None
                        };
                        step.ConditionLowerThan = separator == "<";
                        step.ConditionValue = int.Parse(tokens[0].Split(separator)[1]);
                    }

                    workflowSteps.Add(step);
                }
            }

            return workflows;
        }

        private IEnumerable<PartsRange> ComputePartRangesFor(IDictionary<string, List<WorkflowStep>> workflows)
        {
            var processedRanges = new List<PartsRange> { };

            var rangesToProcess = new Queue<PartsRange>();
            rangesToProcess.Enqueue(new PartsRange());

            while (rangesToProcess.Count > 0)
            {
                var range = rangesToProcess.Dequeue();

                print($". {range}");

                var workflowId = range.Destination;
                var stepNumber = range.NextStepNumberWithinWorkflow;
                var step = workflows[workflowId][stepNumber];

                foreach (var subrange in range.SplitOn(step))
                {
                    if (subrange.Destination is "A" or "R")
                    {
                        processedRanges.Add(subrange);
                    }
                    else
                    {
                        rangesToProcess.Enqueue(subrange);
                    }
                }
            }

            return processedRanges;
        }

        private enum InputData
        {
            Example,
            Puzzle,
        }
    }
}