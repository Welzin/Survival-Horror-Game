using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtons : MonoBehaviour
{
    private void Start()
    {
        MusicManager m = FindObjectOfType<MusicManager>();
        // Plays first music of the manager when starting
        m.UpdateMusic(0);
        m.PlayLoop();
    }

    public void Play()
    {
        FindObjectOfType<MusicManager>().StopMusic();
        SceneManager.LoadScene(scene);
    }

    public void ShowOptions()
    {
        home.SetActive(false);
        options.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoBack()
    {
        options.SetActive(false);
        home.SetActive(true);
    }

    public string scene;
    public GameObject options;
    public GameObject home;
}