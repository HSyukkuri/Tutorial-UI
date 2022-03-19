using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room {
    public RoomData data;
    public List<Item> itemList;
    public Enemy enemy;
    public string desc;
    public List<RoomData> connectedRoomList;

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

    public List<Item> itemList;
    public List<RoomData> connectedRoomList;
}
