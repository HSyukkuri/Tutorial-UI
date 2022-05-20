using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem{
    
    public static void SaveGame(SaveData data) {

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/SaveData.hsr";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static SaveData LoadGame() {

        string path = Application.persistentDataPath + "/SaveData.hsr";
        Debug.Log(path);

        if (File.Exists(path)) {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;

        }
        else {
            Debug.Log("そんなファイルないですうううううう！！" + path);
            return null;
        }


    }

}
