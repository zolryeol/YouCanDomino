using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSandBoxObjectType
{
    Block01, Block02, Block03,
    Bridge01,
    CastleBlock02, CastleBlock03, CastleBlock05, CastleBlock06, CastleBlock07, CastleBlock09, CastleBlock10, CastleBlock11, CastleBlock12, CastleBlock13,
RecBlock01, RecBlock02, RecBlock03, RecBlock04,
    Road1, Road2, Road3,
    RoadBlock01, RoadBlock02, RoadBlock03, RoadBlock04, RoadBlock05, RoadBlock06, RoadBlock07,
    Stair01,
    TrafficSign01, TrafficSign02, TrafficSign03, TrafficSign04, TrafficSign05, TrafficSign06, TrafficSign07,
    Tree01, Tree02, Tree03, TreeBlock01,
    WoodenBuilding01, WoodenBuilding02, WoodenBuilding03,
}
public class SaveAbleObject : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    eSandBoxObjectType sandBoxObjectType;

    public eSandBoxObjectType SBOT
    {
        get { return sandBoxObjectType; }
    }
}
