using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SaveMissionIndex(int currentMissionIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/MissionData.epicenter";
        FileStream stream = new FileStream(path, FileMode.Create);

        MissionData data = new MissionData(currentMissionIndex);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static MissionData LoadMissionIndex()
    {
        string path = Application.persistentDataPath + "/MissionData.epicenter";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MissionData data = formatter.Deserialize(stream) as MissionData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
