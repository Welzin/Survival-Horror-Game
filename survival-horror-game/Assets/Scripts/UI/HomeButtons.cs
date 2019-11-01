using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtons : MonoBehaviour
{
    public void Play()
    {
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
        home.SetActive(true);
        options.SetActive(false);
    }

    public string scene;
    public GameObject options;
    public GameObject home;
}
