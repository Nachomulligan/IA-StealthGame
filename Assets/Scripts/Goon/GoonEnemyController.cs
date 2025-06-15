using System.Collections.Generic;
using UnityEngine;
public class GoonEnemyController : BaseFlockingEnemyController
{
    [Header("Goon Settings")]
    public float evadeTime = 3f;
    public float zoneArrivalThreshold = 2f;
    public Rigidbody leaderTarget;

    [Header("Flocking Configuration")]
    [SerializeField] private FlockingConfiguration flockingConfig;

    private GoonEnemyModel _goon;
    private GoonStateEvadeConfigurable<StateEnum> _evadeState;
    private GoonStateGoZoneConfigurable<StateEnum> _goZoneState;
    private GoonStateIdleConfigurable<StateEnum> _idleState;

    protected override BaseFlockingEnemyModel GetEnemyModel()
    {
        return GetComponent<GoonEnemyModel>();
    }

    public bool IsLeaderValid()
    {
        return leaderTarget != null && leaderTarget.gameObject != null;
    }

    protected override void InitializedFSM()
    {
        var flockingManager = GetComponent<FlockingManager>();
        _goon = GetEnemyModel() as GoonEnemyModel;
        _fsm = new FSM<StateEnum>();

        // Estados configurables que usan el ScriptableObject
        var patrol = new GoonStatePatrolConfigurable<StateEnum>(_goon, leaderTarget, flockingManager, flockingConfig);
        _evadeState = new GoonStateEvadeConfigurable<StateEnum>(_goon, target, flockingManager, flockingConfig, evadeTime);
        _goZoneState = new GoonStateGoZoneConfigurable<StateEnum>(_goon, zone, flockingManager, flockingConfig, zoneArrivalThreshold);
        _idleState = new GoonStateIdleConfigurable<StateEnum>(_goon, flockingManager, flockingConfig);

        // Transiciones (sin cambios)
        patrol.AddTransition(StateEnum.Evade, _evadeState);
        patrol.AddTransition(StateEnum.GoZone, _goZoneState);

        _evadeState.AddTransition(StateEnum.GoZone, _goZoneState);
        _evadeState.AddTransition(StateEnum.Patrol, patrol);
        _evadeState.AddTransition(StateEnum.Idle, _idleState);

        _goZoneState.AddTransition(StateEnum.Patrol, patrol);
        _goZoneState.AddTransition(StateEnum.Evade, _evadeState);
        _goZoneState.AddTransition(StateEnum.Idle, _idleState);

        _idleState.AddTransition(StateEnum.Evade, _evadeState);
        _idleState.AddTransition(StateEnum.GoZone, _goZoneState);
        _idleState.AddTransition(StateEnum.Patrol, patrol);

        _fsm.SetInit(patrol);
    }

    protected override void InitializedTree()
    {
        // Acciones
        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));
        var evade = new ActionNode(() => _fsm.Transition(StateEnum.Evade));
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));
        var idle = new ActionNode(() => _fsm.Transition(StateEnum.Idle));

        var qIsEvadeTimeOver = new QuestionNode(
            () => _evadeState?.IsEvadeTimeOver ?? false,
            patrol,
            evade
        );

        var qTargetInView = new QuestionNode(
            () => QuestionTargetInView(),
            evade,
            patrol
        );

        var qAtZone = new QuestionNode(
            () => Vector3.Distance(_goon.transform.position, zone.position) <= zoneArrivalThreshold,
            idle,
            goZone
        );

        var qEvadeTimeOverNoLeader = new QuestionNode(
            () => _evadeState?.IsEvadeTimeOver ?? false,
            qAtZone,
            evade
        );

        var qIdleTargetCheck = new QuestionNode(
            () => QuestionTargetInView(),
            evade,
            idle
        );

        var qGoZoneTargetCheck = new QuestionNode(
            () => QuestionTargetInView(),
            evade,
            new QuestionNode(
                () => _goZoneState.HasArrivedAtZone,
                idle,
                goZone
            )
        );

        var qCurrentStateGoZone = new QuestionNode(
            () => _fsm.CurrState() is GoonStateGoZoneConfigurable<StateEnum>,
            qGoZoneTargetCheck,
            goZone
        );

        var qCurrentStateIdle = new QuestionNode(
            () => _fsm.CurrState() is GoonStateIdleConfigurable<StateEnum>,
            qIdleTargetCheck,
            qCurrentStateGoZone
        );

        var qCurrentStateEvadeNoLeader = new QuestionNode(
            () => _fsm.CurrState() is GoonStateEvadeConfigurable<StateEnum>,
            qEvadeTimeOverNoLeader,
            qCurrentStateIdle
        );

        var qCurrentStateEvadeWithLeader = new QuestionNode(
            () => _fsm.CurrState() is GoonStateEvadeConfigurable<StateEnum>,
            qIsEvadeTimeOver,
            qTargetInView
        );

        var qLeaderExists = new QuestionNode(
            () => IsLeaderValid(),
            qCurrentStateEvadeWithLeader, 
            qCurrentStateEvadeNoLeader   
        );

        _root = qLeaderExists;
    }
}
