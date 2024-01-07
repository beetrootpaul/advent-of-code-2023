using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace aoc2023.Day19
{
    internal class PartsRange
    {
        private readonly Dictionary<Category, (long min, long max)> _values = new()
        {
            { Category.X, (1L, 4000L) },
            { Category.M, (1L, 4000L) },
            { Category.A, (1L, 4000L) },
            { Category.S, (1L, 4000L) },
        };

        internal string Destination = "in";
        internal int NextStepNumberWithinWorkflow = 0;

        internal IEnumerable<PartsRange> SplitOn(WorkflowStep step)
        {
            Debug.Log($"Trying to split {this} on {step}");
            if (!step.IsConditional)
            {
                Destination = step.Destination;
                NextStepNumberWithinWorkflow = 0;

                Debug.Log($"no split");

                yield return this;
            }
            else
            {
                var other = Clone();

                other.Destination = step.Destination;
                other.NextStepNumberWithinWorkflow = 0;

                NextStepNumberWithinWorkflow += 1;

                if (step.ConditionLowerThan)
                {
                    other._values[step.ConditionCategory] = (
                        _values[step.ConditionCategory].min,
                        Math.Min(
                            _values[step.ConditionCategory].max,
                            step.ConditionValue - 1
                        )
                    );
                    _values[step.ConditionCategory] = (
                        Math.Max(
                            _values[step.ConditionCategory].min,
                            step.ConditionValue
                        ),
                        _values[step.ConditionCategory].max
                    );
                }
                else
                {
                    other._values[step.ConditionCategory] = (
                        Math.Max(
                            _values[step.ConditionCategory].min,
                            step.ConditionValue + 1
                        ),
                        _values[step.ConditionCategory].max
                    );
                    _values[step.ConditionCategory] = (
                        _values[step.ConditionCategory].min,
                        Math.Min(
                            _values[step.ConditionCategory].max,
                            step.ConditionValue
                        )
                    );
                }

                other.ThrowIfInvalid();
                ThrowIfInvalid();

                Debug.Log($"split into:");

                if (!other.IsEmpty())
                {
                    Debug.Log(other);
                    yield return other;
                }
                if (!IsEmpty())
                {
                    Debug.Log(this);
                    yield return this;
                }
            }
        }

        private bool IsEmpty() => _values.Values.Any(minMax => minMax.max <= minMax.min);

        internal long Combinations() => _values.Values
            .Select(minMax => minMax.max - minMax.min + 1)
            .Aggregate(1L, (acc, next) => acc * next);

        private void ThrowIfInvalid()
        {
            if (_values.Values.Any(minMax => minMax.max < minMax.min))
            {
                throw new Exception("invalid range");
            }
        }

        private PartsRange Clone()
        {
            var range = new PartsRange
            {
                Destination = Destination,
                NextStepNumberWithinWorkflow = NextStepNumberWithinWorkflow,
            };
            range._values[Category.X] = (_values[Category.X].min, _values[Category.X].max);
            range._values[Category.M] = (_values[Category.M].min, _values[Category.M].max);
            range._values[Category.A] = (_values[Category.A].min, _values[Category.A].max);
            range._values[Category.S] = (_values[Category.S].min, _values[Category.S].max);
            return range;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Destination}|{NextStepNumberWithinWorkflow} ");
            sb.Append($"{_values[Category.X].min}_x_{_values[Category.X].max} ");
            sb.Append($"{_values[Category.M].min}_m_{_values[Category.M].max} ");
            sb.Append($"{_values[Category.A].min}_a_{_values[Category.A].max} ");
            sb.Append($"{_values[Category.S].min}_s_{_values[Category.S].max} ");
            return sb.ToString();
        }
    }
}