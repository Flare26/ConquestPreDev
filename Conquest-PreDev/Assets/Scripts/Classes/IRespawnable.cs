using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRespawnable
{
    // Anything that is respawnable
    void TakeDamage(Bullet b);
    void SetSpawn(Vector3 spawnPoint);
    void Respawn(float t);
}
