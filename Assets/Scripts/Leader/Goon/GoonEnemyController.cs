using System.Collections.Generic;
using UnityEngine;
public class GoonEnemyController : BaseFlockingEnemyController
{
    GoonEnemyModel _goon;

    protected override BaseFlockingEnemyModel GetEnemyModel()
    {
        return GetComponent<GoonEnemyModel>();
    }

    protected override void InitializedFSM()

    {
        var flockingManager = GetComponent<FlockingManager>();
        _goon = GetEnemyModel() as GoonEnemyModel;
        _fsm = new FSM<StateEnum>();

        var idle = new GoonStateIdle<StateEnum>();
        var steering = new GoonStateSteering<StateEnum>(_goon, target, flockingManager);

        idle.AddTransition(StateEnum.Walk, steering);
        steering.AddTransition(StateEnum.Idle, idle);

        _fsm.SetInit(steering);
    }

    protected override void InitializedTree()
    {
        _root = new EmptyNode();
    }

}