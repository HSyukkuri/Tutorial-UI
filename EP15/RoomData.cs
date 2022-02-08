using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room {
    public RoomData data;
    public List<Item> itemList;
    public Enemy enemy;

    public Room(RoomData _data) {
        data = _data;
        itemList = new List<Item>();

        foreach(Item item in _data.itemList) {

            itemList.Add(new Item(item));
        }

        if(_data.enemyData != null) {
            enemy = new Enemy(_data.enemyData);
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

    public List<Item> itemList;
    public List<RoomData> connectedRoomList;
}
