using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemData",menuName = "自作データ/ItemData")]
public class ItemData : ScriptableObject
{
    new public string name;

    [TextArea(5,20)]
    public string desc;

    public List<ItemCombine> list_ItemCombine = new List<ItemCombine>();

}

[Serializable]
public class ItemCombine {
    public ItemData combineItem;
    public Item finishedItem;
}

[Serializable]
public class Item {
    public ItemData data;
    public int value;

    public Item(Item _item) {
        data = _item.data;
        value = _item.value;
    }

    public Item(SaveDataOfItem saveData,Database database) {
        data = database.itemDatabase[saveData.ID];
        value = saveData.value;
    }
}

[Serializable]
public class SaveDataOfItem {
    public int ID;
    public int value;

    public SaveDataOfItem(Item item, Database database) {
        ID = database.GetItemID(item.data);
        value = item.value;
    }
}