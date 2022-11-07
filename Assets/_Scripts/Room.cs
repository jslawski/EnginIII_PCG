using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomData roomData;

    private Vector3 floorPrefabScale;

    [SerializeField]
    private GameObject floorPrefab;
    [SerializeField]
    private GameObject borderPrefab;

    [SerializeField]
    private Transform floorParentTransform;
    [SerializeField]
    private Transform borderParentTransform;

    private List<Border> doorways;

    // Start is called before the first frame update
    void Start()
    {
        this.CreateRoom();
    }

    private void CreateRoom()
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

    private void PopulateNorthSouthSides(float offsetY, Quaternion wallRotation, bool createDoorway)
    {
        List<Border> wallObjects = new List<Border>();

        for (int i = 0; i < this.roomData.roomDimensions.x; i++)
        {
            Vector3 instantiationPosition = this.borderParentTransform.position + new Vector3((i * this.floorPrefabScale.x), offsetY, 0.0f);
            GameObject wallInstance = Instantiate(this.borderPrefab, instantiationPosition, wallRotation, this.borderParentTransform);

            Transform wallTransform = wallInstance.GetComponent<Transform>();
            wallTransform.localScale = new Vector3(this.floorPrefabScale.x, wallTransform.localScale.y, wallTransform.localScale.z);

            Border borderComponent = wallInstance.GetComponent<Border>();
            borderComponent.EnableWall();
            wallObjects.Add(borderComponent);
        }

        if (createDoorway == true)
        {
            this.CreateDoorway(wallObjects);
        }
    }

    private void PopulateEastWestSides(float offsetX, Quaternion wallRotation, bool createDoorway)
    {
        List<Border> wallObjects = new List<Border>();

        for (int i = 0; i < this.roomData.roomDimensions.y; i++)
        {
            Vector3 instantiationPosition = this.borderParentTransform.position + new Vector3(offsetX, -(i * this.floorPrefabScale.y), 0.0f);
            GameObject wallInstance = Instantiate(this.borderPrefab, instantiationPosition, wallRotation, this.borderParentTransform);

            Transform wallTransform = wallInstance.GetComponent<Transform>();
            wallTransform.localScale = new Vector3(this.floorPrefabScale.y, wallTransform.localScale.y, wallTransform.localScale.z);

            Border borderComponent = wallInstance.GetComponent<Border>();
            borderComponent.EnableWall();
            wallObjects.Add(borderComponent);
        }

        if (createDoorway == true)
        {
            this.CreateDoorway(wallObjects);
        }
    }

    private void CreateBorders()
    {
        //North Side
        float northYPosition = (this.floorPrefabScale.y / 2.0f);
        this.PopulateNorthSouthSides(northYPosition, new Quaternion(), this.roomData.northEntrance);
        
        //East Side
        float eastXPosition = (((this.roomData.roomDimensions.x - 1) * this.floorPrefabScale.x) + (this.floorPrefabScale.x / 2.0f));
        Quaternion eastRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -90.0f));
        this.PopulateEastWestSides(eastXPosition, eastRotation, this.roomData.eastEntrance);

        //South Side
        float southYPosition = -(((this.roomData.roomDimensions.y - 1) * this.floorPrefabScale.y) + (this.floorPrefabScale.y / 2.0f));       
        Quaternion southRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f));
        this.PopulateNorthSouthSides(southYPosition, southRotation, this.roomData.southEntrance);

        //West Side
        float westXPosition = -(this.floorPrefabScale.x / 2.0f);
        Quaternion westRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 90.0f));
        this.PopulateEastWestSides(westXPosition, westRotation, this.roomData.westEntrance);
    }

    private void CreateDoorway(List<Border> wallObjects)
    {
        int centerIndex = Mathf.RoundToInt(this.roomData.roomDimensions.x / 2.0f) - 1;
        int deviation = Mathf.RoundToInt(this.roomData.roomDimensions.x / 4.0f);
        int randomDeviation = Random.Range(-deviation, deviation + 1);

        Border targetBorder = wallObjects[centerIndex + randomDeviation];

        targetBorder.EnableDoorway();
        targetBorder.gameObject.name = "Doorway";
        this.doorways.Add(targetBorder);
    }
}
