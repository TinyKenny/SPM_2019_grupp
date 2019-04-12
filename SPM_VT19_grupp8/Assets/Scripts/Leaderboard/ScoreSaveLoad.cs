using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class ScoreSaveLoad
{
    public static void SaveScore(string playerName, float playerScore)
    {
        Dictionary<string, float> scores = LoadScores();

        if (!scores.ContainsKey(playerName) || scores[playerName] > playerScore)
        {
            scores[playerName] = playerScore;

            BinaryFormatter formatter = new BinaryFormatter();
            string path = "Assets/highscores.bin";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, scores);
            stream.Close();
        }
    }

    public static Dictionary<string, float> LoadScores()
    {
        string path = "Assets/highscores.bin";
        Dictionary<string, float> scores = new Dictionary<string, float>();
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            scores = formatter.Deserialize(stream) as Dictionary<string, float>;
            stream.Close();
        }
        else
        {
            Debug.LogWarning("SaveFile not found");
        }
        return scores;
    }
}
