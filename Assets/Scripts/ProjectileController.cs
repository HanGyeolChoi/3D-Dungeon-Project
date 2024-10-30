using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    public float projectileSpeed = 20f;
    public WeaponData data;
    private Rigidbody _rigidbody;
    private Vector3 initialPosition;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, initialPosition) > 40) Destroy(gameObject);
        Move(projectileSpeed);
    }
    private void Move(float speed)
    {
        Vector3 dir = transform.up * speed;
        _rigidbody.velocity = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsLayerMatched(enemyLayerMask.value, other.gameObject.layer))
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            damagable.TakeDamage(data.damage);
            Destroy(gameObject);
        }

        else if(IsLayerMatched(groundLayerMask.value, other.gameObject.layer))
        {
            Destroy(gameObject);
        }
    }

    private bool IsLayerMatched(int layerMask, int objectLayer)
    {
        return layerMask == (layerMask | (1 << objectLayer));
    }
}
