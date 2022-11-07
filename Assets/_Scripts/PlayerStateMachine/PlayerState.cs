using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player controller;

    public virtual void Enter(Player controller)
    {
        this.controller = controller;
    }
    public virtual void Exit() { }

    public virtual void UpdateState()
    {
        if (Input.GetKeyDown(this.controller.interactKey))
        {
            this.controller.Interact();
        }
    }

    public virtual void FixedUpdateState() { }

    protected bool PressedInteractKey()
    {
        return (Input.GetKeyDown(this.controller.interactKey));
    }

    protected bool PressedAttackKey()
    {
        return (Input.GetKeyDown(this.controller.attackKey));
    }
}
