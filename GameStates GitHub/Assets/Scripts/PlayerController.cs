using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private float speed, jumpForce;
    private int health;
    private float inputX;
    private bool isFacingRight, ableToShoot;
    private Transform groundPosition, shootPosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject bulletPrefab;


    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        ableToShoot = true;
        health = 4;
        isFacingRight = true;
    }

    private void Start()
    {
        groundPosition = GameObject.Find("Ground Position").GetComponent<Transform>();
        shootPosition = GameObject.Find("Shoot Position").GetComponent<Transform>();
    }

    private void Update()
    {
        //I know is not the best practice to use if statements inside another if, but for this example code is enough, if you want to use it, you can use a switch statement instead.
        if(GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            inputX = Input.GetAxisRaw("Horizontal");
            CheckAndSetDirection();

            if (Input.GetButtonDown("Jump") && IsTouchingTheGround())
            {
                Jump();
            }
            if (Input.GetButtonDown("Fire1") && ableToShoot)
            {
                Shoot();
                StartCoroutine("TimeBetweenShoots");
                ableToShoot = false;
            }
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    IEnumerator TimeBetweenShoots()
    {
        yield return new WaitForSeconds(0.2f);
        ableToShoot = true;
    }

    private void Move()
    {
        rb2D.velocity = new Vector2(inputX * speed, rb2D.velocity.y);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0,20,100,100));
        GUILayout.TextArea("Player Health: " + health);
        GUILayout.EndArea();
    }

    private void CheckAndSetDirection()
    {
        if (rb2D.velocity.x > 0)
        {
            transform.localScale = Vector3.one;
            isFacingRight = true;
        }
        else if (rb2D.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
    }

      private void Jump()
      {
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
      }

    private void Shoot()
    {
       var tempBullet = Instantiate(bulletPrefab, shootPosition.position, Quaternion.identity);
       var tempBulletScript =  tempBullet.GetComponent<Bullet>();

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
        GetComponent<SpriteRenderer>().color = Color.red;
        if (isFacingRight) { transform.localRotation = Quaternion.Euler(0f, 0f, 120f); }
        else { transform.localRotation = Quaternion.Euler(0f, 0f, -120f); }
        GameManager.gameOver = true;
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
