using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveFile
{
    public List<EnemyInfo> EnemyInfoList
    {
        get
        {
            if (enemyInfoList == null)
                enemyInfoList = new List<EnemyInfo>();
            return enemyInfoList;
        }
        private set
        {
            enemyInfoList = value;
        }
    }
    private List<EnemyInfo> enemyInfoList;

    public void AddEnemy(Vector3 position, float health)
    {
        EnemyInfoList.Add(new EnemyInfo(position, health));
    }

    public void RemoveEnemy(Vector3 position, float health)
    {
        EnemyInfoList.Remove(new EnemyInfo(position, health));
    }

    public static void ClearSave()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            File.Delete(Application.persistentDataPath + "/gamesave.save");
        }

    }
}

[System.Serializable]
public class EnemyInfo
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
    public float Health { get; private set; }

    private float x;
    private float y;
    private float z;

    public EnemyInfo(Vector3 position, float health)
    {
        Health = health;
        Position = position;
    }
}
