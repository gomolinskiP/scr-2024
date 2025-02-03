using UnityEngine;

public class weaponScript : MonoBehaviour
{

    [SerializeField] float damage = 10;
    BoxCollider triggerBox;

    private void Start(){
        triggerBox = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other){
        var enemy = other.gameObject.GetComponent<enemyAI>();

        if (enemy != null){
            disableCollisionTrigger();
            if(enemy.isAttacking) enemy.supressAttack();
            enemy.health.HP -= damage;
            enemy.hurtSound.Play();
            enemy.knockedBack(transform.position);
            Debug.Log("enemy hit");
        }
    }

    public void disableCollisionTrigger(){
        triggerBox.enabled = false;
    }

    public void enableCollisionTrigger(){
        triggerBox.enabled = true;
    }

}
