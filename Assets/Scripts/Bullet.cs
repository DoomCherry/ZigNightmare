using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    //-------PROPERTY
    private Rigidbody MyRigidbody
    {
        get
        {
            _rigidbody = _rigidbody == null ? GetComponent<Rigidbody>() : _rigidbody;
            return _rigidbody;
        }
    }




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onShot;
    public event UnityAction OnShot
    {
        add => _onShot.AddListener(value);
        remove => _onShot.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onHitPlayer;
    public event UnityAction OnHitPlayer
    {
        add => _onHitPlayer.AddListener(value);
        remove => _onHitPlayer.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onHitEnemy;
    public event UnityAction OnHitEnemy
    {
        add => _onHitEnemy.AddListener(value);
        remove => _onHitEnemy.RemoveListener(value);
    }
    [SerializeField]
    private UnityEvent _onHitWall;
    public event UnityAction OnHitWall
    {
        add => _onHitWall.AddListener(value);
        remove => _onHitWall.RemoveListener(value);
    }




    //-------FIELD
    [SerializeField]
    private float _damage = 1;
    [SerializeField]
    private float _bulletLive = 5;
    [SerializeField]
    private LayerMask _collideLayers;
    private Rigidbody _rigidbody;




    //-------METODS
    private void Start()
    {
        _onShot?.Invoke();

        this.WaitSecond(_bulletLive, 
            delegate
            {
                Destroy(gameObject);
            });
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public void ShotDirection(Vector3 direction, float power)
    {
        MyRigidbody.velocity = direction.normalized * power;
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layerValue = _collideLayers;
        char[] binary = Convert.ToString(layerValue, 2).ToCharArray();
        Array.Reverse(binary);

        int currentLayer = collision.gameObject.layer;

        if (binary.Length <= currentLayer || binary[currentLayer] != '1')
            return;


        Enemy e = collision.gameObject.GetComponent<Enemy>();
        if (e == true)
        {
            e.TakeDamage(_damage);
            _onHitEnemy?.Invoke();
            Destroy(gameObject);
            return;
        }

        PlayerContorller pp = collision.gameObject.GetComponent<PlayerContorller>();

        if (pp == true)
        {
            pp.TakeDamage(_damage);
            _onHitPlayer?.Invoke();
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
        _onHitWall?.Invoke();
    }
}
