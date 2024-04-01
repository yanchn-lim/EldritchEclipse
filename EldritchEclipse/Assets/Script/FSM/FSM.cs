using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    protected Dictionary<int, FSM_State> _states;
    public FSM_State _currentState;

    public FSM()
    {
        _states = new();
    }
    
    public virtual void AddState(params FSM_State[] state)
    {
        foreach (var s in state)
        {
            _states.Add(s.Id, s);
            Debug.Log($"{s.ToString()} added.");
        }
    }

    public virtual FSM_State GetState(int id)
    {
        return _states[id];
    }

    public virtual void SwitchState(int id)
    {
        SwitchState(GetState(id));
    }

    public virtual void SwitchState(FSM_State NextState)
    {
        if(_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = NextState;

        if(_currentState != null)
            _currentState.Enter();
    }

    public virtual void Update()
    {
        if(_currentState != null)
            _currentState.Update();
    }

    public virtual void FixedUpdate()
    {
        if (_currentState != null)
            _currentState.FixedUpdate();
    }
}
