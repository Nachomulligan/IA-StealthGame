using UnityEngine;
public abstract class BaseFlockingEnemyController : BaseController
{
    [Header("Enemy Controller")]
    protected BaseFlockingEnemyModel _model;
    protected ITreeNode _root;

    protected override void Awake()
    {
        base.Awake();
        _model = GetEnemyModel();
    }
    protected abstract BaseFlockingEnemyModel GetEnemyModel();
    protected override ITreeNode GetDecisionTree() => _root;

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
