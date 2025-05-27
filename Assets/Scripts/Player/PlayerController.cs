using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
public class PlayerController : MonoBehaviour
{
    [Header("Player Controller")]
    private FSM<StateEnum> _fsm;
    private gameManager _gm;

    private PlayerModel _playerModel;
    private PlayerInteractionModel _interactionModel;
    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
        _interactionModel = GetComponent<PlayerInteractionModel>();
        InitializeFSM();

        ServiceLocator.Instance.Register<PlayerController>(this);
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
        idle.AddTransition(StateEnum.Attack, spin);

        walk.AddTransition(StateEnum.Idle, idle);
        walk.AddTransition(StateEnum.Attack, spin);

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
            HandleAttackInput();
        }

        _fsm.OnExecute();

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteractionInput();
        }

        _fsm.OnExecute();
    }

    private void HandleAttackInput()
    {
        if (_playerModel.IsArmed && _playerModel.CanAttack)
        {
            _fsm.Transition(StateEnum.Attack);
        }
        else
        {
            Debug.Log("Doesn't have an attack, can't attack");
        }
    }

    private void HandleInteractionInput()
    {
        if (_interactionModel != null)
        {
            _interactionModel.TryInteract();
        }
    }
    public void KillPlayer()
    {
        _playerModel.Die();
    }
    public void EquipWeapon(GameObject weapon)
    {
        _playerModel.EquipWeapon(weapon);
    }
}
