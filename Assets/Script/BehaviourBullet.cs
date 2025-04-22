using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourBullet : MonoBehaviour
{
    [SerializeField] private BaseBullet bullet;

    private Vector3 _direction;

    public Vector3 Direction
    {
        get => _direction;
        set => _direction = value.normalized;
    }

    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        rb.velocity = _direction * (bullet.speed * Time.deltaTime);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
    }
}