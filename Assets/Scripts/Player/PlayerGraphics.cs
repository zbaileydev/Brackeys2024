using System;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    public GameObject playerGFX;
    public GameObject playerSprite;

    float stretchHeight = 1.1f;
    float stretchWidth = 0.5f;
    float originalWidth;
    float originalHeight;
    float rotSpeed = 5f;
    bool shouldRotate = false;

    float rotateAngle = 10;
    bool facing = true;
    bool hammerRotated = false;

    void Start()
    {
        var playerMovement = GetComponent<PlayerMovement>();
        playerMovement.OnPlayerJump += JumpPlayer;
        playerMovement.OnPlayerLand += LandPlayer;
        playerMovement.OnPlayerMove += MovePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        // If our inputX is not 0, we lerp between 0 and -10 on the Z axis
        // Otherwise rotation will be reset.
        if (shouldRotate) SquishRun();
        else ResetRotation();

    }

    public void MovePlayer(Vector3 movementVector, Vector3 position, Vector3 velocity)
    {
        if (movementVector.sqrMagnitude >= 0.01)
            shouldRotate = true;
        else
            shouldRotate = false;

        if (movementVector.x > 0 && !facing)
        {
            Flip();
            facing = true;
        }
        else if (movementVector.x < 0 && facing)
        {
            Flip();
            facing = false;
        }
    }

    // Warp the scale of the sprite when jumping
    public void JumpPlayer()
    {
        originalWidth = playerSprite.transform.localScale.x;
        originalHeight = playerSprite.transform.localScale.y;
        playerSprite.transform.localScale = new Vector2(stretchWidth, stretchHeight);
    }
    public void LandPlayer() => playerSprite.transform.localScale = new Vector2(originalWidth, originalHeight);

    // Move the player direction to flip all child objects
    // including the sprite and rotation targets
    public void Flip() => playerGFX.transform.localScale = new Vector2((facing ? -1 : 1) * MathF.Abs(playerGFX.transform.localScale.x), playerGFX.transform.localScale.y);

    public void RotateHammer(GameObject target, float direction)
    {
        // Rotate to -80 on Z
        // Make sure we only do this once!
        if (direction == 1f)
        {
            target.transform.localRotation = Quaternion.identity;
            hammerRotated = false;
        }
        else if (!hammerRotated)
        {
            target.transform.Rotate(0f, 0f, 80f, Space.World);
            hammerRotated = true;
        }
    }

    public void SquishRun()
    {
        // Transform the start transform's vector3 position into Euler angles
        Quaternion fromRot = Quaternion.identity;
        Quaternion toRot = Quaternion.Euler(Vector3.forward * rotateAngle * (facing ? -1 : 1));

        // might have to tweak
        float lerp = 0.5f * (1.0f + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * rotSpeed));
        playerGFX.transform.localRotation = Quaternion.Lerp(fromRot, toRot, lerp);
    }

    public void SquishMine()
    {
        // Player goes from 5 degrees to 20 degrees in direction facing
        // we will be updating the toRot in this case
        // as well as adjusting the fromRot back to around +5 degrees Z axis
        // gives the impression of the player swinging backwards to mine
    }

    void ResetRotation()
    {
        playerGFX.transform.localRotation = Quaternion.identity;
    }
}
