using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : BaseEnemyController
{
    public Transform SearchTarget;
    private NPCSSearching<StateEnum> searching;
    private int _entityId;
    private TargetTrackingService _trackingService;

    protected override void Awake()
    {
        base.Awake();
        _entityId = GetInstanceID(); 
    }
    protected override void Start()
    {
        base.Start();

        // Obtener el servicio de tracking
        _trackingService = ServiceLocator.Instance.GetService<TargetTrackingService>();

        // Registrar este NPC en el servicio de tracking
        if (_trackingService != null && target != null)
        {
            _trackingService.RegisterTracker(_entityId, target, _los, transform, 2.5f);
        }
    }

    private void OnDestroy()
    {
        // Limpiar el registro cuando se destruye el objeto
        if (_trackingService != null)
        {
            _trackingService.UnregisterTracker(_entityId);
        }
    }
    protected override BaseEnemyModel GetEnemyModel()
    {
        return GetComponent<RangedEnemyModel>();
    }

    protected override void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

        var idle = new RangedEnemysIdle<StateEnum>(restTime);
        var attack = new NPCSAttack<StateEnum>();
        var chase = new NPCSSteering<StateEnum>(new Pursuit(_model.transform, target, 0, timePrediction));
        var goZone = new NPCSSeek<StateEnum>(zone);
        searching = new NPCSSearching<StateEnum>(_model.transform, SearchTarget, _entityId, 10f);

        List<Vector3> waypoints = new List<Vector3>();
        foreach (var wp in patrolWaypoints)
        {
            if (wp != null)
                waypoints.Add(wp.position);
        }

        var patrol = new NPCSPatrol<StateEnum>(new PatrolToWaypoints(waypoints, _model.transform, 0.5f), 5f);
        var evade = new NPCSSteering<StateEnum>(new Evade(_model.transform, target, 0, timePrediction));

        var stateList = new List<PSBase<StateEnum>> { idle, patrol, attack, chase, goZone, evade, searching };

        idle.AddTransition(StateEnum.Chase, chase);
        idle.AddTransition(StateEnum.Attack, attack);
        idle.AddTransition(StateEnum.GoZone, goZone);
        idle.AddTransition(StateEnum.Patrol, patrol);
        idle.AddTransition(StateEnum.Evade, evade);
        idle.AddTransition(StateEnum.Searching, searching);

        attack.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);
        attack.AddTransition(StateEnum.Searching, searching);

        chase.AddTransition(StateEnum.Idle, idle);
        chase.AddTransition(StateEnum.Attack, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);
        chase.AddTransition(StateEnum.Patrol, patrol);
        chase.AddTransition(StateEnum.Searching, searching);

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Attack, attack);
        goZone.AddTransition(StateEnum.Idle, idle);
        goZone.AddTransition(StateEnum.Searching, searching);

        patrol.AddTransition(StateEnum.Idle, idle);
        patrol.AddTransition(StateEnum.Chase, chase);
        patrol.AddTransition(StateEnum.Evade, evade);
        patrol.AddTransition(StateEnum.Searching, searching);

        evade.AddTransition(StateEnum.Chase, chase);
        evade.AddTransition(StateEnum.Searching, searching);

        searching.AddTransition(StateEnum.Chase, chase);
        searching.AddTransition(StateEnum.Attack, attack);
        searching.AddTransition(StateEnum.GoZone, goZone);
        searching.AddTransition(StateEnum.Idle, idle);

        foreach (var state in stateList)
        {
            state.Initialize(_model, look, _model);
        }

        _fsm.SetInit(idle);
    }

    protected override void InitializedTree()
    {
        var idle = new ActionNode(() => _fsm.Transition(StateEnum.Idle));
        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));
        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Attack));
        var chase = new ActionNode(() => {
            _isChasing = true;
            _fsm.Transition(StateEnum.Chase);
        });
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));
        var evade = new ActionNode(() => _fsm.Transition(StateEnum.Evade));
        var search = new ActionNode(() => _fsm.Transition(StateEnum.Searching));

        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, idle);
        var qSearchOver = new QuestionNode(() => searching?.IsSearchOver ?? false, qGoToZone, search);
        var qTargetOutOfPursuitRange = new QuestionNode(() => !QuestionTargetInPursuitRange(), qSearchOver, chase);
        var qCanAttack = new QuestionNode(() => QuestionCanAttack(), attack, qTargetOutOfPursuitRange);
        var qShouldEvade = new QuestionNode(() => _reactionSystem.DecideIfShouldEvade(), evade, qCanAttack);

        var qIsTired = new QuestionNode(() => (_fsm.CurrState() as NPCSPatrol<StateEnum>)?.IsTired ?? false, idle, patrol);
        var qIsRested = new QuestionNode(() => (_fsm.CurrState() as RangedEnemysIdle<StateEnum>)?.IsRested ?? false, patrol, idle);

        var qCurrentlyPatrolling = new QuestionNode(() => _fsm.CurrState() is NPCSPatrol<StateEnum>, qIsTired, qIsRested);
        var qTargetInView = new QuestionNode(() =>
            (_trackingService?.WasTargetSeenRecently(_entityId) ?? false) || _isChasing,
            qShouldEvade, qCurrentlyPatrolling);

        _root = new QuestionNode(() => target != null, qTargetInView, idle);
    }

    protected override void Update()
    {
        base.Update();
    }
}
