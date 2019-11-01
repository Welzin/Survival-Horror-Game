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
        dialog.SetActive(true);
    }

    public void Ok()
    {
        options.SetActive(false);
        dialog.SetActive(false);
        home.SetActive(true);
    }
    
    public void Cancel()
    {
        dialog.SetActive(false);
    }

    public string scene;
    public GameObject options;
    public GameObject home;
    public GameObject dialog;
}