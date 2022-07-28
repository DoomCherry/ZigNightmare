using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    //-------FIELDS
    [SerializeField]
    private GameObject[] _sceneObjects;
    [SerializeField]
    private float _timeDelay = 5;

    [SerializeField]
    private int _countMin = 1, _countMax = 4;

    [SerializeField]
    private int _spawnLimit = 10;
    [SerializeField]
    private bool _spawnInUpdate = true;

    private int _count = 1;
    private int _currentSpawnTry = 0;
    private float _currentTime = 0;




    //-------EVENTS
    [SerializeField]
    private UnityEvent _onSpawn;
    public event UnityAction OnSpawn
    {
        add => _onSpawn.AddListener(value);
        remove => _onSpawn.RemoveListener(value);
    }




    //-------METODS
    void Update()
    {
        if (_spawnInUpdate)
            Spawn();
    }

    public void Spawn()
    {
        if (_countMax <= _countMin)
        {
            _countMax = _countMin + 1;
        }

        _count = Random.Range(_countMin, _countMax);

        _currentTime += Time.deltaTime;

        if (_currentTime > _timeDelay)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_currentSpawnTry >= _spawnLimit)
                {
                    Destroy(this);
                    return;
                }

                GameObject obj = Instantiate(_sceneObjects[Random.Range(0, _sceneObjects.Length)], transform.position, Quaternion.identity);
                obj.SetActive(true);
                _currentSpawnTry++;
            }

            _currentTime = 0;
            _onSpawn?.Invoke();
        }
    }
}
