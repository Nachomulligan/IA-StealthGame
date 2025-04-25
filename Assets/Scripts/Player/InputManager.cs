using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    static KeyCode _attack = KeyCode.Space;
    public static bool GetKeyAttack()
    {
        return Input.GetKeyDown(_attack);
    }
    public static Vector2 GetMove()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

}


