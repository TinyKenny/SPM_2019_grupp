using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[CreateAssetMenu(menuName =  "States/Menu/Main/Leaderboard State")]
public class MenuLeaderboardState : MenuBaseState
{
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

    // not sure if im actually going to use this
    private class LeaderboardTextLine : IComparable<LeaderboardTextLine>
    {
        public Text textLine;

        public LeaderboardTextLine(Text text)
        {
            textLine = text;
        }

        public int CompareTo(LeaderboardTextLine other)
        {
            return other.textLine.transform.position.y.CompareTo(textLine.transform.position.y);
        }
    }

    public override void Initialize(StateMachine owner)
    {
        Menu = ((MainMenuStateMachine)owner).LeaderboardState;

        foreach(Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }
        Buttons["LeaderboardBack"].onClick.AddListener(Back);

        UpdateLeaderboard();

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(Buttons["LeaderboardBack"].gameObject);
    }

    private void UpdateLeaderboard()
    {
        Text[] leaderboardEntries = Menu.transform.Find("Scores").GetComponentsInChildren<Text>();
        SortedSet<LeaderboardTextLine> sortedLeaderboardEntries = new SortedSet<LeaderboardTextLine>();

        foreach(Text t in leaderboardEntries)
        {
            sortedLeaderboardEntries.Add(new LeaderboardTextLine(t));
        }

        Dictionary<string, float> playerScores = ScoreSaveLoad.LoadScores();
        SortedSet<PlayerTime> playerTimes = new SortedSet<PlayerTime>();

        foreach (KeyValuePair<string, float> kvp in playerScores)
        {
            PlayerTime pt = new PlayerTime(kvp.Key, kvp.Value);
            playerTimes.Add(pt);
        }

        foreach (PlayerTime pt in playerTimes)
        {
            if(sortedLeaderboardEntries.Count <= 0)
            {
                break;
            }
            LeaderboardTextLine textEntry = sortedLeaderboardEntries.Min;
            sortedLeaderboardEntries.Remove(textEntry);

            textEntry.textLine.text = pt.name + ": " + pt.GetTime();
        }

        while(sortedLeaderboardEntries.Count > 0)
        {
            LeaderboardTextLine textEntry = sortedLeaderboardEntries.Min;
            sortedLeaderboardEntries.Remove(textEntry);
            textEntry.textLine.text = " ";
        }
    }
}
