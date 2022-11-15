using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{    
    private AudioSource pickupAudio;

    private void Start()
    {
        this.pickupAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player playerReference = other.gameObject.GetComponent<Player>();            
            GameManager.collectedKeys++;
            this.pickupAudio.Play();
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }    
}
