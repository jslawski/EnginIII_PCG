using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player playerReference = other.gameObject.GetComponent<Player>();            
            GameManager.collectedKeys++;
            Destroy(this.gameObject);
        }
    }    
}
