using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RoomData", menuName = "RoomData")]
public class RoomData : ScriptableObject
{
    public Vector2Int roomDimensions;
    public bool northEntrance;
    public bool eastEntrance;
    public bool southEntrance;
    public bool westEntrance;
}
