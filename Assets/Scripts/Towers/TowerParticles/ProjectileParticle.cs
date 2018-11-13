using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileParticle : MonoBehaviour
{
    public Transform enemyToAttack;
    public float playTime;
    public float timer;
    public float particleVelocity;
    public Rigidbody pSRB;
    public Vector3 enemyPos;

    public delegate void taskDelegate(Collider c);
    public taskDelegate projectileDelegate;

    public void setParms(Collider enemy, float speed)
    {
        enemyToAttack = enemy.gameObject.GetComponent<Transform>();
        particleVelocity = speed;
    }

    private void Start()
    {
       pSRB = GetComponent<Rigidbody>();
        
    }
    // Use this for initialization
    void Attack ()
    {
        if (enemyToAttack != null)
        {
            enemyPos = enemyToAttack.position;
            Vector3 targetVec = enemyPos - this.transform.position;
            transform.LookAt(targetVec);
            targetVec = targetVec.normalized;
            pSRB.velocity = targetVec * Time.deltaTime * particleVelocity;
        }
        else
        {
            //Die();
        }
    }

    void doDamage(taskDelegate td)
    {

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == enemyToAttack.GetComponent<Collider>().gameObject)
        {
            projectileDelegate(enemyToAttack.GetComponent<Collider>());
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }


    void Update()
    {
        Attack();

    }
}
