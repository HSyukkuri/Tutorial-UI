using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room {
    public RoomData data;
    public List<ItemData> itemList;
    public Enemy enemy;

    public Room(RoomData _data) {
        data = _data;
        itemList = new List<ItemData>(_data.itemDataList);
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

    public List<ItemData> itemDataList;
    public List<RoomData> connectedRoomList;
}
