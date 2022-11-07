using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private int maxRooms = 2;

    [SerializeField]
    private Transform parentTransform;

    private List<Room> allRooms;

    [SerializeField]
    private GameObject baseRoomPrefab;

    private RoomData[] allRoomData;

    // Start is called before the first frame update
    void Start()
    {
        this.allRoomData = Resources.LoadAll<RoomData>("RoomData");
        this.allRooms = new List<Room>();
        this.GenerateDungeon();    
    }

    private void GenerateDungeon()
    {
        this.GenerateInitialRoom();
        this.GenerateAdditionalRooms();
        this.SealUnusedDoorways();
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
        int currentRoomIndex = 0;
        float addRoomAtDoorwayChance = 0.0f;
        float addRoomSuccessChance = 1.0f;  //50% chance to add a room at any given doorway

        while (this.allRooms.Count < this.maxRooms)
        {
            //Iterate through each doorway in the room and roll to see if a new room gets added to it
            for (int i = 0; i < this.allRooms[currentRoomIndex].doorways.Count; i++)
            {
                addRoomAtDoorwayChance = Random.Range(0.0f, 1.0f);
                if (addRoomAtDoorwayChance <= addRoomSuccessChance)
                {
                    this.AddRoomAtDoorway(this.allRooms[currentRoomIndex].doorways[i]);
                }

                if (this.allRooms.Count >= this.maxRooms)
                {
                    break;
                }
            }
        }
    }

    private void AddRoomAtDoorway(Border doorway)
    {
        RoomData chosenRoom = this.allRoomData[Random.Range(0, this.allRoomData.Length)];

        GameObject roomInstance = Instantiate(this.baseRoomPrefab, doorway.transform.position, new Quaternion(), this.parentTransform);
        Room roomComponent = roomInstance.GetComponent<Room>();
        roomComponent.roomData = chosenRoom;
        roomComponent.CreateRoom();

        roomInstance.name = ("Room_" + this.allRooms.Count);

        this.AlignRoomDoorways(roomComponent, doorway);

        this.allRooms.Add(roomComponent);
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
    
    private void AlignRoomDoorways(Room newRoom, Border targetDoorway)
    {        
        int attempts = 0;

        //Pick a random doorway
        Border chosenDoorway = newRoom.doorways[Random.Range(0, newRoom.doorways.Count)];

        Debug.LogError("Connecting new " + chosenDoorway.side + " Doorway to old " + targetDoorway.side + " Doorway.");

        float targetZRotation = this.GetTargetRotation(targetDoorway.side, chosenDoorway.side);                                                       

        Debug.LogError("Rotating: " + targetZRotation);

        Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, targetZRotation);

        newRoom.roomTransform.rotation = targetRotation;

        Vector3 moveOffset = targetDoorway.borderTransform.position - chosenDoorway.borderTransform.position;

        Debug.LogError("Offset: " + moveOffset);

        newRoom.roomTransform.position += moveOffset;

        chosenDoorway.connectedToRoom = true;
        targetDoorway.connectedToRoom = true;
    }

    private void SealUnusedDoorways()
    {

    }
}
