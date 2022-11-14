using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{    
    public int minRooms;    
    public int maxRooms;

    public float addRoomSuccessChance = 0.5f;  //50% chance to add a room at any given doorway

    private int totalRooms = 2;

    [SerializeField]
    private Transform parentTransform;

    private List<Room> allRooms;

    [SerializeField]
    private GameObject baseRoomPrefab;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject keyPrefab;

    private RoomData[] allRoomData;    

    private Vector3 playerStartPoint = Vector3.zero;

    private FinishLine finalDoor;

    public void GenerateDungeon()
    {
        this.allRoomData = Resources.LoadAll<RoomData>("RoomData");
        this.allRooms = new List<Room>();

        this.totalRooms = Random.Range(this.minRooms, this.maxRooms);

        this.GenerateInitialRoom();

        while (this.allRooms.Count < this.totalRooms)
        {
            this.GenerateAdditionalRooms();
        }

        this.SetStartAndEndPoints();

        this.SealUnusedDoorways();

        this.SpawnKeys();

        this.SpawnPlayer();
    }

    private void GenerateInitialRoom()
    {
        RoomData chosenRoom = this.allRoomData[Random.Range(0, this.allRoomData.Length)];

        GameObject roomInstance = Instantiate(this.baseRoomPrefab, Vector3.zero, new Quaternion(), this.parentTransform);
        Room roomComponent = roomInstance.GetComponent<Room>();
        roomComponent.roomData = chosenRoom;
        roomComponent.CreateRoom();

        roomInstance.name = ("Room_" + this.allRooms.Count);

        this.allRooms.Add(roomComponent);
    }

    private void GenerateAdditionalRooms()
    {
        float addRoomAtDoorwayChance = 0.0f;

        //Iterate through all rooms
        for (int i = 0; i < this.allRooms.Count; i++)
        {
            //Iterate through all doorways
            for (int j = 0; j < this.allRooms[i].doorways.Count; j++)
            {
                //Skip doorways already connected to another room
                if (this.allRooms[i].doorways[j].connectedToRoom == true)
                {
                    continue;
                }

                //Determine if a room will be added at this doorway
                addRoomAtDoorwayChance = Random.Range(0.0f, 1.0f);
                if (addRoomAtDoorwayChance <= addRoomSuccessChance)
                {
                    this.AddRoomAtDoorway(this.allRooms[i].doorways[j]);
                }

                //Return early if max rooms has been achieved
                if (this.allRooms.Count >= this.totalRooms)
                {
                    return;
                }
            }
        }
    }

    private void AddRoomAtDoorway(Border doorway)
    {
        //Randomly choose RoomData
        int roomDataIndex = Random.Range(0, this.allRoomData.Length);
        RoomData chosenRoom = this.allRoomData[roomDataIndex];

        for (int i = 0; i < this.allRoomData.Length; i++)
        {
            //Instantiate and set up room
            GameObject roomInstance = Instantiate(this.baseRoomPrefab, doorway.transform.position, new Quaternion(), this.parentTransform);
            Room roomComponent = roomInstance.GetComponent<Room>();
            roomComponent.roomData = chosenRoom;
            roomComponent.CreateRoom();
            roomInstance.name = ("Room_" + this.allRooms.Count);

            //Attempt to align the new room with its target doorway
            bool result = this.AlignRoomDoorways(roomComponent, doorway);

            //Add room to master list and exit loop if room is properly placed
            if (result == true)
            {
                this.allRooms.Add(roomComponent);
                break;
            }
            //If the room couldn't fit in ANY orientation, destroy it and choose a new RoomData to repeat the process
            else
            {
                Destroy(roomInstance);
                roomDataIndex = (roomDataIndex + 1) % this.allRoomData.Length;
                chosenRoom = this.allRoomData[roomDataIndex];
            }
        }        
    }
    
    //This was wild to figure out...
    private float GetTargetRotation(DoorwaySide oldDoorway, DoorwaySide newDoorway)
    {
        switch (oldDoorway)
        {
            case DoorwaySide.North:      
                return 180.0f - (float)newDoorway;                
            case DoorwaySide.East:    
                return ((-(float)(oldDoorway)) + (-(float)(newDoorway)));
            case DoorwaySide.South:
                return -(float)newDoorway;
            case DoorwaySide.West:
                return ((-(float)(oldDoorway)) + (-(float)(newDoorway)));
            default:
                return 0.0f;
        }
    }

    private Vector3 GetMoveOffset(Border oldDoorway, Border newDoorway)
    {
        Vector3 offset = oldDoorway.borderTransform.position - newDoorway.borderTransform.position;
        
        switch (oldDoorway.side)
        {
            case DoorwaySide.North:
                offset.y += (newDoorway.borderYScale);
                break;
            case DoorwaySide.East:
                offset.x += (newDoorway.borderYScale);
                break;
            case DoorwaySide.South:
                offset.y -= (newDoorway.borderYScale);
                break;
            case DoorwaySide.West:
                offset.x -= (newDoorway.borderYScale);
                break;
        }

        return offset;
    }

    private bool AlignRoomDoorways(Room newRoom, Border targetDoorway)
    {
        int attempts = 0;

        //Pick a random doorway to start with
        int doorwayIndex = Random.Range(0, newRoom.doorways.Count);
        Border chosenDoorway = newRoom.doorways[doorwayIndex];
                
        for (attempts = 0; attempts < newRoom.doorways.Count; attempts++)
        {
            
            //Rotate
            float targetZRotation = this.GetTargetRotation(targetDoorway.side, chosenDoorway.side);
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, targetZRotation);
            newRoom.roomTransform.rotation = targetRotation;

            //Translate
            Vector3 moveOffset = this.GetMoveOffset(targetDoorway, chosenDoorway);
            newRoom.roomTransform.localPosition += moveOffset;
            
            //Update AABB
            newRoom.UpdateAABBBounds();

            //Update Doorway Sides
            newRoom.UpdateDoorwaySides();
            
            //Check for collision with all currently set rooms
            if (this.IsCollidingWithExistingRoom(newRoom) == true)
            {
                doorwayIndex = (doorwayIndex + 1) % newRoom.doorways.Count;
                chosenDoorway = newRoom.doorways[doorwayIndex];
            }
            else
            {
                break;
            }
            
        }        

        if (attempts < newRoom.doorways.Count)
        {
            //Update doorway status
            chosenDoorway.connectedToRoom = true;
            targetDoorway.connectedToRoom = true;

            newRoom.UpdateAABBBounds();
            newRoom.UpdateDoorwaySides();

            //Modify the size of the larger doorway to match the smaller doorway
            this.MatchDoorwaySizes(chosenDoorway, targetDoorway);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void MatchDoorwaySizes(Border doorway1, Border doorway2)
    {
        if (doorway1.borderXScale > doorway2.borderXScale)
        {
            doorway1.ShrinkDoorway(doorway2.borderXScale);
        }
        else
        {
            doorway2.ShrinkDoorway(doorway1.borderXScale);
        }
    }

    private void SealUnusedDoorways()
    {
        for (int i = 0; i < this.allRooms.Count; i++)
        {
            List<Border> unsealedDoorways = new List<Border>();

            for (int j = 0; j < this.allRooms[i].doorways.Count; j++)
            {
                if (this.allRooms[i].doorways[j].connectedToRoom == false)
                {
                    this.allRooms[i].doorways[j].EnableWall();
                    this.allRooms[i].doorways[j].gameObject.name = "Wall";
                }
                else
                {
                    unsealedDoorways.Add(this.allRooms[i].doorways[j]);
                }
            }

            //Update doorway list for room
            this.allRooms[i].doorways = unsealedDoorways;
        }
    }

    private Room GetFurthestRoom()
    {
        float furthestDistance = 0.0f;
        Room furthestRoom = this.allRooms[this.allRooms.Count - 1];

        for (int i = 0; i < this.allRooms.Count; i++)
        {
            float distanceFromPlayer = Vector3.Distance(this.playerStartPoint, this.allRooms[i].roomTransform.position);
            if (distanceFromPlayer > furthestDistance)
            {
                furthestRoom = this.allRooms[i];
                furthestDistance = distanceFromPlayer;
            }
        }

        return furthestRoom;
    }

    private void SetStartAndEndPoints()
    {
        //Start player on a random tilein the first generated room
        Room startingRoom = this.allRooms[0];
        Transform randomTileTransform = startingRoom.floorTransforms[Random.Range(0, startingRoom.floorTransforms.Count)];
        this.playerStartPoint = new Vector3(randomTileTransform.position.x, randomTileTransform.position.y, -0.5f);

        //Change the open doorway in the last room that was generated to a locked finish door
        Room endingRoom = this.allRooms[this.allRooms.Count - 1];
        for (int i = 0; i < endingRoom.doorways.Count; i++)
        {
            if (endingRoom.doorways[i].connectedToRoom == true)
            {
                endingRoom.isFinalRoom = true;
                endingRoom.doorways[i].EnableFinishLine();
                endingRoom.doorways[i].name = "FinishLine";
                this.finalDoor = endingRoom.doorways[i].GetComponentInChildren<FinishLine>();
                break;
            }
        }
    }

    private void SpawnKeyInRoom(Room spawnRoom)
    {
        int randomTileIndex = Random.Range(0, spawnRoom.floorTiles.Count);
        Transform randomTileTransform = spawnRoom.floorTransforms[randomTileIndex];
        FloorTile randomTile = spawnRoom.floorTiles[randomTileIndex];
        Vector3 instantiationPosition = new Vector3(randomTileTransform.position.x, randomTileTransform.position.y, -0.5f);
        Instantiate(this.keyPrefab, instantiationPosition, new Quaternion(), this.gameObject.GetComponent<Transform>());
        randomTile.SetState(TileState.Key);
    }

    private void SpawnKeys()
    {
        //Determine how many keys will span based on the size of the dungeon
        int minNumKeysToSpawn = (this.allRooms.Count / 3);
        int maxNumKeysToSpawn = (this.allRooms.Count / 2);
        int keysToSpawn = Random.Range(minNumKeysToSpawn, maxNumKeysToSpawn);

        int currentKeyCount = 0;

        while (currentKeyCount < keysToSpawn)
        {
            //Exclude the starting room
            Room randomRoom = this.allRooms[Random.Range(1, this.allRooms.Count)];
            if (randomRoom.isFinalRoom == true || randomRoom.containsKey == true)
            {
                continue;
            }

            this.SpawnKeyInRoom(randomRoom);
            currentKeyCount++;
        }

        GameManager.totalKeys = keysToSpawn;        
    }

    private void SpawnPlayer()
    {
        Instantiate(this.playerPrefab, this.playerStartPoint, new Quaternion());
    }

    private bool IsCollidingWithExistingRoom(Room newRoom)
    {
        for (int i = 0; i < this.allRooms.Count; i++)
        {
            if (newRoom.CollidesWithRoom(this.allRooms[i]) == true)
            {
                return true;
            }
        }

        return false;
    }
}
