using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        float deltaY = Mathf.Sin(Time.timeSinceLevelLoad + Time.deltaTime);
        deltaY -= Mathf.Sin(Time.timeSinceLevelLoad);
        deltaY /= 10;
        transform.position = new Vector3(pos.x, pos.y + deltaY, pos.z);
    }
}
