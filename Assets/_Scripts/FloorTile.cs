using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState { Empty, Lava, Wall, Key, None }

public class FloorTile : MonoBehaviour
{
    [SerializeField]
    public Transform tileTransform;

    [SerializeField]
    private Collider tileCollider;

    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private Material floorMaterial;

    [SerializeField]
    private Material wallMaterial;

    [SerializeField]
    private Material lavaMaterial;

    [HideInInspector]
    public TileState state = TileState.None;

    private int lavaDamage = 5;
    private float lavaDelay = 0.5f;

    private Player player;

    private void UpdateState()
    {
        switch (this.state)
        {
            case TileState.Empty:
            case TileState.Key:
                this.tileCollider.enabled = false;
                this.tileRenderer.material = this.floorMaterial;
                this.gameObject.tag = "Floor";
                break;
            case TileState.Lava:
                this.tileCollider.enabled = true;
                this.tileCollider.isTrigger = true;
                this.tileRenderer.material = this.lavaMaterial;
                this.gameObject.tag = "Lava";
                break;
            case TileState.Wall:
                this.tileCollider.enabled = true;
                this.tileCollider.isTrigger = false;
                this.tileRenderer.material = this.wallMaterial;
                this.gameObject.tag = "Wall";
                break;            
            default:
                Debug.LogError("Unknown TileState: " + this.state.ToString() + " Unable to change state.");
                break;
        }
    }

    public void SetState(TileState newState)
    {
        this.state = newState;
        this.UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && this.state == TileState.Lava)
        {
            this.player = other.gameObject.GetComponent<Player>();
            this.StopAllCoroutines();
            StartCoroutine(this.BurnPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && this.state == TileState.Lava)
        {
            this.player = null;
            this.StopAllCoroutines();
        }
    }

    private IEnumerator BurnPlayer()
    {
        while (this.player != null)
        {
            this.player.TakeDamage(this.lavaDamage);
            yield return new WaitForSeconds(this.lavaDelay);
        }
    }
}
