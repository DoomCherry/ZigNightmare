using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    //-------FIELD
    [Serializable]
    public class FrameEvent
    {
        //-------EVENTS
        [SerializeField]
        private UnityEvent _onFrame;
        public event UnityAction OnFrame
        {
            add => _onFrame.AddListener(value);
            remove => _onFrame.RemoveListener(value);
        }




        //-------METODS
        public void PlayIvent()
        {
            _onFrame?.Invoke();
        }
    }

    [SerializeField]
    private FrameEvent[] _frameEvents;




    //-------METODS
    public void OnFramePlay(int i)
    {
        if(_frameEvents != null && _frameEvents.Length > i)
        {
            _frameEvents[i].PlayIvent();
        }
    }
}
