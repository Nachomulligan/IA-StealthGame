using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Rigidbody target;
    public Transform zone;
    public float timePrediction;
    FSM<StateEnum> _fsm;
    NPCModel _model;
    LineOfSightMono _los;
    public List<Transform> patrolWaypoints;
    ITreeNode _root;
    ISteering _steering;
    private ISteering _patrolSteering;
    private ISteering _chaseSteering;
    private ISteering _goZoneSteering;
    public bool _restTimeOut;
    public bool _patrolTimeOut;
    public int restTime = 5;
    public int waitTime = 5;

    private void Awake()
    {
        _model = GetComponent<NPCModel>();
        _los = GetComponent<LineOfSightMono>();
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

        var idle = new NPCSIdle<StateEnum>();
        var attack = new NPCSAttack<StateEnum>();
        var chase = new NPCSSteering<StateEnum>(_chaseSteering);
        var goZone = new NPCSChase<StateEnum>(zone);
        var patrol = new NPCSPatrol<StateEnum>(_patrolSteering);

        var stateList = new List<PSBase<StateEnum>>();
        stateList.Add(idle);
        stateList.Add(patrol);
        stateList.Add(attack);
        stateList.Add(chase);
        stateList.Add(goZone);

        // TRANSICIONES
        idle.AddTransition(StateEnum.Chase, chase);
        idle.AddTransition(StateEnum.Spin, attack);
        idle.AddTransition(StateEnum.GoZone, goZone);
        idle.AddTransition(StateEnum.Patrol, patrol);

        attack.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);

        chase.AddTransition(StateEnum.Idle, idle);
        chase.AddTransition(StateEnum.Spin, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);
        chase.AddTransition(StateEnum.Patrol, patrol); // 👈 NUEVA transición: chase puede volver a patrullar

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Spin, attack);
        goZone.AddTransition(StateEnum.Idle, idle);

        patrol.AddTransition(StateEnum.Idle, idle);
        patrol.AddTransition(StateEnum.Chase, chase);

        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Initialize(_model, look, _model);
        }

        _fsm.SetInit(idle);
    }

    void InitializedTree()
    {
        var idle = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Idle);
            StartCoroutine(idleTime());
        });

        var patrol = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Patrol);
            StartCoroutine(patrolTimer());
        });

        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Spin));
        var chase = new ActionNode(() => _fsm.Transition(StateEnum.Chase));
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));

        // NUEVAS QUESTIONS
        var qTargetOutOfSight = new QuestionNode(() => !QuestionTargetInView(), patrol, chase);
        var qCanAttack = new QuestionNode(QuestionCanAttack, attack, qTargetOutOfSight);
        var qGoToZone = new QuestionNode(QuestionGoToZone, goZone, idle);

        var qIsTired = new QuestionNode(QuestionIsTired, idle, patrol);
        var qIsRested = new QuestionNode(QuestionIsRested, patrol, idle);

        var qCurrentlyPatrolling = new QuestionNode(() => _fsm.CurrState() is NPCSPatrol<StateEnum>, qIsTired, qIsRested);
        var qTargetInView = new QuestionNode(QuestionTargetInView, qCanAttack, qCurrentlyPatrolling);

        _root = qTargetInView;
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
    public IEnumerator idleTime()
    {
        //Debug.Log("Empieza IdleTime: esperando " + waitTime + " segundos.");
        _restTimeOut = false;
        yield return new WaitForSeconds(waitTime);
        _restTimeOut = true;
        //Debug.Log("Termina IdleTime: puede patrullar.");
    }

    public IEnumerator patrolTimer()
    {
        //Debug.Log("Empieza PatrolTimer: patrullando " + restTime + " segundos.");
        _patrolTimeOut = false;
        yield return new WaitForSeconds(restTime);
        _patrolTimeOut = true;
        //Debug.Log("Termina PatrolTimer: debe descansar (Idle).");
    }
}