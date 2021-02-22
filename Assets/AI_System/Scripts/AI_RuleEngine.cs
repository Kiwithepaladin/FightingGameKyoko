using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUp
{
    public class AI_RuleEngine : MonoBehaviour
    {
        [Header("Attack Pattern")]
        [SerializeField] public List<AI_Behaviour> ai_AttackPattern;
        [HideInInspector] public int currentActionNumber = 0;
        private bool isActionDone;
        [Header("Movement")]
        [SerializeField] private float ai_JumpHeight;
        [SerializeField] private float ai_ChaseSpeed;
        [SerializeField] private float ai_LeapSpeed;
        [Header("Stats")]
        [SerializeField] private float ai_Maxhealth;
        [SerializeField] private float ai_CurrentHealth;
        public float ai_AttackDamage;
        [Header("Projectile")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed;
        public float projectileDamage;

        private PlayerCombos kyoko;
        private Animator ai_Animator;
        private Rigidbody ai_RigidBody;
        [Header("Floating Text")]
        [SerializeField] private GameObject ai_FloatingText;

        private void Start()
        {
            kyoko = GameObject.Find("Kyoko").GetComponent<PlayerCombos>();
            ai_RigidBody = GetComponent<Rigidbody>();
            ai_Animator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (ai_CurrentHealth <= 0)
            {
                StartCoroutine(CheckHealth());
            }
        }
        private void FixedUpdate()
        {
            CheckForNewState();
            ManageRotation();
        }


        #region OnTrigger
        private void OnTriggerEnter(Collider other)
        {
            Utilities.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            if (other.tag.Equals("DamagingCollider"))
            {
                //Dont Delete ME
                //ai_CurrentHealth -= kyoko.currentDamage;
                Utilities.ShowFloatingText(ai_FloatingText, transform, ai_CurrentHealth.ToString(), new Vector3(0, 0.7f, 0), new Vector3(-0.2f, 0.2f));

            }
        }
        private void OnTriggerStay(Collider other)
        {
            //Utilities.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            if (other.tag.Equals("DamagingCollider"))
            {
                KnockBacks();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            Utilities.NotifyTriggerExit(other, gameObject);
            if (other.tag.Equals("DamagingCollider"))
            {
             
            }
        }
        #endregion
        private IEnumerator CheckHealth()
        {
            
            ai_Animator.SetBool("Death", true);
            yield return new WaitForSeconds(ai_Animator.GetNextAnimatorClipInfo(0).Length);
            //Destroy(gameObject, ai_Animator.GetNextAnimatorClipInfo(0).Length);
            if (!Utilities.isPlaying(ai_Animator, "Death", 1f))
            {
                Destroy(gameObject, ai_Animator.GetNextAnimatorClipInfo(0).Length);
            }
            yield return null;
        }
        private void KnockBacks()
        {
            //if (ai_CurrentHealth > 0f)
            //{
                if (PlayerControllerScript.state == KyokoStates.HurricaneKicking)
                {
                    ai_RigidBody.velocity = Vector3.up * 2.5f;
                }
                else if (PlayerControllerScript.state == KyokoStates.BackKick)
                {
                    if (transform.rotation.eulerAngles.y == 0)
                    {
                        ai_RigidBody.velocity = Vector3.right * 5.5f;
                    }
                    else if (transform.rotation.eulerAngles.y == 180)
                    {
                        ai_RigidBody.velocity = Vector3.left * 5.5f;
                    }
                }
                else if (PlayerControllerScript.state == KyokoStates.Stomp)
                {
                    if (transform.rotation.eulerAngles.y == 0)
                    {
                        ai_RigidBody.velocity = new Vector3(1, 1) * 2.5f;
                    }
                    else if (transform.rotation.eulerAngles.y == 180)
                    {
                        ai_RigidBody.velocity = new Vector3(-1, 1) * 2.5f;
                    }
                }
            //}
        }
        private void CheckForNewState()
        {
            //Start new Routine
            if (currentActionNumber == ai_AttackPattern.Count)
            {
                currentActionNumber = 0;
                foreach (AI_Behaviour behaviour in ai_AttackPattern)
                {
                    behaviour.isActionDone = false;
                }
            }
            //Jump
            if (ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Jump)
            {
                if (isActionDone)
                {
                    StopCoroutine(Jump());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Jump());
                }
            }
            else if(ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Leap)
            {
                if (isActionDone)
                {
                    StopCoroutine(Leap());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Leap());
                }
            }
            //Defend
            else if(ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Defend)
            {
                if(isActionDone)
                {
                    StopCoroutine(Defend());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Defend());
                }
            }
            //Attack
            else if(ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Attack)
            {
                if (isActionDone)
                {
                    StopCoroutine(Attack());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Attack());
                }
            }
            //Chase
            else if (ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Chase)
            {
                if (isActionDone)
                {
                    StopCoroutine(Chase());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Chase());
                }
            }
            //Shoot
            else if(ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Shoot)
            {
                if (isActionDone)
                {
                    StopCoroutine(Shoot());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Shoot());
                }
            }
            else if(ai_AttackPattern[currentActionNumber].ai_Action == AI_Actions.Rest)
            {
                if (isActionDone)
                {
                    StopCoroutine(Rest());
                    ai_AttackPattern[currentActionNumber].isActionDone = true;
                    currentActionNumber++;
                    isActionDone = false;
                }
                else
                {
                    StartCoroutine(Rest());
                }
            }
        }
        private void ManageRotation()
        {
            if (transform.position.x - kyoko.transform.position.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (transform.position.x - kyoko.transform.position.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        #region Actions
        private IEnumerator Chase()
        {
            if (!isActionDone)
            {
                ai_RigidBody.velocity = (kyoko.transform.position - transform.position) * ai_ChaseSpeed * Time.deltaTime;
                if(Mathf.Abs(Vector3.Distance(kyoko.transform.position,transform.position)) < 0.6f)
                {
                    isActionDone = true;
                    yield return null;
                }
            }
            
        }
        private IEnumerator Defend()
        {
            if (!isActionDone)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow;
                yield return new WaitForSeconds(1f);
                GetComponent<SpriteRenderer>().color = Color.white;
                isActionDone = true;
            }
        }
        private IEnumerator Attack()
        {
            ai_Animator.SetBool("Attack", true);
            yield return new WaitForSeconds(ai_Animator.GetNextAnimatorClipInfo(0).Length);
            if (!Utilities.isPlaying(ai_Animator, "Attack", 1f) && !isActionDone)
            {
                isActionDone = true;
                ai_Animator.SetBool("Attack", false);
            }
            yield return null;
        }
        private IEnumerator Jump()
        {
            if (!isActionDone)
            {
                if (transform.rotation.eulerAngles.y == 0)
                {
                    ai_RigidBody.AddForce(new Vector3(1, 1, 0) * ai_JumpHeight,ForceMode.Impulse);
                }
                else if (transform.rotation.eulerAngles.y == 180)
                {
                    ai_RigidBody.AddForce(new Vector3(-1, 1, 0) * ai_JumpHeight,ForceMode.Impulse);
                }
                isActionDone = true;
            }
            yield return new WaitForSeconds(1f);
        }
        private IEnumerator Leap()
        {
            if (!isActionDone)
            {
                ai_RigidBody.velocity = new Vector3(kyoko.transform.position.x - transform.position.x,1f) * ai_LeapSpeed * Time.deltaTime;
                isActionDone = true;
            }
            yield return new WaitForSeconds(1f);
        }
        private IEnumerator Shoot()
        {
            ai_Animator.SetBool("Attack", true);
            Vector3 offSet = Vector3.zero;
            yield return new WaitForSeconds(ai_Animator.GetNextAnimatorClipInfo(0).Length);
            if (transform.rotation.eulerAngles.y == 0f)
            {
                offSet = new Vector3(-1, 0f);
            }
            else if (transform.rotation.eulerAngles.y == 180f)
            {
                offSet = new Vector3(1, 0f);
            }
            if (!Utilities.isPlaying(ai_Animator,"Attack",1f) && !isActionDone)
            {  
                var projectile = Instantiate(projectilePrefab, transform.position + offSet, Quaternion.Euler(0,0,90f),transform);
                projectile.GetComponent<Rigidbody>().velocity = new Vector3(kyoko.transform.position.x - projectile.transform.position.x, 0f, 0f) * projectileSpeed * Time.deltaTime;
                Destroy(projectile, 0.5f);
                ai_Animator.SetBool("Attack", false);
                isActionDone = true;
            }
            yield return new WaitForSeconds(1f);
        }
        private IEnumerator Rest()
        {
            yield return new WaitForSeconds(1f);
            if (!isActionDone)
            {
                isActionDone = true;
            }
        }
        #endregion
    }
}
