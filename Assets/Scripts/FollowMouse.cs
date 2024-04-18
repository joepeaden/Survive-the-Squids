using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        if(isActiveAndEnabled)
        {
            transform.position = Input.mousePosition;
        }
    }
}
