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

    public List<FloorTile> floorTiles;
    public List<Transform> floorTransforms;

    //AABB Variables
    [HideInInspector]
    public float minXPos;
    [HideInInspector]
    public float maxXPos;
    [HideInInspector]
    public float minYPos;
    [HideInInspector]
    public float maxYPos;

    public bool isFinalRoom = false;
    public bool containsKey = false;

    public void CreateRoom()
    {
        this.doorways = new List<Border>();
        this.floorTransforms = new List<Transform>();

        float randomTileScaleX = Random.Range(this.roomData.minMaxTileScaleX[0], this.roomData.minMaxTileScaleX[1] + 1);
        float randomTileScaleY = Random.Range(this.roomData.minMaxTileScaleY[0], this.roomData.minMaxTileScaleY[1] + 1);

        this.floorPrefabScale = new Vector3(randomTileScaleX, randomTileScaleY, 1.0f);

        this.CreateFloors();
        this.CreateBorders();

        this.UpdateAABBBounds();
    }

    private void CreateFloors()
    {
        for (int i = 0; i < this.roomData.roomDimensions.y; i++)
        {
            for (int j = 0; j < this.roomData.roomDimensions.x; j++)
            {
                Vector3 instantiationPosition = this.floorParentTransform.position + 
                    new Vector3((j * this.floorPrefabScale.x), -(i * this.floorPrefabScale.y), 0.0f);

                GameObject floorInstance = Instantiate(this.floorPrefab, instantiationPosition, new Quaternion(), this.floorParentTransform);
                Transform floorTransform = floorInstance.GetComponent<Transform>();
                floorTransform.localScale = this.floorPrefabScale;
                this.floorTransforms.Add(floorTransform);

                FloorTile floorTile = floorInstance.GetComponent<FloorTile>();
                floorTile.SetState(TileState.Empty);
                this.floorTiles.Add(floorTile);
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

            Border borderComponent = wallInstance.GetComponent<Border>();
            borderComponent.SetBorderXScale(this.floorPrefabScale.x);
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
           
            Border borderComponent = wallInstance.GetComponent<Border>();
            borderComponent.SetBorderXScale(this.floorPrefabScale.y);
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

    public void UpdateAABBBounds()
    {
        float xPos1 = 0.0f;
        float xPos2 = 0.0f;
        float yPos1 = 0.0f;
        float yPos2 = 0.0f;

        if (this.roomTransform.up == Vector3.up || this.roomTransform.up == Vector3.down)
        {
            xPos1 = this.floorTransforms[0].position.x;
            xPos2 = this.floorTransforms[this.roomData.roomDimensions.x - 1].position.x;
            yPos1 = this.floorTransforms[0].position.y;
            yPos2 = this.floorTransforms[(this.roomData.roomDimensions.x * this.roomData.roomDimensions.y) - 1].position.y;
        }
        else
        {
            xPos1 = this.floorTransforms[0].position.x;
            xPos2 = this.floorTransforms[(this.roomData.roomDimensions.x * this.roomData.roomDimensions.y) - 1].position.x;
            yPos1 = this.floorTransforms[0].position.y;
            yPos2 = this.floorTransforms[this.roomData.roomDimensions.x - 1].position.y;            
        }

        if (xPos1 < xPos2)
        {
            this.minXPos = xPos1;
            this.maxXPos = xPos2;
        }
        else
        {
            this.minXPos = xPos2;
            this.maxXPos = xPos1;
        }

        if (yPos1 < yPos2)
        {
            this.minYPos = yPos1;
            this.maxYPos = yPos2;
        }
        else
        {
            this.minYPos = yPos2;
            this.maxYPos = yPos1;
        }

        //Account for size of floor
        if (this.roomTransform.up == Vector3.up || this.roomTransform.up == Vector3.down)
        {
            this.minXPos -= (this.floorPrefabScale.x / 2.0f);
            this.maxXPos += (this.floorPrefabScale.x / 2.0f);
            this.minYPos -= (this.floorPrefabScale.y / 2.0f);
            this.maxYPos += (this.floorPrefabScale.y / 2.0f);
        }
        else
        {
            this.minXPos -= (this.floorPrefabScale.y / 2.0f);
            this.maxXPos += (this.floorPrefabScale.y / 2.0f);
            this.minYPos -= (this.floorPrefabScale.x / 2.0f);
            this.maxYPos += (this.floorPrefabScale.x / 2.0f);
        }

        //Account for width of borders
        this.minXPos -= (this.doorways[0].borderYScale / 2.0f);
        this.maxXPos += (this.doorways[0].borderYScale / 2.0f);
        this.minYPos -= (this.doorways[0].borderYScale / 2.0f);
        this.maxYPos += (this.doorways[0].borderYScale / 2.0f);
    }

    public bool CollidesWithRoom(Room checkRoom)
    {
        return (this.minXPos < checkRoom.maxXPos &&
                this.maxXPos > checkRoom.minXPos &&
                this.minYPos < checkRoom.maxYPos &&
                this.maxYPos > checkRoom.minYPos);
    }

    public void UpdateDoorwaySides()
    {
        for (int i = 0; i < this.doorways.Count; i++)
        {
            if (this.doorways[i].borderTransform.up == Vector3.up)
            {
                this.doorways[i].side = DoorwaySide.North;
            }
            else if (this.doorways[i].borderTransform.up == Vector3.right)
            {
                this.doorways[i].side = DoorwaySide.East;
            }
            else if (this.doorways[i].borderTransform.up == Vector3.down)
            {
                this.doorways[i].side = DoorwaySide.South;
            }
            else if (this.doorways[i].borderTransform.up == Vector3.left)
            {
                this.doorways[i].side = DoorwaySide.West;
            }

            this.doorways[i].gameObject.name = (this.doorways[i].side.ToString() + "_Doorway");
        }
    }
}
