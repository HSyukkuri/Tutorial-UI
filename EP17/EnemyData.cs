using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy {
    public EnemyData data { get; private set; }
    public float hp = 1;

    public Enemy(EnemyData _data) {
        data = _data;
    }

    public void TakeDamage(float damage) {
        hp -= damage * 0.1f / data.con;
    }
}


[CreateAssetMenu(fileName = "EnemyData", menuName = "自作データ/EnemyData")]
public class EnemyData : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public bool isToxic = false;

    public float con = 1;
    public float dex = 1;
    public float atk = 1;
    
}
