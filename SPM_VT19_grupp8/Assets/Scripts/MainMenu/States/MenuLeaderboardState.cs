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

    public override void Initialize(StateMachine owner)
    {
        Menu = ((MainMenuStateMachine)owner).LeaderboardState;

        foreach(Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        Buttons["LeaderboardBack"].onClick.AddListener(Back);



        Text[] leaderboardEntries = Menu.transform.Find("Scores").GetComponentsInChildren<Text>();


        Dictionary<string, float> playerScores = ScoreSaveLoad.LoadScores();
        SortedSet<PlayerTime> playerTimes = new SortedSet<PlayerTime>();



        //foreach (Text t in Menu.transform.Find("Scores").GetComponentsInChildren<Text>())
        //{

        //}






        base.Initialize(owner);



    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(Buttons["LeaderboardBack"].gameObject);
    }
}
