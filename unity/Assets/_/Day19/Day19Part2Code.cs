using System;
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
    public class Day19Part2Code : MonoBehaviour
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
                    print(
                        (
                            step.IsConditional
                                ? $"{step.ConditionCategory}{(step.ConditionLowerThan ? '<' : '>')}{step.ConditionValue}:"
                                : ""
                        ) +
                        step.Destination
                    );
                }
            }

            print("<color=yellow>=== COMPUTE ===</color>");
            var ranges = ComputePartRangesFor(workflows)
                .ToList();
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

        private Dictionary<string, List<Step>> Parse(string inputFilePath)
        {
            Dictionary<string, List<Step>> workflows = new();

            var lines = File.ReadAllLines($"{Application.streamingAssetsPath}/{inputFilePath}")
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && line[0] != '{')
                .ToList();

            foreach (var line in lines)
            {
                var workflowId = line.Split("{")[0];
                var rules = line.Split("{")[1].Split("}")[0];

                var workflowSteps = new List<Step>();
                workflows.Add(workflowId, workflowSteps);

                foreach (var rule in rules.Split(","))
                {
                    var step = new Step();

                    var tokens = rule.Split(":");
                    if (tokens.Length == 1)
                    {
                        step.Destination = tokens[0];
                    }
                    else
                    {
                        step.Destination = tokens[1];
                        if (tokens[0].Split("<").Length > 1)
                        {
                            step.ConditionCategory = tokens[0].Split("<")[0].ToCharArray()[0];
                            step.ConditionLowerThan = true;
                            step.ConditionValue = int.Parse(tokens[0].Split("<")[1]);
                        }
                        else if (tokens[0].Split(">").Length > 1)
                        {
                            step.ConditionCategory = tokens[0].Split(">")[0].ToCharArray()[0];
                            step.ConditionLowerThan = false;
                            step.ConditionValue = int.Parse(tokens[0].Split(">")[1]);
                        }
                    }

                    workflowSteps.Add(step);
                }
            }

            return workflows;
        }

        private IEnumerable<PartsRange> ComputePartRangesFor(IDictionary<string, List<Step>> workflows)
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
                        processedRanges.Add(range);
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

        private class Step
        {
            internal char ConditionCategory = '-';
            internal bool ConditionLowerThan = false;
            internal int ConditionValue = 0;
            internal string Destination = "?";

            internal bool IsConditional => ConditionCategory != '-';
        }

        private class PartsRange
        {
            private int _xMin = 1;
            private int _xMax = 4_000;
            private int _mMin = 1;
            private int _mMax = 4_000;
            private int _aMin = 1;
            private int _aMax = 4_000;
            private int _sMin = 1;
            private int _sMax = 4_000;

            internal string Destination = "in";
            internal int NextStepNumberWithinWorkflow = 0;

            internal IEnumerable<PartsRange> SplitOn(Step step)
            {
                var subranges = new List<PartsRange>();

                if (!step.IsConditional)
                {
                    Destination = step.Destination;
                    NextStepNumberWithinWorkflow = 0;
                    subranges.Add(this);
                }
                else
                {
                    var other = Clone();
                    other.Destination = step.Destination;
                    other.NextStepNumberWithinWorkflow = 0;
                    NextStepNumberWithinWorkflow += 1;

                    switch (step.ConditionCategory)
                    {
                        case 'x' when step.ConditionLowerThan:
                            other._xMax = Math.Min(_xMax, step.ConditionValue - 1);
                            _xMin = Math.Max(_xMin, step.ConditionValue);
                            break;
                        case 'x':
                            other._xMin = Math.Max(_xMin, step.ConditionValue + 1);
                            _xMax = Math.Min(_xMax, step.ConditionValue);
                            break;
                        case 'm' when step.ConditionLowerThan:
                            other._mMax = Math.Min(_mMax, step.ConditionValue - 1);
                            _mMin = Math.Max(_mMin, step.ConditionValue);
                            break;
                        case 'm':
                            other._mMin = Math.Max(_mMin, step.ConditionValue + 1);
                            _mMax = Math.Min(_mMax, step.ConditionValue);
                            break;
                        case 'a' when step.ConditionLowerThan:
                            other._aMax = Math.Min(_aMax, step.ConditionValue - 1);
                            _aMin = Math.Max(_aMin, step.ConditionValue);
                            break;
                        case 'a':
                            other._aMin = Math.Max(_aMin, step.ConditionValue + 1);
                            _aMax = Math.Min(_aMax, step.ConditionValue);
                            break;
                        case 's' when step.ConditionLowerThan:
                            other._sMax = Math.Min(_sMax, step.ConditionValue - 1);
                            _sMin = Math.Max(_sMin, step.ConditionValue);
                            break;
                        case 's':
                            other._sMin = Math.Max(_sMin, step.ConditionValue + 1);
                            _sMax = Math.Min(_sMax, step.ConditionValue);
                            break;
                    }

                    if (!other.IsEmpty())
                    {
                        subranges.Add(other);
                    }
                    if (!IsEmpty())
                    {
                        subranges.Add(this);
                    }
                }

                return subranges;
            }

            private bool IsEmpty() => _xMax <= _xMin || _mMax <= _mMin || _aMax <= _aMin || _sMax <= _sMin;

            internal long Combinations() =>
                (long)(_xMax - _xMin + 1) *
                (long)(_mMax - _mMin + 1) *
                (long)(_aMax - _aMin + 1) *
                (long)(_sMax - _sMin + 1);

            private PartsRange Clone()
            {
                return new PartsRange()
                {
                    _xMin = _xMin,
                    _xMax = _xMax,
                    _mMin = _mMin,
                    _mMax = _mMax,
                    _aMin = _aMin,
                    _aMax = _aMax,
                    _sMin = _sMin,
                    _sMax = _sMax,
                    Destination = Destination,
                    NextStepNumberWithinWorkflow = NextStepNumberWithinWorkflow,
                };
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{Destination}|{NextStepNumberWithinWorkflow} ");
                sb.Append($"{_xMin}_x_{_xMax} ");
                sb.Append($"{_mMin}_m_{_mMax} ");
                sb.Append($"{_aMin}_a_{_aMax} ");
                sb.Append($"{_sMin}_s_{_sMax} ");
                return sb.ToString();
            }
        }
    }
}