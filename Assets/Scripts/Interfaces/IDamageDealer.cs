using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageDealer
{
    bool IsSelfDetect { get; }
    float Damage { get; }

    LayerMask DamagableTarget { get; }

    void SetDamage(float damage);
}
