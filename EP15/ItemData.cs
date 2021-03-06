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
}