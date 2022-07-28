using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UnitContainer))]
public class UnitContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UnitContainer self = (UnitContainer)target;
        GUILayout.BeginVertical("Base: ", GUI.skin.box);
        GUILayout.Label("Base: ");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Unit type: ");
        self._name = (UnitType)GUILayout.Toolbar((int)self._name, System.Enum.GetNames(typeof(UnitType)));
        GUILayout.EndHorizontal();

        self._icon = (Sprite)EditorGUILayout.ObjectField("Icon: ", self._icon, typeof(Sprite), false);
        self._movespeed = EditorGUILayout.FloatField("Max movement speed: ", self._movespeed);
        self._velocity = EditorGUILayout.FloatField("Velocity: ", self._velocity);
        self._maxHp = EditorGUILayout.FloatField("Max Hp: ", self._maxHp);
        GUILayout.EndVertical();

        GUILayout.Space(5);

        GUILayout.BeginVertical($"{self._name}: ", GUI.skin.box);
        GUILayout.Label($"{self._name}: ");

        switch (self._name)
        {
            case UnitType.Player:
                self.playerInfo.jumpPower = EditorGUILayout.FloatField("Jump power: ", self.playerInfo.jumpPower);
                self.playerInfo.dashPower = EditorGUILayout.FloatField("Dash power: ", self.playerInfo.dashPower);
                self.playerInfo._dashTime = EditorGUILayout.FloatField("Dash time: ", self.playerInfo._dashTime);
                self.playerInfo.maxStamina = EditorGUILayout.FloatField("Max stamina: ", self.playerInfo.maxStamina);
                self.playerInfo.staminaToOnceDash = EditorGUILayout.FloatField("Stamina per once dash: ", self.playerInfo.staminaToOnceDash);
                self.playerInfo.staminaRecoverPerSecond = EditorGUILayout.FloatField("Stamina recover per second: ", self.playerInfo.staminaRecoverPerSecond);
                break;
            case UnitType.RobotEnemy:
                self.robotEnemy.viewSpeed = EditorGUILayout.FloatField("View speed: ", self.robotEnemy.viewSpeed);
                self.robotEnemy._minDistanceToTarget = EditorGUILayout.FloatField("Min distance to target: ", self.robotEnemy._minDistanceToTarget);
                self.robotEnemy._speedOnSkill = EditorGUILayout.FloatField("Speed on skill: ", self.robotEnemy._speedOnSkill);
                self.robotEnemy._viewSpeedOnSkill = EditorGUILayout.FloatField("View speed on skillt: ", self.robotEnemy._viewSpeedOnSkill);
                self.robotEnemy._agressiveZoneRadius = EditorGUILayout.FloatField("Agressive zone radius: ", self.robotEnemy._agressiveZoneRadius);
                break;
            default:
                break;
        }
        GUILayout.EndVertical();

        EditorUtility.SetDirty(self);
    }
}
#endif