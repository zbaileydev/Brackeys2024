using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Action OnPlayerJump;
    //           movement, position, velocity
    public Action<Vector3, Vector3, Vector3> OnPlayerMove;
    public Action OnPlayerLand;

    public Transform[] groundChecks;
    public LayerMask groundLayer;
    public float movementSpeed = 10f;
    public float jumpThrust = 15f;

    // Vector3 forward = Vector3.right;
    float groundCheckRadius = 0.1f;
    bool isGrounded = true;
    bool stepSounds = false;
    bool jump = false;
    // Vector3 velocity;
    float inputX;

    Rigidbody2D rb;
    //AudioManager am;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //am = AudioManager.instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Application.Quit();

        // if (rb.velocity.y <= 0.01f && !isGrounded)
        // {
        //     isGrounded = true;
        //     OnPlayerLand?.Invoke();
        // }

        List<Collider2D> colliders = new();
        foreach (var check in groundChecks)
            colliders.AddRange(Physics2D.OverlapCircleAll(check.position, groundCheckRadius, groundLayer));

        isGrounded = colliders.Any(x => x.gameObject != gameObject);


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            OnPlayerJump?.Invoke();
            jump = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/sfx_player_jump");
            //am.PauseClip(am.footstepSFX);
        }

        inputX = Input.GetAxisRaw("Horizontal");

        // if (Input.GetMouseButtonDown(1) && !weaponAnimation.isPlaying && currentWeapon != null)
        // {
        //     weaponAnimation.Play();
        // }
        // if(!weaponAnimation.isPlaying) currentWeapon.AttackCycleEnded(); //this is stupid to do this in update instead of some event, if someone can figure out how to fix it please do
    }

    void FixedUpdate()
    {
        MoveHorizontal();
    }

    void MoveHorizontal()
    {
        Vector3 movement = Vector3.right * inputX * movementSpeed;

        transform.Translate(movement * Time.deltaTime);

        OnPlayerMove?.Invoke(movement, transform.position, rb.velocity);

        if (movement.x != 0 && isGrounded && !stepSounds)
        {
            //am.PlayClip(am.footstepSFX, am.footstepSFXVolume, true); //Play the footsteps indefinitely (LOOP).
            stepSounds = true;
        }

        // if (isGrounded || airControl)
        // {
        //     Vector3 targetVelocity = movement + Vector3.up * rb.velocity.y;
        //     rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        // }
        if (isGrounded && jump)
        {
            rb.AddForce(Vector3.up * jumpThrust, ForceMode2D.Impulse);
            jump = false;
        }

        if (movement.x == 0)
        {
            //am.PauseClip(am.footstepSFX);
            stepSounds = false;
        }

    }

    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     Vector3 normal;
    //     if (Vector3.Dot(Vector3.up, other.GetContact(0).normal) > 0.3)
    //         normal = other.GetContact(0).normal;
    //     else
    //         normal = Vector3.up;
    //     forward = Vector3.Cross(normal, Vector3.forward).normalized;
    // }
}
