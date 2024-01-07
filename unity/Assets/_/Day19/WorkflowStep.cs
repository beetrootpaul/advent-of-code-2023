namespace aoc2023.Day19
{
    internal class WorkflowStep
    {
        internal Category ConditionCategory = Category.None;
        internal bool ConditionLowerThan = false;
        internal int ConditionValue = 0;
        internal string Destination = "R";

        internal bool IsConditional => ConditionCategory != Category.None;
    }
}