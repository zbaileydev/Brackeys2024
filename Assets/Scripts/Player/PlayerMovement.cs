using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Action OnPlayerJump;
    //           movement, position, velocity
    public Action<Vector3, Vector3, Vector3> OnPlayerMove;
    public Action OnPlayerLand;

    public WorldGenerator worldGenerator;
    public Transform[] groundChecks;
    public LayerMask groundLayer;
    public float movementSpeed = 10f;
    public float jumpThrust = 15f;
    public bool airControl = true;

    float groundCheckRadius = 0.1f;
    bool isGrounded = true;
    bool stepSounds = false;
    bool jump = false;
    Vector3 velocity;
    Vector3 movement;

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

        for (int i = 0; i < colliders.Count; i++)
            if (colliders[i].gameObject != gameObject)
                isGrounded = true;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            OnPlayerJump?.Invoke();
            jump = true;
            //am.PauseClip(am.footstepSFX);
        }

        float inputX = Input.GetAxis("Horizontal");
        movement = new Vector3(movementSpeed * inputX, 0);

        if (Input.GetMouseButtonDown(0))
            worldGenerator.DeleteTileAt(transform.position + Vector3.down);
    }

    void FixedUpdate()
    {
        MoveHorizontal();
    }


    void MoveHorizontal()
    {
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
}
