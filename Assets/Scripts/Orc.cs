using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Orc : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public gameplay knight;
    private Rigidbody2D rb;
    private Animator animator; // Reference to the animator

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackRange = 2f;
    private bool isAttacking = false;

    private bool isDamaged = false;

    public AudioSource orcAudioSource;
    public AudioClip orcAttackSound;
    public AudioClip orcDeathSound;

    public CapsuleCollider2D orcCollider;  // Parent's collider for attacks

    private bool playerInRange = false;

    public GameManager gameManager;
    public int health = 10;
    public GameObject statue;

    private int orcsDefeated = 0;
    private int totalOrcs = 5;
    public SimpleGameEvent onGameWinEvent;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        orcCollider = GetComponent<CapsuleCollider2D>();
        gameManager = GameManager.Instance;
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                knight = playerObj.GetComponent<gameplay>();
            }
        }
    }

    private void Update()
    {
        // Check if player reference is valid
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // Don't process attack/movement logic if damaged
        if (isDamaged)
        {
            rb.velocity = Vector2.zero;
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsWalking", false);
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If in attack range and not already attacking
        if ((distanceToPlayer <= attackRange) && !isAttacking)
        {
            StartAttack();
        }

        // If not attacking, continue moving
        if (!isAttacking)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Set walking animation
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsAttacking", false);

        // Flip sprite based on direction
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * direction,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    public void StartAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // Stop moving
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsWalking", true); // set it back to true just to trigger the walking animation back
    }

    // Called by Animation Event at the end of attack animation
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsWalking", true);
    }

    // Optional: Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public async Task TakeDamage(int damage)
    {
        if (isDamaged)
        {
            return;
        }
        Debug.Log($"Orc took {damage} damage!");
        isDamaged = true;
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsWalking", false);

        health -= damage;

        // Increment score on every hit
        gameManager.IncreaseScore(5);  // 5 points per hit

        // Check if orc dies
        if (health <= 0)
        {
            gameManager.IncreaseScore(10);  // Bonus points for kill
            OrcDefeated();
            PlayDeathSound();
            animator.SetTrigger("Kill");
            StartCoroutine(DestroyOrcBody());
            // SECTION - Spawn statue here to go to next level
            // Get the statue's rigidbody and make it dynamic
            if (statue != null)
            {
                Rigidbody2D statueRb = statue.GetComponent<Rigidbody2D>();
                if (statueRb != null)
                {
                    statueRb.bodyType = RigidbodyType2D.Dynamic;
                    // Make the statue very heavy when it falls
                    statueRb.mass = 100f;
                }
            }
        }
        else
        {
            animator.SetTrigger("Damaged");
            ResetAfterDamage();
        }
    }

    private IEnumerator DestroyOrcBody()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    // Now called by coroutine instead of animation event
    public async Task ResetAfterDamage()
    {
        await Task.Delay(1000); // 1 second damage state
        isDamaged = false;
        animator.SetBool("IsWalking", true);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            knight = other.GetComponent<gameplay>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Called by Animation Event during attack animation

    public async void StartDamageCoroutine()
    {
        if (knight != null && playerInRange && !knight.invincible)
        {
            knight.TakeDamage(1);
            await knight.Damaged();
        }
    }

    public void PlayAttackSound()
    {
        orcAudioSource.PlayOneShot(orcAttackSound);
    }

    public void PlayDeathSound()
    {
        orcAudioSource.PlayOneShot(orcDeathSound);
    }

    public void OrcDefeated()
    {
        orcsDefeated++;
        if (SceneManager.GetActiveScene().name == "SecondScene" && orcsDefeated >= totalOrcs)
        {
            onGameWinEvent.Raise(null); // Instead of GameManager.Instance.GameWin()
        }
    }
}
