using System.Collections;
// using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class enemyAI : MonoBehaviour
{
    GameObject Player;
    NavMeshAgent agent;
    Rigidbody rb;
    Animator enemyAnim;
    BoxCollider attackHitbox;
    [SerializeField] LayerMask groundLayer, playerLayer;
    [SerializeField] public AudioSource hurtSound;
    [SerializeField] public AudioSource dieSound;
    [SerializeField] Score score;
    [SerializeField] float walkSpeed;

    Vector3 ogPosition;

    GameObject head;

    [SerializeField] float attackRange;
    bool playerInAttackRange;
    bool isDead = false;
    public bool isAttacking = false;

    public healthPoints health;
    public healthPoints playerHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // agent.speed = 0.5f;
        Player = GameObject.Find("Player");
        playerHealth = Player.GetComponent<healthPoints>();
        score = Player.GetComponent<Score>();
        health = GetComponent<healthPoints>();
        rb = GetComponent<Rigidbody>();
        enemyAnim = GetComponentInChildren<Animator>();
        head = new List<GameObject>(GameObject.FindGameObjectsWithTag("enemyAttackHitbox")).Find(g => g.transform.IsChildOf(this.transform));
        // head = GameObject.FindGameObjectWithTag("enemyAttackHitbox");

        attackHitbox = head.GetComponent<BoxCollider>();

        ogPosition = transform.position;
        Debug.Log(attackHitbox);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth.HP > 0){
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

            if(agent.enabled == true && !isAttacking){
                if(!playerInAttackRange) followPlayer();
                else attack();
            }
        }
        else{
            enemyAnim.SetTrigger("playerDead");
        }

        if(transform.position.y < -5f && !isDead){
            rb.linearVelocity = Vector3.zero;
            transform.position = ogPosition;
            health.HP = 30;
            score.points += 1;
        }
    }

    void followPlayer(){
        agent.speed = walkSpeed;
        agent.SetDestination(Player.transform.position);


        // else agent.SetDestination(destinationPoint);

        // if(Vector3.Distance(transform.position, destinationPoint) < 10) destPointSet = false;
    }

    void attack(){
        isAttacking = true;

        agent.SetDestination(transform.position);
        Debug.Log("attacking");
        StartCoroutine(AttackCorout());
    }


    public void HeadCollision(Collider other){
        var player = other.gameObject.GetComponent<playerMove>();
        Debug.Log("HeadCollision");
        if (player != null){
            // disableCollisionTrigger();
            player.playerHP.HP -= 10;
            player.hurtSound.Play();
            if(player.playerHP.HP <= 0) player.deathSound.Play();
            // enemy.knockedBack(transform.position);
            Debug.Log("player hit");
        }
    }

    public void knockedBack(Vector3 weaponPosition){
        Vector3 direction = (transform.position - weaponPosition).normalized;
        if(agent.enabled == true)
            StartCoroutine(KnockbackCoroutine(direction));
        else rb.AddForce(direction, ForceMode.Impulse);
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction){
        enemyAnim.SetTrigger("hit");
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(direction, ForceMode.Impulse);

        if(health.HP <= 0){
            isDead = true;
            dieSound.Play();
            enemyAnim.SetTrigger("dead");
            score.points += 1;
            StartCoroutine(Respawn());
        }

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(
            () => rb.linearVelocity.magnitude < 0.01f
        );

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        if(!isDead){
            enemyAnim.SetTrigger("gettingUp");
            agent.Warp(transform.position);
            agent.enabled = true;
        }
    }

    private IEnumerator AttackCorout(){
        enemyAnim.SetTrigger("attack");
        for(int f = 0; f < 60; f++){
            // transform.LookAt(Player.transform);
            var targetRotation = Quaternion.LookRotation(Player.transform.position - transform.position);
        
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }

        enableCollisionTrigger();
        Debug.Log("enemy hitbox active");
        yield return new WaitForSeconds(0.40f);
        // disableCollisionTrigger();
        Debug.Log("enemy hitbox inactive");
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    private IEnumerator Respawn(){
        float newY;

        yield return new WaitForSeconds(3f);
        while(transform.position.y > -5f){
            newY = transform.position.y - 0.1f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }
        transform.position = ogPosition;
        isDead = false;
        health.HP = 30;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        enemyAnim.SetTrigger("respawn");
        agent.Warp(transform.position);
        yield return new WaitForSeconds(1f);
        agent.enabled = true;
    }

    public void supressAttack(){
        StopCoroutine(AttackCorout());
        disableCollisionTrigger();
        Debug.Log("enemy hitbox inactive");
        isAttacking = false;
    }

    public void disableCollisionTrigger(){
        attackHitbox.enabled = false;
    }

    public void enableCollisionTrigger(){
        attackHitbox.enabled = true;
    }

}


