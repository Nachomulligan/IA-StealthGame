using System.Collections.Generic;
using UnityEngine;
public class GoonEnemyController : BaseFlockingEnemyController
{
    [Header("Goon Settings")]
    public float evadeTime = 3f;
    public float zoneArrivalThreshold = 2f;
    public Rigidbody leaderTarget;

    private GoonEnemyModel _goon;
    private GoonStateEvade<StateEnum> _evadeState;
    private GoonStateGoZone<StateEnum> _goZoneState;
    private GoonStateIdle<StateEnum> _idleState;

    // Propiedad pública para verificar si está evadiendo
    public bool IsEvading { get; private set; }

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

        var patrol = new GoonStatePatrol<StateEnum>(_goon, leaderTarget, flockingManager);
        _evadeState = new GoonStateEvade<StateEnum>(_goon, target, flockingManager, evadeTime);
        _goZoneState = new GoonStateGoZone<StateEnum>(_goon, zone, flockingManager, zoneArrivalThreshold);
        _idleState = new GoonStateIdle<StateEnum>(_goon, flockingManager);

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

    protected override void Update()
    {
        base.Update();

        // Actualizar el estado de evading basado en el estado actual de la FSM
        IsEvading = _fsm.CurrState() is GoonStateEvade<StateEnum>;

        // Notificar al model sobre el cambio de estado
        if (_goon != null)
        {
            _goon.SetEvadingState(IsEvading);
        }
    }

    protected override void InitializedTree()
    {
        var patrol = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Patrol);
        });

        var evade = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Evade);
        });

        var goZone = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.GoZone);
        });

        var idle = new ActionNode(() =>
        {
            _fsm.Transition(StateEnum.Idle);
        });

        var qLeaderExists = new QuestionNode(
            () => IsLeaderValid(),
            new QuestionNode(
                () => _fsm.CurrState() is GoonStateEvade<StateEnum>,
                new QuestionNode(
                    () => _evadeState?.IsEvadeTimeOver ?? false,
                    patrol,
                    evade
                ),
                new QuestionNode(
                    () => QuestionTargetInView(),
                    evade,
                    patrol
                )
            ),
            new QuestionNode(
                () => _fsm.CurrState() is GoonStateEvade<StateEnum>,
                new QuestionNode(
                    () => _evadeState?.IsEvadeTimeOver ?? false,
                    new QuestionNode(
                        () => Vector3.Distance(_goon.transform.position, zone.position) <= zoneArrivalThreshold,
                        idle,
                        goZone
                    ),
                    evade
                ),
                new QuestionNode(
                    () => _fsm.CurrState() is GoonStateIdle<StateEnum>,
                    new QuestionNode(
                        () => QuestionTargetInView(),
                        evade,
                        idle
                    ),

                    new QuestionNode(
                        () => _fsm.CurrState() is GoonStateGoZone<StateEnum>,
                        new QuestionNode(
                            () => QuestionTargetInView(),
                            evade,
                            new QuestionNode(
                                () => _goZoneState.HasArrivedAtZone,
                                idle,
                                goZone
                            )
                        ),
                        goZone
                    )
                )
            )
        );

        _root = qLeaderExists;
    }
}