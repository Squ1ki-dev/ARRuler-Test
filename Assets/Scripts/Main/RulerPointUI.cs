using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RulerPointUI : MonoBehaviour
{
    [SerializeField] private RectTransform pointA;
    [SerializeField] private RectTransform pointB;

    [SerializeField] private Image imgPointA;
    [SerializeField] private Image imgPointB;
    [SerializeField] private Image imgLine;
    [SerializeField] private Image imgBg;

    [SerializeField] private RectTransform textBg;
    [SerializeField] private TextMeshProUGUI textValue;

    [SerializeField] private Sprite spriteCompleteLine;
    [SerializeField] private Sprite spriteProgressLine;

    [SerializeField] private Color colorCompleteLine;
    [SerializeField] private Color colorProgressLine;

    public void SetPosition(Vector3 scrPointA, Vector3 scrPointB, Camera mainCamera = null)
    {
        pointA.anchoredPosition = scrPointA;
        pointB.anchoredPosition = scrPointB;

        Vector3 middlePoint = (scrPointA + scrPointB) / 2f;
        textBg.anchoredPosition = middlePoint;

        Vector2 direction = scrPointB - scrPointA;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (mainCamera != null)
        {
            Vector2 cameraDirection = (textBg.position - Camera.main.transform.position).normalized;
            float dotProduct = Vector2.Dot(direction.normalized, cameraDirection);

            if (dotProduct <= 0)
                angle += 180;
        }

        textBg.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void SetTextValue(string text) => textValue.text = text + "cm";

    public void SetCompleteLine()
    {
        imgLine.sprite = spriteCompleteLine;
        imgLine.color = colorCompleteLine;
        imgPointA.color = colorCompleteLine;
        imgPointB.color = colorCompleteLine;
        imgBg.color = Color.white;
        textValue.color = Color.black;
        imgLine.rectTransform.sizeDelta = new Vector2(imgLine.rectTransform.sizeDelta.x, 10f);
    }

    public void SetProgressLine()
    {
        imgLine.sprite = spriteProgressLine;
        imgLine.color = colorProgressLine;
        imgPointA.color = colorProgressLine;
        imgPointB.color = colorProgressLine;
        imgBg.color = colorProgressLine;
        textValue.color = Color.white;
        imgLine.rectTransform.sizeDelta = new Vector2(imgLine.rectTransform.sizeDelta.x, 20f);
    }
}