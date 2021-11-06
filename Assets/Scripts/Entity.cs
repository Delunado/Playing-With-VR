using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Entity : MonoBehaviour
{
    #region State Vars&Methods
    protected StateBase currentState;

    public void SetState(StateBase state)
    {
        //Para hacer build
        if (currentState != null)
            currentState.OnStateExit();

        //Debug.Log("Estado: " + state);
        currentState = state; //Aqui cambiamos al nuevo estado

        if (currentState != null)
            currentState.OnStateEnter();
    }

    public void SetState(SimplePriorityQueue<StateBase, int> stateQueue)
    {
        if (stateQueue.TryDequeue(out StateBase state))
        {
            SetState(state);
        }
    }
    #endregion

    protected void InitialReferences()
    {
        
    }

    protected virtual void Awake()
    {
        InitialReferences();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentState.Tick();
    }

    protected virtual void FixedUpdate()
    {
        currentState.FixedTick();
    }
}
