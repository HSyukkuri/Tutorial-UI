using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<ItemData> itemDataList = new List<ItemData>();
    public EquipItemData equip { get; private set; }

    public void EquipItem(int index) {
        if(index < 0 || index > itemDataList.Count - 1) {
            return;
        }

        if(itemDataList[index].GetType() != typeof(EquipItemData)) {
            return;
        }

        if(equip != itemDataList[index]) {
            equip = (EquipItemData)itemDataList[index];
        }
        else {
            equip = null;
        }

    }
}
