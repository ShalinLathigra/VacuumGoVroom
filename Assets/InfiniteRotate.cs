using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteRotate : MonoBehaviour
{

    float smooth = 20.0f;

    // Update is called once per frame
    void Update()
    {
        
        //Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * smooth, Vector3.up);
    }
}
