using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //public GameObject hitEffect;

   void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(gameObject);
    }
}
