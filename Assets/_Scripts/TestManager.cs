using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dungeonGeneratorPrefab;   
    private GameObject dungeonGeneratorInstance;

    [SerializeField]
    private Slider minRoomSlider;
    [SerializeField]
    private Slider maxRoomSlider;

    [SerializeField]
    private TextMeshProUGUI minRoomValue;
    [SerializeField]
    private TextMeshProUGUI maxRoomValue;

    private void Update()
    {
        this.minRoomValue.text = this.minRoomSlider.value.ToString();
        this.maxRoomValue.text = this.maxRoomSlider.value.ToString();
    }

    private void ClearCurrentDungeon()
    {
        Destroy(GameObject.Find("Sphere(Clone)"));
        Destroy(this.dungeonGeneratorInstance);     
    }

    public void OnGeneratePressed()
    {
        this.ClearCurrentDungeon();

        this.dungeonGeneratorInstance = Instantiate(this.dungeonGeneratorPrefab, Vector3.zero, new Quaternion());

        DungeonGenerator dungeonGeneratorComponent = this.dungeonGeneratorInstance.GetComponent<DungeonGenerator>();
        dungeonGeneratorComponent.minRooms = (int)this.minRoomSlider.value;
        dungeonGeneratorComponent.maxRooms = (int)this.maxRoomSlider.value;
        dungeonGeneratorComponent.GenerateDungeon();
    }
}
