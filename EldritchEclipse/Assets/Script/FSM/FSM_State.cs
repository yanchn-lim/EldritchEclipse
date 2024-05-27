using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM_State
{
    protected FSM _fsm;
    protected int _id;

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void Exit() { }

    public FSM_State(int id,FSM fsm)
    {
        _id = id;
        _fsm = fsm;
    }

    public int Id { get { return _id; } }
}
