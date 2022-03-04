using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCharStats : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float turnSpeed = 30;
    public float defaultRotateSpeed = 5;
    public MultiCharController mcc;
    public float jumpCoolDown;
    public float jumpRate = 0.5f;
    public float jumpStrength = 50;
    // Start is called before the first frame update
    void Start()
    {
        mcc = this.GetComponent<MultiCharController>();
        UpdateRotateSpeed(defaultRotateSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        jumpCoolDown += Time.deltaTime;
    }

    public void MoveWithGround(Vector2 movement)
    {
        mcc.MoveWithGround(movement * MoveSpeed);
    }

    public void LookDirectionGround(Vector2 movement)
    {
        mcc.MoveDirection(movement * turnSpeed);
    }

    public void UpdateRotateSpeed(float newSpeed)
    {
        mcc.rotateSpeed = newSpeed;
    }

    public void Jump()
    {
        if(jumpCoolDown > jumpRate)
        {
            mcc.Jump(jumpStrength);
            jumpCoolDown = 0;
        }
    }

    public void ToggleGravBoots()
    {
        mcc.magnoboots = !mcc.magnoboots;
    }
}
