using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorHandler : SingleMonoBehaviour<SelectorHandler>
{
    //-------PROPERTY
    public IReadOnlyList<ITarget> AllTarget => _allTargets;




    //-------FIELD
    private List<ITarget> _allTargets = new List<ITarget>();




    //-------EVENTS




    //-------METODS
    public void RegisterTarget(ITarget target)
    {
        if (!_allTargets.Contains(target))
            _allTargets.Add(target);
    }

    public void ForgetTarget(ITarget target)
    {
        _allTargets.Remove(target);
    }
}
