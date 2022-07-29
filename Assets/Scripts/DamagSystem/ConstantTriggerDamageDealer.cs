using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ConstantTriggerDamageDealer : SimpleDamageDealer
{
    //-------PROPERTY
    public override bool IsSelfDetect => true;




    //-------FIELD
    [SerializeField]
    private DamageDetector _selfDetector;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onDamageDeal;
    public event UnityAction OnDamageDeal
    {
        add => _onDamageDeal.AddListener(value);
        remove => _onDamageDeal.RemoveListener(value);
    }




    //-------METODS
    private void FixedUpdate()
    {
        SetDamage(Damage);
    }

    public override void SetDamage(float damage)
    {
        IEnumerable<DamageDetector> detectors = Physics.OverlapSphere(Collider.bounds.center, Collider.bounds.extents.x).Select(n => n.gameObject.GetComponent<DamageDetector>()).Where(n => n != null && n != _selfDetector);

        if (detectors.Count() > 0)
        {
            foreach (var item in detectors)
            {
                if (item.Limiter != null)
                {
                    item.Limiter.TakeDamage(_damage);
                    _onDamageDeal?.Invoke();
                }
            }
        }
    }
}
