using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room {
    public RoomData data;
    public ItemData item;
    public EnemyData enemy;

    public Room(RoomData _data) {
        data = _data;
        item = _data.itemData;
        enemy = _data.enemyData;
    }
}

[CreateAssetMenu(fileName = "RoomData",menuName = "自作データ/RoomData")]
public class RoomData : ScriptableObject
{
    public new string name;
    public string desc;
    public Sprite sprite;
    public EnemyData enemyData;

    public ItemData itemData;
    public List<RoomData> connectedRoomList;
}
