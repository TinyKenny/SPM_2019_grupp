﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveFile
{
    public Dictionary<string, System.Tuple<PositionInfo, PositionInfo, int>> EnemyInfoList
    {
        get
        {
            if (enemyInfoList == null)
                enemyInfoList = new Dictionary<string, System.Tuple<PositionInfo, PositionInfo, int>>();
            return enemyInfoList;
        }
        private set
        {
            enemyInfoList = value;
        }
    }

    public Dictionary<string, bool> AmmmoPickupList
    {
        get
        {
            if (ammoPickupList == null)
                ammoPickupList = new Dictionary<string, bool>();
            return ammoPickupList;
        }
        private set
        {
            ammoPickupList = value;
        }
    }

    public Dictionary<string, bool> CheckpointPickupList
    {
        get
        {
            if (checkpointPickupList == null)
                checkpointPickupList = new Dictionary<string, bool>();
            return checkpointPickupList;
        }
        private set
        {
            checkpointPickupList = value;
        }
    }

    public int LevelIndex { get; set; }
    public bool IsEmpty { get; private set; }
    public PlayerVariables PlayerInfo { get; set; }
    public float LevelTime { get; set; } = 0f;

    private PositionInfo playerRotation;
    private Dictionary<string, System.Tuple<PositionInfo, PositionInfo, int>> enemyInfoList;
    private Dictionary<string, bool> ammoPickupList;
    private Dictionary<string, bool> checkpointPickupList;

    public SaveFile()
    {
        IsEmpty = true;
    }

    public void AddEnemy(Vector3 position, Vector3 rotation, string name, int state)
    {
        EnemyInfoList[name] = System.Tuple.Create(new PositionInfo(position), new PositionInfo(rotation), state);
        if (IsEmpty)
            IsEmpty = false;
    }

    public void RemoveEnemy(string name)
    {
        EnemyInfoList.Remove(name);
    }

    public void AddAmmoPickup(string name, bool active)
    {
        AmmmoPickupList[name] = active;
        if (IsEmpty)
            IsEmpty = false;
    }

    public void AddCheckpoint(string name, bool active)
    {
        CheckpointPickupList[name] = active;
        if (IsEmpty)
            IsEmpty = false;
    }

    public void RemoveAmmoPickup(string name)
    {
        AmmmoPickupList.Remove(name);
    }

    public void RemoveCheckpoint(string name)
    {
        CheckpointPickupList.Remove(name);
    }

    public void AddPlayerInfo(Vector3 position, float yRotation, int ammo, float shield, float timeSlowEnergy)
    {
        PlayerInfo = new PlayerVariables(position, yRotation, ammo, shield, timeSlowEnergy);
        if (IsEmpty)
            IsEmpty = false;
    }

    public static void ClearSave()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            File.Delete(Application.persistentDataPath + "/gamesave.save");
        }

    }

    public static void CreateSave()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, GameController.GameControllerInstance.CurrentSave);
        file.Close();
    }

    public static void LoadSave()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            GameController.GameControllerInstance.CurrentSave = (SaveFile)bf.Deserialize(file);
            file.Close();
        }
    }

    public static int GetContinueLevelBuildindex()
    {
        int levelIndex = 0;
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            SaveFile sF = (SaveFile)bf.Deserialize(file);
            levelIndex = sF.LevelIndex;
            file.Close();
        }
        return levelIndex;
    }

    public static void SaveGame()
    {
        ClearSave();

        CreateSave();
    }

    public void FinishLevel(float levelTime, int ammoAmount)
    {
        IsEmpty = true;
        PlayerInfo.AmmoAmount = ammoAmount;
        PlayerInfo.SpawnPosition = null;
        enemyInfoList.Clear();
        ammoPickupList.Clear();
        LevelTime = levelTime;

        SaveGame();
    }
}

[System.Serializable]
public class PositionInfo
{
    public Vector3 Position
    {
        private set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
        get
        {
            return new Vector3(x, y, z);
        }
    }

    private float x;
    private float y;
    private float z;

    public PositionInfo(Vector3 position)
    {
        Position = position;
    }
}
