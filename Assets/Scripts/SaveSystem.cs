using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    // straight outta brackeys

    public static void SaveProgress(PlayerProgression prog)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/progression.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerProgressionData data = new PlayerProgressionData(prog);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static PlayerProgressionData LoadProgress()
    {
        string path = Application.persistentDataPath + "/progression.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerProgressionData data = (PlayerProgressionData) formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save path not found!");
            return null;
        }
    }
}
