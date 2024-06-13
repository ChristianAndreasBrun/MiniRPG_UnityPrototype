using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControl : MonoBehaviour
{
    public Transform target;
    public float speedCam;


    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 finalPosition = target.position;
        finalPosition.z = -10;
        transform.position = Vector3.Lerp(transform.position, finalPosition, speedCam * Time.deltaTime);
    }
}
