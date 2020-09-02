using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Steering))]
public class MovementBehavior : MonoBehaviour
{
    public enum BehaviorState
    {
        WANDER,
        SEEK, 
        ARRIVE,
        FLEE,
        FLOCK,
        CAPTURE
    };

    public BehaviorState behaviorState = BehaviorState.WANDER;
    Steering steeringBasics;
    SteeringBehaviors steering;
    //AIRigidbody rb;
    Sensor sensor;
    Vision vision;
    

    //Shooting shoot;
    public Transform target;
    public Resource resourceObject;
    public HealthResource healthObject;
    public Territory territoryObject;
    public Drone droneObject;
    public Drone drone; // this drone
    public Faction1 faction1;
    public Faction2 faction2;
    public World world;


    //GameObject debugRing;

    [Header("Flocking")]
    public float cohesionWeight = 1.5f;
    public float separationWeight = 2f;
    public float velocityMatchWeight = 1f;

    [Header("Arrive")]
    public float arriveSize;

    [Header("Priority")]
    public float wanderAtt;
    public float seekAtt;
    public float arriveAtt;
    public float fleeAtt;
    public float flockAtt;
    public float captureAtt;

    //----------------------------------------

    private string wander = "WANDER";
    private string seek = "SEEK";
    private string flee = "FLEE";
    private string arrive = "ARRIVE";
    private string flock = "FLOCK";
    private string capture = "CAPTURE";

    void Awake()
    {
        //		DebugDraw debugDraw = gameObject.GetComponent<DebugDraw> ();
        //		debugRing = debugDraw.createRing (Vector3.zero, wanderRadius);
        //rb = GetComponent<AIRigidbody>();
    }

    void Start()
    {
        GenerateRandomAttributes();
        GetBehaviorPriority();
        steeringBasics = GetComponent<Steering>();

        steering = GetComponent<SteeringBehaviors>();
        //shoot = GetComponent<Shooting>();
        sensor = transform.Find("Sensor").GetComponent<Sensor>();
        vision = transform.Find("Vision").GetComponent<Vision>();
    }

    void FixedUpdate()
    {
        GetVisionTargets();
        GetBehaviorPriority();

        Vector3 accel = Vector3.zero;
        switch (behaviorState)
        {
            case BehaviorState.WANDER:
                accel = steering.GetSteeringWander();
                break;
            case BehaviorState.SEEK:
                accel = steeringBasics.SeekEnemy(faction2.transform.position);
                break;
            case BehaviorState.ARRIVE:
                accel = Arrive();
                break;
            case BehaviorState.FLEE:
                accel = steering.GetSteeringFlee(faction2.transform.position);
                break;
            case BehaviorState.FLOCK:
                accel = steering.Flock(accel);
                break;
            case BehaviorState.CAPTURE:
                accel = Capture();
                break;
            default:
                Debug.Log("Unknown State");
                break;
        }
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

    private Vector3 Arrive()
    {
        Vector3 accel = steeringBasics.Arrive(resourceObject.transform.position, this);
        if (accel == Vector3.zero)
        {
            resourceObject.resourceHealth -= Time.deltaTime;
            drone.HealthRegen();
        }
        return accel;
    }

    private Vector3 Capture()
    {
        Vector3 accel = steeringBasics.Arrive(territoryObject.transform.position, this);
        if (accel == Vector3.zero)
        {
            territoryObject.capturePoint += Time.deltaTime;
        }

        return accel;
    }

    private void GetBehaviorPriority()
    {
        var dictionary = new Dictionary<string, float>(5);
        dictionary.Add(wander, wanderAtt);
        dictionary.Add(seek, seekAtt);
        dictionary.Add(arrive, arriveAtt);
        dictionary.Add(flee, fleeAtt);
        dictionary.Add(flock, flockAtt);
        dictionary.Add(capture, captureAtt);

        /** Order by values. LINQ
         * 
        var items = from pair in dictionary
                    orderby pair.Value ascending
                    select pair;


        // Display results.
        /*
        foreach (KeyValuePair<string, int> pair in items)
        {
            Debug.Log("Key: " + pair.Key + " Value: " +  pair.Value);
        }
        */

        // Reverse sort.
        // ... Can be looped over in the same way as above.
        var items = from pair in dictionary
                    orderby pair.Value descending
                    select pair;

        /*
        Debug.Log("0: " + items.ElementAt(0));
        */

        /**
        // CHECK WHEN IN VISION
        // WANDER IS DEFAULT
        // CHECK FOR VISION
        // IF THERE IS AN ALLY NEARBY 
        //      CHECK PRIORITY OF WANDER AND FLOCK
        // IF THERE IS AN ENEMY
        //      CHECK PRIORITY OF FLEE AND SEEK
        // IF THERE IS A RESOURCE-
        //      ARRIVE AT RESOURCE
        // IF THERE IS AN ALLY AND AN ENEMY
        //      CHECK PRIORITY OF SEEK, FLEE, FLOCK
        // IF THERE IS AN ALLY AND A RESOURCE
        //      CHECK PRIORITY OF FLOCK, ARRIVE
        // IF THERE IS AN ENEMY AND A RESOURCE
        //      CHECK PRIORITY SEEK, FLEE, ARRIVE
        // IF THERE IS AN ENEMY, ALLY, AND A RESOURCE
        //      CHECK PRIORITY OF SEEK, ARRIVE, FLEE, FLOCK
        //**********************************************************************************/
        if (dictionary.Count != 0)
        {
            // NONE
            if (faction1 == null && faction2 == null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[wander] > dictionary[flock])
                {
                    behaviorState = BehaviorState.WANDER;
                }
                else
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
            // ALLY ONLY
            else if (faction1 != null && faction2 == null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[wander] > dictionary[flock])
                {
                    behaviorState = BehaviorState.WANDER;
                } 
                else
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
            // ENEMY ONLY
            else if (faction2 != null && faction1 == null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[seek] > dictionary[flee])
                {
                    behaviorState = BehaviorState.SEEK;
                } 
                else
                {
                    behaviorState = BehaviorState.FLEE;
                }
            }
            // RESOURCE ONLY
            else if (resourceObject != null && faction1 == null && faction2 == null && territoryObject == null)
            {
                behaviorState = BehaviorState.ARRIVE;
            }
            // TERRITORY ONLY
            else if (territoryObject != null && resourceObject == null && faction1 == null && faction2 == null)
            {
                if (territoryObject.territoryState == Territory.TerritoryState.FACTION2 || 
                    territoryObject.territoryState == Territory.TerritoryState.UNCAPTURED)
                {
                    behaviorState = BehaviorState.CAPTURE;
                } else
                {
                    behaviorState = BehaviorState.WANDER;
                }
            }
            // TERRITORY AND ENEMY
            else if (territoryObject != null && faction2 != null && resourceObject == null && faction1 == null)
            {
                if (dictionary[capture] > dictionary[flee])
                {
                    behaviorState = BehaviorState.CAPTURE;
                } 
                else
                {
                    behaviorState = BehaviorState.FLEE;
                }
            } 
            
            // ALLY AND ENEMY
            else if (faction1 != null && faction2 != null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[seek] > dictionary[flee] && dictionary[seek] > dictionary[flock])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[flee] > dictionary[seek] && dictionary[flee] > dictionary[flock])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (dictionary[flock] > dictionary[seek] && dictionary[flock] > dictionary[flee])
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
            
            // ALLY AND RESOURCE
            else if(faction1 != null && resourceObject != null && faction2 == null && territoryObject == null)
            {
                behaviorState = BehaviorState.ARRIVE;

                /*
                if (dictionary[flock] > dictionary[arrive])
                {
                    behaviorState = BehaviorState.FLOCK;
                }
                else
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                */
            }
            
            // ENEMY AND RESOURCE
            else if (faction2 != null && resourceObject != null && faction1 == null && territoryObject == null)
            {
                if (dictionary[seek] > dictionary[arrive] && dictionary[seek] > dictionary[flee])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[flee] > dictionary[seek] && dictionary[flee] > dictionary[arrive])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (dictionary[arrive] > dictionary[seek] && dictionary[arrive] > dictionary[flee])
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
            }
            //ENEMY, ALLY, AND RESOURCE
            else if(faction1 != null && faction2 != null && resourceObject != null && territoryObject == null)
            {
                //SEEK, ARRIVE, FLEE, FLOCK
                if (dictionary[seek] >= dictionary[arrive] && dictionary[seek] >= dictionary[flee] 
                                                            && dictionary[seek] >= dictionary[flock])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[arrive] >= dictionary[seek] && dictionary[arrive] >= dictionary[flee]
                                                            && dictionary[arrive] >= dictionary[flock])
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                else if (dictionary[flee] >= dictionary[seek] && dictionary[flee] >= dictionary[arrive]
                                                            && dictionary[flee] >= dictionary[flock])
                {
                    behaviorState = BehaviorState.FLEE;
                }

                else if (dictionary[flock] >= dictionary[seek] && dictionary[flock] >= dictionary[arrive]
                                                            && dictionary[flock] >= dictionary[flee])
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            } 
            else
            {
                if (dictionary[wander] > dictionary[flock])
                {
                    behaviorState = BehaviorState.WANDER;
                }
                else
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
        }
        else
        {
            Debug.Log("List is empty");
        }

    }

    private void GetVisionTargets()
    {
        if (resourceObject != null && !vision.targets.Contains(resourceObject.gameObject))
        {
            resourceObject = null;
        }
        if (droneObject != null && !vision.targets.Contains(droneObject.gameObject))
        {
            droneObject = null;
        }
        if (faction1 != null && !vision.targets.Contains(faction1.gameObject))
        {
            faction1 = null;
        }
        if (faction2 != null && !vision.targets.Contains(faction2.gameObject))
        {
            faction2 = null;
        }
        if (territoryObject != null && !vision.targets.Contains(territoryObject.gameObject))
        {
            territoryObject = null;
        }
        if (healthObject != null && !vision.targets.Contains(healthObject.gameObject))
        {
            healthObject = null;
        }

        foreach (GameObject target in vision.targets)
        {
            //Debug.Log("Inside foreach");
            // SET UP TYPES OF OBJECTS
            if (target.GetComponent<Resource>())
            {
                if (resourceObject == null)
                    resourceObject = target.GetComponent<Resource>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(resourceObject.transform.position, transform.position))
                    resourceObject = target.GetComponent<Resource>();
            }
            else if (target.GetComponent<HealthResource>())
            {
                if (healthObject == null)
                    healthObject = target.GetComponent<HealthResource>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(healthObject.transform.position, transform.position))
                    healthObject = target.GetComponent<HealthResource>();
            }
            else if (target.GetComponent<Drone>())
            {
                if (droneObject == null)
                    droneObject = target.GetComponent<Drone>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(droneObject.transform.position, transform.position))
                    droneObject = target.GetComponent<Drone>();
            }
            else if (target.GetComponent<Faction1>())
            {
                if (faction1 == null)
                    faction1 = target.GetComponent<Faction1>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(faction1.transform.position, transform.position))
                    faction1 = target.GetComponent<Faction1>();
            }
            else if (target.GetComponent<Faction2>())
            {
                if (faction2 == null)
                    faction2 = target.GetComponent<Faction2>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(faction2.transform.position, transform.position))
                    faction2 = target.GetComponent<Faction2>();
            }
            else if (target.GetComponent<Territory>())
            {
                if (territoryObject == null)
                    territoryObject = target.GetComponent<Territory>();
                else if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(territoryObject.transform.position, transform.position))
                    territoryObject = target.GetComponent<Territory>();
            }
        }
    }

    private void GenerateRandomAttributes()
    {
        //float[] attributes = { Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)};
        float[] att = new float[6];
        att[0] = Random.Range(0f, 1f); // Wander
        att[1] = Random.Range(0f, 1f); // Seek
        att[2] = Random.Range(0f, 1f); // Arrive
        att[3] = Random.Range(0f, 1f); // Flee
        att[4] = Random.Range(0f, 1f); // Flock
        att[5] = Random.Range(0f, 1f); // Capture

        float sum = 0;
        foreach (float val in att)
        {
            sum += val;
        }

        wanderAtt = (att[0] / sum) * 100f;
        seekAtt = (att[1] / sum) * 100f;
        arriveAtt = (att[2] / sum) * 100f;
        fleeAtt = (att[3] / sum) * 100f;
        flockAtt = (att[4] / sum) * 100f;
        captureAtt = (att[5] / sum) * 100f;
    }
}
