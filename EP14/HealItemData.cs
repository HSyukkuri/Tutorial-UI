using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealData",menuName = "自作データ/ItemData - Heal")]
public class HealItemData : ItemData
{
    public float healValue = 1;

    public bool isAntidote = false;
}
