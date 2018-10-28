﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileParticle : MonoBehaviour
{
    public ParticleSystem particleSystemForTower;
    public Transform enemyToAttack;
    public float playTime;
    public float timer;
    public float particleVelocity;
    public Rigidbody pSRB;
    public Vector3 enemyPos;


    public void setParms(ParticleSystem particleSystem, Collider enemy, float speed)
    {
        enemyToAttack = enemy.gameObject.GetComponent<Transform>();
        particleSystemForTower = particleSystem;
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
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Die();
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
