using UnityEngine;

public class WreckingBall : MonoBehaviour
{
   private Transform _ball;
    [SerializeField] private Vector3 _axis;
    [SerializeField] private float _height;
    [SerializeField] private float _speed;
    [SerializeField] private float _ropeLength;

    private void Start()
    {
        _ball = transform.GetChild(0);
    }

    private void Update()
    {
        Debug.DrawLine(transform.position,_ball.position);

        var angle = _height * Mathf.Sin(Time.time * _speed);
        transform.rotation = Quaternion.AngleAxis(angle, _axis);
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _ball.position = -transform.up * _ropeLength;
    }
#endif

}
