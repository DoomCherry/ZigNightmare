using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SimpleDamageDealer : MonoBehaviour, IDamageDealer
{
    //-------PROPERTY
    public Collider Collider => _customCollider != null ? _customCollider : GetComponent<Collider>();
    public virtual bool IsSelfDetect => false;
    public float Damage => _damage;

    public LayerMask DamagableTarget => _damagableTarget;




    //-------FIELD
    [SerializeField]
    private Collider _customCollider;
    [SerializeField]
    protected LayerMask _damagableTarget;
    [SerializeField]
    protected float _damage;





    //-------METODS
    public virtual void SetDamage(float damage) 
    {
        _damage = damage;
    }
}
