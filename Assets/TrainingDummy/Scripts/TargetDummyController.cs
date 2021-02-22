using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUp
{
    public class TargetDummyController : MonoBehaviour
    {
        private PlayerControllerScript kyoko;
        [SerializeField] private float dummy_MaxHealth;
        [SerializeField] private float dummy_CurrentHealth;
        [SerializeField] private GameObject floatingText;
        [SerializeField] private Vector3 floatingTextOffset;
        [SerializeField] private float takingDamageCooldown;
        private float timeBetweenTakingDamage;
        [SerializeField] private bool canTakeDamage;
        private Vector3 randomizeIntensity = new Vector3(0.5f, 0, 0);
        private Animator dummyAnim;
        private SpriteRenderer dummyRenderer;
        private Rigidbody dummy_rigidBody;
        private void Start()
        {
            kyoko = GameObject.Find("Kyoko").GetComponent<PlayerControllerScript>();
            dummyAnim = GetComponent<Animator>();
            dummyRenderer = GetComponent<SpriteRenderer>();
            timeBetweenTakingDamage = 0;
            dummy_rigidBody = GetComponent<Rigidbody>();

        }
        private void Update()
        {
            TakingDamage();
            if (dummy_CurrentHealth <= 0f)
                Destroy(gameObject);
            if (transform.position.x - kyoko.transform.position.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        private void FixedUpdate()
        {
            //dummy_rigidBody.position = new Vector3(
            //       transform.position.x
            //      , Mathf.Clamp(dummy_rigidBody.position.y, 0, 0.78f)
            //      , transform.position.z);



        }
        private void OnTriggerEnter(Collider other)
        {
            Utilities.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            if (other.tag.Equals("DamagingCollider"))
            {
                if (floatingText && dummy_CurrentHealth > 0f && canTakeDamage)
                {
                    dummy_CurrentHealth -= kyoko.currentDamage;
                    ShowFloatingText();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Utilities.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
            if (other.tag.Equals("DamagingCollider"))
            {
                if (floatingText && dummy_CurrentHealth > 0f && canTakeDamage)
                {
                    dummyAnim.SetBool("TakingDamage", true);
                    if (PlayerControllerScript.state == KyokoStates.HurricaneKicking)
                    {
                        dummy_rigidBody.velocity = Vector3.up * 15;
                    }
                    if(PlayerControllerScript.state == KyokoStates.BackKick)
                    {
                        if(transform.rotation.y == 0)
                        {
                            dummy_rigidBody.velocity = Vector3.right * 2.5f;
                        }
                        else
                        {
                            dummy_rigidBody.velocity = Vector3.left * 2.5f;
                        }
                    }
                    if(PlayerControllerScript.state == KyokoStates.Stomp)
                    {
                        if (transform.rotation.y == 0)
                        {
                            dummy_rigidBody.velocity = new Vector3(1,1) * 0.5f;
                        }
                        else
                        {
                            dummy_rigidBody.velocity = new Vector3(-1,1) * 0.5f;
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
                dummyAnim.SetBool("TakingDamage", false);
                dummy_rigidBody.velocity = Vector3.zero;
            }
        }
        private void TakingDamage()
        {
            if(timeBetweenTakingDamage < 0)
            {
                canTakeDamage = false;
                timeBetweenTakingDamage = takingDamageCooldown;
            }
            else
            {
                timeBetweenTakingDamage -= Time.deltaTime;
                canTakeDamage = true;
            }
        }
        private void ShowFloatingText()
        {
            //DestoryPreviousFloatingTexts();
            var floatingTextInst = Instantiate(floatingText, transform.position, Quaternion.identity,transform);
            floatingTextInst.GetComponent<TextMesh>().text = dummy_CurrentHealth.ToString();
            floatingTextInst.transform.localPosition += floatingTextOffset;
            floatingTextInst.transform.localPosition += new Vector3(Random.Range(-randomizeIntensity.x, randomizeIntensity.x)
            , Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z));
            Destroy(floatingTextInst,0.5f);
        }
        private void DestoryPreviousFloatingTexts()
        {
            GameObject[] previousFloatingText = GameObject.FindGameObjectsWithTag("FloatingText");
            foreach (var item in previousFloatingText)
            {
                if (previousFloatingText.Length > 1)
                {
                    Destroy(item);
                }
            }
        }
    }
}
