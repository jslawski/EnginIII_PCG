using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public override void Enter(Player controller)
    {
        base.Enter(controller);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (this.PressedMoveKey() == true)
        {
            this.controller.ChangeState(new MoveState());
        }
        else if (this.PressedAttackKey())
        {
            this.controller.ChangeState(new AttackState());
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    private bool PressedMoveKey()
    {
        return (Input.GetKey(this.controller.upKey) || Input.GetKey(this.controller.downKey) ||
                Input.GetKey(this.controller.leftKey) || Input.GetKey(this.controller.rightKey));
    }    
}
