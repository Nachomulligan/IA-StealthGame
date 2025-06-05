using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyController : BaseController
{
    [Header("Enemy Controller")]
    public List<Transform> patrolWaypoints;
    public int restTime = 5;
    public int waitTime = 5;

    protected BaseEnemyModel _model;
    protected NPCReactionSystem _reactionSystem;
    protected ITreeNode _root;

    protected override void Awake()
    {
        base.Awake();
        _model = GetEnemyModel();
        _reactionSystem = GetComponent<NPCReactionSystem>();
    }

    protected abstract BaseEnemyModel GetEnemyModel();
    protected override ITreeNode GetDecisionTree() => _root;

    // Métodos comunes para las preguntas del árbol de decisión
    protected bool QuestionTargetInPursuitRange()
    {
        if (target == null) return false;
        bool inRange = Vector3.Distance(_model.Position, target.position) <= _model.PursuitRange;
        if (!inRange)
            _isChasing = false;
        return inRange;
    }

    protected bool QuestionCanAttack()
    {
        if (target == null) return false;
        return Vector3.Distance(_model.Position, target.position) <= _model.attackRange;
    }

    protected bool QuestionGoToZone()
    {
        return Vector3.Distance(_model.transform.position, zone.transform.position) > 0.25f;
    }

    protected bool QuestionTargetInView()
    {
        if (target == null) return false;
        return _los.LOS(target.transform);
    }
}