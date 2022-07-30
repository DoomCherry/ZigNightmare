using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelfDetectDamageDealer : SimpleDamageDealer
{
    //-------PROPERTY
    public override bool IsSelfDetect => true;




    //-------FIELD
    [SerializeField]
    private DamageDetector _selfDetector;
    private Limits _limitsMask;
    private float _limitTime = 0;
    private float _pushPower = 0;




    //-------METODS
    public void SetDamage(float damage, Limits limits, float limitTime, float pushPower = 0, float upperPower = 0)
    {
        _damage = damage;
        _limitsMask = limits;
        _limitTime = limitTime;
        _pushPower = pushPower;

        IEnumerable<DamageDetector> detectors = Physics.OverlapSphere(Collider.bounds.center, Collider.bounds.size.magnitude / transform.lossyScale.x).Select(n => n.gameObject.GetComponent<DamageDetector>()).Where(n => n != null && n != _selfDetector);

        if (detectors.Count() > 0)
        {
            foreach (var item in detectors)
            {
                if (item.Limiter != null)
                {
                    item.Limiter.TakeDamage(_damage);
                    item.SetLimitsByTime(_limitsMask, _limitTime);

                    Vector3 oldVelosicty = item.Limiter.MyRigidbody.velocity;

                    Vector3 selfPosition = transform.position;
                    Vector3 targetPosition = item.Limiter.MyRigidbody.position;

                    Vector3 pushDrection = (targetPosition - selfPosition).normalized * pushPower;

                    item.Limiter.MyRigidbody.velocity = new Vector3(pushDrection.x, upperPower, pushDrection.z);
                }
            }
        }
    }
}
