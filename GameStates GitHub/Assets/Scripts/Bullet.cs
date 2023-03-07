using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private float speedBullet = 15f;
    private int damageBullet = 1;
    private Vector2 _bulletDirection;

    public Vector2 BulletDirection { get => _bulletDirection; set => _bulletDirection = value; }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb2D.velocity = BulletDirection * speedBullet;  
    }

    public int DamageBullet()
    {
        return damageBullet;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
