using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Tests
{
    public class PlayModeTest
    {
        private GameObject testObject;
       
        [SetUp]
        public void Setup()
        {
            testObject = GameObject.Instantiate(new GameObject(), new Vector3(0,0,0), Quaternion.identity);
        }

        [UnityTest]
        public IEnumerator Check_World_Population_System()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            World world = GameObject.FindWithTag("World").GetComponent<World>();
            int maxPop1 = 0;
            int maxPop2 = 0;
            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                if (territory.territoryState == GeneticAlgorithm.TerritoryState.FACTION1)
                {
                    maxPop1++;
                }
                else if (territory.territoryState == GeneticAlgorithm.TerritoryState.FACTION2)
                {
                    maxPop2++;
                }
            }
            Assert.AreEqual(maxPop1 * world.initOwnedMultiplier, world.maxPopulation1);
            Assert.AreEqual(maxPop2 * world.initOwnedMultiplier, world.maxPopulation2);
            yield return null;

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(3, 0, 0), Quaternion.identity);
            GameObject t1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(3, 3, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            GeneticAlgorithm territory1 = t1.GetComponent<GeneticAlgorithm>();
            Assert.NotNull(drone1);
            Assert.NotNull(territory1);
            drone1.capture = 100f;
            territory1.capturePoint = territory1.maxCapturePoint;
            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(maxPop1 * world.initOwnedMultiplier + 1 * world.capturedMultiplier, world.maxPopulation1);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone2_Capturing_A_Territory()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject t1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(3, 0, 0), Quaternion.identity);
            Faction2 drone1 = d1.GetComponent<Faction2>();
            drone1.name = "Faction1 Gen 0";
            GeneticAlgorithm territory1 = t1.GetComponent<GeneticAlgorithm>();
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone1);
            Assert.NotNull(territory1);
            Assert.Greater(0, territory1.capturePoint);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone1_Capturing_A_Territory()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject t1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(3, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            GeneticAlgorithm territory1 = t1.GetComponent<GeneticAlgorithm>();
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone1);
            Assert.NotNull(territory1);
            Assert.Less(0, territory1.capturePoint);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone_Health_Decay_And_Death()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, -4, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            drone1.health = 1f;
            drone1.name = "Faction1 Test 0";
            yield return new WaitForSeconds(2f);

            
            bool nBool = false;
            if (d1 == null)
            {
                nBool = true;
            }
            Assert.IsTrue(nBool);
            //Assert.Null(d1);
            yield return null;
            
        }

        [UnityTest]
        public IEnumerator Check_Genetic_Algorithm_Parent_Selection_Function()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            World world = GameObject.FindWithTag("World").GetComponent<World>();
            Time.timeScale = 3f;
            yield return new WaitForSecondsRealtime(15f);

            Assert.Less(0, world.Faction1FirstParent.fitnessScore);
            Assert.Less(0, world.Faction1SecondParent.fitnessScore);
            Assert.GreaterOrEqual(world.Faction1FirstParent.fitnessScore, world.Faction1SecondParent.fitnessScore);

            Assert.Less(0, world.Faction2FirstParent.fitnessScore);
            Assert.Less(0, world.Faction2SecondParent.fitnessScore);

            Assert.GreaterOrEqual(world.Faction2FirstParent.fitnessScore, world.Faction2SecondParent.fitnessScore);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Genetic_Algorithm_Mutation_Function()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, -4, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            float wander = drone1.wander;
            float seek = drone1.seek;
            float flee = drone1.flee;
            float flock = drone1.flock;
            float arrive = drone1.arrive;
            float capture = drone1.capture;
            float attack = drone1.attack;
            float speed = drone1.speed;
            float visionRange = drone1.visionRange;
            float hungerMeter = drone1.hungerMeter;
            float maxHealth = drone1.maxHealth;

            GameObject t1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(10, 0, 0), Quaternion.identity);
            GeneticAlgorithm territory1 = t1.GetComponent<GeneticAlgorithm>();
            int attributeChanged;
            int count = 0;
            while (count < 100)
            {
                count++;
                attributeChanged = territory1.Mutation(drone1);
                switch (attributeChanged)
                {
                    case 0:
                        Assert.AreNotEqual(wander, drone1.wander);
                        break;
                    case 1:
                        Assert.AreNotEqual(seek, drone1.seek);
                        break;
                    case 2:
                        Assert.AreNotEqual(arrive, drone1.arrive);
                        break;
                    case 3:
                        Assert.AreNotEqual(flee, drone1.flee);
                        break;
                    case 4:
                        Assert.AreNotEqual(flock, drone1.flock);
                        break;
                    case 5:
                        Assert.AreNotEqual(capture, drone1.capture);
                        break;
                    case 6:
                        Assert.AreNotEqual(maxHealth, drone1.maxHealth);
                        break;
                    case 7:
                        Assert.AreNotEqual(attack, drone1.attack);
                        break;
                    case 8:
                        Assert.AreNotEqual(speed, drone1.speed);
                        break;
                    case 9:
                        Assert.AreNotEqual(visionRange, drone1.visionRange);
                        break;
                    case 10:
                        Assert.AreNotEqual(hungerMeter, drone1.hungerMeter);
                        break;
                }
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Check_Genetic_Algorithm_Crossover_Function()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, -4, 0), Quaternion.identity);
            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 4, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            Faction1 drone2 = d2.GetComponent<Faction1>();
            GameObject t1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(0, 0, 0), Quaternion.identity);
            GeneticAlgorithm territory1 = t1.GetComponent<GeneticAlgorithm>();

            territory1.InitializeRandomAttributes(drone1);
            territory1.InitializeRandomAttributes(drone2);

            Assert.NotNull(territory1);
            yield return null;

            World world = GameObject.FindWithTag("World").GetComponent<World>();
            Assert.NotNull(world);

            drone1.name = "Faction1 Parent 0";
            drone2.name = "Faction1 Parent 0";
            world.Faction1FirstParent = territory1.SetAttributes(drone1);
            world.Faction1SecondParent = territory1.SetAttributes(drone2);

            World.DroneAttributes[] droneAttributes = new World.DroneAttributes[] { world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent, world.Faction1FirstParent,
                                                            world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent, world.Faction1SecondParent };
            World.DroneAttributes tempDroneAtt;

            GameObject d3 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(4, 4, 0), Quaternion.identity);
            Faction1 drone3 = d3.GetComponent<Faction1>();

            //Copied Crossover - needs to be changed
            //Shuffles content of crossOrder
            for (int i = 0; i < droneAttributes.Length; i++)
            {
                int rnd = Random.Range(0, droneAttributes.Length);
                tempDroneAtt = droneAttributes[rnd];
                droneAttributes[rnd] = droneAttributes[i];
                droneAttributes[i] = tempDroneAtt;
            }
            drone3.wander = droneAttributes[0].wander;
            drone3.seek = droneAttributes[1].seek;
            drone3.arrive = droneAttributes[2].arrive;
            drone3.flee = droneAttributes[3].flee;
            drone3.flock = droneAttributes[4].flock;
            drone3.maxHealth = droneAttributes[5].maxHealth;
            drone3.health = drone3.maxHealth;
            drone3.attack = droneAttributes[6].attack;
            drone3.speed = droneAttributes[7].speed;
            drone3.capture = droneAttributes[8].capture;
            drone3.visionRange = droneAttributes[9].visionRange;
            drone3.hungerMeter = droneAttributes[10].hungerMeter;
            //-------------------------------------

            int drone1Stats = 0;
            int drone2Stats = 0;
            int temp;
            
            temp = drone3.wander == drone1.wander ? drone1Stats++ : drone2Stats++;
            temp = drone3.seek == drone1.seek ? drone1Stats++ : drone2Stats++;
            temp = drone3.arrive == drone1.arrive ? drone1Stats++ : drone2Stats++;
            temp = drone3.flee == drone1.flee ? drone1Stats++ : drone2Stats++;
            temp = drone3.flock == drone1.flock ? drone1Stats++ : drone2Stats++;
            temp = drone3.capture == drone1.capture ? drone1Stats++ : drone2Stats++;
            temp = drone3.maxHealth == drone1.maxHealth ? drone1Stats++ : drone2Stats++;
            temp = drone3.attack == drone1.attack ? drone1Stats++ : drone2Stats++;
            temp = drone3.speed == drone1.speed ? drone1Stats++ : drone2Stats++;
            temp = drone3.visionRange == drone1.visionRange ? drone1Stats++ : drone2Stats++;
            temp = drone3.hungerMeter == drone1.hungerMeter ? drone1Stats++ : drone2Stats++;

            Assert.AreEqual(6, drone1Stats);
            Assert.AreEqual(5, drone2Stats);
            yield return new WaitForSeconds(0.1f);
        }

        [UnityTest]
        public IEnumerator Check_Faction2_Territory_Spawner_of_Drones()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject t2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Faction2"), new Vector3(0, 0, 0), Quaternion.identity);
            yield return new WaitForSeconds(3f);

            Assert.AreEqual(2, GameObject.FindObjectsOfType<Faction2>().Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Faction1_Territory_Spawner_of_Drones()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject t1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Faction1"), new Vector3(0, 0, 0), Quaternion.identity);
            yield return new WaitForSeconds(3f);

            Assert.AreEqual(2, GameObject.FindObjectsOfType<Faction1>().Length);
            yield return null;
        }


        [UnityTest]
        public IEnumerator Check_Drone2_Flock_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(0, 2, 0), Quaternion.identity);
            GameObject d3 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(3, 1, 0), Quaternion.identity);
            Faction2 drone1 = d1.GetComponent<Faction2>();
            Faction2 drone2 = d2.GetComponent<Faction2>();
            Faction2 drone3 = d3.GetComponent<Faction2>();
            drone1.flock = 100f;
            drone2.flock = 100f;
            drone3.flock = 100f;
            yield return new WaitForSeconds(1f);

            Assert.NotNull(drone1);
            Assert.NotNull(drone2);
            Assert.NotNull(drone3);
            Assert.AreEqual(Drone.BehaviorState.FLOCK, drone1.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.FLOCK, drone2.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.FLOCK, drone3.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone1_Flock_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 2, 0), Quaternion.identity);
            GameObject d3 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(3, 1, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            Faction1 drone2 = d2.GetComponent<Faction1>();
            Faction1 drone3 = d2.GetComponent<Faction1>();
            drone1.flock = 100f;
            drone2.flock = 100f;
            drone3.flock = 100f;
            yield return new WaitForSeconds(1f);

            Assert.NotNull(drone1);
            Assert.NotNull(drone2);
            Assert.NotNull(drone3);
            Assert.AreEqual(Drone.BehaviorState.FLOCK, drone1.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.FLOCK, drone2.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.FLOCK, drone3.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drones_Damage_Function()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(3, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            Faction2 drone2 = d2.GetComponent<Faction2>();
            drone1.seek = 100f;
            drone2.seek = 100f;
            drone1.health = 20f;
            drone2.health = 20f;
            float maxHealth1 = drone1.health;
            float maxHealth2 = drone2.health;

            yield return new WaitForSeconds(1.5f);
            Assert.NotNull(drone1);
            Assert.NotNull(drone2);
            Assert.AreEqual(Drone.BehaviorState.SEEK, drone1.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.SEEK, drone2.behaviorState);
            // Minimum damage is 5
            Assert.Greater(maxHealth1 - 5f, drone1.health);
            Assert.Greater(maxHealth2 - 5f, drone2.health);
            yield return null;
        }


        [UnityTest]
        public IEnumerator Check_Drones_Seek_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(3, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            Faction2 drone2 = d2.GetComponent<Faction2>();
            drone1.seek = 100f;
            drone2.seek = 100f;
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone1);
            Assert.NotNull(drone2);
            Assert.AreEqual(Drone.BehaviorState.SEEK, drone1.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.SEEK, drone2.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drones_Flee_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(1, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            Faction2 drone2 = d2.GetComponent<Faction2>();
            drone1.flee = 100f;
            drone2.flee = 100f;
            yield return new WaitForSeconds(0.3f);

            Assert.NotNull(drone1);
            Assert.AreEqual(Drone.BehaviorState.FLEE, drone1.behaviorState);
            Assert.AreEqual(Drone.BehaviorState.FLEE, drone2.behaviorState);
            yield return new WaitForSeconds(3f);
            Assert.AreNotEqual(Drone.BehaviorState.FLEE, drone1.behaviorState);
            Assert.AreNotEqual(Drone.BehaviorState.FLEE, drone2.behaviorState);
            yield return null;

        }

        [UnityTest]
        public IEnumerator Check_Drone2_Capture_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject uncapTerritory = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(3, 0, 0), Quaternion.identity);
            Faction2 drone2 = d2.GetComponent<Faction2>();
            drone2.name = "Faction2 Gen 0";
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone2);
            Assert.AreEqual(Drone.BehaviorState.CAPTURE, drone2.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone1_Capture_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject uncapTerritory = MonoBehaviour.Instantiate((GameObject)Resources.Load("Territory Uncaptured"), new Vector3(3, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            drone1.name = "Faction1 Gen 0";
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone1);
            Assert.AreEqual(Drone.BehaviorState.CAPTURE, drone1.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone2_Repair_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(-2, 0, 0), Quaternion.identity);
            Faction2 drone2 = d2.GetComponent<Faction2>();
            drone2.health = 10f;
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone2);
            Assert.AreEqual(Drone.BehaviorState.ARRIVE, drone2.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone1_Repair_Behavior_Priority()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(-2, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            drone1.health = 10f;
            yield return new WaitForSeconds(2f);

            Assert.NotNull(drone1);
            Assert.AreEqual(Drone.BehaviorState.ARRIVE, drone1.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone2_Wander_Behavior_Priority_When_Nothing_Is_Around()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }
            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(0, 0, 0), Quaternion.identity);
            Faction2 drone2 = d2.GetComponent<Faction2>();
            yield return new WaitForSeconds(1f);
            
            Assert.NotNull(drone2);
            // Based on the behavior ruleset, Drone is defaulted to wander when nothing is around
            Assert.AreEqual(Drone.BehaviorState.WANDER, drone2.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Drone1_Wander_Behavior_Priority_When_Nothing_Is_Around()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }
            foreach (Resource r in GameObject.FindObjectsOfType<Resource>())
            {
                GameObject.Destroy(r.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();
            yield return new WaitForSeconds(0.1f);
            
            Assert.NotNull(drone1);
            // Based on the behavior ruleset, Drone is defaulted to wander when nothing is around
            Assert.AreEqual(Drone.BehaviorState.WANDER, drone1.behaviorState);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Instantiation_Of_Faction_2_Drone()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(0, GameObject.FindObjectsOfType<Drone>().Length);

            GameObject d2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone2"), new Vector3(0, 0, 0), Quaternion.identity);
            Faction2 drone2 = d2.GetComponent<Faction2>();

            Assert.NotNull(drone2);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Instantiation_Of_Faction_1_Drone()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                GameObject.Destroy(territory.gameObject);
            }

            foreach (Drone d in GameObject.FindObjectsOfType<Drone>())
            {
                GameObject.Destroy(d.gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(0, GameObject.FindObjectsOfType<Drone>().Length);

            GameObject d1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Drone1"), new Vector3(0, 0, 0), Quaternion.identity);
            Faction1 drone1 = d1.GetComponent<Faction1>();

            Assert.NotNull(drone1);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_If_Sounds_Settings_Are_Saved_On_Scene_Transitions()
        {
            SceneManager.LoadScene("Menu");
            yield return null;
            
            AudioManager audioManager = GameObject.Find("AudioManager").transform.gameObject.GetComponent<AudioManager>();

            Assert.NotNull(audioManager);
            yield return null;

            GameObject settings = GameObject.Find("Canvas").transform.Find("Settings").gameObject;
            Slider sfxSlider = settings.transform.Find("SFXSlider").GetComponent<Slider>();
            Slider musicSlider = settings.transform.Find("MusicSlider").GetComponent<Slider>();
            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            GameObject background = mainMenu.transform.Find("MenuBG").gameObject;
            Button playButton = background.transform.Find("PlayButton").GetComponent<Button>();

            Assert.NotNull(mainMenu);
            Assert.NotNull(background);
            Assert.NotNull(playButton);
            Assert.NotNull(sfxSlider);
            Assert.NotNull(musicSlider);

            float menuSfxVol = sfxSlider.value;
            float menuMenuVol = musicSlider.value;
            yield return null;

            playButton.onClick.Invoke();
            yield return null;

            GameObject simCanvas = GameObject.Find("SimCanvas").transform.gameObject;
            MainMenu menu = simCanvas.transform.Find("Menu").gameObject.GetComponent<MainMenu>();
            GameObject panel = simCanvas.transform.Find("Panel").gameObject;
            GameObject simSettings = panel.transform.Find("Settings").gameObject;
            Slider simSfxSlider = simSettings.transform.Find("SFXSlider").GetComponent<Slider>();
            Slider simMusicSlider = simSettings.transform.Find("MusicSlider").GetComponent<Slider>();


            Assert.AreEqual(menuSfxVol, simSfxSlider.value);
            Assert.AreEqual(menuMenuVol, simMusicSlider.value);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_If_Drones_Are_In_Bounds()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            Time.timeScale = 3f;
            for (int x = 0; x < 12; x++)
            {
                yield return new WaitForSeconds(5f);
                foreach (Drone resource in GameObject.FindObjectsOfType<Drone>())
                {
                    Assert.LessOrEqual(-54, resource.transform.position.x);
                    Assert.GreaterOrEqual(54, resource.transform.position.x);

                    Assert.LessOrEqual(-30, resource.transform.position.y);
                    Assert.GreaterOrEqual(30, resource.transform.position.y);
                }
            }
            yield return null;
        }
        [UnityTest]
        public IEnumerator Check_If_Heal_Stations_Are_In_Bounds()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            Assert.AreEqual(5, GameObject.FindObjectsOfType<Resource>().Length);

            foreach (Resource resource in GameObject.FindObjectsOfType<Resource>())
            {
                Assert.LessOrEqual(-45, resource.transform.position.x);
                Assert.GreaterOrEqual(45, resource.transform.position.x);

                Assert.LessOrEqual(-22, resource.transform.position.y);
                Assert.GreaterOrEqual(22, resource.transform.position.y);
            }
        }

                [UnityTest]
        public IEnumerator Check_If_Territories_Are_In_Bounds()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            Assert.AreEqual(10, GameObject.FindObjectsOfType<GeneticAlgorithm>().Length);

            foreach (GeneticAlgorithm territory in GameObject.FindObjectsOfType<GeneticAlgorithm>())
            {
                Assert.LessOrEqual(-45, territory.transform.position.x);
                Assert.GreaterOrEqual(45, territory.transform.position.x);

                Assert.LessOrEqual(-22, territory.transform.position.y);
                Assert.GreaterOrEqual(22, territory.transform.position.y);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Simulation_Scene_Game_Objects()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            GameObject worldObject = GameObject.Find("World").transform.gameObject;
            World world = worldObject.GetComponent<World>();
            GameObject territories = GameObject.Find("Territories").transform.gameObject;
            GameObject resources = GameObject.Find("Resources").transform.gameObject;
            SpriteRenderer bg = GameObject.Find("Background").transform.gameObject.GetComponent<SpriteRenderer>();
            GameObject environment = GameObject.Find("Environment").transform.gameObject;
            GameObject borderColliders = GameObject.Find("BorderColliders").transform.gameObject;

            Assert.NotNull(worldObject);
            Assert.NotNull(world);
            Assert.NotNull(territories);
            Assert.NotNull(resources);
            Assert.NotNull(bg);
            Assert.NotNull(environment);
            Assert.NotNull(borderColliders);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Stats_UI_Slider()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            GameObject simCanvas = GameObject.Find("SimCanvas").transform.gameObject;
            GameObject sliderPanel = simCanvas.transform.Find("SliderPanel").gameObject;
            RectTransform rectTransform = simCanvas.transform.Find("SliderPanel").GetComponent<RectTransform>();
            GameObject stats = sliderPanel.transform.Find("Stats").gameObject;
            Button sliderButton = stats.transform.Find("SliderButton").GetComponent<Button>();

            Assert.NotNull(simCanvas);
            Assert.NotNull(sliderPanel);
            Assert.NotNull(stats);
            Assert.NotNull(sliderButton);
            Assert.NotNull(rectTransform);
            yield return null;

            Time.timeScale = 1f;
            sliderButton.onClick.Invoke();
            yield return new WaitForSeconds(1f);

            Assert.Greater(495, rectTransform.anchoredPosition.x);
            Assert.Less(485, rectTransform.anchoredPosition.x);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Simulation_Scene_Settings_Objects()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            GameObject simCanvas = GameObject.Find("SimCanvas").transform.gameObject;
            MainMenu menu = simCanvas.transform.Find("Menu").gameObject.GetComponent<MainMenu>();
            GameObject panel = simCanvas.transform.Find("Panel").gameObject;
            GameObject settings = panel.transform.Find("Settings").gameObject;
            Button settingsButton = panel.transform.Find("SettingsButton").GetComponent<Button>();
            
            Assert.IsFalse(settings.activeSelf);
            Assert.NotNull(menu);
            Assert.NotNull(settingsButton);

            settingsButton.onClick.Invoke();
            yield return null;

            Assert.IsTrue(settings.activeSelf);
            yield return null;

            Button returnButton = settings.transform.Find("ReturnButton").GetComponent<Button>();
            Button mainMenuButton = settings.transform.Find("MainMenuButton").GetComponent<Button>();
            Button quitButton = settings.transform.Find("QuitButton").GetComponent<Button>();
            Slider sfxSlider = settings.transform.Find("SFXSlider").GetComponent<Slider>();
            Slider musicSlider = settings.transform.Find("MusicSlider").GetComponent<Slider>();
            Assert.NotNull(returnButton);
            Assert.NotNull(mainMenuButton);
            Assert.NotNull(quitButton);
            Assert.NotNull(sfxSlider);
            Assert.NotNull(musicSlider);
            yield return null;

            returnButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(settings.activeSelf);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Simulation_Scene_Pause_And_FastForward_Buttons()
        {
            SceneManager.LoadScene("Simulation");
            yield return null;

            GameObject simCanvas = GameObject.Find("SimCanvas").transform.gameObject;
            GameObject panel = simCanvas.transform.Find("Panel").gameObject;
            Button pauseButton = panel.transform.Find("PauseButton").GetComponent<Button>();
            Button fastForwardButton = panel.transform.Find("FastForwardButton").GetComponent<Button>();
            yield return null;

            Assert.NotNull(simCanvas);
            Assert.NotNull(panel);
            Assert.NotNull(pauseButton);
            Assert.NotNull(fastForwardButton);
            Assert.AreEqual(1, Time.timeScale);
            yield return null;

            pauseButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(0, Time.timeScale);
            yield return null;

            fastForwardButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(0, Time.timeScale);
            yield return null;

            pauseButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(1, Time.timeScale);
            yield return null;

            fastForwardButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, Time.timeScale);
            yield return null;

            fastForwardButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(3, Time.timeScale);
            yield return null;

            pauseButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(0, Time.timeScale);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Scene_Transition_From_Menu_To_Simulation_To_Menu()
        {
            SceneManager.LoadScene("Menu");
            yield return null;

            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            GameObject background = mainMenu.transform.Find("MenuBG").gameObject;
            Button playButton = background.transform.Find("PlayButton").GetComponent<Button>();

            Assert.NotNull(mainMenu);
            Assert.NotNull(background);
            Assert.NotNull(playButton);
            yield return null;

            playButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual("Simulation", SceneManager.GetActiveScene().name);

            GameObject simCanvas = GameObject.Find("SimCanvas").transform.gameObject;
            GameObject panel = simCanvas.transform.Find("Panel").gameObject;
            Button settingsButton = panel.transform.Find("SettingsButton").GetComponent<Button>();

            Assert.NotNull(simCanvas);
            Assert.NotNull(panel);
            Assert.NotNull(settingsButton);
            yield return null;

            settingsButton.onClick.Invoke();
            yield return null;

            GameObject settings = panel.transform.Find("Settings").gameObject;
            Button mainMenuButton = settings.transform.Find("MainMenuButton").GetComponent<Button>();

            Assert.NotNull(mainMenuButton);
            yield return null;

            mainMenuButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual("Menu", SceneManager.GetActiveScene().name);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Main_Menu_Scene_Transition_To_Simulation()
        {
            SceneManager.LoadScene("Menu");
            yield return null;

            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            GameObject background = mainMenu.transform.Find("MenuBG").gameObject;
            Button playButton = background.transform.Find("PlayButton").GetComponent<Button>();
            playButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual("Simulation", SceneManager.GetActiveScene().name);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Audio_Manager_Object_Transition_From_Menu_To_Simulation_Scenes()
        {
            SceneManager.LoadScene("Menu");
            yield return null;

            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            GameObject background = mainMenu.transform.Find("MenuBG").gameObject;
            Button playButton = background.transform.Find("PlayButton").GetComponent<Button>();

            playButton.onClick.Invoke(); 
            yield return null;

            AudioManager audioManager = GameObject.Find("AudioManager").transform.gameObject.GetComponent<AudioManager>();

            Assert.NotNull(audioManager);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Audio_Manager_Object()
        {
            SceneManager.LoadScene("Menu");
            yield return null;

            AudioManager audioManager = GameObject.Find("AudioManager").transform.gameObject.GetComponent<AudioManager>();

            Assert.NotNull(audioManager);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Main_Menu_Scene_Settings_Objects()
        {
            SceneManager.LoadScene("Menu");
            yield return null;

            GameObject settings = GameObject.Find("Canvas").transform.Find("Settings").gameObject;
            Slider sfxSlider = settings.transform.Find("SFXSlider").GetComponent<Slider>();
            Slider musicSlider = settings.transform.Find("MusicSlider").GetComponent<Slider>();
            Button playButton = settings.transform.Find("ReturnButton").GetComponent<Button>();

            Assert.IsFalse(settings.activeSelf);
            Assert.NotNull(settings);
            Assert.NotNull(sfxSlider);
            Assert.NotNull(musicSlider);
            Assert.NotNull(playButton);
            yield return null;
        }


        [UnityTest]
        public IEnumerator Check_Main_Menu_Scene_Objects()
        {
            SceneManager.LoadScene("Menu");
            yield return null;

            GameObject background = GameObject.Find("Background").transform.gameObject;
            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            GameObject menuBG = mainMenu.transform.Find("MenuBG").gameObject;
            Button playButton = menuBG.transform.Find("PlayButton").GetComponent<Button>();
            Button settingsButton = menuBG.transform.Find("SettingsButton").GetComponent<Button>();
            Button quitButton = menuBG.transform.Find("QuitButton").GetComponent<Button>();

            Assert.NotNull(background);
            Assert.NotNull(mainMenu);
            Assert.IsTrue(mainMenu.activeSelf);
            Assert.NotNull(menuBG);
            Assert.NotNull(playButton);
            Assert.NotNull(settingsButton);
            Assert.NotNull(quitButton);
            yield return null;
        }

        // ---------------------------------------------------
        [UnityTest]
        public IEnumerator Check_Test_Object_Position()
        {
            GameObject droneObject = new GameObject("Drone");
            yield return null;

            Assert.AreEqual(new Vector3(0, 0, 0), testObject.transform.position);
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(testObject);
        }
    }
}
