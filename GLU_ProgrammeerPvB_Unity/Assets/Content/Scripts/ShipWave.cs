using UnityEngine;

public class ShipWave : MonoBehaviour
{

    [SerializeField] private Vector2 _noiseRange;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveHeight = 0.4f;
    [SerializeField] private float _rotationSpeed = 0.4f;
    [SerializeField] private Vector3 _rotationMultiplier = Vector3.one;
    [SerializeField] private Vector3 _rotationTimeOffset = Vector3.one;

    private float randomOffset;

    private void Awake()
    {
        randomOffset = Random.value * 1000;
    }

    private void Update()
    {
        HandleMovement();
    }
	
    private void HandleMovement()
    {
        // Get noise
        var noiseTime = Time.time * _moveSpeed + randomOffset;
        var noise = Mathf.PerlinNoise(noiseTime,noiseTime);
        noise = Mathf.Lerp(_noiseRange.x, _noiseRange.y, noise);
		
		
		
        // Get the target position
        var targetLocalPosition = Vector3.zero;
        targetLocalPosition.y = noise * _moveHeight;
		
        // Set the target position
        transform.localPosition = targetLocalPosition;
		
		
		
        // Get target rotation
        var targetRotation = Quaternion.identity;
        targetRotation.x = Mathf.Sin(Time.time * _rotationSpeed + _rotationTimeOffset.x + randomOffset) * (_rotationMultiplier.x / 180) * noise;
        targetRotation.y = Mathf.Sin(Time.time * _rotationSpeed + _rotationTimeOffset.y + randomOffset) * (_rotationMultiplier.y / 180) * noise;
        targetRotation.z = Mathf.Sin(Time.time * _rotationSpeed + _rotationTimeOffset.z + randomOffset) * (_rotationMultiplier.z / 180) * noise;
		
        // Set the target rotation
        transform.localRotation = targetRotation;
    }
}