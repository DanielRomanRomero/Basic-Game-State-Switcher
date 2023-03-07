using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private Transform myPosition;
    [SerializeField] private float speed, jumpForce;
    private int health;
    private bool isFacingRight, ableToShoot, isAlive;
    [SerializeField] private Transform groundPosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private  GameObject bulletPrefab;
    [SerializeField] private float targetDistance; // Set this to change the distance that enemy takes to stop moving and start shotting.

    private GameObject player;
    private Transform target;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        myPosition = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ableToShoot = true;
        isAlive = true;
        health = 2;
    }


    private void Start()
    {
        player = GameObject.Find("Player");  
    }
    private void Update()
    {
        if (isAlive)
        {
            MoveToPlayerAndShoot();
            CheckAndSetDirection();
        }
    }


    IEnumerator TimeBetweenShoots()
    {
        yield return new WaitForSeconds(1f);
        ableToShoot = true;
    }

    IEnumerator DestroyAfterSomeTime()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    private void GetPositionPlayer()
    {
        target = player.GetComponent<Transform>();
    }

    private void MoveToPlayerAndShoot()
    {
        GetPositionPlayer();
        float distance = Vector2.Distance(myPosition.position, target.position);

        if (distance > targetDistance && IsTouchingTheGround())
        {
            rb2D.velocity = target.transform.position - myPosition.transform.position;
            rb2D.velocity = rb2D.velocity.normalized;
            rb2D.velocity = rb2D.velocity * speed;
        }
        else if(distance < targetDistance && ableToShoot)
        {
            Shoot();
            StartCoroutine("TimeBetweenShoots");
            ableToShoot = false;
        }

        if (PlayerIsInHigherPosition() && IsTouchingTheGround())
        {
            Jump();
        }
    }

    private void CheckAndSetDirection()
    {
        if(target.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
        else if(target.position.x > transform.position.x)
        {
            transform.localScale = Vector3.one;
            isFacingRight = true;
        }
    }
   
  
    private void Jump()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
    }

    private void Shoot()
    {
        var tempBullet = Instantiate(bulletPrefab, shootPosition.position, Quaternion.identity);
        var tempBulletScript = tempBullet.GetComponent<Bullet>();

        if (isFacingRight)
        {
            tempBulletScript.BulletDirection = Vector2.right;
        }
        else
        {
            tempBulletScript.BulletDirection = Vector2.left;
        }

    }

    private void Death()
    {
        spriteRenderer.color = Color.red;
        isAlive = false;

        if (isFacingRight) { myPosition.localRotation = Quaternion.Euler(0f, 0f, 90f); }
        else { myPosition.localRotation = Quaternion.Euler(0f, 0f, -90f); }

        GetComponent<BoxCollider2D>().enabled = false;
        SpawnerEnemies.enemiesDefeat++;
        StartCoroutine("DestroyAfterSomeTime");
    }

    private bool IsTouchingTheGround()
    {
        if (Physics2D.Raycast(groundPosition.position, Vector2.down, 0.2f, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool PlayerIsInHigherPosition()
    {
        if(target.position.y > myPosition.position.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            var tempDamage = other.GetComponent<Bullet>();
            var damageNumber = tempDamage.DamageBullet();
            health = health - damageNumber;
            Destroy(other.gameObject);

            if (health <= 0)
            {
                health = 0;
                Death();
            }
        }
    }
}
