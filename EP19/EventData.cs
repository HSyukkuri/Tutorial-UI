using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "自作データ/EventData")]
public class EventData : ScriptableObject
{
    public Sprite image;
    public AudioClip sound;

    public List<RoomOverride> roomOverrideList = new List<RoomOverride>();

    [TextArea(5,20)]
    public List<string> textList = new List<string>();

    public EventData nextEvent;

}
