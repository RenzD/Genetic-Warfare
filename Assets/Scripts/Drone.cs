using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public abstract class Drone : MonoBehaviour
{
    public float health = 20f;
    public float attack;
    public float speed;
    public float maxHealth = 30;
    public float wander;
    public float seek;
    public float arrive;
    public float flee;
    public float flock;
    public float capture;
    public float visionRange;

    public Slider slider;


    public virtual void DestroyWhenHealthIsZero()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void SetHealth()
    {
        float norm = (health - 0f) / (maxHealth - 0f);
        slider.value = norm;
    }

    public virtual void IsAttacked()
    {
        if (health > 0)
        {
            health -= Time.deltaTime;
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
