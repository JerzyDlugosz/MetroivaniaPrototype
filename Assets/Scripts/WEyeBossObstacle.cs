using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WEyeBossObstacle : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D obstacleRigidbody;
    public float destroyXPosition;
    public float obstacleSpeed;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(1);
        }
    }

    private void Update()
    {
        transform.Translate(new Vector3(Time.deltaTime * -obstacleSpeed, 0f, 0f));
        if (transform.localPosition.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}
