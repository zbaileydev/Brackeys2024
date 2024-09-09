using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public WorldGenerator worldGenerator;
    public AudioSource steps;
    public AudioSource music;

    public float speed = 10f;
    public float jumpThrust = 15f;
    //public Image reticleImage;
    public GameObject hammer;

    public float stetchHeight = 0.6f;
    public float stetchWidth = 0.25f;

    public float originalHeight = 0.5f;
    public float originalWidth = 0.3f;

    private float playerOriginalScale = 1f;

    bool isGrounded = true;
    bool stepSounds = false;

    PlayerGraphics playerGraphics;
    Rigidbody2D rb;
    //RectTransform crosshair;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerGraphics = GetComponent<PlayerGraphics>();
        //crosshair = reticleImage.GetComponent<RectTransform>();
        //music.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.Quit();
        }
        // We need to cap the pos within the player's mining range
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 editedPos = new Vector3(mousePos.x, mousePos.y, 0);
        //crosshair.anchoredPosition = editedPos;

        if (rb.velocity.y == 0)
        {
            isGrounded = true;
            // Ensure our player sprite is normal sized again
            playerGraphics.JumpPlayer(originalWidth, originalHeight);
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            playerGraphics.JumpPlayer(stetchWidth, stetchHeight);
            //Jump the gameobject
            HandleJump();
            isGrounded = false;
        }
        // Function to handle our X-axis movement
        MoveHorizontal();

        if(Input.GetMouseButtonDown(0))
            worldGenerator.DeleteTileAt(transform.position + Vector3.down);
    }


    void MoveHorizontal()
    {
        // Get our X input and translate it into movement based on frames
        float inputX = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(speed * inputX, 0, 0);
        movement *= Time.deltaTime;
        // Move the player gameobject based on the X input 
        transform.Translate(movement);

        worldGenerator.xPos = Mathf.RoundToInt(transform.position.x);
        /*
        // Only play SFX if we are moving and havent already started the SFX
        if (inputX != 0 && !stepSounds)
        {
            steps.Play();
            stepSounds = true;
        }
        */

        // If ==0 we are stationary and facing our direction
        if (inputX < 0)
        {
            playerGraphics.FlipPlayer(gameObject, -playerOriginalScale, playerOriginalScale);
            playerGraphics.FlipPlayer(hammer, -1f, 1f);
            playerGraphics.RotateHammer(hammer, -1f);
            playerGraphics.UpdateRotate(true);
        }
        else if (inputX > 0)
        {
            playerGraphics.FlipPlayer(gameObject, playerOriginalScale, playerOriginalScale);
            playerGraphics.FlipPlayer(hammer, 1f, 1f);
            playerGraphics.RotateHammer(hammer, 1f);
            playerGraphics.UpdateRotate(true);
        }
        else if (inputX == 0)
        {
            // No rotation at all if we are not moving
            playerGraphics.UpdateRotate(false);
            //steps.Stop();
            //stepSounds = false;
        }

    }

    void HandleJump()
    {
        rb.AddForce(transform.up * jumpThrust, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Some backup checking in case our velocity is clipped on something
        if (other.gameObject.CompareTag("Floor") && !isGrounded)
        {
            isGrounded = true;
        }

        //                      TEMPORARY
        // TODO: Move to new script that handles loot and inventory ||
        //                                                          \/
        if(other.gameObject.CompareTag("Loot Container"))
        {
            string item = other.gameObject.GetComponent<ILootContainer>().GetLoot();
            if (item != string.Empty)
                Debug.Log($"Collected {item}");
        }
    }
}
