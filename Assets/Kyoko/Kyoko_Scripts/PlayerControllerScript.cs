using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace BeatEmUp
{
    public class PlayerControllerScript : MonoBehaviour
    {
        [SerializeField] public static KyokoStates state;

        [Header("Stats and Modifiers")]
        public float kyokoMaxHealth;
        public float kyokoCurrentHealth;
        public float currentDamage;
        public float kyoko_MaxEnergy;
        public float kyoko_CurrentEnergy;
        [SerializeField] private float kyokoDamageModifier;
        [SerializeField] private bool immovable;


        [Header("AerialAttacks")]
        [SerializeField] private float AerialAttacksTotal;
        [SerializeField] private float AerialAttacksLeft;

        [Header("Walking and Running")]
        [SerializeField] private float walkingSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float airWalkingSpeed;
        [SerializeField] private float movementSpeed;
        [Header("Jumping")]
        [SerializeField] private float fallMultiplier;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private bool isGrounded;
        private float timeBetweenJumps;
        [Header("Dive")]
        [SerializeField] private float diveSpeed;
        [SerializeField] private float diveCooldown;
        private float timeBetweenDives;

        [Header("Damage")]
        [SerializeField] private float kickDamage;
        [SerializeField] private float backElbowDamage;
        [SerializeField] private float backKickDamage;
        [SerializeField] private float dropKickDamage;
        [SerializeField] private float hurricaneKickDamage;
        [SerializeField] private float dragonFeetDamage;
        [SerializeField] private float ultimateDabDamage;
        [SerializeField] private float frankensteinerDamage;
        [SerializeField] private float stompDamage;
        [Header("Cooldowns")]
        [SerializeField] private float kickCooldown;
        private float timeBetweenKicks;
        [SerializeField] private float backElbowCooldown;
        private float timeBetweenBackElbows;
        [SerializeField] private float backKickCooldown;
        private float timeBetweenBackKicks;
        [SerializeField] private float dropKickCooldown;
        private float timeBetweenDropKicks;
        [SerializeField] private float hurricaneKickCooldown;
        private float timeBetweenHurricaneKicks;
        [SerializeField] private float dragonFeetCooldown;
        private float timeBetweenDragonFeet;
        [SerializeField] private float ultimateDabCooldown;
        private float timeBetweenDabs;
        [SerializeField] private float frankensteinerCooldown;
        private float timeBetweenFrankensteiner;
        [SerializeField] private float stompCooldown;
        private float timeBetweenStomps;

        [Header("Clamping")]
        [SerializeField] private Vector2 zMinMax;

        [Header("Collider")]
        [SerializeField] private BoxCollider damagingColliderRight;
        [SerializeField] private BoxCollider damagingColliderUp;
        [SerializeField] private BoxCollider damagingColliderLeft;

        private Animator kyokoAnim;      
        private AnimatorStateInfo currentStateInfo;
        [HideInInspector] public KyokoStates previousStateEnum, earliestStateEnum;
        private Vector3 m_Velocity = Vector3.zero;

        #region AnimationStates
        //States
        static int currentState;
        static int previousState;
        static int earliestState;
        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int walkingState = Animator.StringToHash("Base Layer.Walking");
        static int runningState = Animator.StringToHash("Base Layer.Running");
        static int diveState = Animator.StringToHash("Base Layer.Dive");
        static int deathState = Animator.StringToHash("Base Layer.Death");
        static int kickState = Animator.StringToHash("Base Layer.Kick");
        static int ultimateDabAttackState = Animator.StringToHash("Base Layer.UltimateDabAttack");
        static int dragonFeetState = Animator.StringToHash("Base Layer.DragonFeet");
        static int hurricaneKickState = Animator.StringToHash("Base Layer.HurricaneKick");
        static int backElbowState = Animator.StringToHash("Base Layer.BackElbow");
        static int backKickState = Animator.StringToHash("Base Layer.BackKick");
        static int dropKickState = Animator.StringToHash("Base Layer.DropKick");
        static int jumpingState = Animator.StringToHash("Base Layer.Jump");
        static int frankensteinerState = Animator.StringToHash("Base Layer.Frankensteiner");
        static int fallingState = Animator.StringToHash("Base Layer.Falling");
        static int stompState = Animator.StringToHash("Base Layer.Stomp");
        #endregion


        private bool facingRight;
        private Rigidbody rigidBody;

        private float doubleTapTime = 0.7f;
        private float stompRequestTime;
        [SerializeField] private bool canTakeDamage;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            movementSpeed = walkingSpeed;
            kyokoAnim = GetComponent<Animator>();
            timeBetweenDives = 0;
            timeBetweenKicks = 0;
            timeBetweenHurricaneKicks = 0;
            timeBetweenDabs = 0;
            timeBetweenBackElbows = 0;
            timeBetweenBackKicks = 0;
            timeBetweenDropKicks = 0;
            timeBetweenDragonFeet = 0;
            timeBetweenFrankensteiner = 0;
            timeBetweenJumps = 0;
            timeBetweenStomps = 0;
            AerialAttacksLeft = AerialAttacksTotal;
            canTakeDamage = true;
            if (kyokoCurrentHealth > kyokoMaxHealth)
                kyokoCurrentHealth = kyokoMaxHealth;
        }
        private void Update()
        {
            if (kyoko_CurrentEnergy >= kyoko_MaxEnergy)
                kyoko_CurrentEnergy = kyoko_MaxEnergy;
            IncreaseEnergy();
            currentStateInfo = kyokoAnim.GetCurrentAnimatorStateInfo(0);
            currentState = currentStateInfo.fullPathHash;
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            IsGroundedHandler();
            CooldownsReduction();
            if (kyokoCurrentHealth <= 0)
            {
                StartCoroutine(Death());
            }
                switch (state)
            {
                case KyokoStates.Idle:
                    rigidBody.useGravity = true;
                    currentDamage = 0f;
                    movementSpeed = 0f;
                    SprintHandler(moveHorizontal, moveVertical);
                    WalkHandler(moveHorizontal, moveVertical);
                    DiveHandler();
                    JumpHandler();
                    KickHandler();
                    HurricaneKickHandler();
                    UltimateDabHandler();
                    BackElbowHandler();
                    BackKickHandler();
                    DropKickHandler();
                    DragonFeetHandler();
                    FrankensteinerHandler();
                    StompHandler();
                    break;
                case KyokoStates.Walking:
                    currentDamage = 0f;
                    movementSpeed = walkingSpeed;
                    SprintHandler(moveHorizontal, moveVertical);
                    IdleHandler(moveHorizontal, moveVertical);
                    DiveHandler();
                    JumpHandler();
                    KickHandler();
                    HurricaneKickHandler();
                    BackElbowHandler();
                    BackKickHandler();
                    DropKickHandler();
                    DragonFeetHandler();
                    FrankensteinerHandler();
                    StompHandler();
                    break;
                case KyokoStates.Running:
                    currentDamage = 0f;
                    movementSpeed = sprintSpeed;
                    IdleHandler(moveHorizontal, moveVertical);
                    WalkHandler(moveHorizontal, moveVertical);
                    DiveHandler();
                    JumpHandler();
                    KickHandler();
                    HurricaneKickHandler();
                    BackElbowHandler();
                    BackKickHandler();
                    DropKickHandler();
                    DragonFeetHandler();
                    FrankensteinerHandler();
                    StompHandler();
                    break;
                case KyokoStates.Jumping:
                    currentDamage = 0f;
                    movementSpeed = airWalkingSpeed;
                    Jump();
                    break;
                case KyokoStates.Falling:
                    currentDamage = 0f;
                    movementSpeed = airWalkingSpeed;
                    Falling();
                    BackElbowHandler();
                    break;
                case KyokoStates.Death:
                    break;
            }
        }
        private void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
            switch (state)
            {
                case KyokoStates.Idle:
                    break;
                case KyokoStates.Walking:
                    movementSpeed = walkingSpeed;
                    rigidBody.velocity = movement * movementSpeed * Time.deltaTime;
                    break;
                case KyokoStates.Running:
                    movementSpeed = sprintSpeed;
                    rigidBody.velocity = movement * movementSpeed * Time.deltaTime;
                    break;
                case KyokoStates.Falling:
                    rigidBody.useGravity = true;
                    movementSpeed = airWalkingSpeed;
                    rigidBody.velocity += movement * movementSpeed * Time.deltaTime;
                    break;
                case KyokoStates.Kicking:
                    currentDamage = kickDamage + kyokoDamageModifier;
                    Kicking();
                    break;
                case KyokoStates.HurricaneKicking:
                    currentDamage = hurricaneKickDamage;
                    HurricaneKicking();
                    break;
                case KyokoStates.UltimateDab:
                    currentDamage = ultimateDabDamage + kyokoDamageModifier;
                    UltimateDabing();
                    break;
                case KyokoStates.BackElbow:
                    currentDamage = backElbowDamage + kyokoDamageModifier;
                    BackElbow();
                    break;
                case KyokoStates.BackKick:
                    currentDamage = backKickDamage + kyokoDamageModifier;
                    BackKick();
                    break;
                case KyokoStates.Diving:
                    currentDamage = 0f;
                    Diving();
                    break;
                case KyokoStates.DropKick:
                    currentDamage = dropKickDamage + kyokoDamageModifier;
                    DropKick();
                    break;
                case KyokoStates.Frankensteiner:
                    currentDamage = frankensteinerDamage + kyokoDamageModifier;
                    Frankensteiner();
                    break;
                case KyokoStates.Stomp:
                    currentDamage = stompDamage + kyokoDamageModifier;
                    Stomp();
                    break;
                case KyokoStates.DragonFeet:
                    currentDamage = dragonFeetDamage + kyokoDamageModifier;
                    DragonFeet();
                    break;


            }
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
            Clamping();
            Utilities.RescaleCollider(damagingColliderRight, this.gameObject.GetComponent<SpriteRenderer>());

            #region Animations
            if (state == KyokoStates.Falling)
            {
                kyokoAnim.SetBool("IsGrounded", false);
            }
            else if (state != KyokoStates.Falling || state != KyokoStates.Jumping)
            {
                kyokoAnim.SetBool("IsGrounded", true);
            }
            if (isGrounded)
            {
                kyokoAnim.SetFloat("Speed", rigidBody.velocity.sqrMagnitude);
                kyokoAnim.SetBool("Sprinting", Input.GetButton("Sprint"));
            }
            #endregion

        }
        private void OnCollisionStay(Collision collision)
        {
            if (collision.collider.tag == "Enemy" && canTakeDamage)
            {
                if (collision.gameObject.GetComponent<AI_RuleEngine>().ai_AttackPattern[collision.gameObject.GetComponent<AI_RuleEngine>().currentActionNumber].ai_Action == AI_Actions.Attack)
                {
                    kyokoCurrentHealth -= collision.gameObject.GetComponent<AI_RuleEngine>().ai_AttackDamage;
                    canTakeDamage = false;
                    Invoke("canTakeDamageSwitch", 1f);
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "EnemyProjectile" && canTakeDamage)
            {
                kyokoCurrentHealth -= other.GetComponentInParent<AI_RuleEngine>().projectileDamage;
                canTakeDamage = false;
                Invoke("canTakeDamageSwitch", 1f);
            }
        }

        #region CharacterUtilties
        private void CooldownsReduction()
        {
            timeBetweenDives -= Time.deltaTime;
            timeBetweenKicks -= Time.deltaTime;
            timeBetweenHurricaneKicks -= Time.deltaTime;
            timeBetweenDabs -= Time.deltaTime;
            timeBetweenBackElbows -= Time.deltaTime;
            timeBetweenBackKicks -= Time.deltaTime;
            timeBetweenDropKicks -= Time.deltaTime;
            timeBetweenDragonFeet -= Time.deltaTime;
            timeBetweenFrankensteiner -= Time.deltaTime;
            timeBetweenJumps -= Time.deltaTime;
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
        private void canTakeDamageSwitch()
        {
            canTakeDamage = true;
        }
        #endregion
        #region Handlers
        //NonAnimationHandlers
        private void IsGroundedHandler()
        {
            Vector3 tempVector3 = new Vector3(0, 0.35f, 0);
            if (Physics.OverlapSphere(transform.position - tempVector3,0.0180f).Length > 1)
            {
                isGrounded = true;
                AerialAttacksLeft = AerialAttacksTotal;
            }
            else if (Physics.OverlapSphere(transform.position - tempVector3, 0.0180f).Length <= 1)
            {
                isGrounded = false;
                
            }
            //if (!isGrounded && previousState == jumpingState)
            //{
            //    earliestState = previousState;
            //    previousState = currentStateInfo.fullPathHash;
            //    earliestStateEnum = previousStateEnum;
            //    previousStateEnum = state;
            //    kyokoAnim.SetBool("Jump", false);
            //    timeBetweenJumps = jumpCooldown;
            //    movementSpeed = walkingSpeed;
            //    state = KyokoStates.Falling;
            //}
        }
        //AnimationHandlers
        private void StompHandler()
        {
            if(Input.GetButtonDown("Stomp"))
            {
                if (Time.time - stompRequestTime < doubleTapTime)
                {
                    state = KyokoStates.Stomp;
                }
                else
                {
                    stompRequestTime = Time.time;
                    state = KyokoStates.Idle;
                }
               
            }
        }
        private void FrankensteinerHandler()
        {
            if(Input.GetButton("Frankensteiner") && timeBetweenFrankensteiner <= 0f)
            {
                state = KyokoStates.Frankensteiner;
            }
        }
        private void UltimateDabHandler()
        {
            if(Input.GetButton("UltimateDab") && timeBetweenDabs <= 0f)
            {
                state = KyokoStates.UltimateDab;
            }
        }
        private void DropKickHandler()
        {
            if (Input.GetButton("DropKick") && timeBetweenDropKicks <= 0f)
            {
                state = KyokoStates.DropKick;
            }
        }
        private void BackKickHandler()
        {
            if (Input.GetButton("Kick") && timeBetweenBackKicks <= 0f && Input.GetKey(KeyCode.W))
            {
                state = KyokoStates.BackKick;
            }
        }
        private void BackElbowHandler()
        {
            if (Input.GetButton("BackElbow") && timeBetweenBackElbows <= 0f)
            {
                state = KyokoStates.BackElbow;
            }
        }
        private void DragonFeetHandler()
        {
            if(Input.GetButton("Kick") && timeBetweenDragonFeet <= 0f && previousState == kickState && earliestState == kickState)
            {
                state = KyokoStates.DragonFeet;
            }
        }
        private void HurricaneKickHandler()
        {
            if (Input.GetButton("Kick") && timeBetweenHurricaneKicks < 0 && Input.GetButton("Stomp")/*&& (previousState == dropKickState || earliestState == dropKickState)*/)
            {
                state = KyokoStates.HurricaneKicking;
            }
        }
        private void KickHandler()
        {
            if (Input.GetButton("Kick") && timeBetweenKicks <= 0)
            {
                state = KyokoStates.Kicking;
            }
        }
        private void IdleHandler(float moveH, float moveV)
        {
            if (moveH == 0f && moveV == 0f)
                state = KyokoStates.Idle;
            else if (currentState == idleState)
                state = KyokoStates.Idle;
           
        }
        private void WalkHandler(float moveH, float moveV)
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical")/*moveH != 0f || moveV != 0f && !immovable && isGrounded*/)
            {
                state = KyokoStates.Walking;
            }
            else if (currentState == walkingState && isGrounded)
            { 
                  state = KyokoStates.Walking;
            }

        }
        private void SprintHandler(float moveH, float moveV)
        {
            if (Input.GetButton("Sprint"))
                state = KyokoStates.Running;
        }
        private void DiveHandler()
        {
            if (Input.GetButton("Dive") && timeBetweenDives <= 0f)
            {
                state = KyokoStates.Diving;
            }
        }
        private void JumpHandler()
        {
            if (Input.GetButton("Jump") && timeBetweenJumps <= 0f)
            {
                state = KyokoStates.Jumping;
            }
            else
            {
                timeBetweenJumps -= Time.fixedDeltaTime;
            }

        }
        #endregion
        #region VirtualController
        public void VirtualBackElbow()
        {
            state = KyokoStates.BackElbow;
        }
        public void VirtualKick()
        {
            state = KyokoStates.Kicking;
        }
        public void VirtualJump()
        {
            state = KyokoStates.Jumping;
        }
        #endregion
        private void IncreaseEnergy()
        {
            kyoko_CurrentEnergy += 50f * Time.deltaTime;
        }
        #region Actions
        private IEnumerator Death()
        {
            kyokoAnim.SetTrigger("Death");
            rigidBody.velocity = Vector3.zero;
            rigidBody.velocity = Vector3.up * 75f * Time.deltaTime;
            state = KyokoStates.Death;
            yield return new WaitForSeconds(2f);
            this.gameObject.SetActive(false);
        }
        private void Stomp()
        {
            kyokoAnim.SetBool("Stomp", true);
            if (Utilities.isPlaying(kyokoAnim, "Stomp") && !immovable)
            {
                damagingColliderRight.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "Stomp") && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("Stomp", false);
                immovable = false;
                stompRequestTime = 0;
                timeBetweenStomps = stompCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;

            }
        }
        private void Frankensteiner()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("Frankensteiner", true);
            if (Utilities.isPlaying(kyokoAnim, "Frankensteiner") && !immovable)
            {
                damagingColliderLeft.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "Frankensteiner") && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderLeft.gameObject.SetActive(false);
                kyokoAnim.SetBool("Frankensteiner", false);
                immovable = false;
                timeBetweenDabs = ultimateDabCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;

            }
        }
        private void UltimateDabing()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("UltimateDab", true);
            if (Utilities.isPlaying(kyokoAnim, "UltimateDabAttack",1f) && !immovable)
            {
                damagingColliderRight.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "UltimateDabAttack") && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("UltimateDab", false);
                immovable = false;
                timeBetweenDabs = ultimateDabCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
                
            }
        }
        private void DragonFeet()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("DragonFeet", true);
            if (Utilities.isPlaying(kyokoAnim, "DragonFeet",1f) && !immovable)
            {
                damagingColliderRight.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "DragonFeet",1f) && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("DragonFeet", false);
                immovable = false;
                timeBetweenDragonFeet = dragonFeetCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }
        }
        private void DropKick()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("DropKick", true);
            if (Utilities.isPlaying(kyokoAnim, "DropKick",0.8f) && !immovable)
            {
                damagingColliderRight.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "DropKick",0.8f) && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("DropKick", false);
                immovable = false;
                rigidBody.velocity = Vector3.zero;
                timeBetweenDropKicks = dropKickCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }
        }
        private void BackKick()
        {
            
            kyokoAnim.SetBool("BackKick", true);
            if (Utilities.isPlaying(kyokoAnim, "BackKick") && !immovable)
            {
                damagingColliderRight.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "BackKick") && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("BackKick", false);
                immovable = false;
                timeBetweenBackKicks = backKickCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }
        }
        private void BackElbow()
        {    
            kyokoAnim.SetBool("BackElbow", true);
            if (Utilities.isPlaying(kyokoAnim, "BackElbow",0.8f) && !immovable && !isGrounded && AerialAttacksLeft > 0)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.useGravity = false;
                AerialAttacksLeft--;
                damagingColliderRight.gameObject.SetActive(true);
                immovable = true;
            }
            else if (Utilities.isPlaying(kyokoAnim, "BackElbow",0.8f) && !immovable && isGrounded)
            {
                damagingColliderRight.gameObject.SetActive(true);
                if(facingRight)
                {
                    rigidBody.AddForce(new Vector2(-50, 0f));
                }
                else
                {
                    rigidBody.AddForce(new Vector2(50, 0f));
                }
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "BackElbow") && immovable && isGrounded)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("BackElbow", false);
                immovable = false;
                timeBetweenBackElbows = backElbowCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "BackElbow") && immovable && !isGrounded)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("BackElbow", false);
                immovable = false;
                timeBetweenBackElbows = backElbowCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Falling;
            }
        }
        private void HurricaneKicking()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("HurricaneKick", true);
            if (Utilities.isPlaying(kyokoAnim, "HurricaneKick",0.8f) && !immovable)
            {
                damagingColliderUp.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "HurricaneKick",0.8f) && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderUp.gameObject.SetActive(false);
                kyokoAnim.SetBool("HurricaneKick", false);
                immovable = false;
                timeBetweenHurricaneKicks = hurricaneKickCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }
        }
        private void Kicking()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("Kicking", true);
            if (Utilities.isPlaying(kyokoAnim, "Kick",0.9f) && !immovable)
            {
                damagingColliderRight.gameObject.SetActive(true);
                rigidBody.velocity = Vector3.zero;
                immovable = true;
            }
            else if (!Utilities.isPlaying(kyokoAnim, "Kick",0.9f) && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                damagingColliderRight.gameObject.SetActive(false);
                kyokoAnim.SetBool("Kicking", false);
                immovable = false;
                timeBetweenKicks = kickCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }

        }
        private void Falling()
        {
            if (Utilities.isPlaying(kyokoAnim, "Falling") && !isGrounded)
            {
                rigidBody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            if (isGrounded)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                rigidBody.velocity = Vector3.zero;
                state = KyokoStates.Idle;
            }
        }
        private void Jump()
        {
            kyokoAnim.SetBool("Jump", true);          
            if (Utilities.isPlaying(kyokoAnim, "Jumping") &&  isGrounded)
            {
                rigidBody.velocity = new Vector3(0f, jumpSpeed);
            }
            else if (!Utilities.isPlaying(kyokoAnim, "Jumping") && !isGrounded)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                kyokoAnim.SetBool("Jump", false);
                timeBetweenJumps = jumpCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Falling;

            }
            //else if (!Utilities.isPlaying(kyokoAnim, "Jumping") && !isGrounded)
            //{
            //    earliestState = previousState;
            //    previousState = currentStateInfo.fullPathHash;
            //    earliestStateEnum = previousStateEnum;
            //    previousStateEnum = state;
            //    kyokoAnim.SetBool("Jump", false);
            //    timeBetweenJumps = jumpCooldown;
            //    movementSpeed = walkingSpeed;
            //    state = KyokoStates.Idle;
            //}
        }
        private void Diving()
        {
            movementSpeed = 0f;
            kyokoAnim.SetBool("Diving", true);
            if (Utilities.isPlaying(kyokoAnim, "Dive") && !immovable)
            {
                if (!facingRight)
                {
                    rigidBody.velocity = Vector3.right * diveSpeed;
                }
                else
                {
                    rigidBody.velocity = Vector3.left * diveSpeed;
                }
                immovable = true;
            }
            else if(!Utilities.isPlaying(kyokoAnim, "Dive") && immovable)
            {
                earliestState = previousState;
                previousState = currentStateInfo.fullPathHash;
                earliestStateEnum = previousStateEnum;
                previousStateEnum = state;
                kyokoAnim.SetBool("Diving", false);
                immovable = false;
                timeBetweenDives = diveCooldown;
                movementSpeed = walkingSpeed;
                state = KyokoStates.Idle;
            }
        }
        #endregion
    }
}
