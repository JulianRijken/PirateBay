using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Pickup : MonoBehaviour
{
    [SerializeField] private Effect _effect;
    [SerializeField] private GameObject _pickupVFX;
    [SerializeField] private AudioClip _pickUpAudio;
    
    private bool _pickedUp = false;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _rigidbody?.AddTorque(Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_pickedUp)
            return;
        
        if (!collision.collider) 
            return;
        
        var pickupInterface = collision.gameObject.GetComponent<ICanPickup>();
        if (pickupInterface == null) 
            return;
        
        if(!pickupInterface.CanPickup())
            return;
        
        _audioSource.PlayOneShot(_pickUpAudio);
        _pickedUp = true;
        OnPickupPickedUp(pickupInterface);
    }

    private void OnPickupPickedUp(ICanPickup pickupInterface)
    {
        // Pickup the pickup...
        pickupInterface.OnPickup(_effect);
        
        // Show pickup effect
        var effect = Instantiate(_pickupVFX, transform.position - Vector3.up, Quaternion.identity);
        effect.gameObject.transform.localScale *= 3f;
        Destroy(effect,5f);

        // Scale down pickup and destroy
        StartCoroutine(ScaleDown());
        IEnumerator ScaleDown()
        {
            while (true)
            {
                transform.localScale += -Vector3.one * Time.deltaTime;
                
                if(transform.lossyScale.x < 0)
                    Destroy(gameObject);

                yield return null;
            }
        }
    }
    

}

[Serializable]
public struct Effect
{
    public EffectType EffectType;
    public float EffectDuration;
}

public enum EffectType
{
    Speed,
    Attack,
    Heal
}