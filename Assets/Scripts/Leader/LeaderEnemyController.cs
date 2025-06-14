using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyController : BaseEnemyController
{
    public Transform SearchTarget;
    private NPCSSearching<StateEnum> searching;
    private TargetTracker targetTracker;
    protected override void Awake()
    {
        base.Awake();
        targetTracker = GetComponent<TargetTracker>();
    }
    protected override BaseEnemyModel GetEnemyModel()
    {
        return GetComponent<LeaderEnemyModel>();
    }
    protected override void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

        var attack = new NPCSAttack<StateEnum>();
        var chase = new NPCSSteering<StateEnum>(new Pursuit(_model.transform, target, 0, timePrediction));
        var goZone = new NPCSSeek<StateEnum>(zone);
        searching = new NPCSSearching<StateEnum>(_model.transform, SearchTarget, targetTracker, 10f);

        List<Vector3> waypoints = new List<Vector3>();
        foreach (var wp in patrolWaypoints)
        {
            if (wp != null)
                waypoints.Add(wp.position);
        }

        var patrol = new NPCSPatrol<StateEnum>(new PatrolToWaypoints(waypoints, _model.transform, 0.5f), 5f);

        var stateList = new List<PSBase<StateEnum>> { patrol, attack, chase, goZone, searching };

        patrol.AddTransition(StateEnum.Chase, chase);
        patrol.AddTransition(StateEnum.Attack, attack);
        patrol.AddTransition(StateEnum.GoZone, goZone);
        patrol.AddTransition(StateEnum.Searching, searching);

        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);
        attack.AddTransition(StateEnum.Patrol, patrol);
        attack.AddTransition(StateEnum.Searching, searching);

        chase.AddTransition(StateEnum.Attack, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);
        chase.AddTransition(StateEnum.Patrol, patrol);
        chase.AddTransition(StateEnum.Searching, searching);

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Attack, attack);
        goZone.AddTransition(StateEnum.Patrol, patrol);
        goZone.AddTransition(StateEnum.Searching, searching);

        searching.AddTransition(StateEnum.Chase, chase);
        searching.AddTransition(StateEnum.Attack, attack);
        searching.AddTransition(StateEnum.GoZone, goZone);
        searching.AddTransition(StateEnum.Patrol, patrol);

        foreach (var state in stateList)
            state.Initialize(_model, look, _model);

        _fsm.SetInit(patrol);
    }

    protected override void InitializedTree()
    {
        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));
        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Attack));
        var chase = new ActionNode(() => {
            _isChasing = true;
            _fsm.Transition(StateEnum.Chase);
        });
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));
        var search = new ActionNode(() => _fsm.Transition(StateEnum.Searching));

        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, patrol);
        var qSearchOver = new QuestionNode(() => searching?.IsSearchOver ?? false, qGoToZone, search);
        var qTargetOutOfPursuitRange = new QuestionNode(() => !QuestionTargetInPursuitRange(), qSearchOver, chase);
        var qCanAttack = new QuestionNode(() => QuestionCanAttack(), attack, qTargetOutOfPursuitRange);
        var qTargetInView = new QuestionNode(() =>
            QuestionTargetInView() || _isChasing || (targetTracker?.WasTargetSeenRecently() ?? false),
            qCanAttack, patrol);

        _root = new QuestionNode(() => target != null, qTargetInView, patrol);
    }

    protected override void Update()
    {
        base.Update();
    }
}
