using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUp
{
    [System.Serializable]
    public class SpawnEnemy
    {
        public GameObject enemy;
        public Vector3 enemyPosition;
    }
    public class CameraStopper : MonoBehaviour
    {
        private GameObject otherGameObject;
        [SerializeField] private SpawnEnemy[] enemiesToSpawn;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "CameraMidPoint")
            {
                otherGameObject = other.transform.parent.gameObject;
                otherGameObject.GetComponent<CameraController>().isFollowing = false;
                gameObject.SetActive(false);
                foreach (SpawnEnemy item in enemiesToSpawn)
                {
                    Instantiate(item.enemy,item.enemyPosition,Quaternion.identity);
                }
            }
            else return;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (SpawnEnemy item in enemiesToSpawn)
            {
                Gizmos.DrawWireSphere(item.enemyPosition, 1f);
            }
        }
    }
}

