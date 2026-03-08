using System;
using Unity.Hierarchy;
using UnityEngine;

public class Koopa : MonoBehaviour
{
    public Sprite shellSprite;
    private bool shelled;
    private bool pushed;
    public float shellSpeed = 12f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!shelled && collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starpower)
            {
                Hit();
            }
            if (collision.transform.DotTest(transform, Vector2.down))
            {
                EnterShell();
            } else
            {
                player.Hit();
            }
        }
    }

    private void Hit()
    {
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shelled && other.CompareTag("Player"))
        {
            if (!pushed)
            {
                Vector2 direction = new Vector2(transform.position.x - other.transform.position.y, 0f);
                PushShell(direction);
            } else
            {
                Player player = other.GetComponent<Player>();
                if (player.starpower)
                {
                    Hit();
                } else
                {
                    player.Hit();                   
                }

            }
        }
    }

    private void PushShell(Vector2 direction)
    {
        pushed = true;

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        EntityMovement movement = GetComponent<EntityMovement>();
        movement.direction = direction.normalized;
        movement.speed = shellSpeed;
        movement.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Shell");
    }
    private void EnterShell()
    {
        shelled = true;

        GetComponent<EntityMovement>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = shellSprite;
    }
}
