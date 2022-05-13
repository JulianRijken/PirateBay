using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CannonSettings", menuName = "ScriptableObjects/CannonSettings", order = 1)]
public class CannonSettings : ScriptableObject
{
    public CannonBall _cannonBall;
    public float _damage = 5f;
    public float _fireSpeedPower = 50;
    public float _xAngleOffsetMin;
    public float _xAngleOffsetMax;
    public float _yAngleOffsetMin;
    public float _yAngleOffsetMax;
    public bool _useCannonDamage = true;
    public float _maxRandomFireDelay;
    public float _accuracyMultiplier = 1f;
}
