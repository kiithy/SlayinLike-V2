using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class gameplay : Singleton<gameplay>
{
    private KnightActions controls;
    private Vector2 moveInput;
    private bool isJumping;
    private bool isJumpHolding;
    private Rigidbody2D knightBody;
    private SpriteRenderer knightSprite;
    public float maxSpeed = 10;
    public float speed = 5;
    public float upSpeed = 4;
    public float doubleJumpSpeed;
    private bool faceRightState = true;
    public Animator knightAnimator;
    private bool jumpedState = false;
    private bool onGroundState = true;
    private bool alive = true;
    private bool moving = false;
    private bool damaged = false;
    public bool invincible = false;
    public int attackCombo = 0;
    private float comboTimer = 0;
    private float comboTimeWindow = 0.5f;
    private int health;
    public GameObject swordHitbox;
    private Collider2D swordCollider;
    private CapsuleCollider2D knightCollider;

    public AudioSource knightAudioSource;
    public AudioClip knightAttack1Sound;
    public AudioClip knightAttack2Sound;
    public AudioClip knightDeathSound;

    private GameManager gameManager;
    private Vector3 startingPosition;
    public GameConstants gameConstants;
    public SimpleGameEvent onGameOverEvent;
    private void Awake()
    {
        base.Awake();
        controls = new KnightActions();
        swordCollider = swordHitbox.GetComponent<Collider2D>();
        swordCollider.enabled = false;
        knightCollider = GetComponent<CapsuleCollider2D>();
    }


    void Move(int value)
    {
        Vector2 movement = new Vector2(value * speed, knightBody.velocity.y);
        knightBody.velocity = movement;
    }

    void FlipKnightSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            knightSprite.flipX = true;
            // Position sword hitbox on left side
            Vector3 localOffset = new Vector3(0, 0f, 0);
            swordHitbox.transform.localPosition = localOffset;
            swordHitbox.transform.localScale = new Vector3(-1, 1, 1);
            Debug.Log("Flip to left");
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            knightSprite.flipX = false;
            // Position sword hitbox on right side
            Vector3 localOffset = new Vector3(0, 0f, 0);
            swordHitbox.transform.localPosition = localOffset;
            swordHitbox.transform.localScale = new Vector3(1, 1, 1);
            Debug.Log("Flip to right");
        }
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
            knightAnimator.SetBool("Moving", moving);
            knightBody.velocity = new Vector2(0, knightBody.velocity.y);
        }
        else
        {
            FlipKnightSprite(value);
            moving = true;
            knightAnimator.SetBool("Moving", moving);
            Move(value);
        }

    }
    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            knightBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            knightAnimator.SetBool("OnGroundState", onGroundState);

        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            knightBody.AddForce(Vector2.up * upSpeed * (doubleJumpSpeed * 10), ForceMode2D.Force);
            jumpedState = false;
        }
    }

    public void Attack()
    {
        if (alive)
        {
            if (comboTimer > 0 && attackCombo == 1)
            {
                attackCombo = 2;
            }
            else
            {
                attackCombo = 1;
            }

            // // Test direct activation
            // activateHitBox();

            knightAnimator.SetInteger("AttackType", attackCombo);
            knightAnimator.SetTrigger("Attack");

            comboTimer = comboTimeWindow; // Reset combo timer
        }
    }

    public void activateHitBox()
    {
        swordCollider.enabled = true;
        Debug.Log("Hitbox activated");
    }

    public void deactivateHitBox()
    {
        swordCollider.enabled = false;
        Debug.Log("Hitbox deactivated");
    }

    private void UpdateComboTimer()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            attackCombo = 0;
            knightAnimator.SetInteger("AttackType", attackCombo);
        }
    }

    private void OnJumpHold(InputAction.CallbackContext context)
    {
        isJumpHolding = context.ReadValueAsButton();
    }

    // Start is called before the first frame update
    void Start()
    {
        knightBody = GetComponent<Rigidbody2D>();
        knightSprite = GetComponent<SpriteRenderer>();
        knightAnimator.SetBool("OnGroundState", onGroundState);
        knightCollider = GetComponent<CapsuleCollider2D>();
        health = gameConstants.playerHealth > 0 ?
        gameConstants.playerHealth : HUDManager.Instance.maxPlayerHealth;
        startingPosition = transform.position;
        SceneManager.activeSceneChanged += SetStartingPosition;
    }

    void SetStartingPosition(Scene current, Scene next)
    {
        if (next.name == "SecondScene")
        {
            this.transform.position = startingPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateComboTimer();
    }


    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("OnGround");
            onGroundState = true;
            knightAnimator.SetBool("OnGroundState", onGroundState);
        }

    }

    public async Task Damaged()
    {
        if (invincible) return;

        invincible = true;
        damaged = true;
        knightAnimator.SetBool("Damaged", damaged);

        await Task.Delay(2000); // 2000ms = 2 seconds

        damaged = false;
        knightAnimator.SetBool("Damaged", damaged);
        invincible = false;
    }

    // IEnumerator ActivateSwordHitbox()
    // {
    //     swordCollider.enabled = true;
    //     yield return new WaitForSeconds(0.2f);
    //     swordCollider.enabled = false;
    // }

    public void PlayAttack1Sound()
    {
        knightAudioSource.PlayOneShot(knightAttack1Sound);
    }

    public void PlayAttack2Sound()
    {
        knightAudioSource.PlayOneShot(knightAttack2Sound);
    }

    public async void TakeDamage(int damage)
    {
        health -= damage;
        GameManager.Instance.SetHealth(health);
        if (health <= 0)
        {
            onGameOverEvent.Raise(null);
            return;
        }
        await Damaged();
    }
}
