using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public interface IKillable
{
    // DECIDING NOW THAT UNITS SHALL BE ENABLED / DISABLED TO KILL! DO NOT DESTROY THEM FROM MEMORY
     void TakeDamage(Bullet b);
     void DeathRoutine();
}
