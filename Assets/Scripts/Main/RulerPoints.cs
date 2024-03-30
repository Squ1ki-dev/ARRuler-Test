using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RulerPoints : MonoBehaviour
{
    private readonly Dictionary<Vector3, Vector3> rotationMap = new Dictionary<Vector3, Vector3>
    {
        { new Vector3(0f, 1f, 0f), new Vector3(0, 0, -90) },
        { new Vector3(0f, -1f, 0f), new Vector3(0, 0, 90) },
        { new Vector3(-1f, 0f, 0f), new Vector3(90, 0, 0) },
        { new Vector3(1f, 0f, 0f), new Vector3(-90, 0, 0) },
        { new Vector3(0f, 0f, 1f), new Vector3(0, -90, 0) },
        { new Vector3(0f, 0f, -1f), new Vector3(0, 90, 0) }
    };

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back
    }

    public Transform pointA;
    public Transform pointB;
    public MeshRenderer mrPointA;
    public MeshRenderer mrPointB;
    public LineRenderer lineRenderer;
    public SpriteRenderer rdTxtBg;

    [SerializeField] private Transform txtBox;
    [SerializeField] private TextMeshPro textValue;
    [SerializeField] private Camera mainCamera;
    public float distance;

    [SerializeField] private RulerPointUI rulerPointUI;

    [SerializeField] private Transform txtSet;
    private RulerManager rulerManager;

    // SCALE RANGE
    DistanceScaleRange scaleRange_point = new DistanceScaleRange(10f, 300f, 0.01f, 0.02f);
    DistanceScaleRange scaleRange_line = new DistanceScaleRange(10f, 300f, 0.002f, 0.009f);
    DistanceScaleRange scaleRange_textBox = new DistanceScaleRange(10f, 300f, 0.2f, 2f);

    private bool isComplete = false;
    
    private float GetCamDisance()
    {
        var camPos = mainCamera.transform.position;
        var dis = Vector3.Distance(camPos, transform.position);
        return dis * 100f;
    }

    private void Update()
    {
        if (isComplete) return;
        
        var dis = GetCamDisance();
        pointA.localScale = UtilityMethod.AdjustScaleBasedOnDistance(dis, scaleRange_point);
        pointB.localScale = UtilityMethod.AdjustScaleBasedOnDistance(dis, scaleRange_point);
        lineRenderer.startWidth = UtilityMethod.AdjustValueBasedOnDistance(dis, scaleRange_line);
        txtBox.localScale = UtilityMethod.AdjustScaleBasedOnDistance(dis, scaleRange_textBox);

        var position = pointA.position;
        Vector3 distanceVector = pointB.position - position;

        txtBox.position = position + distanceVector * 0.5f;
        
        distance = distanceVector.magnitude * 100f;
        var distText = distance.ToString("N0") + "cm";
        textValue.text = distText;

        var posA = pointA.localPosition;
        var posB = pointB.localPosition;
        var ax = posA.x;
        var az = posA.z;
        var bx = posB.x;
        var bz = posB.z;
        var angle = Mathf.Atan2(bz - az, bx - ax) * Mathf.Rad2Deg;

        switch (rulerManager.planeDirType)
        {
            case EnumDefinition.PlaneDirType.horizontal:
                if (angle >= -90 && angle <= 90)
                    angle += 180;
                break;
            case EnumDefinition.PlaneDirType.vertical:
                if (angle <= 0)
                    angle += 180;
                break;
        }

        txtSet.localRotation = Quaternion.Euler(0, 0, angle);
        
        // set ui position
        if (rulerPointUI != null)
        {
            Vector3 viewPointA = mainCamera.WorldToViewportPoint(pointA.position);
            Vector3 viewPointB = mainCamera.WorldToViewportPoint(pointB.position);
        
            if (viewPointA.z > 0 && viewPointB.z > 0)
            {
                var scrPointA = GetScreenPosition(pointA);
                var scrPointB = GetScreenPosition(pointB);
            }
        }
    }

    public Vector3 GetScreenPosition(Transform target)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);
        
        if (screenPosition.z <= 0)
            screenPosition = new Vector3(-1000, -1000, 0);

        return screenPosition;
    }
    
    public void SetInits(Vector3 position, Pose hitPose, RulerManager rulerManager, Camera cam)
    {
        if (hitPose == null) return;
        
        transform.position = position;
        pointA.position = position;
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, position);
        this.rulerManager = rulerManager;
        mainCamera = cam;
        
        gameObject.transform.rotation = hitPose.rotation;
    }
    
    public void SetObj(Vector3 position)
    {
        pointB.position = position;
        lineRenderer.SetPosition(1, position);
    }

    public void SetMainCam(Camera cam) => mainCamera = cam;

    public void Complete()
    {
        isComplete = true;

        lineRenderer.material.SetColor("_LineColor", Color.white);
        lineRenderer.material.SetFloat("_DashLength", 0f);
        mrPointA.material.SetColor("_Color", Color.white);
        mrPointB.material.SetColor("_Color", Color.white);
        rdTxtBg.color = Color.white;
    }

    public void UnComplete()
    {
        isComplete = false;

        lineRenderer.material.SetColor("_LineColor", Color.yellow);
        lineRenderer.material.SetFloat("_DashLength", 0.05f);
        mrPointA.material.SetColor("_Color", Color.yellow);
        mrPointB.material.SetColor("_Color", Color.yellow);
        rdTxtBg.color = new Color32(255, 190, 0, 255);
    }
    
    public Direction GetClosestDirection(Vector3 directionVector)
    {
        directionVector.Normalize();

        float maxDot = -Mathf.Infinity;
        Direction closestDirection = Direction.Up;

        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            Vector3 dirVector = GetDirectionVector(dir);
            float dot = Vector3.Dot(dirVector, directionVector);

            if (dot > maxDot)
            {
                maxDot = dot;
                closestDirection = dir;
            }
        }

        return closestDirection;
    }

    private Vector3 GetDirectionVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Vector3.up;
            case Direction.Down: return Vector3.down;
            case Direction.Left: return new Vector3(90, 0, 0);
            case Direction.Right: return Vector3.right;
            case Direction.Forward: return Vector3.forward;
            case Direction.Back: return Vector3.back;
            default: return Vector3.zero;
        }
    }
}