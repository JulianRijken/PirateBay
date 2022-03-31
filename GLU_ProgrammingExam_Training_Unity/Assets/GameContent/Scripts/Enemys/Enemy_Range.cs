using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range : Enemy
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    
    [SerializeField] private float _interval;
    [SerializeField] private float _rangeRadius = 10;
    [SerializeField] private float _maxDistanceFromNavMesh = 10f;
    
    

    
    
    
    private void Start()
    {
        TargetRotation = RotateTowardsOptions.Player;
        MoveSpeed = _moveSpeed;
        RotateSpeed = _rotateSpeed;

        StartCoroutine(Behaviour());
    }

    private IEnumerator Behaviour()
    {
        while (true)
        {
            MoveToNewLocation();

            yield return new WaitForSeconds(_interval);
        }
    }

    private void MoveToNewLocation()
    {
        if (GetLocationAroundPlayer(_maxDistanceFromNavMesh, _maxDistanceFromNavMesh, out var hit))
        {
            TargetLocation = hit.position;
        }
    }

    // protected override void Update()
    // {
    //     if (HandleExploding()) return;
    //     
    //     var playerPosition = GameManager.PlayerController.transform.position;
    //     TargetLocation = playerPosition;
    //
    //     base.Update();
    // }

    // private bool HandleExploding()
    // {
    //     if (DistanceFromTarget < _explodeRange)
    //     {
    //
    //         
    //         if (_timeInRange > _timeInRangeForExplode)
    //         {
    //             Destroy(gameObject);
    //             return true;
    //         }
    //
    //         
    //         MoveSpeed = 0f;
    //         RotateSpeed = 0f;
    //         
    //         _timeInRange += Time.deltaTime;
    //     }
    //     else
    //     {
    //         MoveSpeed = _moveSpeed;
    //         RotateSpeed = _rotateSpeed;
    //         
    //         _timeInRange -= Time.deltaTime;
    //         _timeInRange = Mathf.Max(0f, _timeInRange);
    //     }
    //
    //     var explodeAlpha = Mathf.Clamp01(_timeInRange / _timeInRangeForExplode);
    //     Debug.Log(explodeAlpha);
    //     
    //     Model.localScale = Vector3.one + (Vector3.one * explodeAlpha * 0.4f);
    //     
    //     return false;
    // }
}
