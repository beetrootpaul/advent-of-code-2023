using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                foreach (var xy in c2.Xys)
                {
                    c1.Xys.Add(xy);
                }
                _connections[xy2] = c1;
            }
            else if (j1 != null && c2 != null)
            {
                j1.Connections.Add(c2);
            }
            else if (c1 != null && j2 != null)
            {
                j2.Connections.Add(c1);
            }
            else
            {
                Log();
                throw new Exception(
                    $"Cannot connect {xy1} with {xy2}. Are these defined? j1,j1,c1,c2 -> {j1 != null},{j2 != null},{c1 != null},{c2 != null}");
            }
        }

        internal void Log()
        {
            var sb = new StringBuilder();

            sb.Append("<color=yellow>P A T H S  G R A P H</color>\n");
            sb.Append($"j# : {_joints.Values.Distinct().Count()} , c# : {_connections.Values.Distinct().Count()}\n");
            foreach (var j in _joints.Values.Distinct())
            {
                sb.Append($"<color=yellow>{j.Xy}</color> > {string.Join("/", j.Connections.Select(c => c.Id))}\n");
            }
            foreach (var c in _connections.Values.Distinct())
            {
                sb.Append($"\n<color=green>[{c.Id}]</color> {string.Join(" | ", c.Xys)}\n");
            }

            Debug.Log(sb.ToString());
        }
    }

    internal class Joint
    {
        internal Vector2Int Xy { get; }

        internal HashSet<Connection> Connections { get; } = new();

        internal Joint(Vector2Int xy)
        {
            Xy = xy;
        }
    }

    internal class Connection
    {
        internal static int NextId = 1;

        internal int Id { get; }

        internal HashSet<Vector2Int> Xys { get; } = new();

        internal Connection()
        {
            Id = NextId++;
        }
    }
}