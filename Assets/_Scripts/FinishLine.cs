using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    private Border borderParent;
    [SerializeField]
    private TextMeshProUGUI remainingKeys;
    [SerializeField]
    private GameObject lockCanvas;

    private bool collidingWithPlayer = false;

    private float delayBetweenKeys = 0.3f;

    private AudioSource finishLineAudio;

    private void Start()
    {
        this.lockCanvas.SetActive(true);

        this.finishLineAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        this.remainingKeys.text = GameManager.totalKeys.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.collidingWithPlayer = true;            
            this.StopAllCoroutines();
            StartCoroutine(this.TakeKeys());

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.collidingWithPlayer = false;
        }
    }

    private IEnumerator TakeKeys()
    {
        this.finishLineAudio.clip = Resources.Load<AudioClip>("Audio/use");

        while (this.collidingWithPlayer && GameManager.collectedKeys > 0)
        {
            GameManager.collectedKeys--;
            GameManager.totalKeys--;

            if (GameManager.totalKeys <= 0)
            {
                this.UnlockDoor();
                break;
            }

            this.finishLineAudio.Play();
            
            yield return new WaitForSeconds(this.delayBetweenKeys);
        }
    }

    private void UnlockDoor()
    {
        this.borderParent.EnableDoorway();
        this.lockCanvas.SetActive(false);
    }
}
