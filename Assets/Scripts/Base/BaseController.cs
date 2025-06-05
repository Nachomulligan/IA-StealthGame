using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    [Header("Base Controller")]
    public Rigidbody target;
    public Transform zone;
    public float timePrediction;

    protected FSM<StateEnum> _fsm;
    protected LineOfSightMono _los;
    protected bool _isChasing;

    protected virtual void Awake()
    {
        _los = GetComponent<LineOfSightMono>();
    }

    protected virtual void Start()
    {
        InitializedFSM();
        InitializedTree();
    }

    protected virtual void Update()
    {
        _fsm.OnExecute();
        GetDecisionTree().Execute();
    }

    private void FixedUpdate()
    {
        _fsm.OnFixExecute();
    }

    protected abstract void InitializedFSM();
    protected abstract ITreeNode GetDecisionTree();
    protected abstract void InitializedTree();
}