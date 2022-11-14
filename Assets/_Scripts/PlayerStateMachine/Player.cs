using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Creature
{    
    [SerializeField]
    private string stateName;

    public PlayerState currentState;

    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode attackKey = KeyCode.Space;
    public KeyCode interactKey = KeyCode.E;

    [HideInInspector]
    public int numHeldKeys = 0;

    protected override void Start()
    {
        base.Start();

        this.ChangeState(new IdleState());

        GameObject.Find("Main Camera").GetComponent<CameraFollow>().playerCreature = this;
    }
    
    protected override void Update()
    {
        base.Update();

        this.currentState.UpdateState();        
    }

    private void FixedUpdate()
    {
        this.currentState.FixedUpdateState();
    }

    public void ChangeState(PlayerState newState)
    {
        if (this.currentState != null)
        {
            this.currentState.Exit();
        }

        this.currentState = newState;
        this.currentState.Enter(this);

        this.stateName = this.currentState.GetType().ToString();
    }

    private void PreventPlayerMovement(ContactPoint impedingContact)
    {
        if (impedingContact.separation < 0)
        {
            this.creatureRb.position += (impedingContact.normal.normalized * Mathf.Abs(impedingContact.separation));
            this.creatureRb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            this.PreventPlayerMovement(collision.GetContact(0));
        }
    }
}
