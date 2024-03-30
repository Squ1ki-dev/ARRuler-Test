using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject UI_InfoAnim_PlaneDetection;
    [SerializeField] private GameObject UI_InfoAnimDistanceFail;
    
    [SerializeField] private GameObject UISetDistanceText;
    [SerializeField] private TextMeshProUGUI txtDistance;
    [SerializeField] private GameObject objAddPointBtn;

    public void EnablePlaneDetectionAnimUI(bool enable) => UI_InfoAnim_PlaneDetection.SetActive(enable);

    public void EnableUISetDistanceText(bool enable)
    {
        UISetDistanceText.SetActive(enable);
        objAddPointBtn.SetActive(enable);
        EnableDistanceFailUI(!enable);
    }

    public void SetDistanceText(float distance) => txtDistance.text = "To the target " + distance.ToString("N0") + " cm";

    private void EnableDistanceFailUI(bool enable)
    {
        if (UI_InfoAnim_PlaneDetection.activeSelf == true) return;
        UI_InfoAnimDistanceFail.SetActive(enable);
    }
}