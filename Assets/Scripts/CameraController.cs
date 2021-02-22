using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isFollowing;

    private Vector3 playerXPos;
    GameObject playerTarget;
    [SerializeField] private float cameraSpeed;

    private void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
    }
    private void LateUpdate()
    {
        playerXPos = new Vector3(playerTarget.transform.position.x, 0f, 0f);
        if (isFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, playerXPos, cameraSpeed);
        }
       // AllEnmiesDead();
    }
    private void AllEnmiesDead()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0 && !isFollowing)
        {
            isFollowing = true;
        }
    }
}
