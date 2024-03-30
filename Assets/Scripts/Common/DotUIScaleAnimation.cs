using UnityEngine;
using UnityEngine.UI;

public class DotUIScaleAnimation : MonoBehaviour
{
    [SerializeField] private Image dot;
    [SerializeField] private Image pivot;

    private float maxScale = 1.0f;
    private float minScale = 0f;
    private float maxDistance = 200f;

    private void Update()
    {
        float distance = Vector2.Distance(dot.rectTransform.anchoredPosition, pivot.rectTransform.anchoredPosition);

        float scale = Mathf.Clamp(1 - (distance / maxDistance), minScale, maxScale);

        dot.rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}
