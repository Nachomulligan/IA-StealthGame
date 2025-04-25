using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSSpin<T> : PSBase<T>
{
    float _timer;
    float _seconds = 4;
    T _inputFinish;
    public PSSpin(T inputFinish, float seconds = 4)
    {
        _inputFinish = inputFinish;
        _seconds = seconds;
    }
    public override void Enter()
    {
        base.Enter();
        _timer = _seconds;
        _attack.Attack();
        _move.Move(Vector3.zero);
    }
    public override void Execute()
    {
        base.Execute();
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            StateMachine.Transition(_inputFinish);
        }
    }
}
