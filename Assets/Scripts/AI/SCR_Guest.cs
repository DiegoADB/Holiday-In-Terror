using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class SCR_Guest : MonoBehaviour
{
    protected bool bIsDead = false;
    protected bool bIsTakingDamage = false;

    [SerializeField]
    protected float enemyHP = 0.0f;
    protected Interactable damagingWeapon;

    [Header("Detection")]
    [SerializeField]
    protected float detectNoiseRange;
    [SerializeField]
    protected float detectionTimeout = 1.0f;
    protected float detectionTimer = 0.0f;
    protected Transform playerTransform;
    public bool canKillYou = false;

    [Header("Ragdoll")]
    [SerializeField]
    protected float ragForce;
    [SerializeField]
    protected GameObject ragdoll;
    protected bool bIsHeadshot = false;

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

    [Header("Audio")]
    [SerializeField]
    protected AudioClip takingDamageClip;
    [SerializeField]
    protected AudioClip idleClip;
    [SerializeField]
    protected AudioClip detectedClip;
    [SerializeField]
    protected AudioClip bloodClip;
    [SerializeField]
    protected AudioSource myAudio;
    [SerializeField]
    protected AudioSource bloodAudio;


    protected VFX_Blood bloodScript;

    public enum GuestState
    {
        IDLE,
        DETECTED_PLAYER,
        TAKING_DAMAGE,
        INVESTIGATING,
        STAGGERED,
        DEAD
    }
    public GuestState currentState = GuestState.IDLE;

    protected virtual void Start()
    {
        myNav = GetComponent<NavMeshAgent>();
        myAnim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bloodScript = FindObjectOfType<VFX_Blood>();
    }

    protected void Update()
    {
        StateMachine();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (!other.gameObject.transform.parent.GetComponent<Interactable>().isPenetrable)
            {
                damagingWeapon = other.gameObject.transform.parent.GetComponent<Interactable>();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (!other.gameObject.transform.parent.GetComponent<Interactable>().isPenetrable)
            {
                SpawnBlood(other.ClosestPointOnBounds(transform.position));
            }
        }
        else if (other.CompareTag("Flashlight"))
        {
            SetState(GuestState.STAGGERED);
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

    public virtual void SpawnBlood(Vector3 _pos)
    {
        GameObject blood = bloodScript.InstantiateBlood();
        blood.transform.position = _pos;
        bloodAudio.clip = bloodClip;
        bloodAudio.Play();
    }

    protected virtual void SetTakingDamage(bool _state)
    {
        bIsTakingDamage = _state;
    }

    public virtual void SetState(GuestState _newState)
    {
        currentState = _newState;
    }

    public virtual void SetInteractable(Interactable _interactable)
    {
        damagingWeapon = _interactable;
        bIsHeadshot = true;
    }

    public virtual void ClearWeapon()
    {
        damagingWeapon = null;
    }

    public virtual void AnimationUpdate()
    {
        myAnim.SetBool("Taking_Damage", bIsTakingDamage);
        myAnim.SetBool("Dead", bIsDead);
        myAnim.SetFloat("Movespeed", movespeed, 0.2f, Time.deltaTime);
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
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) <= 0.5f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    currentWaypointIndex = 0;
                }
            }
        }
        else
        {
            idleSpeed = 0.0f;
            detectedSpeed = 0.0f;
            movespeed = idleSpeed;
        }
    }

    protected virtual void DetectPlayer()
    {
        if (detectNoiseRange <= 0)
        {
            return;
        }
        Vector3 startPosition = transform.position + (transform.up);
        Vector3 endPosition = playerTransform.position;
        
        RaycastHit hit;
        if (Vector3.Distance(transform.position, playerTransform.position) < 3)
        {
            if (Physics.Linecast(startPosition, endPosition, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    SetState(GuestState.DETECTED_PLAYER);
                }
            }
        }

        
    }

    protected void PlaySound(AudioClip _clip)
    {
        if (!_clip)
            return;
        myAudio.Stop();
        myAudio.clip = _clip;
        myAudio.Play();
    }

    protected virtual void StateMachine()
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
                        bOnEnterState = true;
                    }
                }
                break;
            case GuestState.STAGGERED:
                {
                    if (bOnEnterState)
                    {
                        myAnim.SetTrigger("Staggered");
                        bOnEnterState = false;
                    }


                    if (damagingWeapon)
                    {
                        SetState(GuestState.TAKING_DAMAGE);
                    }
                    else if (bIsDead)
                    {
                        SetState(GuestState.DEAD);
                    }

                    if (currentState != GuestState.STAGGERED)
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


                            SCR_BotonesAI[] botones = FindObjectsOfType<SCR_BotonesAI>();
                            for (int i = 0; i < botones.Length; i++)
                            {
                                botones[i].guests.Remove(this);
                            }

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
