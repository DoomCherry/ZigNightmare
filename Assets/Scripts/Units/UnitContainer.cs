using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Player,
    RobotEnemy,
}

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
[System.Serializable]
public class UnitContainer : ScriptableObject
{
    [SerializeField]
    public Sprite _icon;
    [SerializeField]
    public UnitType _name;
    [SerializeField]
    public float _movespeed;
    [SerializeField]
    public float _velocity;
    [SerializeField]
    public float _maxHp;

    [System.Serializable]
    public struct PlayerInfo
    {
        [SerializeField]
        public float jumpPower;
        [SerializeField]
        public float dashPower;
        [SerializeField]
        public float _dashTime;
        [SerializeField]
        public float maxStamina;
        [SerializeField]
        public float staminaToOnceDash;
        [SerializeField]
        public float staminaRecoverPerSecond;
    }

    [SerializeField]
    public PlayerInfo playerInfo;

    [System.Serializable]
    public struct RobotEnemy
    {
        [SerializeField]
        public float viewSpeed;
        [SerializeField]
        public float _minDistanceToTarget;
        [SerializeField]
        public float _speedOnSkill;
        [SerializeField]
        public float _viewSpeedOnSkill;
        [SerializeField]
        public float _agressiveZoneRadius;
    }

    [SerializeField]
    public RobotEnemy robotEnemy;
}
