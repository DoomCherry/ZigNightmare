using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControler : SingleMonoBehaviour<LevelControler>
{
    //-------PROPERTY




    //-------FIELD
    private Spawner[] _spawnerList;

    private List<Enemy> _enemyList = new List<Enemy>();




    //-------EVENTS




    //-------METODS
    protected override void Start()
    {
        base.Start();
        _spawnerList = FindObjectsOfType<Spawner>();
    }

    public void RegistryEnemy(Enemy enemy)
    {
        _enemyList.Add(enemy);
    }

    public void EnableAllSpawners()
    {
        for (int i = 0; i < _spawnerList.Length; i++)
        {
            _spawnerList[i].gameObject.SetActive(true);
        }
    }

    public void DesableAllSpawners()
    {
        for (int i = 0; i < _spawnerList.Length; i++)
        {
            _spawnerList[i].gameObject.SetActive(false);
        }
    }

    public void DestroyAllEnemy()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i] == null)
            {
                _enemyList.RemoveAt(i);
                i--;
            }
            else
            {
                _enemyList[i].DamageController.Demolish();
                _enemyList.RemoveAt(i);
                i--;
            }
        }
    }

    public void RestartLevel()
    {
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }
}
