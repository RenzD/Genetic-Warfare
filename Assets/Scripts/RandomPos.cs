using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPos : MonoBehaviour
{
    float timeLeft = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            //transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0f);
            timeLeft = 4;
        }
    }
}
