using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    void Start()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        ShowMainMenu();
        menu.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_dd.GetKey(Controls.Pause).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Pause).Item2))
        {
            if (optionsMenu.activeSelf)
            {
                ShowMainMenu();
            }
            else if (restartMenu.activeSelf)
            {
                ShowMainMenu();
            }
            else if (menu.gameObject.activeSelf)
            {
                ShowMenu(false);
            }
            else
            {
                ShowMenu(true);
            }
        }
    }

    public void RestartWithTutorial()
    {
        _dd.TutorialDone = false;
        Restart();
    }

    public void RestartWithoutTutorial()
    {
        _dd.TutorialDone = true;
        Restart();
    }

    private void Restart()
    {
        ShowMenu(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowOptions()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
        restartMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        restartMenu.SetActive(false);
    }

    public void ShowRestartChoice()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(false);
        restartMenu.SetActive(true);
    }

    public void ShowMenu(bool show)
    {
        if (show)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        _dd.GamePause = show;
        menu.gameObject.SetActive(show);
        hud.gameObject.SetActive(!show);
    }

    public void GoBackToHome()
    {
        ShowMenu(false);
        SceneManager.LoadScene(homeScene);
    }

    public Canvas menu;
    public Canvas hud;
    public GameObject restartMenu;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public string homeScene;

    private DontDestroyOnLoad _dd;
}
