﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class SCR_BotonesAI : SCR_Guest
{
    public List<SCR_Guest> guests = new List<SCR_Guest>();

    protected override void Start()
    {
        myNav = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bloodScript = FindObjectOfType<VFX_Blood>();
        SCR_Guest[] myGuests = FindObjectsOfType<SCR_Guest>();
        for (int i = 0; i < myGuests.Length; i++)
        {
            guests.Add(myGuests[i]);
        }
    }

    protected void CheckForScreams()
    {
        bool bShouldInvestigate = false;

        for (int i = 0; i < guests.Count; i++)
        {
            if (guests[i].currentState == GuestState.DETECTED_PLAYER)
            {
                bShouldInvestigate = true;
                break;
            }
        }

        if (bShouldInvestigate)
        {
            SetState(GuestState.INVESTIGATING);
        }
    }

    protected override void StateMachine()
    {
        switch (currentState)
        {
            case GuestState.IDLE:
                {
                    if (bOnEnterState)
                    {
                        PlaySound(idleClip);
                        movespeed = idleSpeed;
                        bOnEnterState = false;
                    }
                    NavigateWaypoints();
                    CheckForScreams();
                    DetectPlayer();
                    if (bIsDead)
                    {
                        SetState(GuestState.DEAD);
                    }
                    else if (damagingWeapon)
                    {
                        SetState(GuestState.TAKING_DAMAGE);
                    }


                    if (currentState != GuestState.IDLE)
                    {
                        bOnEnterState = true;
                    }
                }
                break;
            case GuestState.DETECTED_PLAYER:
                {
                    if (bOnEnterState)
                    {
                        PlaySound(detectedClip);
                        movespeed = detectedSpeed;
                        detectionTimer = 0.0f;
                        bOnEnterState = false;
                    }
                    myNav.SetDestination(playerTransform.position);
                    detectionTimer += Time.deltaTime;
                    if (detectionTimer >= detectionTimeout && canKillYou)
                    {
                        SceneManager.LoadScene("END");
                    }
                    else if (bIsDead)
                    {
                        SetState(GuestState.DEAD);
                    }
                    else if (damagingWeapon)
                    {
                        SetState(GuestState.TAKING_DAMAGE);
                    }


                    if (currentState != GuestState.DETECTED_PLAYER)
                    {
                        bOnEnterState = true;
                    }
                }
                break;
            case GuestState.TAKING_DAMAGE:
                {
                    if (bOnEnterState)
                    {
                        myNav.isStopped = true;
                        PlaySound(takingDamageClip);

                        SetTakingDamage(true);
                        bOnEnterState = false;
                    }
                    float damage = 100.0f;
                    if (damagingWeapon)
                    {
                        damage = damagingWeapon.damage;
                    }

                    TakeDamage(damage);


                    if (bIsDead)
                    {
                        SetState(GuestState.DEAD);
                    }
                    else if (!damagingWeapon || damagingWeapon.damage == 0)
                    {
                        SetState(GuestState.DETECTED_PLAYER);
                    }

                    if (currentState != GuestState.TAKING_DAMAGE)
                    {
                        SetTakingDamage(false);
                        myNav.isStopped = false;
                        bOnEnterState = true;
                    }
                }
                break;
            case GuestState.INVESTIGATING:
                {
                    if (bOnEnterState)
                    {
                        bOnEnterState = false;
                    }
                    myNav.SetDestination(playerTransform.position);
                    DetectPlayer();

                    if (damagingWeapon)
                    {
                        TakeDamage(0);
                        SetState(GuestState.TAKING_DAMAGE);
                    }
                   

                    if (currentState != GuestState.INVESTIGATING)
                    {
                        bOnEnterState = true;
                    }
                }
                break;
            case GuestState.DEAD:
                {
                    if (bOnEnterState)
                    {
                        GameObject myRagdoll = Instantiate(ragdoll);
                        myRagdoll.transform.position = transform.position;
                        myRagdoll.transform.rotation = transform.rotation;
                        if (damagingWeapon)
                        {
                            damagingWeapon.GetComponentInChildren<BoxCollider>().enabled = false;
                            damagingWeapon.GetComponent<Rigidbody>().useGravity = false;
                            damagingWeapon.GetComponent<Rigidbody>().isKinematic = true;
                            damagingWeapon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                            damagingWeapon.transform.GetChild(0).SetParent(myRagdoll.transform.Find("mixamorig:Hips"));

                            myRagdoll.transform.GetChild(0).transform.position = damagingWeapon.transform.position;
                            myRagdoll.transform.GetChild(0).transform.rotation = damagingWeapon.transform.rotation;




                            damagingWeapon.enabled = false;
                        }

                        ragdoll.GetComponent<Rigidbody>().AddForce(Vector3.up * ragForce);
                        Destroy(gameObject);
                        Destroy(myRagdoll, 5.0f);
                        bOnEnterState = false;
                    }


                    if (currentState != GuestState.DEAD)
                    {
                        bOnEnterState = true;
                    }
                }
                break;
        }
        AnimationUpdate();
        myNav.speed = movespeed;
    }

}
