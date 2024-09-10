using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    public GameObject playerSprite;

    float rotSpeed = 5f;
    bool shouldRotate = false;
    Transform originalStartRot;
    Transform originalEndRot;

    Vector3 m_from = new Vector3(0.0F, 0.0F, 0.0F);
    Vector3 m_to = new Vector3(0.0F, 0.0F, -10.0F);


    bool hammerRotated = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If our inputX is not 0, we lerp between 0 and -10 on the Z axis 
        // Otherwise rotation will be reset.
        if (shouldRotate) {SquishRun();}
        else {ResetRotation();}

    }

    public void UpdateRotate(bool rot)
    {
        shouldRotate = rot;
    }

    // Warp the scale of the sprite when jumping
    public void JumpPlayer(float x, float y)
    {
        playerSprite.transform.localScale = new Vector2(x, y);
    }

    // Move the player direction to flip all child objects
    // including the sprite and rotation targets
    public void FlipPlayer(GameObject target, float x, float y)
    {
        // Changed from playerSPrite to target.....
        target.transform.localScale = new Vector2(x, y);
    }

    public void RotateHammer(GameObject target, float direction)
    {
        // Rotate to -80 on Z
        // Make sure we only do this once!
        if (direction == 1f)
        {
            target.transform.localRotation = Quaternion.identity;
            hammerRotated = false;
        }
        else if(!hammerRotated)
        {
            target.transform.Rotate(0f, 0f, 80f, Space.World);
            hammerRotated = true;
        }

    }

    public void SquishRun()
    {
        // Transform the start transform's vector3 position into Euler angles
        Quaternion fromRot = Quaternion.Euler(m_from);
        Quaternion toRot = Quaternion.Euler(m_to);

        // might have to tweak
        float lerp = 0.5f * (1.0f + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * rotSpeed));
        playerSprite.transform.localRotation = Quaternion.Lerp(fromRot, toRot, lerp);
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
        playerSprite.transform.localRotation = Quaternion.identity;
    }
}
