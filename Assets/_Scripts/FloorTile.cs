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
}
