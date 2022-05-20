using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Damages the player based on it's distance from the center of the map and a damage curve
/// </summary>
public class RedSea : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _damageInterval;
    [SerializeField] private float _redSeaDistance;
    [SerializeField] private float _smoothDistance;
    [SerializeField] private AnimationCurve _damageOverWeight;
    
    [SerializeField] private Vector3 _mapCenter;
    [SerializeField] private PlayerShipController _player;
    [SerializeField] private PostProcessVolume _volume;

    private float _damageTimer = 0f;
    
    /// <summary>
    /// Damages the player when its distance is further than the red sea distance
    /// </summary>
    private void Update()
    {
        var distance = Vector3.Distance(_mapCenter, _player.transform.position);
        var weight = Mathf.Clamp01(Mathf.Abs(_redSeaDistance - distance) / _smoothDistance);
        if (distance < _redSeaDistance)
            weight = 0f;
        
        _volume.weight = weight;

        if (distance > _redSeaDistance)
        {
            if (_damageTimer > _damageInterval)
            {
                _player.OnHealthChange(-_damage * _damageOverWeight.Evaluate(weight));
                _damageTimer = 0f;
            }
            else
            {
                _damageTimer += Time.deltaTime;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_mapCenter,_redSeaDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_mapCenter,_redSeaDistance + _smoothDistance);
    }
#endif
}
