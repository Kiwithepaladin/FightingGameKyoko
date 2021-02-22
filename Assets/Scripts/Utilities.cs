using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUp
{
    public class Utilities : MonoBehaviour
    {
       public static bool isPlaying(Animator anim, string stateName,float nt = 0.7f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime < nt)
                return true;
            else
                return false;
        }
        public static void RescaleCollider(BoxCollider collider,SpriteRenderer sr)
        {
            
            Vector3 S = sr.GetComponent<SpriteRenderer>().sprite.bounds.size;
            collider.size = S - new Vector3(0.6f, 0f) + new Vector3(0,0,0.3f);
            collider.center = new Vector2((S.x / 2), 0);
        }
        public static bool IsObjectVisible(Renderer renderer)
        {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), renderer.bounds);
        }
        public static bool CompareVector3(Vector3 vectorOne, Vector3 vectorTwo)
        {
            if (Vector3.Distance(vectorOne, vectorTwo) > 1f)
                return true;
            return false;
        }
        public static void ShowFloatingText(GameObject floatingText, Transform transform, string message, Vector3 floatingTextOffset, Vector3 randomizeIntensity)
        {
            //DestoryPreviousFloatingTexts();
            var floatingTextInst = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
            floatingTextInst.GetComponent<TextMesh>().text = message.ToString();
            floatingTextInst.transform.localPosition += floatingTextOffset;
            floatingTextInst.transform.localPosition += new Vector3(UnityEngine.Random.Range(-randomizeIntensity.x, randomizeIntensity.x)
            , UnityEngine.Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            UnityEngine.Random.Range(-randomizeIntensity.z, randomizeIntensity.z));
            Destroy(floatingTextInst, 0.5f);
        }
        #region BetterOnTrigger
        public delegate void _OnTriggerExit(Collider c);

        Collider thisCollider;
        bool ignoreNotifyTriggerExit = false;

        // Target callback
        Dictionary<GameObject, _OnTriggerExit> waitingForOnTriggerExit = new Dictionary<GameObject, _OnTriggerExit>();

        public static void NotifyTriggerEnter(Collider c, GameObject caller, _OnTriggerExit onTriggerExit)
        {
            Utilities thisComponent = null;
            Utilities[] ftncs = c.gameObject.GetComponents<Utilities>();
            foreach (Utilities ftnc in ftncs)
            {
                if (ftnc.thisCollider == c)
                {
                    thisComponent = ftnc;
                    break;
                }
            }
            if (thisComponent == null)
            {
                thisComponent = c.gameObject.AddComponent<Utilities>();
                thisComponent.thisCollider = c;
            }
            // Unity bug? (!!!!): Removing a Rigidbody while the collider is in contact will call OnTriggerEnter twice, so I need to check to make sure it isn't in the list twice
            // In addition, force a call to NotifyTriggerExit so the number of calls to OnTriggerEnter and OnTriggerExit match up
            if (thisComponent.waitingForOnTriggerExit.ContainsKey(caller) == false)
            {
                thisComponent.waitingForOnTriggerExit.Add(caller, onTriggerExit);
                thisComponent.enabled = true;
            }
            else
            {
                thisComponent.ignoreNotifyTriggerExit = true;
                thisComponent.waitingForOnTriggerExit[caller].Invoke(c);
                thisComponent.ignoreNotifyTriggerExit = false;
            }
        }

        public static void NotifyTriggerExit(Collider c, GameObject caller)
        {
            if (c == null)
                return;

            Utilities thisComponent = null;
            Utilities[] ftncs = c.gameObject.GetComponents<Utilities>();
            foreach (Utilities ftnc in ftncs)
            {
                if (ftnc.thisCollider == c)
                {
                    thisComponent = ftnc;
                    break;
                }
            }
            if (thisComponent != null && thisComponent.ignoreNotifyTriggerExit == false)
            {
                thisComponent.waitingForOnTriggerExit.Remove(caller);
                if (thisComponent.waitingForOnTriggerExit.Count == 0)
                {
                    thisComponent.enabled = false;
                }
            }
        }
        private void OnDisable()
        {
            if (gameObject.activeInHierarchy == false)
                CallCallbacks();
        }
        private void Update()
        {
            if (thisCollider == null)
            {
                // Will GetOnTriggerExit with null, but is better than no call at all
                CallCallbacks();

                Component.Destroy(this);
            }
            else if (thisCollider.enabled == false)
            {
                CallCallbacks();
            }
        }
        void CallCallbacks()
        {
            ignoreNotifyTriggerExit = true;
            foreach (var v in waitingForOnTriggerExit)
            {
                if (v.Key == null)
                {
                    continue;
                }

                v.Value.Invoke(thisCollider);
            }
            ignoreNotifyTriggerExit = false;
            waitingForOnTriggerExit.Clear();
            enabled = false;
        }
        #endregion
    }
}
