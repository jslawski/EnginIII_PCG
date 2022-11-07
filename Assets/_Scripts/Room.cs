using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData roomData;

    public Transform roomTransform;

    private Vector3 floorPrefabScale;

    [SerializeField]
    private GameObject floorPrefab;
    [SerializeField]
    private GameObject borderPrefab;
    
    [SerializeField]
    private Transform floorParentTransform;
    [SerializeField]
    private Transform borderParentTransform;

    public List<Border> doorways;

    public void CreateRoom()
    {
        this.doorways = new List<Border>();

        this.floorPrefabScale = this.floorPrefab.GetComponent<Transform>().lossyScale;

        this.CreateFloors();
        this.CreateBorders();
    }

    private void CreateFloors()
    {
        for (int i = 0; i < this.roomData.roomDimensions.y; i++)
        {
            for (int j = 0; j < this.roomData.roomDimensions.x; j++)
            {
                Vector3 instantiationPosition = this.floorParentTransform.position + 
                    new Vector3((j * this.floorPrefabScale.x), -(i * this.floorPrefabScale.y), 0.0f);

                Instantiate(this.floorPrefab, instantiationPosition, new Quaternion(), this.floorParentTransform);
            }
        }
    }

    private void PopulateNorthSouthSides(float offsetY, DoorwaySide doorwaySide, bool createDoorway)
    {
        List<Border> wallObjects = new List<Border>();

        for (int i = 0; i < this.roomData.roomDimensions.x; i++)
        {
            Vector3 instantiationPosition = this.borderParentTransform.position + new Vector3((i * this.floorPrefabScale.x), offsetY, 0.0f);
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, (float)doorwaySide);

            GameObject wallInstance = Instantiate(this.borderPrefab, instantiationPosition, targetRotation, this.borderParentTransform);

            Transform wallTransform = wallInstance.GetComponent<Transform>();
            wallTransform.localScale = new Vector3(this.floorPrefabScale.x, wallTransform.localScale.y, wallTransform.localScale.z);

            Border borderComponent = wallInstance.GetComponent<Border>();
            borderComponent.EnableWall();
            wallObjects.Add(borderComponent);
        }

        if (createDoorway == true)
        {
            this.CreateDoorway(wallObjects, doorwaySide);
        }
    }

    private void PopulateEastWestSides(float offsetX, DoorwaySide doorwaySide, bool createDoorway)
    {
        List<Border> wallObjects = new List<Border>();

        for (int i = 0; i < this.roomData.roomDimensions.y; i++)
        {
            Vector3 instantiationPosition = this.borderParentTransform.position + new Vector3(offsetX, -(i * this.floorPrefabScale.y), 0.0f);
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, (float)doorwaySide);
            GameObject wallInstance = Instantiate(this.borderPrefab, instantiationPosition, targetRotation, this.borderParentTransform);

            Transform wallTransform = wallInstance.GetComponent<Transform>();
            wallTransform.localScale = new Vector3(this.floorPrefabScale.y, wallTransform.localScale.y, wallTransform.localScale.z);

            Border borderComponent = wallInstance.GetComponent<Border>();
            borderComponent.EnableWall();
            wallObjects.Add(borderComponent);
        }

        if (createDoorway == true)
        {
            this.CreateDoorway(wallObjects, doorwaySide);
        }
    }

    private void CreateBorders()
    {
        //North Side
        float northYPosition = (this.floorPrefabScale.y / 2.0f);
        this.PopulateNorthSouthSides(northYPosition, DoorwaySide.North, this.roomData.northEntrance);
        
        //East Side
        float eastXPosition = (((this.roomData.roomDimensions.x - 1) * this.floorPrefabScale.x) + (this.floorPrefabScale.x / 2.0f));        
        this.PopulateEastWestSides(eastXPosition, DoorwaySide.East, this.roomData.eastEntrance);

        //South Side
        float southYPosition = -(((this.roomData.roomDimensions.y - 1) * this.floorPrefabScale.y) + (this.floorPrefabScale.y / 2.0f));               
        this.PopulateNorthSouthSides(southYPosition, DoorwaySide.South, this.roomData.southEntrance);

        //West Side
        float westXPosition = -(this.floorPrefabScale.x / 2.0f);        
        this.PopulateEastWestSides(westXPosition, DoorwaySide.West, this.roomData.westEntrance);
    }

    private void CreateDoorway(List<Border> wallObjects, DoorwaySide doorwaySide)
    {
        int centerIndex = Mathf.RoundToInt(wallObjects.Count / 2.0f) - 1;
        int deviation = Mathf.RoundToInt(wallObjects.Count / 4.0f);
        int randomDeviation = Random.Range(-deviation, deviation + 1);

        if (centerIndex <= 0)
        {
            randomDeviation = 0;
            centerIndex = 0;
        }

        Border targetBorder = wallObjects[centerIndex + randomDeviation];

        targetBorder.EnableDoorway();
        targetBorder.gameObject.name = (doorwaySide.ToString() + "_Doorway");
        targetBorder.side = doorwaySide;
        this.doorways.Add(targetBorder);
    }
}
