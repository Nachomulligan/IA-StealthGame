using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    StatePathfinding<StateEnum> _statePathfinding;
    public Node start;
    public Node goal;
    PatrolToWaypoints _waypoints;
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
        var seek = new Seek(_model.transform, target.transform);
        var flee = new Flee(_model.transform, target.transform);
        var pursuit = new Pursuit(_model.transform, target, 0, timePrediction);
        var evade = new Evade(_model.transform, target, 0, timePrediction);

        // Creamos lista de Vector3 a partir de los transforms de los waypoints
        List<Vector3> waypoints = new List<Vector3>();
        foreach (var wp in patrolWaypoints)
        {
            if (wp != null)
                waypoints.Add(wp.position);
        }

        // Creamos el patrol steering
        var patrol = new PatrolToWaypoints(waypoints, _model.transform, 0.5f);

        // Asignamos el steering
        _steering = patrol; // Cambiamos esto a patrol para probar el patrullaje
    }
    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

        var idle = new NPCSIdle<StateEnum>();
        var attack = new NPCSAttack<StateEnum>();
        var chase = new NPCSSteering<StateEnum>(_steering);
        var goZone = new NPCSChase<StateEnum>(zone);
        var patrol = new NPCPatrolState<StateEnum>(_waypoints);

        var stateList = new List<PSBase<StateEnum>>();
        stateList.Add(idle);
        stateList.Add(attack);
        stateList.Add(chase);
        stateList.Add(goZone);
        stateList.Add(patrol);

        attack.AddTransition(StateEnum.Patrol, patrol);

        idle.AddTransition(StateEnum.Chase, chase);
        idle.AddTransition(StateEnum.Spin, attack);
        idle.AddTransition(StateEnum.GoZone, goZone);

        attack.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Patrol, patrol);
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.GoZone, goZone);

        chase.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Patrol, patrol);
        chase.AddTransition(StateEnum.Spin, attack);
        chase.AddTransition(StateEnum.GoZone, goZone);

        goZone.AddTransition(StateEnum.Chase, chase);
        goZone.AddTransition(StateEnum.Spin, attack);
        goZone.AddTransition(StateEnum.Idle, idle);
        attack.AddTransition(StateEnum.Patrol, patrol);

        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Initialize(_model, look, _model);
        }

        _fsm.SetInit(idle);
    }

    void InitializedTree()
    {
        var idle = new ActionNode(() => _fsm.Transition(StateEnum.Idle));
        var attack = new ActionNode(() => _fsm.Transition(StateEnum.Spin));
        var chase = new ActionNode(() => _fsm.Transition(StateEnum.Chase));
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));
        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));

        var qCanAttack = new QuestionNode(QuestionCanAttack, attack, chase);
        var qGoToZone = new QuestionNode(QuestionGoToZone, goZone, idle);
        var qTargetInView = new QuestionNode(QuestionTargetInView, qCanAttack, qGoToZone);

        _root = qCanAttack;
    }
    bool QuestionCanAttack()
    {
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


    //agregue el astar para el pathfinding
   
    public void ASTARPlusVector()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.SetPathAStarPlusVector();
    }
}