using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Phase currentState = null;
    private Phase previousState = null;

    public void ChangeState(Phase newState)
    {
        if (this.currentState != null)
        {
            
            this.currentState.Exit();
        }
        this.previousState = this.currentState;

        this.currentState = newState;
        this.currentState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        var runningState = this.currentState;
        if (this.currentState != null)
        {
           
            runningState.Execute();
        }
        else
        {
            return;
        }
    }

    public void SwitchToPreviousState()
    {
        ChangeState(this.previousState);
    }

    public PhaseBuilder.PhaseType ReturnCurrentState()
    {
        if (this.currentState != null)
        {
            return this.currentState.phaseType;
        }
        else
            return new PhaseBuilder.PhaseType();
    }

}
