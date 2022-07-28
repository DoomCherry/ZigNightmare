using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillName
{
    Minigun,
    CenaHit,
    DownScorpion,
    Blast,
    Uppercut
}

[CreateAssetMenu(fileName = "Skill", menuName = "Skill")]
[System.Serializable]
public class SkillContainer : ScriptableObject
{
    [SerializeField]
    public Sprite _icon;
    [SerializeField]
    public SkillName _name;
    [SerializeField]
    public float coldown;

    [System.Serializable]
    public struct MinigunInfo
    {
        [SerializeField]
        public float bulletDamage;
        [SerializeField]
        public float torsionSpeed;
        [SerializeField]
        public float shotingTime;
        [SerializeField]
        public float bulletSpeed;
        [SerializeField]
        public float waitToShoting;
        [SerializeField]
        public float shotSpeed;
    }

    [SerializeField]
    public MinigunInfo minigunInfo;

    [System.Serializable]
    public struct CenaPunchInfo
    {
        [SerializeField]
        public float punchDamage;
        [SerializeField]
        public float maxWaitingTime;
        [SerializeField]
        public float punchSpeed;
    }

    [SerializeField]
    public CenaPunchInfo cenaPunchInfo;

    [System.Serializable]
    public struct DownScorpionInfo
    {
        [SerializeField]
        public float damagePerUnit;
        [SerializeField]
        public float maxDistance;
        [SerializeField]
        public float projectileSpeed;
        [SerializeField]
        public float grabSpeed;
        [SerializeField]
        public float stopDistance;
    }

    [SerializeField]
    public DownScorpionInfo downScorpionInfo;

    [System.Serializable]
    public struct BlastInfo
    {
        [SerializeField]
        public float blastDamage;
        [SerializeField]
        public float blastPrepareTime;

        [SerializeField]
        public float blastPushingForce;
        [SerializeField]
        public Limits blastEnemyLimits;
        [SerializeField]
        public float blastLimitTime;
    }

    [SerializeField]
    public BlastInfo blastInfo;

    [System.Serializable]
    public struct UppercutInfo
    {
        [SerializeField]
        public float uppercutDamage;
        [SerializeField]
        public float uppercutUpVelocity;
        [SerializeField]
        public float uppercutPushingForce;
        [SerializeField]
        public Limits uppercutEnemyLimits;
        [SerializeField]
        public float uppercutLimitTime;

        [SerializeField]
        public float powerUppercutDamage;
        [SerializeField]
        public float powerUppercutUpVelocity;
        [SerializeField]
        public float powerUppercutPushingForce;
        [SerializeField]
        public Limits powerUppercutEnemyLimits;
        [SerializeField]
        public float powerUppercutLimitTime;

        [SerializeField]
        public float chargeTime;
    }
    [SerializeField]
    public UppercutInfo uppercutInfo;
}
