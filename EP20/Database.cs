using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database",menuName = "����f�[�^/Database")]
public class Database : ScriptableObject
{
    public List<ItemData> itemDatabase;
    public List<RoomData> roomDatabase;
    public List<EnemyData> enemyDatabase;
    public List<EventData> eventDatabase;

    public int GetItemID(ItemData data)
    {
        int index = itemDatabase.IndexOf(data);

        if (index == -1) {
            Debug.LogError("����ȃA�C�e���̓f�[�^�x�[�X��ɖ����ł��I�I�I�I�I�I�I�I�I�I�I�I");
        }
        
        return index;
    }

    public int GetRoomID(RoomData data)
    {
        for (int i = 0; i < roomDatabase.Count; i++)
        {
            if(roomDatabase[i] == data)
            {
                return i;
            }
        }

        Debug.LogError("����ȕ����̓f�[�^��ɂȂ��ł��������������������������������������������I");
        return -1;
    }

    public int GetEnemyID(EnemyData data)
    {
        for (int i = 0; i < enemyDatabase.Count; i++)
        {
            if(enemyDatabase[i] == data)
            {
                return i;
            }
        }

        Debug.Log("����ȓG�̓f�[�^��ɂ˂����������������������������������I�I�I");
        return -1;
    }

    public int GetEventID(EventData data)
    {
        for (int i = 0; i < eventDatabase.Count; i++)
        {
            if(eventDatabase[i] == data)
            {
                return i;
            }
        }

        Debug.LogError("����ȃC�x���g�̓f�[�^��ɂ���܂���I�I�I�I�I�I�I�I");
        return -1;
    }
}
