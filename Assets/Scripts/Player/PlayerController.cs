using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
public class PlayerController : MonoBehaviour
{
    [Header("Player Controller")]
    FSM<StateEnum> _fsm;
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

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
            if (isArmed) 
            {
                _fsm.Transition(StateEnum.Spin);
            }
            else
            {
                Debug.Log("Dosent have an attack, cant attack");
            }
        }

        _fsm.OnExecute();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused)
        {
            return; // Si está pausado, NO ejecutamos el FSM
        }

        if (InputManager.GetKeyAttack())
        {
            if (isArmed)
            {
                _fsm.Transition(StateEnum.Spin);
            }
            else
            {
                Debug.Log("Doesn't have an attack, can't attack");
            }
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

        var playerModel = GetComponent<PlayerModel>();
        if (playerModel != null)
        {
            playerModel.EnableAttack();
        }
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

    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Detenemos el tiempo del juego
            Cursor.lockState = CursorLockMode.None; // Liberamos el mouse
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f; // Volvemos a correr el tiempo
            Cursor.lockState = CursorLockMode.Locked; // Bloqueamos el mouse de nuevo
            Cursor.visible = false;
        }
    }

    public void Die()
    {
        Debug.Log("The player has died.");
        Destroy(gameObject);
    }

}