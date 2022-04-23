using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public List<Room> roomList;

    public Room GetRoom(RoomData data) {
        foreach(Room room in roomList) {
            if(room.data == data) {
                return room;
            }
        }

        Debug.LogError("データ上に無い部屋を指定されたぞ！！！！！");
        return null;
    }


    //コンストラクタ
    public World(List<RoomData> roomDataList) {
        roomList = new List<Room>();
        foreach(RoomData rData in roomDataList) {
            Room newRoom = new Room(rData);
            roomList.Add(newRoom);
        }
    }

    public World(SaveDataOfRoom[] roomSaveDataArray, DataBase database) {
        roomList = new List<Room>();
        foreach(SaveDataOfRoom roomSaveData in roomSaveDataArray) {
            Room room = new Room(roomSaveData, database);
            roomList.Add(room);
        }
    }
}
