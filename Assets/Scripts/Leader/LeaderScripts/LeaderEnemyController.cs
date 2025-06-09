using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyController : BaseEnemyController
{
    [Header("Line of Sight Settings")]
    public LineOfSightMono goonLineOfSight; // Line of sight específico para goons

    private NPCSSearching<StateEnum> searching;
    private float _lastTimeSawTarget = float.NegativeInfinity;
    private float _timeTargetVisibleThreshold = 2.5f;
    private GoonManager _goonManager;

    protected override void Awake()
    {
        base.Awake();
        // Obtener el GoonManager del ServiceLocator
        _goonManager = ServiceLocator.Instance.GetService<GoonManager>();

        // Si no se asignó un LineOfSight específico para goons, usar el mismo que para el target
        if (goonLineOfSight == null)
        {
            goonLineOfSight = _los;
        }
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
        searching = new NPCSSearching<StateEnum>(_model.transform, 10f);

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
        var search = new ActionNode(() => {
            if (searching != null && target != null)
            {
                searching.SetSearchTarget(target.transform);
            }
            _fsm.Transition(StateEnum.Searching);
        });

        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, patrol);
        var qSearchOver = new QuestionNode(() => searching?.IsSearchOver ?? false, qGoToZone, search);
        var qTargetOutOfPursuitRange = new QuestionNode(() => !QuestionTargetInPursuitRange(), qSearchOver, chase);
        var qCanAttack = new QuestionNode(() => QuestionCanAttack(), attack, qTargetOutOfPursuitRange);

        // Lógica de detección del target (player)
        var qTargetInView = new QuestionNode(() => TargetWasSeenRecently() || _isChasing, qCanAttack, patrol);

        // Nueva pregunta: ¿Hay algún goon evadiendo que pueda ver?
        var qGoonEvadingInSight = new QuestionNode(() => QuestionGoonEvadingInSight(), search, qTargetInView);

        _root = new QuestionNode(() => target != null, qGoonEvadingInSight, patrol);
    }

    protected override void Update()
    {
        base.Update();
        if (QuestionTargetInView())
        {
            _lastTimeSawTarget = Time.time;
        }
    }

    private bool TargetWasSeenRecently()
    {
        return (Time.time - _lastTimeSawTarget) < _timeTargetVisibleThreshold;
    }

    // Nueva función para verificar si hay goons evadiendo en el campo de visión
    private bool QuestionGoonEvadingInSight()
    {
        if (_goonManager == null)
        {
            return false;
        }

        return _goonManager.HasEvadingGoonInSight(_los);
    }
}