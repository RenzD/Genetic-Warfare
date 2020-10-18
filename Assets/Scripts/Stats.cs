using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public Text wanderText;
    public Text seekText;
    public Text fleeText;
    public Text flockText;
    public Text repairText;
    public Text captureText;
    public Text maxHealthText;
    public Text currentHealthText;
    public Text attackText;
    public Text speedText;
    public Text visionRangeText;
    public Text hungerText;
    public Text fitnessScoreText;

    public Text teamText;
    public Text typeText;
    public Text genText;

    World world;
    CameraController cam;
    private void Start()
    {
        world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    public void SetStats(float wander, float seek, float flee, float flock, float repair, float capture, 
                float maxHealth, float currentHealth, float attack, float speed, float visionRange, float hunger, float fitnessScore)
    {
        wanderText.text = wander.ToString();
        seekText.text = seek.ToString();
        fleeText.text = flee.ToString();
        flockText.text = flock.ToString();
        repairText.text = repair.ToString();
        captureText.text = capture.ToString();

        fitnessScoreText.text = fitnessScore.ToString();
        maxHealthText.text = maxHealth.ToString();
        currentHealthText.text = currentHealth.ToString();
        attackText.text = attack.ToString();
        speedText.text = speed.ToString();
        visionRangeText.text = visionRange.ToString();

        int h = (int) (hunger * 100);
        hungerText.text = h.ToString() + "%";
    }

    public void SetHeader(string team, string type, string gen) 
    {
        teamText.text = team;
        typeText.text = type;
        genText.text = "Gen #" + gen;
    }

    public void UpdateDroneStats(float health, float fitnessScore)
    {
        currentHealthText.text = health.ToString();
        fitnessScoreText.text = fitnessScore.ToString();
    }

    public void SetFaction1Parent1Stats()
    {
        if (world.Faction1FirstParent.fitnessScore != 0)
        {
            World.DroneAttributes drone = world.Faction1FirstParent;
            SetTexts(drone, "red");
        }
    }

    public void SetFaction1Parent2Stats()
    {
        if (world.Faction1SecondParent.fitnessScore != 0)
        {
            World.DroneAttributes drone = world.Faction1SecondParent;
            SetTexts(drone, "red");
        }
    }

    public void SetFaction2Parent1Stats()
    {
        if (world.Faction2FirstParent.fitnessScore != 0)
        {
            World.DroneAttributes drone = world.Faction2FirstParent;
            SetTexts(drone, "blue");

        }
    }

    public void SetFaction2Parent2Stats()
    {
        if (world.Faction2SecondParent.fitnessScore != 0)
        {
            World.DroneAttributes drone = world.Faction2SecondParent;
            SetTexts(drone, "blue");
        }
    }



    public void SetTexts(World.DroneAttributes drone, string team)
    {
        cam.parentBool = true;
        wanderText.text = drone.wander.ToString();
        seekText.text = drone.seek.ToString();
        fleeText.text = drone.flee.ToString();
        flockText.text = drone.flock.ToString();
        repairText.text = drone.arrive.ToString();
        captureText.text = drone.capture.ToString();

        fitnessScoreText.text = ((float)Math.Round(drone.fitnessScore, 2)).ToString();
        maxHealthText.text = drone.maxHealth.ToString();
        currentHealthText.text = "-";//((float)Math.Round(drone.health, 2)).ToString();
        attackText.text = drone.attack.ToString();
        speedText.text = drone.speed.ToString();
        visionRangeText.text = drone.visionRange.ToString();

        int h = (int)(drone.hungerMeter * 100);
        hungerText.text = h.ToString() + "%";

        SetHeader(team, "parent", drone.generation);
    }
}
