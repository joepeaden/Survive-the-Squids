using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterWait : MonoBehaviour
{
    public bool shouldDestroy;
    public float timeToDisable;
    float spawnTime;

    void OnEnable   ()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime > timeToDisable)
        {
            if (shouldDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
