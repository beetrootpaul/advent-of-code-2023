using UnityEngine;

public class AdjustTextWidthToFitCamera : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _mainCamera.pixelWidth);
        }
    }
}