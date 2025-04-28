using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemysController : MonoBehaviour
{
    public Rigidbody target;
    public Transform zone;
    public float timePrediction;
    FSM<StateEnum> _fsm;
    RangedEnemyModel _model;
    LineOfSightMono _los;
    public List<Transform> patrolWaypoints;
    ITreeNode _root;
    ISteering _steering;
    private ISteering _patrolSteering;
    private ISteering _chaseSteering;
    private ISteering _goZoneSteering;
    private ISteering _evadeSteering;
    public bool _restTimeOut;
    public bool _patrolTimeOut;
    private bool _isChasing;
    public int restTime = 5;
    public int waitTime = 5;
    private NPCReactionSystem _reactionSystem;

    private void Awake()
    {
        _model = GetComponent<RangedEnemyModel>();
        _los = GetComponent<LineOfSightMono>();
        _reactionSystem = GetComponent<NPCReactionSystem>();
    }

    void Start()
    {
        InitializedSteering();
        InitializedFSM();
        InitializedTree();
    }

    void Update()
    {
        if (target != null)
        {
            _fsm.OnExecute();
            _root.Execute();
        }
    }

    private void FixedUpdate()
    {
        _fsm.OnFixExecute();
    }

    void InitializedSteering()
    {
        _chaseSteering = new Pursuit(_model.transform, target, 0, timePrediction);
        _evadeSteering = new Evade(_model.transform, target, 0, timePrediction);

        List<Vector3> waypoints = new List<Vector3>();
        foreach (var wp in patrolWaypoints)
        {
            if (wp != null)
                waypoints.Add(wp.position);
        }
        _patrolSteering = new PatrolToWaypoints(waypoints, _model.transform, 0.5f);
    }

    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

        var idle = new RangedEnemysIdle<StateEnum>();
        var attack = new RangedEnemysAttack<StateEnum>();
        var chase = new RangedEnemysSteering<StateEnum>(_chaseSteering);
        var goZone = new RangedEnemysChase<StateEnum>(zone);
        var patrol = new RangedEnemysPatrol<StateEnum>(_patrolSteering);
        var evade = new RangedEnemysEvade<StateEnum>(_evadeSteering);

        var stateList = new List<PSBase<StateEnum>> { idle, patrol, attack, chase, goZone, evade };

        // Transiciones
        idle.AddTransition(StateEnum.Chase, chase);
        idle.AddTransition(StateEnum.Spin, attack);
        idle.AddTransition(StateEnum.GoZone, goZone);
        idle.AddTransition(StateEnum.Patrol, patrol);
        idle.AddTransition(StateEnum.Evade, evade);

        attack.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);

        chase.AddTransition(StateEnum.Idle, idle);
        chase.AddTransition(StateEnum.Spin, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);
        chase.AddTransition(StateEnum.Patrol, patrol);

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Spin, attack);
        goZone.AddTransition(StateEnum.Idle, idle);

        patrol.AddTransition(StateEnum.Idle, idle);
        patrol.AddTransition(StateEnum.Chase, chase);
        patrol.AddTransition(StateEnum.Evade, evade);

        evade.AddTransition(StateEnum.Chase, chase);

        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Initialize(_model, look, _model);
        }

        _fsm.SetInit(idle);
    }

    //initialize desicion tree
    void InitializedTree()
    {
        //idle transition node w timer

        var idle = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Idle);
            StartCoroutine(idleTime());
        });

        //patrol transition node w timer
        var patrol = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Patrol);
            StartCoroutine(patrolTimer());
        });

        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Spin));
        var chase = new ActionNode(() => {
            _isChasing = true;
            _fsm.Transition(StateEnum.Chase);
        });
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));
        var evade = new ActionNode(() => _fsm.Transition(StateEnum.Evade));

        var qTargetOutOfPursuitRange = new QuestionNode(() => !QuestionTargetInPursuitRange(), patrol, chase);
        var qCanAttack = new QuestionNode(() => QuestionCanAttack(), attack, qTargetOutOfPursuitRange);
        var qShouldEvade = new QuestionNode(QuestionShouldEvade, evade, qCanAttack);
        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, idle);

        var qIsTired = new QuestionNode(() => QuestionIsTired(), idle, patrol);
        var qIsRested = new QuestionNode(() => QuestionIsRested(), patrol, idle);

        var qCurrentlyPatrolling = new QuestionNode(() => _fsm.CurrState() is NPCSPatrol<StateEnum>, qIsTired, qIsRested);

        var qTargetInView = new QuestionNode(() => QuestionTargetInView() || _isChasing, qShouldEvade, qCurrentlyPatrolling);

        _root = qTargetInView;
    }
    
    //Questions
    bool QuestionTargetInPursuitRange()
    {
        if (target == null) return false;
        bool inRange = Vector3.Distance(_model.Position, target.position) <= _model.pursuitRange;
        if (!inRange)
            _isChasing = false; // Out of range, come back
        return inRange;
    }

    bool QuestionShouldEvade()
    {
        return _reactionSystem.DecideIfShouldEvade();
    }

    bool QuestionCanAttack()
    {
        if (target == null) return false;
        return Vector3.Distance(_model.Position, target.position) <= _model.attackRange;
    }

    bool QuestionGoToZone()
    {
        return Vector3.Distance(_model.transform.position, zone.transform.position) > 0.25f;
    }

    bool QuestionTargetInView()
    {
        if (target == null) return false;
        return _los.LOS(target.transform);
    }

    bool QuestionIsRested()
    {
        return _restTimeOut;
    }

    bool QuestionIsTired()
    {
        return _patrolTimeOut;
    }

    //timers
    public IEnumerator idleTime()
    {
        _restTimeOut = false;
        yield return new WaitForSeconds(waitTime);
        _restTimeOut = true;
    }

    public IEnumerator patrolTimer()
    {
        _patrolTimeOut = false;
        yield return new WaitForSeconds(restTime);
        _patrolTimeOut = true;
    }
}
