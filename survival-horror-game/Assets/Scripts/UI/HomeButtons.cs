using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtons : MonoBehaviour
{
    private void Start()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        ShowHome();

        // Plays first music of the manager when starting
        MusicManager m = FindObjectOfType<MusicManager>();
        m.UpdateMusic(0);
        m.PlayLoop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_dd.GetKey(Controls.Pause).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Pause).Item2))
        {
            if (options.activeSelf)
            {
                ShowHome();
                return;
            }
            if (startChoice.activeSelf)
            {
                ShowHome();
                return;
            }
        }
    }

    public void PlayWithTuto()
    {
        _dd.TutorialDone = false;
        Play();
    }

    public void PlayWithoutTuto()
    {
        _dd.TutorialDone = true;
        Play();
    }

    private void Play()
    {
        FindObjectOfType<MusicManager>().StopMusic();
        SceneManager.LoadScene(scene);
    }

    public void ShowOptions()
    {
        home.SetActive(false);
        options.SetActive(true);
        startChoice.SetActive(false);
    }

    public void ShowHome()
    {
        options.SetActive(false);
        home.SetActive(true);
        startChoice.SetActive(false);
    }

    public void ShowStartChoice()
    {
        options.SetActive(false);
        home.SetActive(false);
        startChoice.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public string scene;
    public GameObject options;
    public GameObject home;
    public GameObject startChoice;

    private DontDestroyOnLoad _dd;
}