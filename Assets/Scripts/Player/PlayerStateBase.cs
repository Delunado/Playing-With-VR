using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase : StateBase
{
    protected PlayerController player;
    public PlayerStateBase(PlayerController player) : base(player)
    {
        this.player = player;
    }
}
