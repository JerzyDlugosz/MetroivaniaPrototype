using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public GameObject laser;
    private SpriteRenderer laserSpriteRenderer;
    private BoxCollider2D laserCollider;
    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private float startAttackOffset;

    public bool usedByController = false;

    [SerializeField]
    private float damage = 1;

    [SerializeField]
    private List<Sprite> laserSprites = new List<Sprite>();

    public Coroutine laserCoroutine;

    private void Start()
    {
        laserSpriteRenderer = laser.GetComponent<SpriteRenderer>();
        laserCollider = laser.GetComponent<BoxCollider2D>();
        SetLaser(0f, 0);
        if (!usedByController)
        {
            if (attackDelay == 0)
            {
                SetLaser(0.75f, 3);
            }
            else
            {
                StartLaserCoroutine();
            }
        }
    }

    public void SetData(float _attackDelay, float _attackOffset)
    {
        attackDelay = _attackDelay;
        startAttackOffset = _attackOffset;
    }

    public void StartLaserCoroutine()
    {
        laserCoroutine = StartCoroutine(AttackCoroutine());
    }

    public void StopLaserCoroutine()
    {
        StopCoroutine(laserCoroutine);
        laserCoroutine = null;
        SetLaser(0f, 0);
    }

    IEnumerator AttackCoroutine()
    {
        laserCollider.enabled = false;
        laserSpriteRenderer.sprite = laserSprites[0];
        yield return new WaitForSeconds(startAttackOffset);
        do
        {
            SetLaser(0.25f, 1);
            yield return new WaitForSeconds(0.1f);
            SetLaser(0.5f, 2);
            yield return new WaitForSeconds(0.1f);
            SetLaser(0.75f, 3);

            yield return new WaitForSeconds(attackDelay);

            SetLaser(0.5f, 2);
            yield return new WaitForSeconds(0.1f);
            SetLaser(0.25f, 1);
            yield return new WaitForSeconds(0.1f);
            SetLaser(0f, 0);

            yield return new WaitForSeconds(attackDelay);
        } while (true);
    }

    private void SetLaser(float colliderSize, int laserSprite)
    {
        if(laserSprites.Count > 0)
        {
            if (colliderSize == 0)
            {
                laserCollider.enabled = false;
            }
            else
            {
                laserCollider.enabled = true;
                laserCollider.size = new Vector2(colliderSize, 1f);
            }
            laserSpriteRenderer.sprite = laserSprites[laserSprite];
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(damage);
        }
    }
}
