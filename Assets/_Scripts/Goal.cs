using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private GameObject victoryCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.victoryCanvas.SetActive(true);
            GameManager.dungeonComplete = true;
        }
    }
}
