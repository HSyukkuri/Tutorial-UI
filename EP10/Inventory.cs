using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<ItemData> itemDataList = new List<ItemData>();
    public EquipItemData equip { get; private set; }

    Main main;

    public void UseItem(int index) {
        //インデックスがおかしくないかチェック
        if(index < 0 || index > itemDataList.Count - 1) {
            return;
        }

        switch (itemDataList[index].GetType().ToString()) {

            case nameof(EquipItemData): {
                    //指定されたアイテムが現在装備しているアイテムじゃなかったら装備
                    if (equip != itemDataList[index]) {
                        equip = (EquipItemData)itemDataList[index];
                    }
                    else {
                        //現在装備しているアイテムなら、装備解除
                        equip = null;
                    }
                    return;
                }

            case nameof(HealItemData): {
                    HealItemData healItem = (HealItemData)itemDataList[index];
                    main.Heal(healItem.healValue);
                    itemDataList.Remove(itemDataList[index]);
                    return;
                }


        }

    }

    public Inventory(Main _main) {
        main = _main;
    }
}
