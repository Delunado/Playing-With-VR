using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public abstract class StateBase
{
    protected Entity entity;
    protected SimplePriorityQueue<StateBase, int> stateQueue;

    public abstract void Tick();
    public abstract void FixedTick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public StateBase(Entity entity)
    {
        stateQueue = new SimplePriorityQueue<StateBase, int>();
    }

    protected void AddState(StateBase state, int priority)
    {
        if (state != null)
            stateQueue.Enqueue(state, priority);
    }
}
