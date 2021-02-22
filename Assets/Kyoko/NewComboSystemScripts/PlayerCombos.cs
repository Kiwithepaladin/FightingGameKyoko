using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BeatEmUp
{
    public class PlayerCombos : MonoBehaviour
    {

        [Header("Stats and Modifiers")]
        public float kyokoMaxHealth;
        public float kyokoCurrentHealth;
        public float currentDamage;
        public float kyoko_MaxEnergy;
        public float kyoko_CurrentEnergy;


        [Header("Walking and Running")]
        [SerializeField] private float walkingSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float airWalkingSpeed;
        [SerializeField] private float movementSpeed;

        [Header("Clamping")]
        [SerializeField] private Vector2 zMinMax;

        [Header("Attacks")]
        public Attack backElbow;
        public Attack kick;
        public Attack stomp;
        public float comboLeeway = 0.2f;

        public List<Combo> combos;


        [Header("Components")]
        Animator playerAnimator;
        Attack currentAttack = null;
        ComboInput lastInput = null;

        List<int> currentCombos = new List<int>();

        float timer = 0;
        float leeway = 0;
        bool skip = false;
        private bool immovable;
        Rigidbody rigidBody;
        private bool facingRight = false;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            playerAnimator = GetComponent<Animator>();
            PrimeCombos();
        }

        
        void Update()
        {
            #region Input
            if (currentAttack != null)
            {
                if (timer > 0)
                    timer -= Time.deltaTime;
                else
                {
                    currentAttack = null;
                }
                return;
            }

            if (currentCombos.Count > 0)
            {
                leeway += Time.deltaTime;
                if (leeway >= comboLeeway)
                {
                    if (lastInput != null)
                    {
                        Attack(GetAttackFromType(lastInput.type));
                        lastInput = null;
                    }
                    ResetCombos();
                }
            }
            else
            {
                leeway = 0;
            }
            ComboInput input = null;
            if (Input.GetButtonDown("Kick"))
            {
                input = new ComboInput(KyokoStates.Kicking);
                immovable = true;
            }
            if (Input.GetButtonDown("Stomp"))
            {
                input = new ComboInput(KyokoStates.Stomp);
                immovable = true;
            }
            if (Input.GetButtonDown("BackElbow"))
            {
                input = new ComboInput(KyokoStates.BackElbow);
                immovable = true;
            }

            if (input == null)
            {
                playerAnimator.Play("Idle");
                immovable = false;
                return;
            }
            lastInput = input;

            List<int> remove = new List<int>();
            for (int i = 0; i < currentCombos.Count; i++)
            {
                Combo c = combos[currentCombos[i]];
                if (c.ContinueCombo(input))
                    leeway = 0;
                else
                {
                    remove.Add(i);
                }
            }
            if (skip)
            {
                skip = false;
                return;
            }

            for (int i = 0; i < combos.Count; i++)
            {
                if (currentCombos.Contains(i)) continue;
                if (combos[i].ContinueCombo(input))
                {
                    currentCombos.Add(i);
                    leeway = 0;
                }
            }

            foreach (int i in remove)
                currentCombos.RemoveAt(i);

            if (currentCombos.Count <= 0)
                Attack(GetAttackFromType(input.type));
            #endregion
        }


        void FixedUpdate()
        {

            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            Clamping();
            if (moveHorizontal < 0 && !facingRight && !immovable)
            {
                Flip();
                transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            }
            else if (moveHorizontal > 0 && facingRight && !immovable)
            {
                Flip();
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            if (!immovable)
            {
                rigidBody.velocity = movement * walkingSpeed * Time.deltaTime;
            }
        }
        void ResetCombos()
        {
            leeway = 0;
            for (int i = 0; i < currentCombos.Count; i++)
            {
                Combo c = combos[currentCombos[i]];
                c.ResetCombo();
            }
            currentCombos.Clear();
        }
        void Attack(Attack att)
        {
            currentAttack = att;
            timer = att.length;
            playerAnimator.Play(att.name, -1, 0);
        }
        Attack GetAttackFromType(KyokoStates t)
        {
            if (t == KyokoStates.Kicking)
                return kick;
            if (t == KyokoStates.Stomp)
                return stomp;
            if (t == KyokoStates.BackElbow)
                return backElbow;
            return null;
        }
        private void PrimeCombos()
        {
            for (int i = 0; i < combos.Count; i++)
            {
                Combo c = combos[i];
                c.onInput.AddListener(() =>
                {
                    skip = true;
                    Attack(c.comboAttack);
                    ResetCombos();
                });
            }
        }
        private void Flip()
        {
            facingRight = !facingRight;
            Vector3 thisScale = transform.localScale;
            thisScale.x *= -1;
            //transform.localScale = thisScale;
        }
        private void Clamping()
        {
            rigidBody.position = new Vector3(
                   transform.position.x
                  , transform.position.y
                  , Mathf.Clamp(rigidBody.position.z, zMinMax.x, zMinMax.y));
        }
    }
}
