﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    bool active;

    [Header("Control Variables")]
    ZombieStats zs;
    public bool isAlive;
    public bool isAttacking;

    [Header("Detection variables")]

    public SphereCollider hearArea;
    public float sightRadius;
    public float hearRadiusWithTarget;
    public float noisetrigger;
    public float sightDistance;
    public LayerMask PlayerMask;


    [Header("Zombie Variables")]
    public PlayerStats target;
    private Rigidbody rb;
    public float zombieSpeed;
    public float rotationSpeed;

    [Header("Movement Variables")]
    NavMeshAgent agent;

    private Animator anim;
    Vector3 playerlastpos;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetActive());
        zs = GetComponent<ZombieStats>();
        isAlive = true;
        isAttacking = false;
        playerlastpos = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        hearArea.radius = sightRadius;
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            if (!isAlive)
            {
                return;
            }

            if (!isAttacking)
            {
                if (target)
                {

                    float distance = Vector3.Distance(target.transform.position, transform.position);

                    if (distance < sightDistance)
                    {
                        if (!CheckIfBehindObject())
                        {
                            Debug.Log("SEEKING PLAYER");
                            playerlastpos = target.transform.position;
                            agent.SetDestination(target.transform.position);
                        }
                        else
                        {
                            playerlastpos = target.transform.position;
                            agent.SetDestination(target.transform.position);
                            target = null;
                        }
                        
                    }

                    if (distance > sightDistance)
                    {
                        Debug.LogWarning("TARGET IS OUT. TARGET NULL");
                        target = null;

                    }
                }
                else if (!target && playerlastpos != Vector3.zero)
                {
                    //moveTowardsLastPos(playerlastpos);
                }
                else
                {
                    if (anim.GetBool("isWalking"))
                    {
                        anim.SetBool("isWalking", false);
                    }
                }
            }
        }
    }

    private bool CheckIfBehindObject()
    {
        bool targetBehindObject = false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward * sightDistance, out hit, sightDistance, PlayerMask))
        {
            if (hit.collider.gameObject.tag != "zombie" || hit.collider.gameObject.tag != "player")
            {
                targetBehindObject = true;
            }
        }
        return targetBehindObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive)
        {
            return;
        }

        if (other.gameObject.tag == "player")
        {
            target = other.gameObject.GetComponent<PlayerStats>();
            hearArea.radius = hearRadiusWithTarget;
        }
    }

    //void moveTowardsPlayer(Vector3 playerpos, float distance)
    //{
    //    Debug.Log(distance);
    //    if (distance < distanceToAttack)
    //    {
    //        Attack();
    //        return;
    //    }

    //    if (!anim.GetBool("isWalking"))
    //    {
    //        anim.SetBool("isWalking", true);
    //    }

    //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerpos - transform.position), rotationSpeed * Time.deltaTime);

    //    Vector3 direction = transform.position += transform.forward * zombieSpeed * Time.fixedDeltaTime;

    //    agent.SetDestination(direction);
    //}

    //void moveTowardsLastPos(Vector3 lastpos)
    //{

    //    if (Vector3.Distance(lastpos, gameObject.transform.position) < distanceToAttack)
    //    {
    //        playerlastpos = Vector3.zero;
    //        hearArea.radius = sightRadius;
    //        return;
    //    }

    //    if (!anim.GetBool("isWalking"))
    //    {
    //        anim.SetBool("isWalking", true);
    //    }

    //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lastpos - transform.position), rotationSpeed * Time.deltaTime);

    //    Vector3 direction = transform.position += transform.forward * zombieSpeed * Time.fixedDeltaTime;

    //    agent.SetDestination(direction);
    //}

    public void Die()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        transform.rotation = new Quaternion(-90, transform.rotation.y, transform.rotation.z, 0);
        isAlive = false;
        
    }

    public void Attack()
    {
        isAttacking = true;
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", true);
    }

    public void AnimAttack()
    {
        RaycastHit hit;

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.forward), out hit, 2f))
        {
            Debug.Log("ZOMBIE HIT TAG:"+hit.transform.gameObject.tag);

            if (hit.collider.GetType() == typeof(CapsuleCollider) && hit.transform.gameObject.tag.Equals("player"))
            {
                Debug.Log("HITTING PLAYER");
                hit.transform.gameObject.GetComponent<PlayerStats>().GetDamage(zs.attackDamage);
            }

        }
    }

    private IEnumerator SetActive()
    {
        yield return new WaitForSeconds(3);
        active = true;
    }


}
