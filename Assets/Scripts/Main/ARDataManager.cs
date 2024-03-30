using UnityEngine;
using System.Collections.Generic;

public class ARDataManager : MonoBehaviour
{
    private TapeRulerSaveData tapeRulerSaveData;

    public void SaveTapeRulerData(List<RulerPoints> rulerPoints, string dataName, string customorCode)
    {
        tapeRulerSaveData = new TapeRulerSaveData();
        tapeRulerSaveData.dataName = dataName;
        tapeRulerSaveData.customorCode = customorCode;

        tapeRulerSaveData.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        for (int i = 0; i < rulerPoints.Count; i++)
        {
            RulerData rulerData = new();
            rulerData.pointA = rulerPoints[i].pointA.position;
            rulerData.pointB = rulerPoints[i].pointB.position;
            rulerData.distance = rulerPoints[i].distance;
            tapeRulerSaveData.rulerDatas.Add(rulerData);
        }

        string json = JsonUtility.ToJson(tapeRulerSaveData);

        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/TapeRulerData"))
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/TapeRulerData");

        string path = Application.persistentDataPath + "/TapeRulerData/" + dataName + ".json";
        System.IO.File.WriteAllText(path, json);
        Debug.Log("Saved: " + path);
    }
}
