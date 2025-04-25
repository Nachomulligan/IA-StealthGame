using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CameraModel _model;
    private void Awake()
    {
        _model = GetComponent<CameraModel>();
    }
    private void Update()
    {
        var target = _model.CheckTarget();
        if (LineOfSight.LOS(_model.transform, target, _model.range, _model.angle, _model.obsMask))
        {
            _model.IsDetecting = true;
        }
        else
        {
            _model.IsDetecting = false;
        }
    }
}
