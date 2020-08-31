using UnityEngine;

public class WanderUnit : MonoBehaviour
{
    Steering steeringBasics;
    Wander wander;

    void Start()
    {
        steeringBasics = GetComponent<Steering>();
        wander = GetComponent<Wander>();
    }

    void FixedUpdate()
    {
        Vector3 accel = wander.GetSteering();

        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }
}