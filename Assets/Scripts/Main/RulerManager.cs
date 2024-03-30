using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Serialization;

public class RulerManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager rayManager;
    [SerializeField] private Camera camera;

    [Header("UI Components")]
    //[SerializeField] private TextMeshProUGUI txtUserDisance;
    [SerializeField] private Image closePointUI;
    [SerializeField] private Button btnAddRulerPoint;

    [Header("Mesh Renderers")]
    [SerializeField] private MeshRenderer mrPivitCenter;
    [SerializeField] private MeshRenderer mrPivitEdge;

    [Header("Transforms")]
    [SerializeField] private Transform trPivitObj;

    [SerializeField] private Transform trPivotCenter;
    [SerializeField] private Transform trRulerPool;
    [SerializeField] private Transform trRulerPointUIPool;
    
    [Header("Prefab References")]
    [SerializeField] private RulerPoints prefabRulerPoint;
    [SerializeField] private RulerPointUI prefabRulerPointUI;

    [Header("Data Managers")]
    [SerializeField] private ARDataManager arDataManager;
    [SerializeField] private PolygonMeshCreator polygonMeshCreator;

    // Ruler Points
    private RulerPoints curRulerPoint;
    private List<RulerPoints> rulerPointPoolList = new List<RulerPoints>();

    // State Variables
    private RaycastHit hit;
    [SerializeField] private Transform curHitTr;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector3 rulerPosSave;
    private float lastDistance = -1;
    private Vector2 scrCenterVec;

    private bool isSurfaceDetected = false;
    private bool isFirstRulerPoint = false;
    private bool isFirstDetectionPlane = false;
    private bool distTxt = false;

    private Pose hitPose;
    private Vector3 hitUpSide;
    public EnumDefinition.PlaneDirType planeDirType;

    [Header("Ruler UI Controller")]
    [SerializeField] private UIController uiController;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Material curveLineMaterial;

    [SerializeField] private ResultText resultTextPrefab;
    [SerializeField] private MeshDimensionDrawer meshDimensionDrawerPrefab;
    private DistanceScaleRange pivotScaleRange = new DistanceScaleRange(10f, 300f, 1.0f, 0.3f);
        
    private bool IsFirstDeteactionPlane()
    {
        return hits.Count > 0;
    }
        
    private bool IsRotationHorizontal(Transform transform)
    {
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        
        bool isForwardHorizontal = Mathf.Abs(Vector3.Dot(forward, Vector3.up)) < 0.1f;

        bool isUpVertical = Vector3.Dot(up, Vector3.up) > 0.9f;

        return isForwardHorizontal && isUpVertical;
    }
        
    private float ConvertToCentimeters(float meters)
    {
        return meters * 100f;
    }
        
    private EnumDefinition.PlaneDirType GetDetectPlaneType(Transform transform)
    {
        return IsRotationHorizontal(transform) ? EnumDefinition.PlaneDirType.horizontal : EnumDefinition.PlaneDirType.vertical;
    }
        
    private void Start() => Init();
    private void Init()
    {
        scrCenterVec = new Vector2(Screen.width / 2, Screen.height / 2);
        SetBtnEvent();
        uiController.EnablePlaneDetectionAnimUI(true);
    }
    private void Update()
    {
        RaycastFromCamera();
        HandleSurfaceDetection();
        EnablePlaneDetactionAnim();
        EnableDisatanceText();
    }
        
    private void SetBtnEvent() => btnAddRulerPoint.onClick.AddListener(MakeRulerPoint);
        
    private void RaycastFromCamera()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit))
        {
            isFirstRulerPoint = hit.transform.CompareTag("firstRulerPoint");
            curHitTr = hit.transform;
            HandleFirstRulerPoint();
        }
        else
        {
            curHitTr = null;
            ResetPivotCenterPosition();
        }
    }

    private void HandleFirstRulerPoint()
    {
        if (!isFirstRulerPoint)
        {
            ResetPivotCenterPosition();
            return;
        }

        // World position to 2D position
        Vector3 screenPos = camera.WorldToScreenPoint(rulerPointPoolList[0].pointA.position);
        closePointUI.rectTransform.anchoredPosition = screenPos;

        // UI to 3D position
        Vector3 screenPoint = closePointUI.rectTransform.position;
        screenPoint.z = camera.nearClipPlane + 0.13f; // Adjust depth as needed
        Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);

        trPivotCenter.transform.position = Vector3.Lerp(trPivotCenter.transform.position, worldPoint, Time.deltaTime * 10f);
    }

    private void ResetPivotCenterPosition()
    {
        // Smoothly interpolate back to the starting position
        trPivotCenter.transform.localPosition = Vector3.Lerp(trPivotCenter.transform.localPosition, Vector3.zero, Time.deltaTime * 15f);
        isFirstRulerPoint = false;
    }
        
    private void EnableDisatanceText() => uiController.EnableUISetDistanceText(isSurfaceDetected);
        
    private void HandleSurfaceDetection()
    {
        isSurfaceDetected = rayManager.Raycast(scrCenterVec, hits, TrackableType.PlaneWithinPolygon);
        UpdateAlpha(isSurfaceDetected);

        if (!isSurfaceDetected) return;

        UpdateDistance();
        rulerPosSave = hits[0].pose.position;
        hitPose = hits[0].pose;
    }

    private void EnablePlaneDetactionAnim()
    {
        if (isFirstDetectionPlane) return;

        if (IsFirstDeteactionPlane())
        {
            isFirstDetectionPlane = true;
            uiController.EnablePlaneDetectionAnimUI(false);
        }
    }
        
    private void UpdateDistance()
    {
        float closestDistance = float.MaxValue;
            
        foreach (var hit in hits)
        {
            float currentDistance = Vector3.Distance(hit.pose.position, trPivitObj.position);
            if (currentDistance <= closestDistance)
            {
                closestDistance = currentDistance;
                trPivitObj.rotation = Quaternion.Lerp(trPivitObj.rotation, hit.pose.rotation, 0.05f);
            }
        }

        if (closestDistance != lastDistance)
        {
            lastDistance = closestDistance;
            closestDistance = ConvertToCentimeters(closestDistance);
            uiController.SetDistanceText(closestDistance);

            if (curRulerPoint != null)
            {
                if (isFirstRulerPoint)
                    rulerPosSave = rulerPointPoolList[0].pointA.position;
                curRulerPoint.SetObj(rulerPosSave);
            }

            // Size adjustment
            trPivitObj.localScale = UtilityMethod.AdjustScaleBasedOnDistance(closestDistance, pivotScaleRange);
        }
    }
        
    private void AdjustScaleBasedOnDistance(float limitDist)
    {
        limitDist = Mathf.Clamp(limitDist, 10f, 300f);

        // Calculate scale based on distance
        float scale = Mathf.Lerp(1.0f, 0.3f, (limitDist - 10f) / (300f - 10f));

        // apply scale
        trPivitObj.localScale = new Vector3(scale, scale, scale);
    }

    private void AdjustScaleBasedOnDistance(float distance, Transform transform, DistanceScaleRange range)
    {
        distance = Mathf.Clamp(distance, range.minDistance, range.maxDistance);
        // Calculate scale based on distance
        float scale = Mathf.Lerp(range.minScale, range.maxScale,
            (distance - range.minDistance) / (range.maxDistance - range.minDistance));

        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void UpdateAlpha(bool isSurfaceDetected)
    {
        var mat = mrPivitCenter.material;

        float targetAlpha = isSurfaceDetected ? 1.0f : 0.0f; // opaque when found, transparent when not found
        float currentAlpha = mat.GetFloat("_Alpha");
        float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10f); // smooth transition

        mrPivitCenter.material.SetFloat("_Alpha", newAlpha);
        mrPivitEdge.material.SetFloat("_Alpha", newAlpha);
    }

    private void MakeRulerPoint()
    {
        if (isSurfaceDetected)
        {
            var curHitObjTr = curHitTr;

            if (isFirstRulerPoint)
            {
                if (rulerPointPoolList.Count < 3)
                {
                    Debug.Log("At least two line segments are required.");
                    return;
                }

                curRulerPoint.Complete();
                curRulerPoint = null;

                polygonMeshCreator.InitMeshCreater(rulerPointPoolList, out GameObject meshObject);

                // Output mesh info
                var vectors = PolygonPlaneCalculator.GetVectorsByNDRO_RulerPoints(rulerPointPoolList);
                var info = PolygonPlaneCalculator.CalculateDimensions(vectors);
                ResultInfo resultInfo = new ResultInfo(info.width, info.height, info.plane);

                // Save JSON
                rulerPointPoolList[0].pointA.tag = "Untagged";
                arDataManager.SaveTapeRulerData(rulerPointPoolList, "test_customer", "test_customer_code");

                Debug.Log($"width: {info.width}, height: {info.height}, plane: {info.plane}");

                // xz : y = 0
                var firstPoint = rulerPointPoolList.Select(s => s.pointA.position).ToList();
                var secondPoint = rulerPointPoolList.Select(s => s.pointB.position).ToList();
                var points = firstPoint.Concat(secondPoint).ToList();
                Debug.Log($"points count: {points.Count}");
                
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = new Vector3(points[i].x, 0, points[i].z);
                }

                // info 
                MeshDimensionDrawer drawer = Instantiate(meshDimensionDrawerPrefab,
                        meshObject.transform);
                
                drawer.planeType = info.plane;
                drawer.rulerManager = this;
                drawer.DrawDimensions(meshObject, resultTextPrefab, resultInfo);

                rulerPointPoolList.Clear();
                planeDirType = EnumDefinition.PlaneDirType.none;

                return;
            }

            var dri = curHitObjTr.GetComponent<ARPlane>().classification;
            Debug.Log("dri: " + dri);

            if (curRulerPoint != null && planeDirType != GetDetectPlaneType(curHitObjTr)) return;

            // only when 2 lines are connected
            if (curRulerPoint != null)
                curRulerPoint.Complete();

            RulerPoints tObj = Instantiate(prefabRulerPoint, trRulerPool, curHitTr);
            tObj.transform.position = Vector3.zero;
            tObj.transform.localScale = Vector3.one;

            if (curRulerPoint != null)
                rulerPosSave = curRulerPoint.pointB.position;
            else
            {
                tObj.pointA.tag = "firstRulerPoint";
                planeDirType = GetDetectPlaneType(curHitObjTr);

                Debug.Log("planeDirType: " + planeDirType);
            }

            tObj.SetInits(rulerPosSave, hitPose, this, camera);
            tObj.SetMainCam(camera);
            rulerPointPoolList.Add(tObj);
            curRulerPoint = tObj;
        }
    }
}