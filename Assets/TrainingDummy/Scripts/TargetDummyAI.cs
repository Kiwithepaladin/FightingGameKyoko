using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUp
{
    public enum TargetDummyStates
    {
        Idle,
        Chase,
        Attack,
        Defend,
        Jump,
        Retreat,
        UnderAttack,
        Recovery,
        Death,
    }
    public class TargetDummyAI : MonoBehaviour
    {
        private Rigidbody td_RigidBody;
        private PlayerControllerScript kyoko;
        private SpriteRenderer td_SpriteRenderer;
        private Animator td_Animator;
        [Header("Floating_Text")]
        [SerializeField] private GameObject td_FloatingText;
        [Header("Current State")]
        public TargetDummyStates td_CurrentState;
        public TargetDummyStates td_PreviousState;
        [Header("Movement")]
        [SerializeField] private float td_ChaseSpeed;
        [SerializeField] private float td_JumpHeight;
        [SerializeField] private float td_RetreatSpeed;
        [SerializeField] private float retreatDuration;
        [SerializeField] private float retreatTimer;
        [SerializeField] private bool td_IsRetreating;
        private bool isAnimationDone;
        private Vector3 direction;
        [Header("Attacks")]
        [SerializeField] private float attackCooldown;
        private float timeBetweenAttacks = 0;
        public float td_AttackDamage;
        [SerializeField] private float recoveryRate;
        [SerializeField] private float numberOfConcecutiveAttacks = 0;
        [SerializeField] private bool td_IsGrounded;
        [SerializeField] private float td_MaxHealth;
        [SerializeField] private float td_CurrentHealth;
        [SerializeField] private Vector2 zMinMax;
        private bool td_IsRetreatingDone;

        private void Start()
        {
            td_Animator = GetComponent<Animator>();
            kyoko = GameObject.Find("Kyoko").GetComponent<PlayerControllerScript>();
            td_RigidBody = GetComponent<Rigidbody>();
            td_SpriteRenderer = GetComponent<SpriteRenderer>();
            td_SpriteRenderer.color = new Color(0, 1, 0, 1);
            td_CurrentHealth = td_MaxHealth;
            retreatTimer = retreatDuration;
        }

        private void Update()
        {
            Rotate();
            IsGroundedHandler();
            direction = new Vector3(kyoko.transform.position.x - transform.position.x, 0,kyoko.transform.position.z - transform.position.z);
            CooldownReduction();
           
        }
        private void FixedUpdate()
        {
            //Retreat();
            Clamping();
            switch (td_CurrentState)
            {
                case TargetDummyStates.Idle:
                    GetNewState();
                    break;
                case TargetDummyStates.Attack:
                    Attack();
                    break;
                case TargetDummyStates.Defend:
                    Defend();
                    break;
                case TargetDummyStates.Chase:
                    Chase();
                    break;
                case TargetDummyStates.Jump:
                    Jump();
                    break;
                case TargetDummyStates.Retreat:
                    Retreat();
                    break;
                case TargetDummyStates.Death:
                    Death();
                    break;
                case TargetDummyStates.UnderAttack:
                    UnderAttack();
                    break;
                case TargetDummyStates.Recovery:
                    Invoke("GetNewState", recoveryRate);
                    break;
            }
        }
        private void Clamping()
        {
            td_RigidBody.position = new Vector3(
                   transform.position.x
                  , transform.position.y
                  , Mathf.Clamp(td_RigidBody.position.z, zMinMax.x, zMinMax.y));
        }
        #region OnTrigger
        private void OnTriggerEnter(Collider other)
        {
            Utilities.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            if (other.tag.Equals("DamagingCollider"))
            {
                td_CurrentHealth -= kyoko.currentDamage;
                Utilities.ShowFloatingText(td_FloatingText, transform, td_CurrentHealth.ToString(), new Vector3(0, 0.7f, 0), new Vector3(-0.2f, 0.2f));
                
            }
        }
        private void OnTriggerStay(Collider other)
        {
            //Utilities.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            if (other.tag.Equals("DamagingCollider"))
            {
                td_CurrentState = TargetDummyStates.UnderAttack;
                if (td_CurrentHealth > 0f)
                {
                    if (PlayerControllerScript.state == KyokoStates.HurricaneKicking)
                    {
                        td_RigidBody.velocity = Vector3.up * 2.5f;
                    }
                    else if (PlayerControllerScript.state == KyokoStates.BackKick)
                    {
                        if (transform.rotation.eulerAngles.y == 0)
                        {
                            td_RigidBody.velocity = Vector3.right * 5.5f;
                        }
                        else if(transform.rotation.eulerAngles.y == 180)
                        {
                            td_RigidBody.velocity = Vector3.left * 5.5f;
                        }
                    }
                    else if (PlayerControllerScript.state == KyokoStates.Stomp)
                    {
                        if (transform.rotation.eulerAngles.y == 0)
                        {
                            td_RigidBody.velocity = new Vector3(1, 1) * 2.5f;
                        }
                        else if(transform.rotation.eulerAngles.y == 180)
                        {
                            td_RigidBody.velocity = new Vector3(-1, 1) * 2.5f;
                        }
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            Utilities.NotifyTriggerExit(other, gameObject);
            if (other.tag.Equals("DamagingCollider"))
            {
                SwitchToRecovaryState();
            }
        }
        #endregion
        private void GetNewState()
        {
            if (td_CurrentHealth > 0)
            {
                if (td_CurrentHealth <= 50f && !td_IsRetreating && !td_IsRetreatingDone)
                {
                    td_IsRetreating = true;
                    td_CurrentState = TargetDummyStates.Retreat;
                }
                if (td_CurrentState != TargetDummyStates.Attack)
                {
                    numberOfConcecutiveAttacks = 0;
                }
                if (Mathf.Abs(kyoko.transform.position.x - transform.position.x) < 0.4f && td_IsGrounded /*&& numberOfConcecutiveAttacks < 2 */ && !td_IsRetreating)
                {
                    td_CurrentState = TargetDummyStates.Attack;
                }
                if (Mathf.Abs(kyoko.transform.position.x - transform.position.x) > 0.4f && td_IsGrounded /*&& numberOfConcecutiveAttacks < 2 */  && !td_IsRetreating)
                {
                    td_CurrentState = TargetDummyStates.Chase;
                }
                if (numberOfConcecutiveAttacks == 3 && td_IsGrounded && !td_IsRetreating)
                {
                    td_CurrentState = TargetDummyStates.Defend;
                }
                if (td_IsGrounded && td_PreviousState == TargetDummyStates.Defend && !td_IsRetreating)
                {
                    td_CurrentState = TargetDummyStates.Jump;
                }
            }
            else if (td_CurrentHealth <= 0)
            {
                td_CurrentState = TargetDummyStates.Death;
            }

        }
        private void UnderAttack()
        {
        }
        private void SwitchToRecovaryState()
        {
            td_Animator.SetBool("Attack", false);
            td_CurrentState = TargetDummyStates.Recovery;
        }
        private void Rotate()
        {
            if (transform.position.x - kyoko.transform.position.x > 0 && !td_IsRetreating)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (transform.position.x - kyoko.transform.position.x < 0 && !td_IsRetreating)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (transform.position.x - kyoko.transform.position.x > 0 && td_IsRetreating)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (transform.position.x - kyoko.transform.position.x < 0 && td_IsRetreating)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        private void IsGroundedHandler()
        {
            Vector3 tempVector3 = new Vector3(0, 0.471f, 0);
            if (Physics.OverlapSphere(transform.position - tempVector3, 0.0360f).Length > 1)
            {
                td_IsGrounded = true;
            }
            else if (Physics.OverlapSphere(transform.position - tempVector3, 0.0360f).Length <= 1)
            {
                td_IsGrounded = false;

            }
        }
        private void Chase()
        {
            td_RigidBody.velocity = direction * td_ChaseSpeed * Time.deltaTime;
            GetNewState();
        }
        private void Retreat()
        {
            if (0 < retreatTimer)
            {
                Debug.LogError("Running Away in fear");
                td_RigidBody.velocity = (new Vector3(transform.position.x - kyoko.transform.position.x,0f).normalized * td_RetreatSpeed) * Time.deltaTime;
                retreatTimer -= Time.deltaTime;
                td_IsRetreating = true;
            }
            else if(0 > retreatTimer)
            {
                td_IsRetreating = false;
                td_IsRetreatingDone = true;
            }
            GetNewState();
        }
        private void Death()
        {
            td_Animator.SetBool("Attack", false);
            td_Animator.SetBool("Death", true);
            Destroy(gameObject,td_Animator.GetCurrentAnimatorClipInfo(0).Length - 0.3f);

        }
        private void Attack()
        {
            td_Animator.SetBool("Attack", true);
            if (Utilities.isPlaying(td_Animator, "Attack", 1f) && !isAnimationDone && timeBetweenAttacks < 0)
            {
                if (td_PreviousState == TargetDummyStates.Attack)
                {
                    numberOfConcecutiveAttacks++;
                }
                isAnimationDone = true;
                timeBetweenAttacks = attackCooldown;
            }
            else if (!Utilities.isPlaying(td_Animator, "Attack",1f) && isAnimationDone)
            {
                td_Animator.SetBool("Attack", false);
                isAnimationDone = false;
                td_PreviousState = td_CurrentState;
                GetNewState();
            }
        }
        private void Defend()
        {
            td_SpriteRenderer.color = new Color(0, 1, 0, 1);
            td_PreviousState = td_CurrentState;
            GetNewState();
        }
        private void Jump()
        {
            if (transform.rotation.eulerAngles.y == 0 && td_IsGrounded)
            {
                td_RigidBody.velocity = new Vector3(1, 1,0) * td_JumpHeight * Time.deltaTime;
            }
            else if (transform.rotation.eulerAngles.y == 180 && td_IsGrounded)
            {
                td_RigidBody.velocity = new Vector3(-1, 1,0) * td_JumpHeight * Time.deltaTime;
            }
            td_PreviousState = td_CurrentState;
            GetNewState();
        }
        private void CooldownReduction()
        {
            timeBetweenAttacks -= Time.deltaTime;
        }
    }
}
