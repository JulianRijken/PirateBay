using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bomb : Enemy
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _explodeDamage;
    [SerializeField] private float _explodeRange;
    [SerializeField] private float _timeInRangeForExplode;
    
    private float _timeInRange = 0f;
    
    private void Start()
    {
        TargetRotation = RotateTowardsOptions.Player;
        MoveSpeed = _moveSpeed;
        RotateSpeed = _rotateSpeed;
    }

    protected override void Update()
    {
        if (HandleExploding()) return;
        
        var playerPosition = GameManager.ActivePlayerController.transform.position;
        TargetLocation = playerPosition;

        base.Update();
    }

    private bool HandleExploding()
    {
        if (DistanceFromTarget < _explodeRange)
        {

            
            if (_timeInRange > _timeInRangeForExplode)
            {
                Destroy(gameObject);
                return true;
            }

            
            MoveSpeed = 0f;
            RotateSpeed = 0f;
            
            _timeInRange += Time.deltaTime;
        }
        else
        {
            MoveSpeed = _moveSpeed;
            RotateSpeed = _rotateSpeed;
            
            _timeInRange -= Time.deltaTime;
            _timeInRange = Mathf.Max(0f, _timeInRange);
        }

        var explodeAlpha = Mathf.Clamp01(_timeInRange / _timeInRangeForExplode);
        Debug.Log(explodeAlpha);
        
        Model.localScale = Vector3.one + (Vector3.one * explodeAlpha * 0.4f);
        
        return false;
    }
}
