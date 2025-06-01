using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyController : MonoBehaviour
{
    public Rigidbody target;
    public Transform zone;
    public float timePrediction;
    FSM<StateEnum> _fsm;
    protected LeaderEnemyModel _model;
    protected LineOfSightMono _los;
    public List<Transform> patrolWaypoints;
    ITreeNode _root;
    private bool _isChasing;

    protected virtual void Awake()
    {
        _model = GetComponent<LeaderEnemyModel>();
        _los = GetComponent<LineOfSightMono>();
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

    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

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

        var stateList = new List<PSBase<StateEnum>> { patrol, attack, chase, goZone };

        // Transitions
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);
        attack.AddTransition(StateEnum.Patrol, patrol);

        chase.AddTransition(StateEnum.Attack, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);
        chase.AddTransition(StateEnum.Patrol, patrol);

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Attack, attack);
        goZone.AddTransition(StateEnum.Patrol, patrol);

        patrol.AddTransition(StateEnum.Chase, chase);
        patrol.AddTransition(StateEnum.Attack, attack);
        patrol.AddTransition(StateEnum.GoZone, goZone);

        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Initialize(_model, look, _model);
        }

        _fsm.SetInit(patrol); // Inicia directamente en patrol
    }

    void InitializedTree()
    {
        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));
        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Attack));
        var chase = new ActionNode(() => {
            _isChasing = true;
            _fsm.Transition(StateEnum.Chase);
        });
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));

        // Pregunta si debe volver a la zona
        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, patrol);

        // Pregunta si el target est? fuera del rango de pursuit
        var qTargetOutOfPursuitRange = new QuestionNode(() => !QuestionTargetInPursuitRange(), qGoToZone, chase);

        // Pregunta si puede atacar
        var qCanAttack = new QuestionNode(() => QuestionCanAttack(), attack, qTargetOutOfPursuitRange);

        // Pregunta si el target est? en vista
        var qTargetInView = new QuestionNode(() => QuestionTargetInView() || _isChasing, qCanAttack, patrol);

        _root = new QuestionNode(() => target != null, qTargetInView, patrol);
    }

    // Questions
    bool QuestionTargetInPursuitRange()
    {
        if (target == null) return false;
        bool inRange = Vector3.Distance(_model.Position, target.position) <= _model.PursuitRange;
        if (!inRange)
            _isChasing = false; // Fuera de rango, volver
        return inRange;
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
}
