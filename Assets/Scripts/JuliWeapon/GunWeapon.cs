using UnityEngine.Events;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    public UnityEvent OnGunShoot;
    public float FireCoolDown;

    //by default gun is semi
    public bool Automatic;

    private float CurrentCoolDown;

    void Start()
    {
        CurrentCoolDown = FireCoolDown;
    }

    void Update()
    {
        if (Automatic)
        {
            if (Input.GetMouseButton(0))
            { }

            if (CurrentCoolDown <= 0)
            {
                OnGunShoot.Invoke();
                CurrentCoolDown = FireCoolDown;
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if CurrentCoolDown <= 0f;
                    {
                        OnGunShoot.Invoke();
                        CurrentCoolDown = FireCoolDown;
                    }

                }
            }

            CurrentCoolDown -= Time.deltaTime;
        }

      