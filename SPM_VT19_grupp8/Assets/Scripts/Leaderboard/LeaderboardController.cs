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
        foreach(KeyValuePair<string, float> kvp in playerScores)
        {
            PlayerTime pt = new PlayerTime(kvp.Key, kvp.Value);
            playerTimes.Add(pt);
        }

        SortedSet<PlayerTime>.Enumerator enumerator = playerTimes.GetEnumerator();
        bool hasNext = playerTimes.Count > 0;

        for(int i = 0; i < leaderboardEntries.Length && playerTimes.Count > 0; i++)
        {
            string entryText = " ";
            if (hasNext) {
                PlayerTime pt = enumerator.Current;
                hasNext = enumerator.MoveNext();

                entryText = pt.name + ": " + pt.GetTime();
            }
            leaderboardEntries[i].text = entryText;
        }


    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
