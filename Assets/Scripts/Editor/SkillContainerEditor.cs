using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SkillContainer))]
public class SkillContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SkillContainer self = (SkillContainer)target;

        GUILayout.BeginVertical("Base: ", GUI.skin.box);
        GUILayout.Label("Base: ");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Skill type: ");
        self._name = (SkillName)GUILayout.Toolbar((int)self._name, System.Enum.GetNames(typeof(SkillName)));
        GUILayout.EndHorizontal();

        self._icon = (Sprite)EditorGUILayout.ObjectField("Icon: ", self._icon, typeof(Sprite), false);
        self.coldown = EditorGUILayout.FloatField("Coldown: ", self.coldown);
        GUILayout.EndVertical();

        GUILayout.Space(5);

        GUILayout.BeginVertical($"{self._name}: ", GUI.skin.box);
        GUILayout.Label($"{self._name}: ");

        switch (self._name)
        {
            case SkillName.Minigun:
                self.minigunInfo.bulletDamage = EditorGUILayout.FloatField("Bullet damage: ", self.minigunInfo.bulletDamage);
                self.minigunInfo.bulletSpeed = EditorGUILayout.FloatField("Bullet speed: ", self.minigunInfo.bulletSpeed);
                self.minigunInfo.shotingTime = EditorGUILayout.FloatField("Shoting time: ", self.minigunInfo.shotingTime);
                self.minigunInfo.torsionSpeed = EditorGUILayout.FloatField("Torsion speed: ", self.minigunInfo.torsionSpeed);
                self.minigunInfo.waitToShoting = EditorGUILayout.FloatField("Wait to shoting: ", self.minigunInfo.waitToShoting);
                break;

            case SkillName.CenaHit:
                self.cenaPunchInfo.punchDamage = EditorGUILayout.FloatField("Hit damage: ", self.cenaPunchInfo.punchDamage);
                self.cenaPunchInfo.maxWaitingTime = EditorGUILayout.FloatField("Max waiting time: ", self.cenaPunchInfo.maxWaitingTime);
                self.cenaPunchInfo.punchSpeed = EditorGUILayout.FloatField("Punch speed: ", self.cenaPunchInfo.punchSpeed);
                break;
            case SkillName.DownScorpion:
                self.downScorpionInfo.damagePerUnit = EditorGUILayout.FloatField("Damage per unit: ", self.downScorpionInfo.damagePerUnit);
                self.downScorpionInfo.grabSpeed = EditorGUILayout.FloatField("Grab speed: ", self.downScorpionInfo.grabSpeed);
                self.downScorpionInfo.projectileSpeed = EditorGUILayout.FloatField("Projectile speed: ", self.downScorpionInfo.projectileSpeed);
                self.downScorpionInfo.maxDistance = EditorGUILayout.FloatField("Max distance: ", self.downScorpionInfo.maxDistance);
                self.downScorpionInfo.stopDistance = EditorGUILayout.FloatField("Stop distance: ", self.downScorpionInfo.stopDistance);
                break;
            case SkillName.Blast:
                self.blastInfo.blastDamage = EditorGUILayout.FloatField("Blast damage: ", self.blastInfo.blastDamage);
                self.blastInfo.blastPrepareTime = EditorGUILayout.FloatField("Blast prepare time: ", self.blastInfo.blastPrepareTime);
                self.blastInfo.blastPushingForce = EditorGUILayout.FloatField("Blast pushing force: ", self.blastInfo.blastPushingForce);
                self.blastInfo.blastEnemyLimits = (Limits)EditorGUILayout.EnumFlagsField("Enemy limits after blast", self.blastInfo.blastEnemyLimits);
                self.blastInfo.blastLimitTime = EditorGUILayout.FloatField("Blast limits time: ", self.blastInfo.blastLimitTime);
                break;
            case SkillName.Uppercut:
                self.uppercutInfo.uppercutDamage = EditorGUILayout.FloatField("Uppercut damage: ", self.uppercutInfo.uppercutDamage);
                self.uppercutInfo.uppercutPushingForce = EditorGUILayout.FloatField("Uppercut pushing force: ", self.uppercutInfo.uppercutPushingForce);
                self.uppercutInfo.uppercutUpVelocity = EditorGUILayout.FloatField("Uppercut up velocity: ", self.uppercutInfo.uppercutUpVelocity);
                self.uppercutInfo.uppercutEnemyLimits = (Limits)EditorGUILayout.EnumFlagsField("Enemy limits after uppercut", self.uppercutInfo.uppercutEnemyLimits);
                self.uppercutInfo.uppercutLimitTime = EditorGUILayout.FloatField("Uppercut limits time: ", self.uppercutInfo.uppercutLimitTime);
                EditorGUILayout.Space(5);
                self.uppercutInfo.powerUppercutDamage = EditorGUILayout.FloatField("Power uppercut damage: ", self.uppercutInfo.powerUppercutDamage);
                self.uppercutInfo.powerUppercutPushingForce = EditorGUILayout.FloatField("Power uppercut pushing force: ", self.uppercutInfo.powerUppercutPushingForce);
                self.uppercutInfo.powerUppercutUpVelocity = EditorGUILayout.FloatField("Power uppercut up velocity: ", self.uppercutInfo.powerUppercutUpVelocity);
                self.uppercutInfo.powerUppercutEnemyLimits = (Limits)EditorGUILayout.EnumFlagsField("Enemy limits after power uppercut", self.uppercutInfo.powerUppercutEnemyLimits);
                self.uppercutInfo.powerUppercutLimitTime = EditorGUILayout.FloatField("Power uppercut limits time: ", self.uppercutInfo.powerUppercutLimitTime);
                EditorGUILayout.Space(5);
                self.uppercutInfo.chargeTime = EditorGUILayout.FloatField("Charge time: ", self.uppercutInfo.chargeTime);
                break;
            default:
                break;
        }
        GUILayout.EndVertical();

        EditorUtility.SetDirty(self);
    }
}

#endif
