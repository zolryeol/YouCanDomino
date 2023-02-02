using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmLocateCenterOfMaker : MonoBehaviour
{
    Vector3 makerCenter;
    // Start is called before the first frame update
    void Start()
    {
        makerCenter = GameObject.Find("DominoMaker").GetComponent<Renderer>().bounds.center;

        transform.position = makerCenter;

        Debug.Log("ผพลอดย " + makerCenter);
    }

}
