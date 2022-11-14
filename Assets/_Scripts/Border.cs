using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorwaySide { North = 0, East = -90, South = -180, West = 90}

public class Border : MonoBehaviour
{
    [SerializeField]
    private Transform[] clampWalls;
    [SerializeField]
    private Transform wall;
    [SerializeField]
    private Transform doorway;
    [SerializeField]
    private Transform finishLine;

    public Transform borderTransform;

    [HideInInspector]
    public bool connectedToRoom = false;

    public DoorwaySide side;

    public float borderYScale = 0.25f;

    public float borderXScale = 1.0f;

    public void SetBorderXScale(float xScale)
    {
        Vector3 newScale = new Vector3(xScale, this.borderYScale, 1.0f);
        Vector3 newclampWallScale = new Vector3(xScale / 2.0f, this.borderYScale, 1.0f);

        this.wall.localScale = newScale;
        this.doorway.localScale = newScale;
        this.finishLine.localScale = newScale;

        this.clampWalls[0].localScale = newclampWallScale;
        this.clampWalls[1].localScale = newclampWallScale;

        this.borderXScale = xScale;
    }

    public void EnableWall()
    {
        this.wall.gameObject.SetActive(true);
        this.doorway.gameObject.SetActive(false);
        this.finishLine.gameObject.SetActive(false);
    }

    public void EnableDoorway()
    {        
        this.doorway.gameObject.SetActive(true);        
        this.finishLine.gameObject.SetActive(false);
        this.wall.gameObject.SetActive(false);
    }

    public void EnableFinishLine()
    {
        this.finishLine.gameObject.SetActive(true);
        this.doorway.gameObject.SetActive(false);
        this.wall.gameObject.SetActive(false);
    }

    public void ShrinkDoorway(float newDoorwayWidth)
    {        
        float oldDoorwayWidth = this.borderXScale;

        this.SetBorderXScale(newDoorwayWidth);        

        float adjacentOpenSpaceWidth = ((oldDoorwayWidth - newDoorwayWidth) / 2.0f);

        //Setup Clamp Walls
        //Enable
        this.clampWalls[0].gameObject.SetActive(true);
        this.clampWalls[1].gameObject.SetActive(true);

        //Set Scale
        Vector3 newClampWallLocalScale = new Vector3(adjacentOpenSpaceWidth, this.borderYScale, 1.0f);
        this.clampWalls[0].localScale = newClampWallLocalScale;
        this.clampWalls[1].localScale = newClampWallLocalScale;

        //Translate to the center of the new open spaces
        float leftWallXPosition = this.doorway.localPosition.x - ((this.doorway.localScale.x / 2.0f) + (adjacentOpenSpaceWidth / 2.0f));
        float rightWallXPosition = this.doorway.localPosition.x + ((this.doorway.localScale.x / 2.0f) + (adjacentOpenSpaceWidth / 2.0f));
        this.clampWalls[0].localPosition = new Vector3(leftWallXPosition, this.doorway.localPosition.y, 1.0f);
        this.clampWalls[1].localPosition = new Vector3(rightWallXPosition, this.doorway.localPosition.y, 1.0f);
    }
}
