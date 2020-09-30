using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class PlayModeTest
    {
        [UnityTest]
        public IEnumerator Check_Initial_Main_Menu_State()
        {
            SceneManager.LoadScene("Menu");
            Time.timeScale = 1f;
            yield return new WaitForFixedUpdate();

            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            Assert.NotNull(mainMenu);
            Assert.IsTrue(mainMenu.activeSelf);
            Assert.AreEqual(2, mainMenu.GetComponentsInChildren<Button>().Length);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Check_Main_Menu_Transition_To_Simulation()
        {
            SceneManager.LoadScene("Menu");
            Time.timeScale = 1f;
            yield return new WaitForFixedUpdate();

            GameObject mainMenu = GameObject.Find("Canvas").transform.Find("MainMenu").gameObject;
            Button startButton = mainMenu.transform.Find("StartButton").GetComponent<Button>();
            startButton.onClick.Invoke();
            yield return new WaitForSecondsRealtime(.01f);

            Assert.AreEqual("Simulation", SceneManager.GetActiveScene().name);
            yield return null;
        }
    }
}
