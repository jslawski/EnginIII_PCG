using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    public override void Enter(Player controller)
    {
        base.Enter(controller);

        this.controller.Attack();        
    }

    public override void Exit()
    {
        base.Exit();        
    }

    public override void UpdateState()
    {
        //Don't allow players to pick up weapons or move while they're attacking
        //Transition to the idle state once the attack is completed
        if (this.controller.attackZone.attacking == false)
        {
            this.controller.ChangeState(new IdleState());
        }
    }    
}
