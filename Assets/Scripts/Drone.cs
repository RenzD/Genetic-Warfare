using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Drone : MonoBehaviour
{
    // Start is called before the first frame update

    public float health = 10;


    // Update is called once per frame
    void FixedUpdate()
    {
        HealthDegen();
        DestroyObject();
    }

    private void DestroyObject()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void HealthDegen()
    {
        if (health > 0)
        {
            health -= Time.deltaTime;
        }
    }

    public void HealthRegen()
    {
        if (health < 50)
        {
            health += (Time.deltaTime * 3);
        }
    }
}
