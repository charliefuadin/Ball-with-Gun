using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendering : MonoBehaviour
{

    public LineRenderer linerender;

    public GameObject firstPoint;
    public GameObject secondPoint;

    // Update is called once per frame
    void Update()
    {
        linerender.SetPosition(0, firstPoint.transform.position);
        linerender.SetPosition(1, secondPoint.transform.position);
    }
}
