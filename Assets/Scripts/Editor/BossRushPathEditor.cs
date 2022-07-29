using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class BossRushPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PathCreator self = (PathCreator)target;

        self._stargLineColor = EditorGUILayout.ColorField("Start color: ",self._stargLineColor);
        self._endLineColor = EditorGUILayout.ColorField("End color: ", self._endLineColor);

        if (GUILayout.Button("Add point"))
        {
            self.AddPoint();
        }

        if (GUILayout.Button("Clear point"))
        {
            self.ClearPoint();
        }

        EditorUtility.SetDirty(self);
    }
}
#endif
