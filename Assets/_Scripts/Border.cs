using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorwaySide { North = 0, East = -90, South = -180, West = 90}

public class Border : MonoBehaviour
{
    [SerializeField]
    private GameObject wallObject;
    [SerializeField]
    private GameObject doorwayObject;
    
    public Transform borderTransform;

    [HideInInspector]
    public bool connectedToRoom = false;

    public DoorwaySide side;

    public void EnableWall()
    {
        this.wallObject.SetActive(true);
        this.doorwayObject.SetActive(false);
    }

    public void EnableDoorway()
    {
        this.doorwayObject.SetActive(true);
        this.wallObject.SetActive(false);
    }
}
