using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueparticalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerBase player;

    [SerializeField] Rigidbody2D playerRb;

    [SerializeField] private LayerMask groundLayer;

    [Header("MovementPartical")]
    [SerializeField] ParticleSystem movementParticle;

    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;

    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;

    [Header("WallHitPartical")]
    [SerializeField] ParticleSystem wallHitParticle;

    float counter;
    bool isOnGrounded;
    

    private void Update()
    {
        MovementPartical();
        
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0) {
            
            isOnGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0) {
            isOnGrounded = false;
        }
    }
    private void MovementPartical() 
    {
        counter += Time.deltaTime;

        if (isOnGrounded && Mathf.Abs(playerRb.linearVelocity.x) > occurAfterVelocity) {
            if (counter > dustFormationPeriod) {
                movementParticle.Play();
                counter = 0;
            }
        }

    }
    private void OnEnable() {
        player.OnTouched += WallHitPartical;
    }

    private void OnDisable() {
        player.OnTouched -= WallHitPartical;
    }

    private void WallHitPartical() 
    {

        wallHitParticle.Play();
    }




}