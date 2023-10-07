using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform Player;


    Vector2 startPosition;

    float startZ;

    [SerializeField]
    private float paralaxMulti = 1;


    Vector2 travel => (Vector2)cam.transform.position - startPosition;
    float distanceFromPlayer => (transform.position.z - Player.position.z) * paralaxMulti;
    float clippingPlane => cam.transform.position.z + (distanceFromPlayer > 0 ? cam.farClipPlane : cam.nearClipPlane);
    float parallaxFactor => Mathf.Abs(distanceFromPlayer) / clippingPlane;


    private void Start()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player").transform;
        }
        if(cam == null)
        {
            cam = Camera.main;
        }

        cam = Camera.main;
        Player = GameManagerScript.instance.player.transform;
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    private void Update()
    {
        Vector3 pos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
