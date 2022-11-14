using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    private Player playerReference;
    private TextMeshProUGUI[] textElements;
    
    // Start is called before the first frame update
    void Start()
    {
        this.textElements = GetComponentsInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //Lazily load player reference when the player is instantiated
        if (this.playerReference == null)
        {
            GameObject potentialFoundObject = GameObject.Find("Player(Clone)");
            if (potentialFoundObject != null)
            {
                this.playerReference = potentialFoundObject.GetComponent<Player>();
            }
            else
            {
                return;
            }
        }

        this.textElements[0].text = "HP: " + this.playerReference.currentHitPoints.ToString();
        this.textElements[1].text = "Keys: " + GameManager.collectedKeys.ToString() + "/" + GameManager.totalKeys.ToString();
    }
}
