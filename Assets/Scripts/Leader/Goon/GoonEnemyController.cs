using System.Collections.Generic;
using UnityEngine;
public class GoonEnemyController : BaseFlockingEnemyController
{
    [Header("Goon Settings")]
    public float evadeTime = 3f;
    public Rigidbody leaderTarget; // Target para seguir como líder (diferente del player)
    private GoonEnemyModel _goon;

    private GoonStateEvade<StateEnum> _evadeState;

    protected override BaseFlockingEnemyModel GetEnemyModel()
    {
        return GetComponent<GoonEnemyModel>();
    }

    // Método para verificar si el líder sigue siendo válido
    public bool IsLeaderValid()
    {
        return leaderTarget != null && leaderTarget.gameObject != null;
    }

    protected override void InitializedFSM()
    {
        var flockingManager = GetComponent<FlockingManager>();
        _goon = GetEnemyModel() as GoonEnemyModel;
        _fsm = new FSM<StateEnum>();

        // Crear los estados
        var patrol = new GoonStatePatrol<StateEnum>(_goon, leaderTarget, flockingManager);
        _evadeState = new GoonStateEvade<StateEnum>(_goon, target, flockingManager, evadeTime);
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
        var patrol = new ActionNode(() => {
            Debug.Log($"[{Time.time:F2}] → TRANSITION: PATROL");
            _fsm.Transition(StateEnum.Patrol);
        });

        var evade = new ActionNode(() => {
            Debug.Log($"[{Time.time:F2}] → TRANSITION: EVADE");
            _fsm.Transition(StateEnum.Evade);
        });

        var goZone = new ActionNode(() => {
            Debug.Log($"[{Time.time:F2}] → TRANSITION: GO ZONE");
            _fsm.Transition(StateEnum.GoZone);
        });

        var doNothing = new ActionNode(() => {
            Debug.Log($"[{Time.time:F2}] → NO TRANSITION (still in EVADE)");
        });

        // Pregunta si el líder es válido (existe y no fue destruido)
        var qLeaderExists = new QuestionNode(
            () => IsLeaderValid(),

            // SI el líder existe → comportamiento normal
            new QuestionNode(
                () => _fsm.CurrState() is GoonStateEvade<StateEnum>,

                // Si estamos en evade → ¿terminó el tiempo?
                new QuestionNode(
                    () => _evadeState?.IsEvadeTimeOver ?? false,
                    patrol,
                    doNothing
                ),

                // Si NO estamos en evade → ¿vemos al jugador?
                new QuestionNode(
                    () => QuestionTargetInView(),
                    evade,
                    patrol
                )
            ),

            // SI el líder es null → ir a zona
            new QuestionNode(
                () => QuestionGoToZone(),
                goZone,
                patrol // Si ya llegó a la zona, puede patrullar sin líder
            )
        );

        _root = qLeaderExists;
    }

    // Método adicional para verificar si debe ir a zona cuando no hay líder
    protected bool QuestionShouldGoToZoneWithoutLeader()
    {
        // Cuando no hay líder, siempre debe ir a zona si no está cerca
        return QuestionGoToZone();
    }
}