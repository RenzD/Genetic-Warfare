using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float resourceHealth = 5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (resourceHealth <= 0)
        {
            transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0f);
            resourceHealth = 5f;
        }
    }
}
