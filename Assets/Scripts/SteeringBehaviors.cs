using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    AIRigidbody rb;
    Steering steeringBasics;
    Sensor sensor;
    Sensor2 sensor2;
    MovementBehavior mb;

    [Header("Wander")]
    public float wanderRadius = 1.2f;
    public float wanderDistance = 2f;
    public float wanderJitter = 40f;
    Vector3 wanderTarget;

    [Header("Flee")]
    public float panicDist = 3.5f;
    public bool decelerateOnStop = true;
    public float fleeMaxAcceleration = 10f;
    public float fleeTimeToTarget = 0.1f;

    [Header("Flocking")]
    public float cohesionWeight = 1.5f;
    public float separationWeight = 2f;
    public float velocityMatchWeight = 1f;

    [Header("Velocity Match")]
    public float facingCosine = 90;
    public float timeToTarget = 0.1f;
    public float maxAcceleration = 4f;

    float facingCosineVal;

    [Header("Cohesion")]
    public float cohFacingCosine = 120f;

    float cohFacingCosineVal;

    [Header("Separation")]
    public float sepMaxAcceleration = 25;
    public float maxSepDist = 1f;

    void Awake()
    {
        rb = GetComponent<AIRigidbody>();
        steeringBasics = GetComponent<Steering>();
        sensor = transform.Find("Sensor").GetComponent<Sensor>();
        sensor2 = transform.Find("Sensor").GetComponent<Sensor2>();
        mb = GetComponent<MovementBehavior>();

        //Velocity Match
        facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);
        
        //Cohesion
        cohFacingCosineVal = Mathf.Cos(cohFacingCosine * Mathf.Deg2Rad);
    }

    void Start()
    {
        float theta = Random.value * 2 * Mathf.PI;
        wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta), wanderRadius * Mathf.Sin(theta), 0f);
    }

    public Vector3 Flock(Vector3 accel)
    {
        accel += GetSteeringCohesion(sensor.targets) * cohesionWeight;
        accel += GetSteeringSeparation(sensor.targets) * separationWeight;
        accel += GetSteeringVelocity(sensor.targets) * velocityMatchWeight;

        if (accel.magnitude < 0.005f)
        {
            accel = GetSteeringWander();
        }
        return accel;
    }

    public Vector3 Flock2(Vector3 accel)
    {
        accel += GetSteeringCohesion(sensor2.targets) * cohesionWeight;
        accel += GetSteeringSeparation(sensor2.targets) * separationWeight;
        accel += GetSteeringVelocity(sensor2.targets) * velocityMatchWeight;

        if (accel.magnitude < 0.005f)
        {
            accel = GetSteeringWander();
        }
        return accel;
    }

    public Vector3 GetSteeringWander()
    {
        /* Get the jitter for this time frame */
        float jitter = wanderJitter * Time.deltaTime;

        wanderTarget += new Vector3(Random.Range(-1f, 1f) * jitter, Random.Range(-1f, 1f) * jitter, 0f);

        /* Make the wanderTarget fit on the wander circle again */
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        /* Move the target in front of the character */
        Vector3 targetPosition = transform.position + transform.right * wanderDistance + wanderTarget;

        //Debug.DrawLine(transform.position, targetPosition);

        return steeringBasics.Seek(targetPosition);
    }

    public Vector3 GetSteeringFlee(Vector3 targetPosition)
    {
        /* Get the direction */
        Vector3 acceleration = transform.position - targetPosition;

        /* If the target is far way then don't flee */
        if (acceleration.magnitude > panicDist)
        {
            /* Slow down if we should decelerate on stop */
            if (decelerateOnStop && rb.Velocity.magnitude > 0.001f)
            {
                /* Decelerate to zero velocity in time to target amount of time */
                acceleration = -rb.Velocity / fleeTimeToTarget;

                if (acceleration.magnitude > fleeMaxAcceleration)
                {
                    acceleration = GiveMaxAccel(acceleration);
                }

                return acceleration;
            }
            else
            {
                rb.Velocity = Vector3.zero;
                return Vector3.zero;
            }
        }

        return GiveMaxAccel(acceleration);
    }

    Vector3 GiveMaxAccel(Vector3 v)
    {
        v.Normalize();

        /* Accelerate to the target */
        v *= fleeMaxAcceleration;

        return v;
    }

    public Vector3 GetSteeringVelocity(ICollection<GameObject> targets)
    {
        Vector3 accel = Vector3.zero;
        int count = 0;
        Rigidbody2D rb2D;
        foreach (GameObject r in targets)
        {
            rb2D = GetComponent<Rigidbody2D>();
            Vector3 rbPos = rb2D.position;
            if (steeringBasics.IsFacing(rbPos, facingCosineVal))
            {
                /* Calculate the acceleration we want to match this target */
                Vector3 rbVel = rb2D.velocity;
                Vector3 a = rbVel - rb.Velocity;
                /* Rather than accelerate the character to the correct speed in 1 second, 
                 * accelerate so we reach the desired speed in timeToTarget seconds 
                 * (if we were to actually accelerate for the full timeToTarget seconds). */
                a = a / timeToTarget;

                accel += a;

                count++;
            }
        }

        if (count > 0)
        {
            accel = accel / count;

            /* Make sure we are accelerating at max acceleration */
            if (accel.magnitude > maxAcceleration)
            {
                accel = accel.normalized * maxAcceleration;
            }
        }

        return accel;
    }

    public Vector3 GetSteeringCohesion(ICollection<GameObject> targets)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;
        Rigidbody2D rb2D;
        /* Sums up everyone's position who is close enough and in front of the character */
        foreach (GameObject r in targets)
        {
            rb2D = r.GetComponent<Rigidbody2D>();
            Vector3 rbPos = rb2D.position;
            if (steeringBasics.IsFacing(rbPos, cohFacingCosineVal))
            {
                centerOfMass += rbPos;
                count++;
            }
        }

        if (count == 0)
        {
            return Vector3.zero;
        }
        else
        {
            centerOfMass = centerOfMass / count;

            return steeringBasics.Arrive(centerOfMass, mb);
        }
    }

    public Vector3 GetSteeringSeparation(ICollection<GameObject> targets)
    {
        Vector3 acceleration = Vector3.zero;
        Rigidbody2D rb2D;
        CircleCollider2D col2D;
        foreach (GameObject r in targets)
        {
            rb2D = r.GetComponent<Rigidbody2D>();
            col2D = r.GetComponent<CircleCollider2D>();
            /* Get the direction and distance from the target */
            Vector3 direction = rb.ColliderPosition - rb2D.transform.TransformPoint(col2D.offset); ;
            float dist = direction.magnitude;

            if (dist < maxSepDist)
            {
                /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                float objectRadius = Mathf.Max(rb2D.transform.localScale.x, rb2D.transform.localScale.y) * col2D.radius;
                var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - rb.Radius - objectRadius);

                /* Added separation acceleration to the existing steering */
                direction = rb.ConvertVector(direction);
                direction.Normalize();
                acceleration += direction * strength;
            }
        }

        return acceleration;
    }
}

