using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codice.Client.BaseCommands;
using GluonGui.Dialog;
using UnityEngine;

namespace aoc2023.day23
{
    internal class PathsGraph
    {
        private readonly Dictionary<Vector2Int, Joint> _joints = new();
        private readonly Dictionary<Vector2Int, Connection> _connections = new();

        internal void RecordJointAt(Vector2Int xy)
        {
            if (_joints.ContainsKey(xy))
            {
                Log();
                throw new Exception($"There is already a joint recorded at {xy}");
            }

            var j = new Joint(xy);
            _joints[xy] = j;
        }

        internal void RecordConnectionAt(Vector2Int xy)
        {
            if (_connections.ContainsKey(xy))
            {
                Log();
                throw new Exception($"There is already a connection recorded at {xy}");
            }
            var c = new Connection();
            c.Xys.Add(xy);
            _connections[xy] = c;
        }

        internal void Connect(Vector2Int xy1, Vector2Int xy2)
        {
            if (xy1 == xy2)
            {
                return;
            }

            _joints.TryGetValue(xy1, out var j1);
            _joints.TryGetValue(xy2, out var j2);
            _connections.TryGetValue(xy1, out var c1);
            _connections.TryGetValue(xy2, out var c2);

            if (c1 != null && c2 != null)
            {
                c1.Xys.UnionWith(c2.Xys);
                c1.Joints.UnionWith(c2.Joints);
                _connections[xy2] = c1;
            }
            else if (j1 != null && c2 != null)
            {
                j1.Connections.Add(c2);
                c2.Joints.Add(j1);
            }
            else if (c1 != null && j2 != null)
            {
                j2.Connections.Add(c1);
                c1.Joints.Add(j2);
            }
            else
            {
                Log();
                throw new Exception(
                    $"Cannot connect {xy1} with {xy2}. Are these defined? j1,j1,c1,c2 -> {j1 != null},{j2 != null},{c1 != null},{c2 != null}");
            }
        }

        internal bool IsVisited(Vector2Int xy)
        {
            _joints.TryGetValue(xy, out var j);
            _connections.TryGetValue(xy, out var c);

            return j is { Visited: true } || c is { Visited: true };
        }

        public void MarkEverythingNotVisited()
        {
            foreach (var j in _joints.Values.Distinct())
            {
                j.Visited = false;
            }
            foreach (var c in _connections.Values.Distinct())
            {
                c.Visited = false;
            }
        }

        public void MarkAdjacentJointsAndConnectionsVisitedAt(Vector2Int xy)
        {
            _joints.TryGetValue(xy, out var j);
            _connections.TryGetValue(xy, out var c);

            if (j != null)
            {
                j.Visited = true;
                foreach (var jc in j.Connections)
                {
                    jc.Visited = true;
                }
            }
            if (c != null)
            {
                c.Visited = true;
                foreach (var cj in c.Joints)
                {
                    cj.Visited = true;
                }
            }
        }

        internal IEnumerable<(int recentLength, int maxLengthSoFar)> SearchForLongestPath(Vector2Int start,
            Vector2Int end)
        {
            if (!_joints.TryGetValue(start, out var j1))
            {
                Log();
                throw new Exception(
                    $"Tried to search for the longest path from a non-joint {start}");
            }
            if (!_joints.TryGetValue(end, out var j2))
            {
                Log();
                throw new Exception(
                    $"Tried to search for the longest path to a non-joint {end}");
            }

            foreach (var result in SearchForLongestPathBetween(j1, j2))
            {
                yield return result;
            }
        }

        private IEnumerable<(int recentLength, int maxLengthSoFar)> SearchForLongestPathBetween(Joint j1, Joint j2)
        {
            if (j1 == j2)
            {
                yield return (0, 0);
            }

            Debug.Log($"search between {j1.Xy} and {j2.Xy} ...");

            var maxLengthSoFar = 0;

            var hasProgressAnyFurther = false;

            j1.Visited = true;
            foreach (var c in j1.Connections.Where(c => !c.Visited))
            {
                c.Visited = true;
                foreach (var jNext in c.Joints.Where(j => !j.Visited))
                {
                    var lengthToAdd = 1 + c.Xys.Count;
                    foreach (var next in SearchForLongestPathBetween(jNext, j2))
                    {
                        hasProgressAnyFurther = true;
                        maxLengthSoFar = Math.Max(maxLengthSoFar, next.maxLengthSoFar + lengthToAdd);
                        yield return (next.recentLength + lengthToAdd, maxLengthSoFar);
                    }
                }
                c.Visited = false;
            }
            j1.Visited = false;

            if (!hasProgressAnyFurther)
            {
                throw new Exception("PROBLEM");
            }
        }

        internal void Log()
        {
            var sb = new StringBuilder();

            sb.Append("<color=yellow>P A T H S  G R A P H</color>\n");
            sb.Append($"j# : {_joints.Values.Distinct().Count()} , c# : {_connections.Values.Distinct().Count()}\n");
            foreach (var j in _joints.Values.Distinct())
            {
                sb.Append(
                    $"<color=yellow>{j.Id}: {j.Xy}</color> > {string.Join("/", j.Connections.Select(c => c.Id))}\n");
            }
            foreach (var c in _connections.Values.Distinct())
            {
                sb.Append(
                    $"\n<color=green>[{c.Id} ({string.Join("/", c.Joints.Select(j => j.Id))})]</color> {string.Join(" | ", c.Xys)}\n");
            }

            Debug.Log(sb.ToString());
        }
    }

    internal class Joint
    {
        internal static int NextId = 1;

        internal int Id { get; }

        internal Vector2Int Xy { get; }

        internal bool Visited { get; set; }

        internal HashSet<Connection> Connections { get; } = new();

        internal Joint(Vector2Int xy)
        {
            Id = NextId++;
            Xy = xy;
        }
    }

    internal class Connection
    {
        internal static int NextId = 1;

        internal int Id { get; }

        internal bool Visited { get; set; }

        internal HashSet<Vector2Int> Xys { get; } = new();
        internal HashSet<Joint> Joints { get; } = new();

        internal Connection()
        {
            Id = NextId++;
        }
    }
}