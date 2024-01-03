using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace aoc2023.Day22
{
    public class Brick : MonoBehaviour
    {
        public static readonly Brick Ground = new()
        {
            XyzMin = new Vector3Int(-999_999_999, -999_999_999, 0),
            XyzMax = new Vector3Int(999_999_999, 999_999_999, 0)
        };

        private static int _nextId = 1;
        public readonly int Id = _nextId++;

        [field: SerializeField] public Vector3Int XyzMin { get; set; }
        [field: SerializeField] public Vector3Int XyzMax { get; set; }
        [field: SerializeField] public bool SafeToDisintegrate { get; set; }

        [field: SerializeField] private Material? RegularBrickMaterial { get; set; }
        [field: SerializeField] private Material? MarkedBrickMaterial { get; set; }

        internal readonly ICollection<int> SupportingBricks = new HashSet<int>();
        internal readonly ICollection<int> SupportedBricks = new HashSet<int>();

        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            transform.position = XyzMin + (Vector3)(Size - Vector3Int.one) / 2f;
            transform.localScale = Size;

            _meshRenderer.materials = SafeToDisintegrate switch
            {
                true when MarkedBrickMaterial != null => new[] { MarkedBrickMaterial },
                false when RegularBrickMaterial != null => new[] { RegularBrickMaterial },
                _ => _meshRenderer.materials
            };
        }

        private Vector3Int Size => XyzMax - XyzMin + Vector3Int.one;

        public void FallTo(int newZMin)
        {
            XyzMax = new Vector3Int(XyzMax.x, XyzMax.y, newZMin + Size.z - 1);
            XyzMin = new Vector3Int(XyzMin.x, XyzMin.y, newZMin);
        }

        public void ConnectAsSupporting(Brick lowerBrick)
        {
            SupportingBricks.Add(lowerBrick.Id);
            lowerBrick.SupportedBricks.Add(Id);
        }

        public override string ToString()
        {
            var supporting = $"|{string.Join(",", SupportingBricks.OrderBy(id => id))}| -> ";
            var xCoords = $"{XyzMin.x}{(Size.x > 1 ? $"_{XyzMax.x}" : "")}";
            var yCoords = $"{XyzMin.y}{(Size.y > 1 ? $"_{XyzMax.y}" : "")}";
            var zCoords = $"{XyzMin.z}{(Size.z > 1 ? $"_{XyzMax.z}" : "")}";
            var id = $"{Id}:";
            var coords =
                $"[ {xCoords} , {yCoords} , {zCoords} ]";
            var attributes = $"{(SafeToDisintegrate ? " (X)" : "")}";
            var supported = $" -> |{string.Join(",", SupportedBricks.OrderBy(id => id))}|";
            return $"{supporting}{id}{coords}{attributes}{supported}";
        }
    }
}