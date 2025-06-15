using System.Collections.Generic;
using UnityEngine;

public interface IFlocking
{
    Vector3 GetDir(List<IBoid> boids, IBoid self);
    bool IsActive { get; set; }
    FlockingType FlockingType { get; }
    void SetMultiplier(float multiplier);
    float GetMultiplier();
    void SetActive(bool active);
}