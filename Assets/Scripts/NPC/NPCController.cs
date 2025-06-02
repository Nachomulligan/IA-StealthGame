using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Rigidbody target;
    public Transform zone;
    public float timePrediction;
    FSM<StateEnum> _fsm;
    protected NPCModel _model;
    protected LineOfSightMono _los;
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
    protected NPCReactionSystem _reactionSystem;
    StatePathfinding<StateEnum> _statePathfinding;
    public Node start;
    public Node goal;


    protected virtual void Awake()
    {
        _model = GetComponent<NPCModel>();
        _los = GetComponent<LineOfSightMono>();
        _reactionSystem = GetComponent<NPCReactionSystem>();
    }

    void Start()
    {
        InitializedFSM();
        InitializedTree();
    }

    void Update()
    {
         _fsm.OnExecute();
         _root.Execute();
    }

    private void FixedUpdate()
    {
        _fsm.OnFixExecute();
    }

    protected virtual PSBase<StateEnum> CreateIdleState()
    {
        return new NPCSIdle<StateEnum>(restTime);
    }
    void InitializedFSM() 
    {
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

        var idle = new NPCSIdle<StateEnum>(restTime);
        var attack = new NPCSAttack<StateEnum>();
        var chase = new NPCSSteering<StateEnum>(new Pursuit(_model.transform, target, 0, timePrediction));
        var goZone = new NPCSSeek<StateEnum>(zone);

        List<Vector3> waypoints = new List<Vector3>();
        foreach (var wp in patrolWaypoints)
        {
            if (wp != null)
                waypoints.Add(wp.position);
        }

        var patrol = new NPCSPatrol<StateEnum>(new PatrolToWaypoints(waypoints, _model.transform, 0.5f), 5f);
        var evade = new NPCSSteering<StateEnum>(new Evade(_model.transform, target, 0, timePrediction)); 

        var stateList = new List<PSBase<StateEnum>> { idle, patrol, attack, chase, goZone, evade };

        // Transitions
        idle.AddTransition(StateEnum.Chase, chase);
        idle.AddTransition(StateEnum.Attack, attack);
        idle.AddTransition(StateEnum.GoZone, goZone);
        idle.AddTransition(StateEnum.Patrol, patrol);
        idle.AddTransition(StateEnum.Evade, evade);

        attack.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);

        chase.AddTransition(StateEnum.Idle, idle);
        chase.AddTransition(StateEnum.Attack, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);
        chase.AddTransition(StateEnum.Patrol, patrol);

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Attack, attack);
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
        var idle = new ActionNode(() => _fsm.Transition(StateEnum.Idle));

        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));

        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Attack));
        var chase = new ActionNode(() => {
            _isChasing = true; 
            _fsm.Transition(StateEnum.Chase);
        });
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));
        var evade = new ActionNode(() => _fsm.Transition(StateEnum.Evade));

        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, idle);
        var qTargetOutOfPursuitRange = new QuestionNode(() => !QuestionTargetInPursuitRange(), qGoToZone, chase);
        var qCanAttack = new QuestionNode(() => QuestionCanAttack(), attack, qTargetOutOfPursuitRange);
        var qShouldEvade = new QuestionNode(QuestionShouldEvade, evade, qCanAttack);

        var qIsTired = new QuestionNode(() =>
            (_fsm.CurrState() as NPCSPatrol<StateEnum>)?.IsTired ?? false,
            idle, patrol);
        var qIsRested = new QuestionNode(() =>
            (_fsm.CurrState() as NPCSIdle<StateEnum>)?.IsRested ?? false,
            patrol, idle);

        var qCurrentlyPatrolling = new QuestionNode(() => _fsm.CurrState() is NPCSPatrol<StateEnum>, qIsTired, qIsRested);

        var qTargetInView = new QuestionNode(() => QuestionTargetInView() || _isChasing, qShouldEvade, qCurrentlyPatrolling);

        _root = new QuestionNode(() => target != null, qTargetInView, idle);;
    }

    //Questions
    bool QuestionTargetInPursuitRange()
    {
        if (target == null) return false;
        bool inRange = Vector3.Distance(_model.Position, target.position) <= _model.PursuitRange;
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
    public void ASTARPlusVector()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.SetPathAStarPlusVector();
    }
}