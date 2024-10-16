using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LampSr : MonoBehaviour
{
    public GameObject rootObject;

    public float walkSpeed = 5;
    public float[] walkBounds = new float[] { -20, 20 };
    public float nodSpeed = 0.5f;
    public float jumpSpeed = 5;
    public float[] jumpBounds = new float[] { 0, 3 };
    public float fallSpeed = 1;
    public float getUpSpeed = 1;

    private bool nodUp = true;
    private bool jumpUp = true;
    private bool goRight = true;
    private bool isFalling = false;
    private bool isDead = false;
    private bool isGettingUp = false;

    private float[] nodLowerBounds;
    private float[] nodUpperBounds;
    private float[] nodBaseBounds;

    private float originalWalkSpeed;
    private float originalJumpSpeed;

    private Limb Base => rootObject.GetComponent<Limb>();
    private Limb UpperArm => Base.child.GetComponent<Limb>();  
    private Limb LowerArm => UpperArm.child.GetComponent<Limb>();
    private Limb Head => LowerArm.child.GetComponent<Limb>(); 

    // Start is called before the first frame update
    void Start()
    {
        originalWalkSpeed = walkSpeed;
        originalJumpSpeed = jumpSpeed;
        Turn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFalling && !isGettingUp && !isDead) {
            Walk();
            Nod();
            Jump();
            CheckInput();
        } else if (isFalling) {
            Fall();
            if (!jumpUp) {
                Jump();
            }
        } else if (isGettingUp) {
            GetUp();
        }
    }

    private void Walk()
    {
        float moveSpeed = goRight ? walkSpeed : -walkSpeed;

        if (moveSpeed == 0) {
            return;
        }

        float targetX = walkBounds[goRight ? 1 : 0];

        if (Base.MoveLimb(moveSpeed, targetX)) {
            Turn();
        }
    }

    private void Nod()
    {
        float speed = nodUp ? nodSpeed : -nodSpeed;
        float reversedSpeed = nodUp ? -nodSpeed : nodSpeed;

        Base.Rotate(reversedSpeed, nodUp ? nodBaseBounds[1] : nodBaseBounds[0]);
        UpperArm.Rotate(speed, nodUp ? nodUpperBounds[1] : nodUpperBounds[0]);
        if (LowerArm.Rotate(speed, nodUp ? nodLowerBounds[1] : nodLowerBounds[0]))
        {
            nodUp = !nodUp;
        }
    }

    private void Jump()
    {
        float speed = jumpUp ? jumpSpeed : -jumpSpeed;
        if (Base.IncreaseAltitude(speed, jumpUp ? jumpBounds[1] : jumpBounds[0]))
        {
            // Just landed -> reset speeds (they may have been modified by ImmediateJump)
            if (!jumpUp) {
                walkSpeed = originalWalkSpeed;
                jumpSpeed = originalJumpSpeed;
            }

            jumpUp = !jumpUp;
        }
    }

    private void ImmediateJump()
    {
        jumpUp = true;
        walkSpeed = 0;
        jumpSpeed = originalJumpSpeed * 2;
    }

    private void LongJump()
    {
        jumpUp = true;
        walkSpeed = originalWalkSpeed * 4;
        jumpSpeed = originalJumpSpeed;
    }

    private void Collapse()
    {
        isFalling = true;
        jumpUp = false;
    }

    private void Fall()
    {
        float realFallSpeed = goRight ? -fallSpeed : fallSpeed;
        float targetAngle = goRight ? -90 : 90;
        if (Base.Rotate(realFallSpeed, targetAngle))
        {
            isFalling = false;
            isDead = true;
            Invoke("StartGettingUp", 1f);
        }
    }

    private void StartGettingUp()
    {
        isDead = false;
        isGettingUp = true;
    }

    private void GetUp()
    {
        float realGetUpSpeed = goRight ? getUpSpeed : -getUpSpeed;
        float targetAngle = 0;
        if (Base.Rotate(realGetUpSpeed, targetAngle))
        {
            isGettingUp = false;
        }
    }

    private void Turn()
    {
        goRight = !goRight;
        if (goRight) {
            Base.Rotate0();
            UpperArm.Rotate0();
            LowerArm.Rotate0();
            nodBaseBounds = new float[] { 0, 30 };
            nodUpperBounds = new float[] { -30, 0 };
            nodLowerBounds = new float[] { -30, 0 };
        } else {
            Base.Rotate0();
            UpperArm.Rotate0();
            LowerArm.Rotate180();
            nodBaseBounds = new float[] { -30, 0 };
            nodUpperBounds = new float[] { 30, 0 };
            nodLowerBounds = new float[] { 180, 210 };
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && goRight) {
            Turn();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && !goRight) {
            Turn();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ImmediateJump();   
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            LongJump();
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            Collapse();
        }
    }
}
