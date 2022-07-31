using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControler : SingleMonoBehaviour<LevelControler>
{
    //-------PROPERTY




    //-------FIELD

    private List<Enemy> _enemyList = new List<Enemy>();




    //-------EVENTS




    //-------METODS
    protected override void Start()
    {
        base.Start();
    }

    public void RegistryEnemy(Enemy enemy)
    {
        _enemyList.Add(enemy);
    }

    public void EnableAllSpawners()
    {
    }

    public void DesableAllSpawners()
    {
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

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
