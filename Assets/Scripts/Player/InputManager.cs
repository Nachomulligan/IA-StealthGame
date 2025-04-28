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
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (move.magnitude > 1)
            move.Normalize();
        return move;
    }
}


