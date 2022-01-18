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

    public bool TryCombine(int index, int combineIndex) {
        //インデックスがおかしくないかチェック
        if(index < 0 || index > itemDataList.Count - 1) {
            return false;
        }

        if(combineIndex < 0 || combineIndex > itemDataList.Count - 1) {
            return false;
        }

        //組合せ元と組合せ先が同じアイテムじゃないかチェック
        if(index == combineIndex) {
            return false;
        }

        ItemData item = itemDataList[index];
        ItemData combineItem = itemDataList[combineIndex];

        //組合せ条件に該当があるか検索
        foreach(ItemCombine combine in item.list_ItemCombine) {
            if(combine.combineItem == combineItem) {
                //組合せ可能！
                itemDataList.Add(combine.finishedItem);
                itemDataList.Remove(item);
                itemDataList.Remove(combineItem);
                return true;
            }
        }

        Debug.Log("該当の組み合わせ条件なし");

        return false;

    }

    public Inventory(Main _main) {
        main = _main;
    }
}
