using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace aoc2023.Day22
{
    public class Brick : MonoBehaviour
    {
        public static readonly Brick Ground = new()
        {
            xyzMin = new Vector3Int(-999_999_999, -999_999_999, 0),
            xyzMax = new Vector3Int(999_999_999, 999_999_999, 0)
        };

        private static int _nextId = 1;

        public readonly int Id = _nextId++;
        [SerializeField]
        public Vector3Int xyzMin;
        [SerializeField]
        public Vector3Int xyzMax;
        [SerializeField]
        public bool safeToDisintegrate = false;

        [SerializeField]
        private Material? regularBrickMaterial;
        [SerializeField]
        private Material? markedBrickMaterial;

        internal readonly ICollection<int> SupportingBricks = new HashSet<int>();
        internal readonly ICollection<int> SupportedBricks = new HashSet<int>();
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            transform.position = xyzMin + (Vector3)(Size - Vector3Int.one) / 2f;
            transform.localScale = Size;

            if (safeToDisintegrate && markedBrickMaterial != null)
            {
                _meshRenderer.materials = new[] { markedBrickMaterial };
            }
            if (!safeToDisintegrate && regularBrickMaterial != null)
            {
                _meshRenderer.materials = new[] { regularBrickMaterial };
            }
        }

        private Vector3Int Size => xyzMax - xyzMin + Vector3Int.one;

        public void FallTo(int newZMin)
        {
            xyzMax = new Vector3Int(xyzMax.x, xyzMax.y, newZMin + Size.z - 1);
            xyzMin = new Vector3Int(xyzMin.x, xyzMin.y, newZMin);
        }

        public void ConnectAsSupporting(Brick lowerBrick)
        {
            SupportingBricks.Add(lowerBrick.Id);
            lowerBrick.SupportedBricks.Add(Id);
        }

        public string Serialized()
        {
            var supporting = $"|{string.Join(",", SupportingBricks.OrderBy(id => id))}| -> ";
            var xCoords = $"{xyzMin.x}{(Size.x > 1 ? $"_{xyzMax.x}" : "")}";
            var yCoords = $"{xyzMin.y}{(Size.y > 1 ? $"_{xyzMax.y}" : "")}";
            var zCoords = $"{xyzMin.z}{(Size.z > 1 ? $"_{xyzMax.z}" : "")}";
            var id = $"{Id}:";
            var coords =
                $"[ {xCoords} , {yCoords} , {zCoords} ]";
            var attributes = $"{(safeToDisintegrate ? " (X)" : "")}";
            var supported = $" -> |{string.Join(",", SupportedBricks.OrderBy(id => id))}|";
            return $"{supporting}{id}{coords}{attributes}{supported}";
        }

        public override string ToString() => Serialized();
    }
}