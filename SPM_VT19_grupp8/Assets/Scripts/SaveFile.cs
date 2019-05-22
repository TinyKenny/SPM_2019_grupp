using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveFile
{
    public Dictionary<string, PositionInfo> EnemyInfoList
    {
        get
        {
            if (enemyInfoList == null)
                enemyInfoList = new Dictionary<string, PositionInfo>();
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

    public PositionInfo PlayerPosition { get; set; }
    public float PlayerRotationY { get; set; }
    public int LevelIndex { get; set; }

    private PositionInfo playerRotation;
    private Dictionary<string, PositionInfo> enemyInfoList;
    private Dictionary<string, bool> ammoPickupList;

    public void AddEnemy(Vector3 position, string name)
    {
        EnemyInfoList[name] = new PositionInfo(position);
    }

    public void RemoveEnemy(string name)
    {
        EnemyInfoList.Remove(name);
    }

    public void AddAmmoPickup(string name, bool active)
    {
        AmmmoPickupList[name] = active;
    }

    public void RemoveAmmoPickup(string name)
    {
        AmmmoPickupList.Remove(name);
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
