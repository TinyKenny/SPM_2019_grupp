using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveFile
{
    public Dictionary<int, EnemyInfo> EnemyInfoList
    {
        get
        {
            if (enemyInfoList == null)
                enemyInfoList = new Dictionary<int, EnemyInfo>();
            return enemyInfoList;
        }
        private set
        {
            enemyInfoList = value;
        }
    }

    public Dictionary<int, bool> AmmmoPickupList
    {
        get
        {
            if (ammoPickupList == null)
                ammoPickupList = new Dictionary<int, bool>();
            return ammoPickupList;
        }
        private set
        {
            ammoPickupList = value;
        }
    }

    public Dictionary<int, bool> CheckpointPickupList
    {
        get
        {
            if (checkpointPickupList == null)
                checkpointPickupList = new Dictionary<int, bool>();
            return checkpointPickupList;
        }
        private set
        {
            checkpointPickupList = value;
        }
    }

    public List<ProjectileInfo> Projectiles
    {
        get
        {
            if (projectiles == null)
                projectiles = new List<ProjectileInfo>();
            return projectiles;
        }
        private set
        {
            projectiles = value;
        }
    }

    public int LevelIndex { get; set; }
    public bool IsEmpty { get; private set; }
    public PlayerVariables PlayerInfo { get; set; }
    public float LevelTime { get; set; } = 0f;
    public bool CheckpointReached { get; private set; } = false;

    private PositionInfo playerRotation;
    private Dictionary<int, EnemyInfo> enemyInfoList;
    private Dictionary<int, bool> ammoPickupList;
    private Dictionary<int, bool> checkpointPickupList;
    private List<ProjectileInfo> projectiles;

    public SaveFile()
    {
        IsEmpty = true;
    }

    public void AddEnemy(Vector3 position, Vector3 rotation, Vector3 lastPlayerLocation, int ID, int state, int currentPatrolPointIndex)
    {
        EnemyInfoList[ID] = new EnemyInfo(position, rotation, lastPlayerLocation, state, currentPatrolPointIndex);
        if (IsEmpty)
            IsEmpty = false;
    }

    public void RemoveEnemy(int ID)
    {
        EnemyInfoList.Remove(ID);
    }

    public void AddAmmoPickup(int ID, bool active)
    {
        AmmmoPickupList[ID] = active;
        if (IsEmpty)
            IsEmpty = false;
    }

    public void AddCheckpoint(int ID, bool active)
    {
        CheckpointPickupList[ID] = active;
        if (IsEmpty)
            IsEmpty = false;
        CheckpointReached = true;
    }

    public void RemoveAmmoPickup(int ID)
    {
        AmmmoPickupList.Remove(ID);
    }

    public void RemoveCheckpoint(int ID)
    {
        CheckpointPickupList.Remove(ID);
    }

    public void AddPlayerInfo(Vector3 position, float yRotation, int ammo, float shield, float timeSlowEnergy, float shieldCooldown)
    {
        PlayerInfo = new PlayerVariables(position, yRotation, ammo, shield, timeSlowEnergy, shieldCooldown);
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

[System.Serializable]
public class EnemyInfo
{
    public PositionInfo Position { get; private set; }
    public PositionInfo Rotation { get; private set; }
    public int CurrentState { get; private set; }
    public int CurrentPatrolPointIndex { get; private set; }
    public PositionInfo LastPlayerLocation { get; private set; }

    public EnemyInfo(Vector3 position, Vector3 rotation, Vector3 lastPlayerLocation, int currentState, int currentPatrolPointIndex)
    {
        Position = new PositionInfo(position);
        Rotation = new PositionInfo(rotation);
        LastPlayerLocation = new PositionInfo(lastPlayerLocation);
        CurrentState = currentState;
        CurrentPatrolPointIndex = currentPatrolPointIndex;
    }
}

[System.Serializable]
public class ProjectileInfo
{
    public PositionInfo Position { get; set; }
    public PositionInfo Rotation { get; set; }

    public ProjectileInfo(Vector3 position, Vector3 rotation)
    {
        Position = new PositionInfo(position);
        Rotation = new PositionInfo(rotation);
    }
}
