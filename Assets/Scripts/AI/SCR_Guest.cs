using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SCR_Guest : MonoBehaviour
{
    protected bool bIsDead = false;
    protected bool bIsTakingDamage = false;
    protected float enemyHP = 0.0f;
    protected GameObject damagingWeapon;

    [Header("Detection")]
    [SerializeField]
    protected float detectNoiseRange;
    [SerializeField]
    protected float detectionTimeout = 1.0f;
    protected float detectionTimer = 0.0f;
    protected Transform playerTransform;

    [Header("Ragdoll")]
    [SerializeField]
    protected float ragForce;
    [SerializeField]
    protected GameObject ragdoll;

    [Header("State Machine")]
    protected bool bOnEnterState = true;

    [Header("Animation")]
    protected Animator myAnim;
    protected float movespeed;

    [Header("Patrol")]
    [SerializeField]
    protected List<Transform> waypoints = new List<Transform>();
    protected int currentWaypointIndex = 0;
    protected NavMeshAgent myNav;
    [SerializeField]
    protected float idleSpeed;
    [SerializeField]
    protected float detectedSpeed;

    [Header("Particles")]
    [SerializeField]
    protected GameObject bloodParticles;

    public enum GuestState
    {
        IDLE,
        DETECTED_PLAYER,
        TAKING_DAMAGE,
        DEAD
    }
    protected GuestState currentState = GuestState.IDLE;

    protected void Start()
    {
        myNav = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected void Update()
    {
        StateMachine();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            //TODO: change to actual weapon class
            damagingWeapon = other.gameObject;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            GameObject blood = Instantiate(bloodParticles);
            blood.transform.position = other.ClosestPointOnBounds(transform.position);
        }
    }


    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            damagingWeapon = null;
        }
    }

    public virtual void TakeDamage(float _dmg)
    {
        if (bIsDead)
            return;

        enemyHP -= _dmg;
        bIsDead = enemyHP <= 0 ? true : false;
    }

    protected virtual void SetTakingDamage(bool _state)
    {
        bIsTakingDamage = _state;
    }

    public virtual void SetState(GuestState _newState)
    {
        currentState = _newState;
    }

    public virtual void AnimationUpdate()
    {
        myAnim.SetBool("Taking_Damage", bIsTakingDamage);
        myAnim.SetBool("Dead", bIsDead);
        myAnim.SetFloat("Movespeed", movespeed);
    }

    protected virtual void NavigateWaypoints()
    {
        if (waypoints.Count > 0)
        {
            if (Vector3.Dot(transform.forward, (waypoints[currentWaypointIndex].position - transform.position).normalized) <= 0.9f)
            {
                movespeed = 0.1f;
            }
            else
            {
                movespeed = idleSpeed;
            }

            myNav.SetDestination(waypoints[currentWaypointIndex].position);
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) <= 2.0f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    currentWaypointIndex = 0;
                }
            }
        }
    }

    protected virtual void DetectPlayer()
    {
        Vector3 startPosition = transform.position + (transform.up * 2);
        Vector3 endPosition = playerTransform.position;
        
        RaycastHit hit;
        if (Physics.Linecast(startPosition, endPosition, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if (Vector3.Dot(transform.position.normalized, playerTransform.position.normalized) > 0.5f)
                {
                    SetState(GuestState.DETECTED_PLAYER);
                }
            }
        }
    }

    protected virtual void StateMachine()
    {
        switch (currentState)
        {
            case GuestState.IDLE:
                {
                    if (bOnEnterState)
                    {
                        movespeed = idleSpeed;
                        bOnEnterState = false;
                    }
                    NavigateWaypoints();
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
                        movespeed = detectedSpeed;
                        detectionTimer = 0.0f;
                        bOnEnterState = false;
                    }

                    detectionTimer += Time.deltaTime;
                    if (detectionTimer >= detectionTimeout)
                    {
                        SceneManager.LoadScene("Game");
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
                        SetTakingDamage(true);
                        bOnEnterState = false;
                    }
                  
                    //TakeDamage(damagingWeapon.damage);
                    if(bIsDead)
                    {
                        SetState(GuestState.DEAD);
                    }
                    else if (!damagingWeapon /*damagingWeapon.damage == 0*/)
                    {
                        SetState(GuestState.DETECTED_PLAYER);
                    }

                    if (currentState != GuestState.TAKING_DAMAGE)
                    {
                        SetTakingDamage(false);
                        bOnEnterState = true;
                    }
                }
                break;
            case GuestState.DEAD:
                {
                    if (bOnEnterState)
                    {
                        GameObject myRagdoll = Instantiate(ragdoll);
                        ragdoll.GetComponent<Rigidbody>().AddForce(ragdoll.transform.up * ragForce);
                        Destroy(gameObject);
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
