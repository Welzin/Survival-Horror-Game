using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        _cinematics = GetComponentsInChildren<Cinematic>();
        _missions = GetComponentsInChildren<Mission>();
        _playerManager = FindObjectOfType<PlayerManager>();
        StartCoroutine(ManageLevelOverTime());
    }

    public bool CinematicStarted()
    {
        foreach (Cinematic cinematic in _cinematics)
        {
            if (cinematic.isLaunched)
                return true;
        }

        return false;
    }

    private void StartCinematic(string name)
    {
        if (CinematicStarted())
        {
            Debug.LogWarning("A cinematic is already playing");
            return;
        }

        foreach(Cinematic cinematic in _cinematics)
        {
            if (cinematic.levelObjectName == name)
            {
                cinematic.Launch();
                return;
            }
        }

        Debug.LogWarning("The cinematic with name \"" + name + "\" doesn't exists");
    }

    private void StartMission(string name)
    {
        foreach(Mission mission in _missions)
        {
            if (mission.levelObjectName == name)
            {
                if (!mission.isLaunched)
                    mission.Launch();
                else
                    Debug.LogWarning("This mission is already started");

                return;
            }
        }

        Debug.LogWarning("The mission with name \"" + name + "\" doesn't exists");
    }

    private IEnumerator ManageLevelOverTime()
    {
        MusicManager m = FindObjectOfType<MusicManager>();
        DontDestroyOnLoad dd = FindObjectOfType<DontDestroyOnLoad>();
        m.UpdateMusic(0);
        m.PlayLoop(2);

        // Start in the bed
        _playerManager.transform.position = initialPosition;

        // We play the intro cinematic
        yield return null;
        
        if (!dd.TutorialDone)
        {
            StartCinematic("Intro");
            yield return WaitEndCinematic();

            StartMission("Tutorial");
            yield return WaitEndMission("Tutorial");
            dd.TutorialDone = true;
        }
        else
        {
            FindObjectOfType<Intro>().ToDoIfNoIntro();
            FindObjectOfType<Tutorial>().ToDoIfNoIntro();
        }

        // Reset all to begin values
        _playerManager.AddStress(-_playerManager.maxStress);
        _playerManager.lamp.actualBattery = _playerManager.lamp.maxBattery;
        _playerManager.hud.batteryBar.ChangeBatteryPercentage(100);

        StartMission("First mission");
    }

    private IEnumerator WaitEndCinematic()
    {
        while (CinematicStarted())
        {
            yield return null;
        }
    }

    private IEnumerator WaitEndMission(string name)
    {
        foreach (Mission mission in _missions)
        {
            if (mission.levelObjectName == name)
            {
                while (mission.isLaunched)
                {
                    yield return null;
                }

                break;
            }
        }
    }

    private Cinematic[] _cinematics;
    private Mission[] _missions;
    private PlayerManager _playerManager;

    public Vector3 initialPosition;
    public List<GameObject> elementsToHideOnCinematic;
}