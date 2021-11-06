using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerStateBase
{
    public PlayerGroundedState(PlayerController player) : base(player)
    {
    }

    public override void FixedTick()
    {
        if (player.CheckMovementModeChanged() && !player.IsGrounded())
        {
            AddState(new PlayerJetpackState(player), 1);
        }

        player.GetMovementInput();

        player.GroundMovement();

        player.SetState(stateQueue);
    }

    public override void Tick()
    {
        
    }
}
