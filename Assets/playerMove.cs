// using System.Numerics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMove : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] weaponScript weapon;
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float runSpeed = 10;
    [SerializeField] float rollSpeed = 18;


    [SerializeField] Transform cam;

    Animator playerAnim;
    public healthPoints playerHP;

    [SerializeField] AudioSource runSound;
    [SerializeField] AudioSource rollSound;
    [SerializeField] AudioSource swingSound;
    [SerializeField] public AudioSource hurtSound;
    [SerializeField] public AudioSource deathSound;


    bool isRolling = false;
    bool isAttacking = false;
    public bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponentInChildren<Animator>();
        weapon.disableCollisionTrigger();
        playerHP = GetComponent<healthPoints>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHP.HP <= 0){
            StartCoroutine(Dying());
        }
        else{
            float horInput = Input.GetAxisRaw("Horizontal") * moveSpeed;
            float verInput = Input.GetAxisRaw("Vertical") * moveSpeed;

            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0;
            camRight.y = 0;

            Vector3 forwardRelative = verInput*camForward;
            Vector3 rightRelative = horInput*camRight;

            Vector3 moveDir = forwardRelative + rightRelative;

            if(!isAttacking) rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
            else rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            if(Input.GetKey(KeyCode.Space) && !isAttacking){
                StartCoroutine(Attack());
            }

            if(Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && !isRolling){
                if(playerAnim.GetBool("isWalking")){
                    StartCoroutine(RollTimeout());
                }
            }
            // && Mathf.Approximately(rb.linearVelocity.y, 0)
            
                // rb.linearVelocity = new Vector3(rb.linearVelocity.x, jump, rb.linearVelocity.z);


            if(rb.linearVelocity.x != 0 || rb.linearVelocity.z != 0 && !isAttacking && !isRolling){

                //face player to movement direction
                transform.forward += new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z) * Time.deltaTime * 2;

                playerAnim.SetBool("isWalking", true);

                if(!runSound.isPlaying && !isRolling) runSound.Play();            
            }
            else{
                playerAnim.SetBool("isWalking", false);

                runSound.Pause();
            }

            if(!isAttacking) swingSound.Stop();
            else transform.forward += moveDir * Time.deltaTime * 2;
        }

        if(!isRolling) rollSound.Pause();
    }

    IEnumerator RollTimeout(){
        moveSpeed = rollSpeed;
        playerAnim.SetTrigger("roll");
        runSound.Pause();
        isRolling = true;
        rollSound.Play();
        yield return new WaitForSeconds(0.3f);
        // yield return new WaitUntil(
        //     () => !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("roll")
        // );        
        moveSpeed = runSpeed;
        isRolling = false;
    }

    IEnumerator Attack(){
        swingSound.Play();
        playerAnim.SetTrigger("attack");
        Debug.Log("attacking started");
        isAttacking = true;
        yield return new WaitForSeconds(0.3f);
        weapon.enableCollisionTrigger();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
        weapon.disableCollisionTrigger();


    }

    IEnumerator Dying(){
        isDead = true;
        playerAnim.SetTrigger("die");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync("GameOver");
    }
}

