using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : NPCController
{
    protected override void Awake()
    {
        _model = GetComponent<RangedEnemyModel>();
        _los = GetComponent<LineOfSightMono>();
        _reactionSystem = GetComponent<NPCReactionSystem>();
    }

    protected override PSBase<StateEnum> CreateIdleState()
    {
        return new RangedEnemysIdle<StateEnum>(restTime);
    }
}