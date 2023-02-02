using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
///  Json으로 저장 및 로드 구현을 위한 스크립트
/// </summary>
/// 
public class JsonSerialize
{
    static List<string> content = new List<string>();
    static List<string> sandBoxContent = new List<string>();
    static DominoCase dominoCase;

    public static DominoMaker dominoMaker;

    public static bool loadingNow = false;

    public void Initialize(DominoCase _dominoCase)
    {
        dominoCase = _dominoCase.GetComponent<DominoCase>();
    }
    public static void SaveDataToJson(Transform _dominosParents)
    {
        string fileName = Path.Combine(Application.persistentDataPath, "dominoData.json");

        Debug.Log(Path.Combine(Application.persistentDataPath, "dominoData.json")); // 위치 추적용 디버그

        if (File.Exists(fileName))
        {
            File.Delete(fileName); // 확인해보고 이미 있으면 지운다
            content.Clear();
        }

        //List<DominoDataForSave> dominoChildrens = new List<DominoDataForSave>();

        // 도미노들이 들어있는 오브젝트안을 조사해서 하나하나 json에 던져 넣는다.
        for (int i = 0; i < _dominosParents.childCount; ++i)
        {
            var child = _dominosParents.GetChild(i).transform;
            var type = child.GetComponent<Domino>().GetDominoType();

            string position = child.transform.position.x + "," + child.transform.position.y + "," + child.transform.position.z;
            string rotation = child.transform.rotation.eulerAngles.x + "," + child.transform.rotation.eulerAngles.y + "," + child.transform.rotation.eulerAngles.z;

            DominoDataForSave savedata = new DominoDataForSave(type, i + 1, position, rotation);

            string json = JsonUtility.ToJson(savedata, true);
            content.Add(json);
        }
        File.AppendAllLines(fileName, content);
    }

    public static void SaveOBJForSandBox()
    {
        string fileName = Path.Combine(Application.persistentDataPath, "SandBoxData.json");

        Debug.Log(Path.Combine(Application.persistentDataPath, "SandBoxData.json")); // 위치 추적용 디버그

        if (File.Exists(fileName))
        {
            File.Delete(fileName); // 확인해보고 이미 있으면 지운다
            sandBoxContent.Clear();
        }


        SaveAbleObject[] objs = GameObject.FindObjectsOfType<SaveAbleObject>(); // 세이버블오브젝트를 죄다 차즌ㄴ다.

        foreach (SaveAbleObject obs in objs)
        {
            if (obs.CompareTag("Replica") == false) continue;

            int objType = (int)obs.SBOT;
            string pos = obs.transform.position.x + "," + obs.transform.position.y + "," + obs.transform.position.z;
            string rota = obs.transform.rotation.eulerAngles.x + "," + obs.transform.eulerAngles.y + "," + obs.transform.eulerAngles.z;
            string scale = obs.transform.localScale.x + "," + obs.transform.localScale.y + "," + obs.transform.localScale.z;

            SandboxDataForSave savedata = new SandboxDataForSave(objType, pos, rota, scale);

            string json = JsonUtility.ToJson(savedata, true);
            sandBoxContent.Add(json);
            File.AppendAllLines(fileName, sandBoxContent);
        }

    }

    public static void ResetSave()
    {
        string fileName = Path.Combine(Application.persistentDataPath, "dominoData.json");

        File.Delete(fileName);
        content.Clear();
    }

    public static IEnumerator LoadDataFromJson(Action _soundAction)
    {
        loadingNow = true;
        UIMG.Instance.UnActiveUndoButton();
        WaitForSeconds ws = new WaitForSeconds(0.1f);

        yield return ws;

        string fileName = Path.Combine(Application.persistentDataPath, "dominoData.json");

        Debug.Log("저장파일 로컬주소" + fileName);

        var cds = GameObject.Find("CreatedDominos").transform;

        if (File.Exists(fileName))
        {
            string jsonFromFile = File.ReadAllText(fileName);

            foreach (var cont in content)
            {
                DominoDataForSave data = JsonUtility.FromJson<DominoDataForSave>(cont);

                string[] posArr = data.position.Split(',');
                string[] rotaArr = data.rotation.Split(',');

                Vector3 pos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                Vector3 rota = new Vector3(float.Parse(rotaArr[0]), float.Parse(rotaArr[1]), float.Parse(rotaArr[2]));

                switch (data.dominoType)
                {
                    case eDominoType.Default:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.Default], pos, Quaternion.Euler(rota), cds);
                        dominoCase.PopAndDestroyInCase(eDominoType.Default);
                        break;
                    case eDominoType.Bomb:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.Bomb], pos, Quaternion.Euler(rota), cds);
                        dominoCase.PopAndDestroyInCase(eDominoType.Bomb);
                        break;
                    case eDominoType.JetPack:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.JetPack], pos, Quaternion.Euler(rota), cds);
                        dominoCase.PopAndDestroyInCase(eDominoType.JetPack);
                        break;
                    case eDominoType.LongLong:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.LongLong], pos, Quaternion.Euler(rota), cds);
                        dominoCase.PopAndDestroyInCase(eDominoType.LongLong);
                        break;
                    case eDominoType.InPortal:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.InPortal], pos, Quaternion.Euler(rota), cds);
                        dominoCase.PopAndDestroyInCase(eDominoType.InPortal);
                        dominoMaker.isInportal = false;
                        break;
                    case eDominoType.OutPortal:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.OutPortal], pos, Quaternion.Euler(rota), cds);
                        dominoMaker.isInportal = true;
                        break;
                }
                yield return ws;
            }
        }
        loadingNow = false;
        _soundAction();
        UIMG.Instance.ActiveUndoButton();
    }

    public static IEnumerator LoadDataFromJsonForSandBox(Action _soundAction)
    {
        loadingNow = true;
        UIMG.Instance.UnActiveUndoButton();
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();

        yield return wffu;

        string fileName = Path.Combine(Application.persistentDataPath, "dominoData.json");

        Debug.Log("저장파일 로컬주소" + fileName);

        var cds = GameObject.Find("CreatedDominos").transform;

        if (File.Exists(fileName))
        {
            string jsonFromFile = File.ReadAllText(fileName);

            foreach (var cont in content)
            {
                DominoDataForSave data = JsonUtility.FromJson<DominoDataForSave>(cont);

                string[] posArr = data.position.Split(',');
                string[] rotaArr = data.rotation.Split(',');

                Vector3 pos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                Vector3 rota = new Vector3(float.Parse(rotaArr[0]), float.Parse(rotaArr[1]), float.Parse(rotaArr[2]));

                switch (data.dominoType)
                {
                    case eDominoType.Default:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.Default], pos, Quaternion.Euler(rota), cds);
                        break;
                    case eDominoType.Bomb:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.Bomb], pos, Quaternion.Euler(rota), cds);
                        break;
                    case eDominoType.JetPack:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.JetPack], pos, Quaternion.Euler(rota), cds);
                        break;
                    case eDominoType.LongLong:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.LongLong], pos, Quaternion.Euler(rota), cds);
                        break;
                    case eDominoType.InPortal:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.InPortal], pos, Quaternion.Euler(rota), cds);
                        dominoMaker.isInportal = false;
                        break;
                    case eDominoType.OutPortal:
                        GameObject.Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.OutPortal], pos, Quaternion.Euler(rota), cds);
                        dominoMaker.isInportal = true;
                        break;
                }
                yield return wffu;
            }
        }
        loadingNow = false;
        _soundAction();
        UIMG.Instance.ActiveUndoButton();
    }

    public static IEnumerator LoadOBJForSandBox()
    {
        GameObject[] makersObj = GameObject.Find("DominoMaker").GetComponent<SandBoxMaker>().sandBoxOBJs;

        WaitForFixedUpdate wfu = new WaitForFixedUpdate();

        yield return wfu;

        string fileName = Path.Combine(Application.persistentDataPath, "SandBoxData.json");

        Debug.Log("저장파일 로컬주소" + fileName);

        if (File.Exists(fileName))
        {
            string jsonFromFile = File.ReadAllText(fileName);

            foreach (var cont in sandBoxContent)
            {
                SandboxDataForSave data = JsonUtility.FromJson<SandboxDataForSave>(cont);

                string[] posArr = data.position.Split(',');
                string[] rotaArr = data.rotation.Split(',');
                string[] scaleArr = data.scale.Split(',');

                Vector3 pos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                Vector3 rota = new Vector3(float.Parse(rotaArr[0]), float.Parse(rotaArr[1]), float.Parse(rotaArr[2]));
                Vector3 scale = new Vector3(float.Parse(scaleArr[0]), float.Parse(scaleArr[1]), float.Parse(scaleArr[2]));

                switch (data.objectType)
                {
                    case eSandBoxObjectType.Block01:
                        var Block01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Block01], pos, Quaternion.Euler(rota));
                        Block01.transform.localScale = scale;
                        Block01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Block02:
                        var Block02 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Block02], pos, Quaternion.Euler(rota));
                        Block02.transform.localScale = scale;
                        Block02.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Block03:
                        var Block03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Block03], pos, Quaternion.Euler(rota));
                        Block03.transform.localScale = scale;
                        Block03.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Bridge01:
                        var Bridge01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Bridge01], pos, Quaternion.Euler(rota));
                        Bridge01.transform.localScale = scale;
                        Bridge01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock02:
                        var CastleBlock02 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock02], pos, Quaternion.Euler(rota));
                        CastleBlock02.transform.localScale = scale;
                        CastleBlock02.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock03:
                        var CastleBlock03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock03], pos, Quaternion.Euler(rota));
                        CastleBlock03.transform.localScale = scale;
                        CastleBlock03.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock05:
                        var CastleBlock05 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock05], pos, Quaternion.Euler(rota));
                        CastleBlock05.transform.localScale = scale;
                        CastleBlock05.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock06:
                        var CastleBlock06 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock06], pos, Quaternion.Euler(rota));
                        CastleBlock06.transform.localScale = scale;
                        CastleBlock06.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock07:
                        var CastleBlock07 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock07], pos, Quaternion.Euler(rota));
                        CastleBlock07.transform.localScale = scale;
                        CastleBlock07.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock09:
                        var CastleBlock09 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock09], pos, Quaternion.Euler(rota));
                        CastleBlock09.transform.localScale = scale;
                        CastleBlock09.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock10:
                        var CastleBlock10 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock10], pos, Quaternion.Euler(rota));
                        CastleBlock10.transform.localScale = scale;
                        CastleBlock10.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock11:
                        var CastleBlock11 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock11], pos, Quaternion.Euler(rota));
                        CastleBlock11.transform.localScale = scale;
                        CastleBlock11.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock12:
                        var CastleBlock12 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock12], pos, Quaternion.Euler(rota));
                        CastleBlock12.transform.localScale = scale;
                        CastleBlock12.tag = "Replica";
                        break;
                    case eSandBoxObjectType.CastleBlock13:
                        var CastleBlock13 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.CastleBlock13], pos, Quaternion.Euler(rota));
                        CastleBlock13.transform.localScale = scale;
                        CastleBlock13.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RecBlock01:
                        var RecBlock01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RecBlock01], pos, Quaternion.Euler(rota));
                        RecBlock01.transform.localScale = scale;
                        RecBlock01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RecBlock02:
                        var RecBlock02 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RecBlock02], pos, Quaternion.Euler(rota));
                        RecBlock02.transform.localScale = scale;
                        RecBlock02.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RecBlock03:
                        var RecBlock03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RecBlock03], pos, Quaternion.Euler(rota));
                        RecBlock03.transform.localScale = scale;
                        RecBlock03.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RecBlock04:
                        var RecBlock04 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RecBlock04], pos, Quaternion.Euler(rota));
                        RecBlock04.transform.localScale = scale;
                        RecBlock04.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Road1:
                        var Road1 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Road1], pos, Quaternion.Euler(rota));
                        Road1.transform.localScale = scale;
                        Road1.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Road2:
                        var Road2 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Road2], pos, Quaternion.Euler(rota));
                        Road2.transform.localScale = scale;
                        Road2.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Road3:
                        var Road3 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Road3], pos, Quaternion.Euler(rota));
                        Road3.transform.localScale = scale;
                        Road3.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock01:
                        var RoadBlock01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock01], pos, Quaternion.Euler(rota));
                        RoadBlock01.transform.localScale = scale;
                        RoadBlock01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock02:
                        var RoadBlock02 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock02], pos, Quaternion.Euler(rota));
                        RoadBlock02.transform.localScale = scale;
                        RoadBlock02.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock03:
                        var RoadBlock03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock03], pos, Quaternion.Euler(rota));
                        RoadBlock03.transform.localScale = scale;
                        RoadBlock03.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock04:
                        var RoadBlock04 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock04], pos, Quaternion.Euler(rota));
                        RoadBlock04.transform.localScale = scale;
                        RoadBlock04.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock05:
                        var RoadBlock05 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock05], pos, Quaternion.Euler(rota));
                        RoadBlock05.transform.localScale = scale;
                        RoadBlock05.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock06:
                        var RoadBlock06 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock06], pos, Quaternion.Euler(rota));
                        RoadBlock06.transform.localScale = scale;
                        RoadBlock06.tag = "Replica";
                        break;
                    case eSandBoxObjectType.RoadBlock07:
                        var RoadBlock07 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.RoadBlock07], pos, Quaternion.Euler(rota));
                        RoadBlock07.transform.localScale = scale;
                        RoadBlock07.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Stair01:
                        var Stair01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Stair01], pos, Quaternion.Euler(rota));
                        Stair01.transform.localScale = scale;
                        Stair01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign01:
                        var TrafficSign01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign01], pos, Quaternion.Euler(rota));
                        TrafficSign01.transform.localScale = scale;
                        TrafficSign01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign02:
                        var b1 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign02], pos, Quaternion.Euler(rota));
                        b1.transform.localScale = scale;
                        b1.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign03:
                        var TrafficSign03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign03], pos, Quaternion.Euler(rota));
                        TrafficSign03.transform.localScale = scale;
                        TrafficSign03.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign04:
                        var TrafficSign04 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign04], pos, Quaternion.Euler(rota));
                        TrafficSign04.transform.localScale = scale;
                        TrafficSign04.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign05:
                        var TrafficSign05 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign05], pos, Quaternion.Euler(rota));
                        TrafficSign05.transform.localScale = scale;
                        TrafficSign05.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign06:
                        var TrafficSign06 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign06], pos, Quaternion.Euler(rota));
                        TrafficSign06.transform.localScale = scale;
                        TrafficSign06.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TrafficSign07:
                        var TrafficSign07 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TrafficSign07], pos, Quaternion.Euler(rota));
                        TrafficSign07.transform.localScale = scale;
                        TrafficSign07.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Tree01:
                        var Tree01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Tree01], pos, Quaternion.Euler(rota));
                        Tree01.transform.localScale = scale;
                        Tree01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Tree02:
                        var Tree02 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Tree02], pos, Quaternion.Euler(rota));
                        Tree02.transform.localScale = scale;
                        Tree02.tag = "Replica";
                        break;
                    case eSandBoxObjectType.Tree03:
                        var Tree03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.Tree03], pos, Quaternion.Euler(rota));
                        Tree03.transform.localScale = scale;
                        Tree03.tag = "Replica";
                        break;
                    case eSandBoxObjectType.TreeBlock01:
                        var TreeBlock01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.TreeBlock01], pos, Quaternion.Euler(rota));
                        TreeBlock01.transform.localScale = scale;
                        TreeBlock01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.WoodenBuilding01:
                        var WoodenBuilding01 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.WoodenBuilding01], pos, Quaternion.Euler(rota));
                        WoodenBuilding01.transform.localScale = scale;
                        WoodenBuilding01.tag = "Replica";
                        break;
                    case eSandBoxObjectType.WoodenBuilding02:
                        var WoodenBuilding02 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.WoodenBuilding02], pos, Quaternion.Euler(rota));
                        WoodenBuilding02.transform.localScale = scale;
                        WoodenBuilding02.tag = "Replica";
                        break;
                    case eSandBoxObjectType.WoodenBuilding03:
                        var WoodenBuilding03 = GameObject.Instantiate(makersObj[(int)eSandBoxObjectType.WoodenBuilding03], pos, Quaternion.Euler(rota));
                        WoodenBuilding03.transform.localScale = scale;
                        WoodenBuilding03.tag = "Replica";
                        break;
                }
                yield return wfu;
            }
        }
        loadingNow = false;


    }

}