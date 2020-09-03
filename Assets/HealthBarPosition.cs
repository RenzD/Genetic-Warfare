using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarPosition : MonoBehaviour
{/*
    Quaternion rotation;
    Vector3 position;
    void Awake()
    {
        rotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = rotation;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
    }*/
    public GameObject player; //Drag the "player" GO here in the Inspector    
    Quaternion rotation;
    void Awake()
    {
        rotation = transform.rotation;
    }

    public void LateUpdate()
    {
        transform.rotation = rotation;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
    }
}
