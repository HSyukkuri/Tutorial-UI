using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Item> itemList = new List<Item>();
    public Item equip { get; private set; }

    Main main;

    public void UseItem(int index) {
        //インデックスがおかしくないかチェック
        if(index < 0 || index > itemList.Count - 1) {
            return;
        }

        switch (itemList[index].data.GetType().ToString()) {

            case nameof(EquipRangeItemData):
            case nameof(EquipItemData): {
                    //指定されたアイテムが現在装備しているアイテムじゃなかったら装備
                    if (equip != itemList[index]) {
                        equip = itemList[index];
                    }
                    else {
                        //現在装備しているアイテムなら、装備解除
                        equip = null;
                    }
                    return;
                }

            case nameof(HealItemData): {
                    HealItemData healItem = (HealItemData)itemList[index].data;
                    main.Heal(healItem);
                    itemList.Remove(itemList[index]);
                    return;
                }

            case nameof(KeyItemData): {
                    KeyItemData keyItemData = (KeyItemData)itemList[index].data;
                    if(main.currentRoom.data == keyItemData.roomOverride.targetRoomData) {
                        //鍵を使える！
                        keyItemData.roomOverride.OverrideRoom(main);
                        main.text.text = "鍵を開けた！";
                        AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                        itemList.Remove(itemList[index]);

                    }
                    else {
                        main.text.text = "ここでは使えない";
                        AudioManager.instance.Play_System(AudioManager.instance.sys_Fail);
                    }

                    return;
                }


        }

    }

    public bool TryCombine(int index, int combineIndex) {
        //インデックスがおかしくないかチェック
        if(index < 0 || index > itemList.Count - 1) {
            return false;
        }

        if(combineIndex < 0 || combineIndex > itemList.Count - 1) {
            return false;
        }

        //組合せ元と組合せ先が同じアイテムじゃないかチェック
        if(index == combineIndex) {
            return false;
        }

        Item item = itemList[index];
        Item combineItem = itemList[combineIndex];

        //組合せ条件に該当があるか検索
        foreach(ItemCombine combine in item.data.list_ItemCombine) {
            if(combine.combineItem == combineItem.data) {
                //組合せ可能！
                itemList.Add(new Item(combine.finishedItem));
                itemList.Remove(item);
                itemList.Remove(combineItem);
                return true;
            }
        }

        //itemが銃でcombineItemがその銃に対応する弾だった場合
        if(item.data is EquipRangeItemData) {
            EquipRangeItemData itemData = (EquipRangeItemData)item.data;
            if(itemData.bullet == combineItem.data) {
                int requireAmount = itemData.capacity - item.value;

                if (requireAmount >= combineItem.value) {
                    item.value += combineItem.value;
                    itemList.Remove(combineItem);
                }
                else {
                    item.value = itemData.capacity;
                    combineItem.value -= requireAmount;
                }

                return true;
            }
        }

        //combineItemが銃でitemがその銃に対応する弾だった場合
        if (combineItem.data is EquipRangeItemData) {
            EquipRangeItemData combineItemData = (EquipRangeItemData)combineItem.data;
            if (combineItemData.bullet == item.data) {
                int requireAmount = combineItemData.capacity - combineItem.value;

                if (requireAmount >= item.value) {
                    combineItem.value += item.value;
                    itemList.Remove(item);
                }
                else {
                    combineItem.value = combineItemData.capacity;
                    item.value -= requireAmount;
                }

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
