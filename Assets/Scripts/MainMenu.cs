using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool pauseToggle = false;
    public bool settingsToggle = false;

    public GameObject pause;
    public GameObject play;
    public GameObject settings;

    public Slider sfx;
    public Slider music;
    public AudioManager audioManager;

    [Range(0f,0.2f)]
    public float volume = 0.1f;
    private void Start()
    {
        if (GameObject.Find("AudioManager"))
        { 
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            settings.SetActive(true);
            audioManager.RefreshAudioManager();
            settings.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowSettings();
        }
    }

    public void StartSimulation()
    {
        SceneManager.LoadScene("Simulation");
        Time.timeScale = 1f;
        audioManager.Play("Theme");
        audioManager.ToggleSceneBool();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        if (!pauseToggle)
        {
            Time.timeScale = 0;
            pauseToggle = true;
            pause.SetActive(false);
            play.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseToggle = false;
            pause.SetActive(true);
            play.SetActive(false);
        }
    }

    public void FastForward()
    {
        if (Time.timeScale <= 3 && Time.timeScale != 0)
        {
            Time.timeScale += 1;
        }   
    }

    public void ShowSettings()
    {
        if (!settingsToggle)
        {
            settings.SetActive(true);
            settingsToggle = !settingsToggle;
        }
        else
        {
            settings.SetActive(false);
            settingsToggle = !settingsToggle;
        }
    }

    public void Return()
    {
        if (settingsToggle)
        {
            settings.SetActive(false);
            settingsToggle = !settingsToggle;
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
        audioManager.ToggleSceneBool();
        audioManager.Play("Menu");
    }
}
