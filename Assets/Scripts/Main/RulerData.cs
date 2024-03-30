using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TapeRulerSaveData
{
    public List<RulerData> rulerDatas = new List<RulerData>();
    public string dataName;
    public string date;
    public string dataType = "TapeRuler";
    public string customorCode;
    public string planeXYZtype;
}

[System.Serializable]
public class RulerData
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float distance;
}
