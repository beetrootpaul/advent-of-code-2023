using Cysharp.Threading.Tasks;
using UnityEngine;

namespace aoc2023.Day11
{
    public class Tile : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void IndicateEmpty()
        {
            _spriteRenderer.color = new Color(.2f, .6f, 0.4f);
        }

        public async UniTask Appear(float durationMillis)
        {
            var initialColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
            var targetColor = _spriteRenderer.color;
            _spriteRenderer.color = initialColor;
            for (var t = 0f; t < durationMillis; t += Time.deltaTime * 1000f)
            {
                _spriteRenderer.color = Color.Lerp(initialColor, targetColor, t / durationMillis);
                await UniTask.Yield();
            }
        }

        public async UniTask Disappear(float durationMillis)
        {
            var initialColor = _spriteRenderer.color;
            var targetColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
            for (var t = 0f; t < durationMillis; t += Time.deltaTime * 1000f)
            {
                _spriteRenderer.color = Color.Lerp(initialColor, targetColor, t / durationMillis);
                await UniTask.Yield();
            }

            Destroy(gameObject);
        }
    }
}