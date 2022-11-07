using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int numRooms = 8;

    [SerializeField]
    private Transform parentTransform;

    private Room currentRoom;

    [SerializeField]
    private GameObject baseRoomPrefab;

    private RoomData[] allRoomData;

    // Start is called before the first frame update
    void Start()
    {
        this.allRoomData = Resources.LoadAll<RoomData>("RoomData");
        this.GenerateDungeon();    
    }

    private void GenerateDungeon()
    {
        this.GenerateInitialRoom();
    }

    private void GenerateInitialRoom()
    {
        RoomData chosenRoom = this.allRoomData[Random.Range(0, this.allRoomData.Length)];

        GameObject roomInstance = Instantiate(this.baseRoomPrefab, Vector3.zero, new Quaternion(), this.parentTransform);
        Room roomComponent = roomInstance.GetComponent<Room>();
        roomComponent.roomData = chosenRoom;
        roomComponent.CreateRoom();
    }
}
