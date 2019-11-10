using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void Start()
    {
        MusicManager m = FindObjectOfType<MusicManager>();
        m.UpdateMusic(1);
        m.PlayLoop();
    }

    public void GetBackToMenu()
    {
        FindObjectOfType<MusicManager>().StopMusic();
        SceneManager.LoadScene(menu);
    }

    public void HereWeGoAgain()
    {
        FindObjectOfType<MusicManager>().StopMusic();
        SceneManager.LoadScene(game);
    }

    public string menu;
    public string game;
}
