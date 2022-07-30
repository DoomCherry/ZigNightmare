using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AgentInfo
{
    public PathAgent pathAgent;
    public UnityEvent onPathCompleate;
}

public class PathPoint : MonoBehaviour
{
    //-------PROPERTY
    public Transform MyTransform => _transform = _transform ??= transform;




    //-------FIELD
    [SerializeField]
    private AgentInfo[] _agentInfos;
    public Transform _transform;




    //-------EVENTS




    //-------METODS
    public void CompleatePath(PathAgent agent)
    {
        AgentInfo FindInfo()
        {
            if (_agentInfos == null)
                return null;

            for (int i = 0; i < _agentInfos.Length; i++)
            {
                if (_agentInfos[i].pathAgent == agent)
                    return _agentInfos[i];
            }

            return null;
        }

        AgentInfo agentSet = FindInfo();
        if (agentSet != null)
            agentSet.onPathCompleate?.Invoke();
    }
}
