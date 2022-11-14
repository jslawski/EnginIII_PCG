using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState { Empty, Lava, Wall, Key, None }

public class FloorTile : MonoBehaviour
{
    [SerializeField]
    private Collider tileCollider;

    [HideInInspector]
    public TileState state = TileState.None;

    private void UpdateState()
    {
        switch (this.state)
        {
            case TileState.Empty:
            case TileState.Key:
                this.tileCollider.enabled = false;
                break;
            case TileState.Lava:
                this.tileCollider.enabled = true;
                this.tileCollider.isTrigger = true;
                break;
            case TileState.Wall:
                this.tileCollider.enabled = true;
                this.tileCollider.isTrigger = false;
                break;            
            default:
                Debug.LogError("Unknown TileState: " + this.state.ToString() + " Unable to change state.");
                break;
        }
    }

    public void SetState(TileState newState)
    {
        this.state = newState;
    }
}
