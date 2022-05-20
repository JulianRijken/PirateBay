using UnityEngine;

/// <summary>
/// Handles the buoyancy for all objects in the water including:
/// The ship class
/// The pickup class
/// The explosive barrel class
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuoyancyEffector3D : MonoBehaviour
{
    
    [SerializeField] private Transform[] _floatingPoints;
    [SerializeField] private float _waterHeight = 0f;
    [SerializeField] public float FloatingPower;
    
    [SerializeField] private float _underWaterDrag = 5f;
    [SerializeField] private float _underWaterAngularDrag = 2f;
    
    private float _normalAngularDrag;
    private float _normalDrag;
    
    private bool _underwater;
    private Rigidbody _rigidbody;

    public bool IsUnderWater => _underwater;

    /// <summary>
    /// Sets the normal drag as default
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _normalAngularDrag = _rigidbody.angularDrag;
        _normalDrag = _rigidbody.drag;
    }

    /// <summary>
    /// Calls HandleBuoyancy();
    /// </summary>
    private void FixedUpdate()
    {
        HandleBuoyancy();
    }
    
    
    /// <summary>
    /// Handles the Buoyancy by adding force to the rigidbody and changing the drag
    /// </summary>
    private void HandleBuoyancy()
    {
        foreach (var floatingPoint in _floatingPoints)
        {
            var waterDifference = floatingPoint.position.y - _waterHeight;
            
            // If floating point is under water
            if (waterDifference < 0)
            {
                _rigidbody.AddForceAtPosition(Vector3.up * (FloatingPower * Mathf.Abs(waterDifference)), floatingPoint.position, ForceMode.Force);

                if (_underwater) 
                    continue;
                
                SetUnderWater(true);
                return;
            }
        }
        
        SetUnderWater(false);
    }

    /// <summary>
    /// Changes the underwater state
    /// </summary>
    /// <param name="underwater"></param>
    private void SetUnderWater(bool underwater)
    {
        _underwater = underwater;

        if (underwater)
        {
            _rigidbody.drag = _underWaterDrag;
            _rigidbody.angularDrag = _underWaterAngularDrag;
        }
        else
        {
            _rigidbody.drag = _normalDrag;
            _rigidbody.angularDrag = _normalAngularDrag;
        }
    }
}