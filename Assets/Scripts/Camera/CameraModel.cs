using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : MonoBehaviour
{
    [Header("Line Of Sight")]
    [Range(1, 360)]
    public float angle;
    public float range;
    public LayerMask obsMask;
    public Action<bool> onChangeIsDetecting = delegate { };

    bool _isDetecting;

    [Space]
    [Header("Target")]
    [SerializeField]
    Transform _target;

    //public delegate void MyDelegate(float a);
    //public delegate void MyDelegate2(float a);
    //MyDelegate _a = delegate { };

    //Action<float> _b;
    //Func<float,GameObject, bool> _c;

    //private void Awake()
    //{
    //    _b += Test;
    //    _b += TestTwo;
    //    _b += Test;
    //    _b += TestTwo;

    //    _b(5);

    //    _b -= Test;
    //}
    //public void Test(float a)
    //{
    //    print("Test");
    //}
    //public void TestTwo(float a)
    //{
    //    print("TestTwo");
    //}
    public Transform CheckTarget()
    {
        return _target;
    }
    public bool IsDetecting
    {
        set
        {
            if (value != _isDetecting) onChangeIsDetecting(value);
            _isDetecting = value;
        }
        get
        {
            return _isDetecting;
        }
    }
    private void OnDestroy()
    {
        onChangeIsDetecting = null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * range);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * range);
    }
}
