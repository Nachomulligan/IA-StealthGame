using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSWalk<T> : PSBase<T>
{
    T _inputToWalk;
    public PSWalk(T inputToIdle)
    {
        _inputToWalk = inputToIdle;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Execute()
    {
        var dir = new Vector3(InputManager.GetMove().x, 0, InputManager.GetMove().y);
        _move.Move(dir);
        if (dir != Vector3.zero)
        {
            _look.LookDir(dir);
        }
        else
        {
            StateMachine.Transition(_inputToWalk);
        }
    }
}
