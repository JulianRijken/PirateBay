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

    
    protected float DistanceFromTarget => Vector3.Distance(_model.position, TargetLocation);
    protected float DistanceFromPlayer => Vector3.Distance(_model.position, GameManager.ActivePlayerController.Location);

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
        EnemyAttributes.OnHealthZero += OnHealthZero;
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
    
    
    protected virtual void OnHealthZero(GameObject instigator)
    {
        Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    protected virtual bool GetLocationAroundPlayer(float radius, float maxDistanceToFindNavMesh, out NavMeshHit hit, int areaMask = 1)
    {
        // Get random position
        var randomOffset = Random.onUnitSphere *  Random.Range(radius, radius);
        var randomPoint = GameManager.ActivePlayerController.Location + new Vector3(randomOffset.x,0,randomOffset.y);
        return NavMesh.SamplePosition(randomPoint, out hit, maxDistanceToFindNavMesh, areaMask);
    }

    private void UpdateRotation()
    {
        var targetRotation = TargetRotation switch
        {
            RotateTowardsOptions.TargetLocation => GetRotationFromDirection(_model.position - TargetLocation),
            
            RotateTowardsOptions.Player => GetRotationFromDirection(GameManager.ActivePlayerController.Location - _model.position),
            
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
