using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    public static void SaveGame(SaveData data) {

        Debug.Log("セーブ開始");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/SaveData.hsr";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("セーブ完了");
    }

    public static SaveData LoadGame() {

        string path = Application.persistentDataPath + "/SaveData.hsr";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else {
            Debug.Log("そんなファイル無いですううううう！！" + path);
            return null;
        }


    }

}