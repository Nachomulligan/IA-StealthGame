using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
public class PlayerController : MonoBehaviour
{
    [Header("Player Controller")]
    FSM<StateEnum> _fsm;

    // NUEVO: evento para avisar a los NPCs
    public static event Action<bool> OnPlayerArmedChanged;
    public bool isArmed = false; // NUEVO

    private void Awake()
    {
        InitializeFSM();
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StateEnum>();
        var move = GetComponent<IMove>();
        var look = GetComponent<ILook>();
        var attack = GetComponent<IAttack>();

        var stateList = new List<PSBase<StateEnum>>();

        var idle = new PSIdle<StateEnum>(StateEnum.Walk);
        var walk = new PSWalk<StateEnum>(StateEnum.Idle);
        var spin = new PSSpin<StateEnum>(StateEnum.Idle);

        idle.AddTransition(StateEnum.Walk, walk);
        idle.AddTransition(StateEnum.Spin, spin);

        walk.AddTransition(StateEnum.Idle, idle);
        walk.AddTransition(StateEnum.Spin, spin);

        spin.AddTransition(StateEnum.Idle, idle);

        stateList.Add(idle);
        stateList.Add(walk);
        stateList.Add(spin);

        for (int i = 0; i < stateList.Count; i++)
        {
            stateList[i].Initialize(move, look, attack);
        }

        _fsm.SetInit(idle);
    }

    private void Update()
    {
        if (InputManager.GetKeyAttack())
        {
            _fsm.Transition(StateEnum.Spin);
        }
        _fsm.OnExecute();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    [Header("Interaction Settings")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private LayerMask interactionLayer;
    private Collider[] interactables = new Collider[4];

    [SerializeField] private Transform weaPoint;
    [SerializeField] private GameObject currentWeapon;

    public void EquipWeapon(GameObject weapon)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(weapon, weaPoint.position, weaPoint.rotation, weaPoint);

        isArmed = (currentWeapon != null); 
        OnPlayerArmedChanged?.Invoke(isArmed); 
    }

    private void TryInteract()
    {
        Debug.Log("Tried interaction");
        int elements = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius, interactables, interactionLayer);

        if (elements == 0)
        {
            Debug.Log("No interactables found");
            return;
        }

        for (int i = 0; i < elements; i++)
        {
            var interactable = interactables[i];
            var interactableComponent = interactable.GetComponent<Iinteractable>();
            if (interactableComponent != null)
            {
                interactableComponent.interaction();
            }
        }
    }
}