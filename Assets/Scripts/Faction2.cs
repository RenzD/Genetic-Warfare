using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class Faction2 : Drone
{
    Steering steeringBasics;
    SteeringBehaviors steering;

    [Header("World")]
    public GameObject worldObject;
    public World world;
    protected override void Start()
    {
        base.Start();
        steeringBasics = GetComponent<Steering>();
        steering = GetComponent<SteeringBehaviors>();
        worldObject = GameObject.FindWithTag("World");
        world = worldObject.GetComponent<World>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (health < 0)
        {
            Destroy(gameObject);
            try
            {
                world.numPopulation2--;
            }
            catch
            {
                Debug.Log("Hello:");
            }
        }
    }

    protected override void GetBehaviorPriority()
    {
        var dictionary = new Dictionary<string, float>(5);
        dictionary.Add(wanderStr, wander);
        dictionary.Add(seekStr, seek);
        dictionary.Add(arriveStr, arrive);
        dictionary.Add(fleeStr, flee);
        dictionary.Add(flockStr, flock);
        dictionary.Add(captureStr, capture);

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
            if (faction2 == null && faction1 == null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[wanderStr] > dictionary[flockStr])
                {
                    behaviorState = BehaviorState.WANDER;
                }
                else
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
            // ALLY ONLY
            else if (faction2 != null && faction1 == null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[wanderStr] > dictionary[flockStr])
                {
                    behaviorState = BehaviorState.WANDER;
                }
                else
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
            // ENEMY ONLY
            else if (faction1 != null && faction2 == null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[seekStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else
                {
                    behaviorState = BehaviorState.FLEE;
                }
            }
            // RESOURCE ONLY
            else if (resourceObject != null && faction2 == null && faction1 == null && territoryObject == null)
            {
                behaviorState = BehaviorState.ARRIVE;
            }
            // TERRITORY ONLY
            else if (territoryObject != null && resourceObject == null && faction2 == null && faction1 == null)
            {
                if (territoryObject.territoryState == Territory.TerritoryState.FACTION1 ||
                    territoryObject.territoryState == Territory.TerritoryState.UNCAPTURED)
                {
                    behaviorState = BehaviorState.CAPTURE;
                }
                else
                {
                    behaviorState = BehaviorState.WANDER;
                }
            }
            // TERRITORY AND ENEMY
            else if (territoryObject != null && faction1 != null && resourceObject == null && faction2 == null)
            {
                if (dictionary[captureStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.CAPTURE;
                }
                else
                {
                    behaviorState = BehaviorState.FLEE;
                }
            }

            // ALLY AND ENEMY
            else if (faction2 != null && faction1 != null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[seekStr] > dictionary[fleeStr] && dictionary[seekStr] > dictionary[flockStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[fleeStr] > dictionary[seekStr] && dictionary[fleeStr] > dictionary[flockStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (dictionary[flockStr] > dictionary[seekStr] && dictionary[flockStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }

            // ALLY AND RESOURCE
            else if (faction2 != null && resourceObject != null && faction1 == null && territoryObject == null)
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
            else if (faction1 != null && resourceObject != null && faction2 == null && territoryObject == null)
            {
                if (dictionary[seekStr] > dictionary[arriveStr] && dictionary[seekStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[fleeStr] > dictionary[seekStr] && dictionary[fleeStr] > dictionary[arriveStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (dictionary[arriveStr] > dictionary[seekStr] && dictionary[arriveStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
            }
            //ENEMY, ALLY, AND RESOURCE
            else if (faction2 != null && faction1 != null && resourceObject != null && territoryObject == null)
            {
                //SEEK, ARRIVE, FLEE, FLOCK
                //Seek enemy
                if (dictionary[seekStr] >= dictionary[arriveStr] && dictionary[seekStr] >= dictionary[fleeStr]
                                                            && dictionary[seekStr] >= dictionary[flockStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                //Get resource
                else if (dictionary[arriveStr] >= dictionary[seekStr] && dictionary[arriveStr] >= dictionary[fleeStr]
                                                            && dictionary[arriveStr] >= dictionary[flockStr])
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                // Flee from enemy
                else if (dictionary[fleeStr] >= dictionary[seekStr] && dictionary[fleeStr] >= dictionary[arriveStr]
                                                            && dictionary[fleeStr] >= dictionary[flockStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                // Flock
                else if (dictionary[flockStr] >= dictionary[seekStr] && dictionary[flockStr] >= dictionary[arriveStr]
                                                            && dictionary[flockStr] >= dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.FLOCK;
                }
            }
            else
            {
                if (dictionary[wanderStr] > dictionary[flockStr])
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

    protected override void DroneBehavior()
    {
        Vector3 accel = Vector3.zero;
        switch (behaviorState)
        {
            case BehaviorState.WANDER:
                accel = steering.GetSteeringWander();
                break;
            case BehaviorState.SEEK:
                accel = SeekEnemy();
                break;
            case BehaviorState.ARRIVE:
                accel = Arrive();
                break;
            case BehaviorState.FLEE:
                accel = steering.GetSteeringFlee(faction1.transform.position);
                break;
            case BehaviorState.FLOCK:
                accel = steering.Flock2(accel);
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

    private Vector3 SeekEnemy()
    {
        Vector3 accel = steeringBasics.SeekEnemy(faction1.transform.position);
        if (accel == Vector3.zero)
        {
            faction1.IsAttacked();
        }
        return accel;
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
            territoryObject.capturePoint -= Time.deltaTime;
        }

        return accel;
    }
}
