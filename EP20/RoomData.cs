using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoomOverride {
    public RoomData targetRoomData;
    public List<Item> itemList;
    public EnemyData enemyData;
    public string desc;
    public List<RoomData> connectedRoomList;
    public EventData eventData;

    public void OverrideRoom(Main main) {
        Room targetRoom = main.world.GetRoom(targetRoomData);
        if(itemList.Count != 0) {
            foreach(Item item in itemList) {
                targetRoom.itemList.Add(new Item(item));
            }
        }

        if(enemyData != null) {
            targetRoom.enemy = new Enemy(enemyData);
        }

        if(desc != "") {
            targetRoom.desc = desc;
        }

        if(connectedRoomList.Count != 0) {
            targetRoom.connectedRoomList = new List<RoomData>(connectedRoomList);
        }

        if(eventData != null) {
            targetRoom.eventData = eventData;
        }
    }
}


public class Room {
    public RoomData data;
    public List<Item> itemList;
    public Enemy enemy;
    public string desc;
    public List<RoomData> connectedRoomList;
    public EventData eventData;

    public Room(RoomData _data) {
        data = _data;
        itemList = new List<Item>();
        

        foreach(Item item in _data.itemList) {

            itemList.Add(new Item(item));
        }

        if(_data.enemyData != null) {
            enemy = new Enemy(_data.enemyData);
        }


        desc = _data.desc;

        connectedRoomList = new List<RoomData>(_data.connectedRoomList);

        if (_data.eventData!= null) {
            eventData = _data.eventData;
        }
    }

    public Room(SaveDataOfRoom saveData, DataBase database) {
        Debug.Log("部屋データロード開始");

        //部屋データ
        data = database.roomDatabase[saveData.ID];

        //アイテムリスト
        itemList = new List<Item>();
        foreach(SaveDataOfItem itemSaveData in saveData.itemList) {
            itemList.Add(new Item(itemSaveData, database));
        }

        Debug.Log("部屋データロード中:アイテムリスト完了");

        //敵
        if(saveData.enemyID != -1) {
            enemy = new Enemy(database.enemyDatabase[saveData.enemyID]);
        }
        else {
            enemy = null;
        }

        Debug.Log("部屋データロード中:敵完了");

        //説明文
        desc = saveData.desc;

        //接続先の部屋
        connectedRoomList = new List<RoomData>();
        foreach(int roomID in saveData.connectedRoomIDList) {
            connectedRoomList.Add(database.roomDatabase[roomID]);
        }

        Debug.Log("部屋データロード中:接続先完了");

        //イベントデータ
        if (saveData.eventID != -1) {
            eventData = database.eventDatabase[saveData.eventID];
        }
        else {
            eventData = null;
        }

        Debug.Log("部屋データロード完了:" + data.name);
    }
}

[Serializable]
public class SaveDataOfRoom {
    public int ID;
    public SaveDataOfItem[] itemList;
    public int enemyID;
    public string desc;
    public int[] connectedRoomIDList;
    public int eventID;

    public SaveDataOfRoom(Room room, DataBase database) {
        Debug.Log("部屋データセーブ開始:" + room.data.name);
        //部屋データ
        ID = database.GetRoomID(room.data);

        //アイテム
        itemList = new SaveDataOfItem[room.itemList.Count];
        for (int i = 0; i < itemList.Length; i++) {
            itemList[i] = new SaveDataOfItem(room.itemList[i], database);
        }

        //敵
        if(room.enemy != null) {
            enemyID = database.GetEnemyID(room.enemy.data);
        }
        else {
            enemyID = -1;
        }


        //説明文
        desc = room.desc;

        //接続された部屋
        connectedRoomIDList = new int[room.connectedRoomList.Count];
        for (int i = 0; i < connectedRoomIDList.Length; i++) {
            connectedRoomIDList[i] = database.GetRoomID(room.connectedRoomList[i]);
        }

        //イベント
        if(room.eventData != null) {
            eventID = database.GetEventID(room.eventData);
        }
        else {
            eventID = -1;
        }

        Debug.Log("部屋データセーブ完了:" + room.data.name);
    }
}


[CreateAssetMenu(fileName = "RoomData",menuName = "自作データ/RoomData")]
public class RoomData : ScriptableObject
{
    public new string name;
    public string desc;
    public Sprite sprite;
    public EnemyData enemyData;
    public int area = 10;
    public EventData eventData;

    public List<Item> itemList;
    public List<RoomData> connectedRoomList;
}
