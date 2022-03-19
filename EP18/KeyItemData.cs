using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyItemData", menuName = "����f�[�^/ItemData-Key")]
public class KeyItemData : ItemData
{
    public RoomData targetRoom;
    public string descOverride;
    public List<RoomData> connectedRoomOverride;


}
