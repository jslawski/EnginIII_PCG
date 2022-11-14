using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RoomData", menuName = "RoomData")]
public class RoomData : ScriptableObject
{
    public Vector2Int roomDimensions;

    public Vector2 minMaxTileScaleX;
    public Vector2 minMaxTileScaleY;

    public bool northEntrance;
    public bool eastEntrance;
    public bool southEntrance;
    public bool westEntrance;
}
