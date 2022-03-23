using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Attributes))]
public class Enemy : MonoBehaviour
{
    
    [SerializeField] private Transform _model;

    protected float MoveSpeed
    {
        get => _agent.speed;
        set => _agent.speed = value;
    }

    protected float Acceleration
    {
        get => _agent.acceleration;
        set => _agent.acceleration = value;
    }

    protected Vector3 TargetLocation
    {
        get => _agent.destination;
        set => _agent.destination = value;
    }

    protected float RotateSpeed;
    protected RotateTowardsOptions TargetRotation;
    
    protected Attributes EnemyAttributes { get; private set; }
    protected Transform Model => _model;

    
    
    private NavMeshAgent _agent;

    
    public delegate void OnRemovedEvent(Enemy enemyRemoved);
    public event OnRemovedEvent OnRemoved;
    
    
    protected enum RotateTowardsOptions
    {
        TargetLocation,
        Player,
        Velocity
    }
    
    
    protected virtual void Awake()
    {
        EnemyAttributes = GetComponent<Attributes>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.angularSpeed = 0f;
        
        EnemyAttributes.OnDeath += OnDeath;
    }

    /// <summary>
    /// Rotates The Enemy Towards The TargetRotation
    /// </summary>
    protected virtual void Update()
    {
        UpdateRotation();
    }
    
    private void OnDestroy()
    {
        OnRemoved?.Invoke(this);
    }
    
    
    protected virtual void OnDeath(GameObject instigator)
    {
        Destroy(gameObject);
    }

    private void UpdateRotation()
    {
        var targetRotation = TargetRotation switch
        {
            RotateTowardsOptions.TargetLocation => GetRotationFromDirection(_model.position - TargetLocation),
            
            RotateTowardsOptions.Player => GetRotationFromDirection(GameManager.PlayerController.Location - _model.position),
            
            RotateTowardsOptions.Velocity => GetRotationFromDirection(_agent.velocity),
        };
        
        _model.localRotation = Quaternion.Slerp(_model.localRotation, targetRotation,RotateSpeed * Time.deltaTime);
    }

    
    private Quaternion GetRotationFromDirection(Vector3 direction)
    {
        var targetRotation = _model.rotation;
        
        if (direction.XZ().magnitude > 0f)
        {
            targetRotation = Quaternion.LookRotation(direction.XZ(), Vector3.up);
        }

        return targetRotation;
    }
}
