using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour, ILook
{
    //M: Model
    //V: View
    //C: Controller
    [SerializeField]
    Animator _anim;
    Rigidbody _rb;
    public float speedRot = 10;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        GetComponent<IAttack>().OnAttack += OnSpinAnim;
    }
    public void Update()
    {
        OnMoveAnim();
    }

    public void LookDir(Vector3 dir)
    {
        transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * speedRot);
    }

    public void OnSpinAnim()
    {
        _anim.SetTrigger("Spin");
    }
    void OnMoveAnim()
    {
        _anim.SetFloat("Vel", _rb.linearVelocity.magnitude);
    }
}
