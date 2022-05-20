using UnityEngine;

/// <summary>
/// Simply rotates the given object this component is on
/// </summary>
public class CloudRotate : MonoBehaviour
{
    [SerializeField] private float _speed;
    
    /// <summary>
    /// Rotates the transform
    /// </summary>
    private void Update()
    {
        transform.Rotate(Vector3.up * _speed);
    }
}
