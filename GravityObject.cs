using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    private Vector3 gravity;
    private MultiCharController mcc;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        ResetGravity();

        mcc = this.GetComponent<MultiCharController>();
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        ResetGravity();
    }

    

    private void LateUpdate()
    {
        
      
    }

    public void AddPull(Vector3 pull)
    {
        gravity += pull;
    }

    private void ResetGravity()
    {
        gravity = new Vector3();
    }

    private void ApplyGravity()
    {
        if(mcc != null)
        {
            mcc.SetGravity(gravity);
        }
        else if(rb != null)
        {
            rb.AddForce(gravity * Time.deltaTime, ForceMode.Acceleration);
        }

    }
}
