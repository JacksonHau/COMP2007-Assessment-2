using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 20;

    [Header("Audio")]
    public AudioClip growlClip;
    public float minGrowlInterval = 5f;
    public float maxGrowlInterval = 12f;

    [Header("FX")]
    public GameObject bloodFX;
    public GameObject deathFX;

    private float currentHealth;
    private float lastAttackTime;
    private Transform player;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerHealth playerHealth;
    private bool isAttacking = false;
    private bool isDead = false;
    private string currentAnim;
    private float nextGrowlTime;

    // NavMeshAgent for movement & avoidance
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Configure agent to match stats
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed * 100f;
        agent.stoppingDistance = attackRange;
        agent.acceleration = moveSpeed * 2f;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerHealth = player?.GetComponent<PlayerHealth>();

        currentHealth = maxHealth;
        PlayAnim("Zombie Idle");
        ScheduleNextGrowl();
    }

    // Update is called once per frame
    void Update()
    {
        // 1) Pause handling: stop AI & audio
        if (PauseMenu.isPaused)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            agent.isStopped = true;
            return;
        }
        // 2) Resume any paused growl
        if (!audioSource.isPlaying && audioSource.time > 0f)
            audioSource.UnPause();

        if (isDead || player == null) return;

        // Growl if it's time
        if (Time.time >= nextGrowlTime)
        {
            PlayGrowl();
            ScheduleNextGrowl();
        }

        if (isAttacking) return;

        // If player dead, idle
        if (playerHealth != null && playerHealth.IsDead())
        {
            agent.isStopped = true;
            PlayAnim("Zombie Idle");
            return;
        }

        // Move or attack based on distance
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            PlayAnim("Zombie Walk");
        }
        else
        {
            agent.isStopped = true;
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackPlayer());
                lastAttackTime = Time.time;
            }
            else
            {
                PlayAnim("Zombie Idle");
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        PlayAnim("Zombie Punch");
        yield return new WaitForSeconds(0.5f);

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            if (bloodFX != null)
                Instantiate(bloodFX, player.position + Vector3.up * 1f, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (bloodFX != null)
            Instantiate(bloodFX, hitPoint, Quaternion.identity);

        if (currentHealth <= 0f)
            Die();
        else
            StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        PlayAnim("Zombie Reaction Hit");
        yield return new WaitForSeconds(0.5f);
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        PlayAnim("Zombie Death");
        if (deathFX != null)
            Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject, 3f);
    }

    void PlayAnim(string animName)
    {
        if (currentAnim == animName) return;
        animator.Play(animName);
        currentAnim = animName;
    }

    void PlayGrowl()
    {
        if (growlClip != null)
            audioSource.PlayOneShot(growlClip);
    }

    void ScheduleNextGrowl()
    {
        nextGrowlTime = Time.time + Random.Range(minGrowlInterval, maxGrowlInterval);
    }
}
