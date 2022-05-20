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

    public Room(SaveDataOfRoom saveData, Database database) {
        data = database.roomDatabase[saveData.ID];

        itemList = new List<Item>();
        foreach (SaveDataOfItem itemSaveData in saveData.itemArray) {
            itemList.Add(new Item(itemSaveData, database));
        }

        if(saveData.enemyID != -1) {
            enemy = new Enemy(database.enemyDatabase[saveData.enemyID]);
        }
        else {
            enemy = null;
        }

        desc = saveData.desc;

        connectedRoomList = new List<RoomData>();
        foreach(int roomID in saveData.connectedRoomArray) {
            connectedRoomList.Add(database.roomDatabase[roomID]);
        }

        if(saveData.eventID != -1) {
            eventData = database.eventDatabase[saveData.eventID];
        }
        else {
            eventData = null;
        }

    }
}

[Serializable]
public class SaveDataOfRoom {
    public int ID;
    public SaveDataOfItem[] itemArray;
    public int enemyID;
    public string desc;
    public int[] connectedRoomArray;
    public int eventID;

    public SaveDataOfRoom(Room room, Database database) {
        ID = database.GetRoomID(room.data);

        itemArray = new SaveDataOfItem[room.itemList.Count];
        for (int i = 0; i < itemArray.Length; i++) {
            itemArray[i] = new SaveDataOfItem(room.itemList[i], database);
        }

        if(room.enemy != null) {
            enemyID = database.GetEnemyID(room.enemy.data);
        }
        else {
            enemyID = -1;
        }

        desc = room.desc;

        connectedRoomArray = new int[room.connectedRoomList.Count];
        for (int i = 0; i < connectedRoomArray.Length; i++) {
            connectedRoomArray[i] = database.GetRoomID(room.connectedRoomList[i]);
        }

        if(room.eventData != null) {
            eventID = database.GetEventID(room.eventData);
        }
        else {
            eventID = -1;
        }


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
