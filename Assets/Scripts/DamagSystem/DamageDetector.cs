using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DamageDetector : MonoBehaviour
{
    //-------PROPERTY
    public ICharacterLimiter Limiter => _customCharacterLimiter != null ? _customCharacterLimiter.GetComponent<ICharacterLimiter>() : (_limiter = _limiter ?? GetComponent<ICharacterLimiter>());




    //-------FIELD
    [SerializeField]
    private GameObject _customCharacterLimiter;
    private ICharacterLimiter _limiter;
    private Coroutine _waitLimits;



    //-------EVENTS
    [SerializeField]
    private UnityEvent<float> _onTakeDamage;
    public event UnityAction<float> OnTakeDamage
    {
        add => _onTakeDamage.AddListener(value);
        remove => _onTakeDamage.RemoveListener(value);
    }

    [SerializeField]
    private UnityEvent<float> _onTakeHeal;
    public event UnityAction<float> OnTakeHeal
    {
        add => _onTakeHeal.AddListener(value);
        remove => _onTakeHeal.RemoveListener(value);
    }




    //-------METODS
    private void OnValidate()
    {
        if (_customCharacterLimiter != null && _customCharacterLimiter.GetComponent<ICharacterLimiter>() == null)
            _customCharacterLimiter = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
      IDamageDealer dealer = collision.gameObject.GetComponent<IDamageDealer>();
        if (dealer != null && !dealer.IsSelfDetect)
        {
            int layerValue = dealer.DamagableTarget.value;
            char[] binary = Convert.ToString(layerValue, 2).ToCharArray();
            Array.Reverse(binary);

            int currentLayer = gameObject.layer;

            if (binary.Length > currentLayer && binary[currentLayer] == '1')
            {
                if (dealer.Damage > 0)
                    _onTakeDamage?.Invoke(dealer.Damage);

                if (dealer.Damage < 0)
                    _onTakeHeal?.Invoke(dealer.Damage);
            }
        }
    }

    public void SetLimitsByTime(Limits limits, float time)
    {
        void SetLimitBy(Limits position, bool isActive)
        {
            if (Limiter == null)
                return;

            switch (position)
            {
                case Limits.Walking:
                    if (isActive)
                        Limiter.FreezeWalking();
                    else
                        Limiter.UnfreezeWalking();
                    break;
                case Limits.Falling:
                    if (isActive)
                        Limiter.FreezeFalling();
                    else
                        Limiter.UnfreezeFalling();
                    break;
                case Limits.Jump:
                    if (isActive)
                        Limiter.JumpFreeze();
                    else
                        Limiter.JumpUnfreeze();
                    break;
                case Limits.Rotation:
                    if (isActive)
                        Limiter.FreezeRotation();
                    else
                        Limiter.UnfreezeRotation();
                    break;
                case Limits.Skills:
                    if (isActive)
                        Limiter.FreezeSkill();
                    else
                        Limiter.UnfreezeSkill();
                    break;
            }
        }

        void SetLimits(bool isActive)
        {
            var values = System.Enum.GetValues(typeof(Limits));
            foreach (Limits item in values)
            {
                if (limits.HasFlag(item))
                {
                    SetLimitBy(item, isActive);
                }
            }
        }
        SetLimits(true);

        if (_waitLimits != null)
        {
            StopCoroutine(_waitLimits);
            _waitLimits = null;
        }

        _waitLimits = this.WaitSecond(time, delegate { SetLimits(false); });
    }
}
