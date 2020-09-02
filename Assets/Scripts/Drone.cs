using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drone : MonoBehaviour
{
    public float health = 20;
    public float maxHealth;
    public float wander;
    public float seek;
    public float arrive;
    public float flee;
    public float flock;
    public float capture;
    public float visionRange;

    public virtual void DestroyWhenHealthIsZero()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }

    public virtual void HealthDegen()
    {
        if (health > 0)
        {
            health -= Time.deltaTime;
        }
    }
    public virtual void HealthRegen()
    {
        if (health < 30)
        {
            health += (Time.deltaTime * 3);
        }
    }
}
