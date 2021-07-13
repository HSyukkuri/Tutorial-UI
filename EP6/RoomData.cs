using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room {
    public RoomData data;
    public ItemData item;

    public Room(RoomData _data) {
        data = _data;
        item = _data.itemData;
    }
}

[CreateAssetMenu(fileName = "RoomData",menuName = "自作データ/RoomData")]
public class RoomData : ScriptableObject
{
    public new string name;
    public string desc;

    public ItemData itemData;
    public List<RoomData> connectedRoomList;
}
