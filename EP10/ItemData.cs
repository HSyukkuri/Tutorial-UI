using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData",menuName = "自作データ/ItemData")]
public class ItemData : ScriptableObject
{
    new public string name;

    [TextArea(5,20)]
    public string desc;

}
