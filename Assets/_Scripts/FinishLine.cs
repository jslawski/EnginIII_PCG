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

    [HideInInspector]
    public int numRequiredKeys = 0;

    private void Start()
    {
        this.lockCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        this.remainingKeys.text = this.numRequiredKeys.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.collidingWithPlayer = true;
            Player playerReference = collision.gameObject.GetComponent<Player>();
            this.StopAllCoroutines();
            StartCoroutine(this.TakeKeys(playerReference));

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.collidingWithPlayer = false;
        }
    }

    private IEnumerator TakeKeys(Player playerReference)
    {
        while (this.collidingWithPlayer && playerReference.numHeldKeys > 0)
        {
            playerReference.numHeldKeys--;
            this.numRequiredKeys--;

            if (this.numRequiredKeys <= 0)
            {
                this.UnlockDoor();
                break;
            }

            yield return new WaitForSeconds(this.delayBetweenKeys);
        }
    }

    private void UnlockDoor()
    {
        this.borderParent.EnableDoorway();
    }
}
