using System.Collections.Generic;
using UnityEngine;

public class SimplePathfindingController : MonoBehaviour
{
    [Header("Pathfinding Setup")]
    public Transform target;

    private NPCModel _model;
    private FSM<StateEnum> _fsm;
    StatePathfindingSimple<StateEnum> _pathfindingState;


    void Awake()
    {
        _model = GetComponent<NPCModel>();
    }

    private void Start()
    {
        InitializeFSM();
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StateEnum>();
        _pathfindingState = new StatePathfindingSimple<StateEnum>(_model.transform, _model, target);

        _fsm.SetInit(_pathfindingState);
    }

    private void Update()
    {
        _fsm.OnExecute();
    }

    private void FixedUpdate()
    {
        _fsm.OnFixExecute();
    }

    public void StartPathfinding()
    {
        if (_pathfindingState != null && target != null)
        {
            _pathfindingState.StartPathfinding();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (_pathfindingState != null)
        {
            _pathfindingState.SetTarget(newTarget);
        }
    }
}