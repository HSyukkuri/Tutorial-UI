using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipRangeData",menuName = "©ìƒf[ƒ^/ItemData-Range")]
public class EquipRangeItemData : EquipItemData
{
    public int capacity;

    public StackItemData bullet;

    public float range = 4f;

    public float HitPercent(float distance) {
        if(range >= distance) {
            return 0.7f + 0.3f * ((range - distance) / range);
        }
        else
        if((range * 1.5f) >= distance){
            return 0.7f - 0.7f * (distance / (range * 1.5f));
        }
        else {
            return 0f;
        }

    }
}
