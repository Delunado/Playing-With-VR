using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJetpackState : PlayerStateBase
{
    public PlayerJetpackState(PlayerController player) : base(player)
    {
    }

    public override void FixedTick()
    {
        if (player.CheckMovementModeChanged())
        {
            AddState(new PlayerGroundedState(player), 1);
        }

        player.GetMovementInput();

        player.JetpackMovement();

        player.SetState(stateQueue);
    }

    public override void Tick()
    {
        
    }

    public override void OnStateEnter()
    {
        player.ResetMovement();
    }
}
