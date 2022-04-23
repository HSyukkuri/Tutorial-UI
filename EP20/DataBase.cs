using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "����f�[�^/Database")]
public class DataBase : ScriptableObject
{
    public List<ItemData> itemDatabase;
    public List<RoomData> roomDatabase;
    public List<EnemyData> enemyDatabase;
    public List<EventData> eventDatabase;

    public int GetItemID(ItemData data) {
        for (int i = 0; i < itemDatabase.Count; i++) {
            if(itemDatabase[i] == data) {
                return i;
            }
        }

        Debug.LogError("����ȃA�C�e���̓f�[�^��ɖ����I�I");
        return -1;
    }

    public int GetRoomID(RoomData data) {
        for (int i = 0; i < roomDatabase.Count; i++) {
            if (roomDatabase[i] == data) {
                return i;
            }
        }

        Debug.LogError("����ȕ����̓f�[�^��ɖ����I�I");
        return -1;
    }

    public int GetEnemyID(EnemyData data) {

        for (int i = 0; i < enemyDatabase.Count; i++) {
            if (enemyDatabase[i] == data) {
                return i;
            }
        }

        Debug.LogError("����ȓG�̓f�[�^��ɖ����I�I");
        return -1;
    }

    public int GetEventID(EventData data) {
        for (int i = 0; i < eventDatabase.Count; i++) {
            if (eventDatabase[i] == data) {
                return i;
            }
        }

        Debug.LogError("����ȃC�x���g�̓f�[�^��ɖ����I�I");
        return -1;
    }
}
