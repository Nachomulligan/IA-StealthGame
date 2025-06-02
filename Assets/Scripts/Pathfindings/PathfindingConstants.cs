using UnityEngine;

public static class PathfindingConstants
{
    public const float nearRadius = 5;
    public static LayerMask nodeMask = LayerMask.GetMask("Node");
    public static LayerMask obsMask = LayerMask.GetMask("Wall");
}