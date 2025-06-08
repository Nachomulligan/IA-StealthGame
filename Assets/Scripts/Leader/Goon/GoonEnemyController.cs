using System.Collections.Generic;
using UnityEngine;
public class GoonEnemyController : BaseFlockingEnemyController
{
    [Header("Goon Settings")]
    public float evadeTime = 3f;
    public Rigidbody leaderTarget; // Target para seguir como líder (diferente del player)
    private GoonEnemyModel _goon;
    private float _lastTimeSawTarget = float.NegativeInfinity;

    private GoonStateEvade<StateEnum> _evadeState;

    protected override BaseFlockingEnemyModel GetEnemyModel()
    {
        return GetComponent<GoonEnemyModel>();
    }

    protected override void InitializedFSM()
    {
        var flockingManager = GetComponent<FlockingManager>();
        _goon = GetEnemyModel() as GoonEnemyModel;
        _fsm = new FSM<StateEnum>();

        // Crear los estados
        var patrol = new GoonStatePatrol<StateEnum>(_goon, leaderTarget, flockingManager);
        _evadeState = new GoonStateEvade<StateEnum>(_goon, target, flockingManager, evadeTime); // Pasamos evadeTime aquí
        var goZone = new GoonStateGoZone<StateEnum>(_goon, zone, flockingManager);

        // Configurar transiciones
        patrol.AddTransition(StateEnum.Evade, _evadeState);
        patrol.AddTransition(StateEnum.GoZone, goZone);
        _evadeState.AddTransition(StateEnum.GoZone, goZone);
        _evadeState.AddTransition(StateEnum.Patrol, patrol);
        goZone.AddTransition(StateEnum.Patrol, patrol);
        goZone.AddTransition(StateEnum.Evade, _evadeState);

        _fsm.SetInit(patrol);
    }

    protected override void InitializedTree()
    {
        // Acciones
        var patrol = new ActionNode(() => _fsm.Transition(StateEnum.Patrol));
        var evade = new ActionNode(() => _fsm.Transition(StateEnum.Evade));
        var goZone = new ActionNode(() => _fsm.Transition(StateEnum.GoZone));

        // Preguntas
        var qGoToZone = new QuestionNode(() => QuestionGoToZone(), goZone, patrol);

        // Solo verificar si el tiempo de evade terminó cuando estamos evadiendo
        var qEvadeTimeOver = new QuestionNode(() =>
            _fsm.CurrState() is GoonStateEvade<StateEnum> && (_evadeState?.IsEvadeTimeOver ?? false),
            patrol, evade);

        var qTargetInView = new QuestionNode(() => QuestionTargetInView(), qEvadeTimeOver, patrol);

        _root = qTargetInView;
    }
    protected override void Update()
    {
        base.Update();
        // Actualizar el tiempo cuando vemos al player (target heredado)
        if (QuestionTargetInView())
        {
            _lastTimeSawTarget = Time.time;
        }
    }
}