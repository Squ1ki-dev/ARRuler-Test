using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class PlaneObj : MonoBehaviour
{
    public ARPlane arPlane;
    public MeshRenderer mrPlane;
    public TextMeshPro txtMesh;
    public GameObject txtObj;
    public Color color;

    GameObject mainCam;
    private void Start() => mainCam = FindObjectOfType<Camera>().gameObject;

    private void Update()
    {
        UpdateLabel();
        UpdatePlaneColor();
    }

    private void SetMainCam(GameObject cam) => mainCam = cam;
    
    private void UpdateLabel()
    {
        txtMesh.text = arPlane.classification.ToString();
        txtObj.transform.position = arPlane.center;
        txtObj.transform.LookAt(mainCam.transform);
        txtObj.transform.Rotate(new Vector3(0, 180, 0));
    }

    private void UpdatePlaneColor()
    {
        switch (arPlane.classification)
        {
            case PlaneClassification.None:
                color = Color.gray;
                break;
            case PlaneClassification.Wall:
                color = Color.blue;
                break;
            case PlaneClassification.Floor:
                color = Color.green;
                break;
            case PlaneClassification.Ceiling:
                color = Color.yellow;
                break;
            case PlaneClassification.Table:
                color = Color.red;
                break;
            case PlaneClassification.Seat:
                color = Color.magenta;
                break;
            case PlaneClassification.Door:
                color = Color.cyan;
                break;
            case PlaneClassification.Window:
                color = Color.white;
                break;
            // Add more cases as needed
            default:
                color = Color.black;
                break;
        }

        color.a = 0.33f; // Make the color transparent
        mrPlane.material.color = color; // Apply the color to the MeshRenderer's material
    }
}