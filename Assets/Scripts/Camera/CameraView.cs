using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public GameObject detectUI;

    private void Awake()
    {
        GetComponent<CameraModel>().onChangeIsDetecting += OnChangeUI;
    }
    private void OnDestroy()
    {
        var model = GetComponent<CameraModel>();
        if (model.onChangeIsDetecting != null)
        {
            model.onChangeIsDetecting -= OnChangeUI;
        }
    }
    void OnChangeUI(bool v)
    {
        detectUI.SetActive(v);
    }
}
