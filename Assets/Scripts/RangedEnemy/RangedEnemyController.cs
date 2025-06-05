using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RangedEnemyController : BaseEnemyController
{
    protected override BaseEnemyModel GetEnemyModel()
    {
        return GetComponent<RangedEnemyModel>();
    }

    protected override void InitializedFSM()
    {
        // Similar a MeleeEnemyController pero usando RangedEnemysIdle
        _fsm = new FSM<StateEnum>();
        var look = GetComponent<ILook>();

        var idle = new RangedEnemysIdle<StateEnum>(restTime); // Usa el idle que rota
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

        // Mismas transiciones que MeleeEnemyController
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

    protected override void InitializedTree()
    {
        // Mismo árbol de decisión que MeleeEnemyController
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
        var qShouldEvade = new QuestionNode(() => _reactionSystem.DecideIfShouldEvade(), evade, qCanAttack);

        var qIsTired = new QuestionNode(() =>
            (_fsm.CurrState() as NPCSPatrol<StateEnum>)?.IsTired ?? false,
            idle, patrol);
        var qIsRested = new QuestionNode(() =>
            (_fsm.CurrState() as NPCSIdle<StateEnum>)?.IsRested ?? false,
            patrol, idle);

        var qCurrentlyPatrolling = new QuestionNode(() => _fsm.CurrState() is NPCSPatrol<StateEnum>, qIsTired, qIsRested);
        var qTargetInView = new QuestionNode(() => QuestionTargetInView() || _isChasing, qShouldEvade, qCurrentlyPatrolling);

        _root = new QuestionNode(() => target != null, qTargetInView, idle);
    }
}