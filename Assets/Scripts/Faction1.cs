using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class Faction1 : Drone
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
        steeringBasics.maxVelocity = speed;

        steering = GetComponent<SteeringBehaviors>();
        steering.panicDist = visionRange + .5f;

        worldObject = GameObject.FindWithTag("World");
        world = worldObject.GetComponent<World>();
    }
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SetHealthSlider();
        world.numPopulation1 = DestroyDrone(world.numPopulation1);
    }

    protected override void GetBehaviorPriority()
    {
        Dictionary<string, float> dictionary = new Dictionary<string, float>(5)
        {
            { wanderStr, wander },
            { seekStr, seek },
            { arriveStr, arrive },
            { fleeStr, flee },
            { flockStr, flock },
            { captureStr, capture }
        };

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
                behaviorState = BehaviorState.WANDER;
            }
            // ALLY ONLY
            // Picks the higher behavior priority between wander and flock
            else if (faction1 != null && faction2 == null && resourceObject == null && territoryObject == null)
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
            // Picks the higher behavior priority between seek and flock
            else if (faction2 != null && faction1 == null && resourceObject == null && territoryObject == null)
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
            // If the drone is hungry, then arrive at resource
            else if (resourceObject != null && faction1 == null && faction2 == null && territoryObject == null && hungryBool)
            {
                behaviorState = BehaviorState.ARRIVE;
            }
            // TERRITORY ONLY
            // If the territory is not its own faction, then capture, else wander
            else if (territoryObject != null && resourceObject == null && faction1 == null && faction2 == null)
            {
                if (territoryObject.territoryState == GeneticAlgorithm.TerritoryState.FACTION2 ||
                    territoryObject.territoryState == GeneticAlgorithm.TerritoryState.UNCAPTURED)
                {
                    behaviorState = BehaviorState.CAPTURE;
                }
                else
                {
                    behaviorState = BehaviorState.WANDER;
                }
            }
            // TERRITORY AND ENEMY
            // If the territory is not owned by its own faction, then pick behavior priority between capture, seek and flee, 
            // Else, seek or flee
            else if (territoryObject != null && faction2 != null && resourceObject == null && faction1 == null)
            {
                if (territoryObject.territoryState == GeneticAlgorithm.TerritoryState.FACTION1)
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
                else
                {
                    if (dictionary[captureStr] > dictionary[seekStr] && dictionary[captureStr] > dictionary[fleeStr])
                    {
                        behaviorState = BehaviorState.CAPTURE;
                    }
                    else if (dictionary[seekStr] > dictionary[captureStr] && dictionary[seekStr] > dictionary[fleeStr])
                    {
                        behaviorState = BehaviorState.SEEK;
                    }
                    else if (dictionary[fleeStr] > dictionary[captureStr] && dictionary[fleeStr] > dictionary[seekStr])
                    {
                        behaviorState = BehaviorState.FLEE;
                    }
                }
            }

            // ALLY AND ENEMY
            // Picks the behavior priority between seek, flee    ** no flock **
            else if (faction1 != null && faction2 != null && resourceObject == null && territoryObject == null)
            {
                if (dictionary[seekStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else
                {
                    behaviorState = BehaviorState.FLEE;
                }

                /* ** with flock **
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
                */
            }

            // ALLY AND RESOURCE
            // If hungry, then arrive at resource
            // Else pick priority between flock and wander
            else if (faction1 != null && resourceObject != null && faction2 == null && territoryObject == null)
            {
                if (hungryBool)
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                else if (dictionary[flockStr] > dictionary[wanderStr])
                {
                    behaviorState = BehaviorState.FLOCK;
                }
                else
                {
                    behaviorState = BehaviorState.WANDER;
                }
            }

            // ALLY AND TERRITORY
            // If its not owned by its own faction, then capture, else wander
            else if (faction1 != null && territoryObject != null && faction2 == null && resourceObject == null)
            {
                if (territoryObject.territoryState != GeneticAlgorithm.TerritoryState.FACTION1)
                {
                    behaviorState = BehaviorState.CAPTURE;
                }
                else
                {
                    behaviorState = BehaviorState.WANDER;
                }
            }

            // RESOURCE AND TERRITORY - capture, arrive
            // Check if hungry and priority over arrive and capture behaviors, also check if territory owned by faction
            else if (resourceObject != null && territoryObject != null && faction1 == null && faction2 == null)
            {
                if (dictionary[arriveStr] > dictionary[captureStr] && hungryBool)
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                else if (territoryObject.territoryState != GeneticAlgorithm.TerritoryState.FACTION1)
                {
                    behaviorState = BehaviorState.CAPTURE;
                }
                else
                {
                    if (hungryBool)
                    {
                        behaviorState = BehaviorState.ARRIVE;
                    }
                    else
                    {
                        behaviorState = BehaviorState.WANDER;
                    }
                }
            }

            // ENEMY AND RESOURCE - seek, flee, arrive, wander
            // Picks behavior priority between seek, flee, arrive, wander
            else if (faction2 != null && resourceObject != null && faction1 == null && territoryObject == null)
            {
                if (dictionary[seekStr] > dictionary[arriveStr] && dictionary[seekStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[fleeStr] > dictionary[seekStr] && dictionary[fleeStr] > dictionary[arriveStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (hungryBool)
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                // If arrive is highest priority, but not hungry choose between seek and flee only
                else
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

            }

            //ALLY, ENEMY AND RESOURCE - seek, arrive, flee, flock, wander
            // Check priority of seek, flee, arrive
            else if (faction1 != null && faction2 != null && resourceObject != null && territoryObject == null)
            {
                if (dictionary[seekStr] > dictionary[arriveStr] && dictionary[seekStr] > dictionary[fleeStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[fleeStr] > dictionary[seekStr] && dictionary[fleeStr] > dictionary[arriveStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (hungryBool)
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                // If arrive is highest priority, but not hungry choose between seek and flee only
                else
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
            }
            // ALLY, RESOURCE, TERRITORY
            // Check if territory is owned by faction
            // Check if hungry
            else if (faction1 != null && resourceObject != null && territoryObject != null && faction2 == null)
            {
                if (dictionary[arriveStr] > dictionary[captureStr] && hungryBool)
                {
                    behaviorState = BehaviorState.ARRIVE;
                }
                else if (territoryObject.territoryState != GeneticAlgorithm.TerritoryState.FACTION1)
                {
                    behaviorState = BehaviorState.CAPTURE;
                }
                else
                {
                    if (hungryBool)
                    {
                        behaviorState = BehaviorState.ARRIVE;
                    }
                    else
                    {
                        behaviorState = BehaviorState.WANDER;
                    }
                }
            }
            // ALLY, ENEMY, TERRITORY
            // If the territory is not owned by its own faction, then pick behavior priority between capture, seek and flee, 
            // Else, seek or flee
            else if (faction1 != null && faction2 != null && territoryObject != null && resourceObject == null)
            {
                if (territoryObject.territoryState == GeneticAlgorithm.TerritoryState.FACTION1)
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
                else
                {
                    if (dictionary[captureStr] > dictionary[seekStr] && dictionary[captureStr] > dictionary[fleeStr])
                    {
                        behaviorState = BehaviorState.CAPTURE;
                    }
                    else if (dictionary[seekStr] > dictionary[captureStr] && dictionary[seekStr] > dictionary[fleeStr])
                    {
                        behaviorState = BehaviorState.SEEK;
                    }
                    else if (dictionary[fleeStr] > dictionary[captureStr] && dictionary[fleeStr] > dictionary[seekStr])
                    {
                        behaviorState = BehaviorState.FLEE;
                    }
                }
            }
            // ENEMY RESOURCE TERRITORY
            else if (faction2 != null && territoryObject != null && resourceObject != null && faction1 == null)
            {
                if (dictionary[seekStr] > dictionary[arriveStr] && 
                    dictionary[seekStr] > dictionary[fleeStr] && 
                    dictionary[seekStr] > dictionary[captureStr])
                {
                    behaviorState = BehaviorState.SEEK;
                }
                else if (dictionary[fleeStr] > dictionary[seekStr] && 
                        dictionary[fleeStr] > dictionary[arriveStr] && 
                        dictionary[fleeStr] > dictionary[captureStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else if (dictionary[captureStr] > dictionary[seekStr] && 
                         dictionary[captureStr] > dictionary[fleeStr] && 
                         dictionary[captureStr] > dictionary[arriveStr])
                {
                    behaviorState = BehaviorState.FLEE;
                }
                else
                {
                    // Arrive is highest
                    if (hungryBool)
                    {
                        behaviorState = BehaviorState.ARRIVE;
                    }
                    // Not hungry, so pick the other 3
                    else if (dictionary[seekStr] > dictionary[fleeStr] &&
                            dictionary[seekStr] > dictionary[captureStr])
                    {
                        behaviorState = BehaviorState.SEEK;
                    }
                    else if (dictionary[fleeStr] > dictionary[seekStr] &&
                            dictionary[fleeStr] > dictionary[captureStr])
                    {
                        behaviorState = BehaviorState.FLEE;
                    }
                    else
                    {
                        behaviorState = BehaviorState.CAPTURE;
                    }
                }
            }
            else
            {
                behaviorState = BehaviorState.WANDER;
            }
        }
        else
        {
            Debug.Log("List is empty");
        }
    }

    protected override void DroneBehavior()
    {
        // Select Behavior
        Vector3 accel = steering.GetSteeringColAvoid(colAvoidSensor.targets);
        if (accel.magnitude < .005f)
        {
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
        }
        //Clears line renderer and resets attack
        Clear();
        //Updates the velocity of the current game object by the given linear
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
    }

    private Vector3 SeekEnemy()
    {
        Vector3 accel = steeringBasics.SeekEnemy(faction2.transform.position);
        if (steeringBasics.GetDist(faction2.transform.position) < (visionRange - 1f)) // Attack range
        {
            Vector2 direction = new Vector2(faction2.transform.position.x - transform.position.x,
                                            faction2.transform.position.y - transform.position.y);
            transform.right = direction;

            attacktimer += Time.deltaTime;
            if (attacktimer > 0.8f)
            {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, faction2.transform.position);
                linetimer = 0f;
                lineRenderer.enabled = true; ;
            }
            if (attacktimer > 1f)
            {
                attacktimer = 0f;
                lineRenderer.enabled = false;
                Attack(faction2);
                Instantiate(impactEffect, faction2.transform.position, Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0f, 360f))));
            }
        } else
        {
            attacktimer = 0f;
            lineRenderer.enabled = false;
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
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, resourceObject.transform.position);
            lineRenderer.enabled = true; ;
        }
        else
        {
            lineRenderer.enabled = false;
        }
        return accel;
    }

    private Vector3 Capture()
    {
        Vector3 accel = steeringBasics.Arrive(territoryObject.transform.position, this);
        if (accel == Vector3.zero)
        {
            territoryObject.capturePoint += Time.deltaTime;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, territoryObject.transform.position);
            lineRenderer.enabled = true; ;
        }
        else
        {
            lineRenderer.enabled = false;
        }
        return accel;
    }
}
