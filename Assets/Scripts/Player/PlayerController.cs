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
    private WeaponInventory _weaponInventory; 

    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
        _interactionModel = GetComponent<PlayerInteractionModel>();
        _weaponInventory = GetComponent<WeaponInventory>(); 

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeaponFromInventory(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeaponFromInventory(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeaponFromInventory(2);
        }
    }
    private void HandleAttackInput()
    {
        if (_playerModel.IsArmed && _playerModel.CanAttack)
        {
          
            GunWeapon gun = _playerModel.currentWeapon?.GetComponent<GunWeapon>();
            if (gun != null)
            {
                gun.Fire();
            }
            else
            {
                _fsm.Transition(StateEnum.Attack);
            }
        }
        else
        {
            Debug.Log("No puedes atacar, no tienes un arma.");
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
    public void PickUpWeapon(Weapon weaponData)
    {
        if (_weaponInventory != null && weaponData != null)
        {
            _weaponInventory.AddWeapon(weaponData);
            Debug.Log($"Picked up weapon: {weaponData.weaponName}");
        }
    }
    public void EquipWeaponFromInventory(int index)
    {
        if (_weaponInventory != null)
        {
            Weapon weaponToEquip = _weaponInventory.GetWeaponToEquip(index);

            if (weaponToEquip != null)
            {
                GameObject weaponInstance = Instantiate(weaponToEquip.weaponPrefab);

                EquipWeapon(weaponInstance);

                Debug.Log($"Equipped weapon: {weaponToEquip.weaponName}");
            }
        }
    }
}