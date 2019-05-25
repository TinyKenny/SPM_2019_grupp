using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardController : MonoBehaviour
{
    public Text[] leaderboardEntries = new Text[10];

    private Dictionary<string, float> playerScores;
    private SortedSet<PlayerTime> playerTimes;

    private class PlayerTime : IComparable<PlayerTime>
    {
        public string name;
        public float time;

        public PlayerTime(string name, float time)
        {
            this.name = name;
            this.time = time;
        }

        public int CompareTo(PlayerTime other)
        {
            return time.CompareTo(other.time);
        }

        public int GetTime()
        {
            return (int)time;
        }
    }
    
    void Start()
    {
        playerScores = ScoreSaveLoad.LoadScores();
        playerTimes = new SortedSet<PlayerTime>();

        foreach(KeyValuePair<string, float> kvp in playerScores)
        {
            PlayerTime pt = new PlayerTime(kvp.Key, kvp.Value);
            playerTimes.Add(pt);
        }

        int textIndex = 0;

        foreach(PlayerTime pt in playerTimes)
        {
            leaderboardEntries[textIndex].text = pt.name + ": " + pt.GetTime();
            textIndex++;
            if(textIndex > 9)
            {
                break;
            }
        }

        for(; textIndex < 10; textIndex++)
        {
            leaderboardEntries[textIndex].text = " ";
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        SaveFile.ClearSave();
    }
}
