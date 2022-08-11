using System.Linq;
using UnityEngine;

public class PauseManager : SingleMonoBehaviour<PauseManager>
{
    //-------PROPERTY
    public bool IsInPause { get; private set; } = false;




    //-------FIELD
    private PathAgent[] _agentList;
    private ICharacterLimiter[] _characterList;
    private ParticleSystem[] _particleSystems;




    //-------EVENTS




    //-------METODS
    private void Awake()
    {
        _characterList = FindObjectsOfType<GameObject>(true).Select(n => n.GetComponent<ICharacterLimiter>())
                                                                     .Where(n => n != null)
                                                                     .ToArray();
        _agentList = FindObjectsOfType<PathAgent>(true);
        _particleSystems = FindObjectsOfType<ParticleSystem>(true);
    }

    public void SetPause(bool isInPause)
    {
        SetPauseForCharacters(isInPause);
        SetPauseForParticles(isInPause);
        SetPauseForAgents(isInPause);

        IsInPause = isInPause;
    }

    private void SetPauseForCharacters(bool isInPause)
    {
        foreach (var item in _characterList)
        {
            if (isInPause)
                item.FullSystemFreeze();
            else
                item.FullSystemUnfreeze();
        }
    }

    private void SetPauseForParticles(bool isInPause)
    {
        foreach (var item in _particleSystems)
        {
            if (isInPause)
                item.Pause();
            else
                item.Play();
        }
    }

    private void SetPauseForAgents(bool isInPause)
    {
        foreach (var item in _agentList)
        {
            if (isInPause)
                item.SpeedMult = 0;
            else
                item.SpeedMult = 1;
        }
    }
}
