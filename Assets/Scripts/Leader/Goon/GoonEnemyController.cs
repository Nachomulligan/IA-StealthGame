using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoonEnemyController : MonoBehaviour
{
    public Rigidbody target;
    FSM<StateEnum> _fsm;
    GoonEnemy _magikarp;

    private void Start()
    {
        _magikarp = GetComponent<GoonEnemy>();
        InitializeFSM();
    }
    void InitializeFSM()
    {
        var leaderBehaviour = GetComponent<LeaderBehaviour>();
        var obs = GetComponent<ObstacleAvoidance>();
        var flocking = GetComponent<FlockingManager>();

        _fsm = new FSM<StateEnum>();

        var idle = new GoonStateIdle<StateEnum>();
        var steering = new GoonStateSteering<StateEnum>(_magikarp, leaderBehaviour, target, flocking, obs);

        idle.AddTransition(StateEnum.Walk, steering);
        steering.AddTransition(StateEnum.Idle, idle);

        _fsm.SetInit(steering);
    }
    void Update()
    {
        _fsm.OnExecute();
    }
}