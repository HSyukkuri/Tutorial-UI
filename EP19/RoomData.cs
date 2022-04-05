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
