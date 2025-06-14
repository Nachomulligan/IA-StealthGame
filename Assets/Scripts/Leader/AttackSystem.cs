using System;
using System.Collections.Generic;

public class AttackSystem
{
    private Dictionary<Enum, Action> _attackActions;

    public AttackSystem()
    {
        _attackActions = new Dictionary<Enum, Action>();
    }

    public void RegisterAttack(Enum attackType, Action attackAction)
    {
        _attackActions[attackType] = attackAction;
    }

    public void ExecuteAttack(Enum attackType)
    {
        if (_attackActions.TryGetValue(attackType, out Action attackAction))
        {
            attackAction?.Invoke();
        }
        else
        {
            UnityEngine.Debug.LogError($"No se encontró una acción para el tipo de ataque: {attackType}");
        }
    }

    public bool HasAttackType(Enum attackType)
    {
        return _attackActions.ContainsKey(attackType);
    }

    public void UnregisterAttack(Enum attackType)
    {
        _attackActions.Remove(attackType);
    }

    public IEnumerable<Enum> GetRegisteredAttackTypes()
    {
        return _attackActions.Keys;
    }
}