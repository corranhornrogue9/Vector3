using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCharController : MonoBehaviour
{
    private Vector3 gravity;
    private CharacterController cc;
    private Vector2 moveWithGround;
    public Vector2 lookDirection;
    private Vector3 size;
    Transform footing;
    Transform body;
    Transform head;
    Transform gravityIndicator;
    Vector3 groundForward;
    Vector3 groundRight;
    Vector3 groundDown;
    Vector2 floorAngle;
    public float rotateSpeed = 5;
    private Vector3 oldGroundSpot = Vector3.positiveInfinity;
    private bool grounded = false;
    private Vector3 momentum;
    private float jetCooldown = 0.2f;
    private float jetCoolDownCountDown;
    private float defaultJetStrength = 1;
    public MultiCharSupplies supplies;
    private  float headTilt;
    public float gravityRotateThreshold = 1;
    public bool magnoboots = false;
    public float bootRange = 2.5f;
    public float bootStrength = 20f;

    // Start is called before the first frame update
    void Start()
    {
        cc = this.GetComponent<CharacterController>();
        supplies = this.GetComponent<MultiCharSupplies>();
        ApplyGravity();
        ResetGravity();
        ResetMoveGround();

        footing = this.transform.Find("Footing");
        body = footing.Find("Body");
        head = body.Find("Head");
        gravityIndicator = this.transform.Find("GravityIndicator");
    }



    // Update is called once per frame
    void Update()
    {
        FindSize();
        ApplyMagnoBoots();
        ApplyGravity();
        if (gravity.magnitude > gravityRotateThreshold)
        {
            FindFooting();
            RotateBody();
            RotateHead();
        }
        else
        {
            SpinInSpace();
        }

        ApplyMoveWithGround();
        ApplyMoveInSpace();
        
        ResetGravity();
        ResetMoveGround();
      


    }

    private void LateUpdate()
    {
        

    }

    public void SetGravity(Vector3 pull)
    {
        //Debug.Log("setting grav = " + pull.magnitude);
        gravity = pull;
    }

    public void Jump(float jump)
    {
        if(grounded)
        {
            Debug.Log("jumping");
            this.momentum = (body.up * jump) + ((Quaternion.AngleAxis(floorAngle.x, body.right) * body.forward) * moveWithGround.y) + ((Quaternion.AngleAxis(floorAngle.y, body.forward) * body.right) * moveWithGround.x);
            Debug.Log("jumping size = " + this.momentum.magnitude);
        }                                
    }

    private  void ApplyGravity()
    {
        

        cc.Move((this.momentum) * Time.deltaTime);
        
        this.momentum += (gravity * Time.deltaTime);
        
    }

    private void ApplyMagnoBoots()
    {
        if(magnoboots)
        {
            var raycastHit = new RaycastHit();

            var direction =  FindClosestGround(bootRange * 2);

            if(FindGroundInDirection(direction, raycastHit) < bootRange * 2)
            {
                Debug.Log("looking for floor angle");

                if (FindFloorAngle(direction, bootRange))
                {
                    //roate buy floor angle
                    var unrotatatedVector = body.up * bootStrength;
                    Debug.Log("angle x =  " + floorAngle.x + " agnle y = " + floorAngle.y);
                    var roatatedVector = Quaternion.AngleAxis(floorAngle.x, body.forward) * unrotatatedVector;
                    roatatedVector = Quaternion.AngleAxis(floorAngle.y, body.right) * roatatedVector;
                    Debug.DrawLine(body.position, body.position + (groundDown * 2), Color.magenta);

                    gravity += groundDown * bootStrength;
                }
            }
        }
    }

    private Vector3 FindClosestGround(float range)
    {
        Vector3 cloeset = -body.up;
        var shortestDistance = float.MaxValue;

        for(int i = 45; i < 360; i+= 45 )
        {
            for(int j = 0; j < 180; j+= 45)
            {
                var testDirection = Quaternion.AngleAxis(i, body.forward) * -body.up;
                testDirection = Quaternion.AngleAxis(j, body.right) * testDirection;
                var distnace = FindGroundInDirection(testDirection, new RaycastHit());
               
                if(distnace < shortestDistance)
                {
                    shortestDistance = distnace;

                    cloeset = testDirection;
                }

            }
        }
        

        return cloeset;
    }


    private void FindSize()
    {
        Vector3 objectSize = Vector3.Scale(body.localScale, body.gameObject.GetComponent<Collider>().bounds.size);
        size = objectSize;
    }

   

    private float FindGroundInDirection(Vector3 direction, RaycastHit hit)
    {
        //Find Direct from face

        return FindGroundPoint(direction, ref hit);

    }

    private float FindGroundPoint(Vector3 direction, ref RaycastHit raycastHit)
    {
        var sizeAcrossBody = cc.bounds.size.y;
        float distnaceToGround = float.MaxValue;
        if (Physics.Raycast(body.position + (direction * (sizeAcrossBody / 2)), direction, out raycastHit, sizeAcrossBody * 3f))
        {
            distnaceToGround = raycastHit.distance;
            Debug.DrawLine(body.position + (direction * (sizeAcrossBody / 2)), raycastHit.point, Color.cyan);

        }
        else
        {
            Debug.DrawLine(body.position + (direction * (sizeAcrossBody / 2)), body.position + (direction * (sizeAcrossBody * 3f)), Color.red);
        }

        return distnaceToGround;
    }

    private bool FindFloorAngle(Vector3 direction, float maxDistnace)
    {
        var directPoint = new RaycastHit();
        var forwardPoint = new RaycastHit();
        var rightPoint = new RaycastHit();
        var roation = Quaternion.FromToRotation(body.up, direction);

        var forwardDirection = Quaternion.AngleAxis(10, roation * body.right ) * direction;
        var rightDirection = Quaternion.AngleAxis(10, roation * body.forward) * direction;
        var retVal = false;
        floorAngle = new Vector2();
        if (FindGroundPoint(direction, ref directPoint) < maxDistnace && FindGroundPoint(forwardDirection, ref forwardPoint) < maxDistnace && FindGroundPoint(rightDirection, ref rightPoint) < maxDistnace)
        {
            floorAngle.x = Vector3.SignedAngle(body.right, rightPoint.point - directPoint.point, body.forward);
            floorAngle.y = Vector3.SignedAngle(body.forward, directPoint.point - forwardPoint.point, body.right);
            groundForward = (directPoint.point - forwardPoint.point).normalized;
            groundRight = (rightPoint.point - directPoint.point).normalized;
            Debug.Log("point right = " + rightPoint.point + " front point = " + directPoint.point + " cxreating vector = " + groundRight);
            Debug.Log("point forward = " + forwardPoint.point + " front point = " + directPoint.point + " cxreating vector = " + groundForward);
            groundDown = Quaternion.AngleAxis(-90, groundRight) * groundForward;
            Debug.DrawLine(body.position, body.position + (groundForward * 5), Color.green);
            Debug.DrawLine(body.position, body.position + (groundRight * 8), Color.black);
            Debug.DrawLine(body.position, body.position + (groundRight * 8), Color.gray);

            Debug.Log("setting floor ange " + floorAngle.x + " "+ floorAngle.y);
            retVal = true;
            //rotation = Quaternion.LookRotation(directPoint.point - forwardPoint.point, directPoint.point - rightPoint.point);
        }
        else
        {
            Debug.Log("did not find");
        }
        return retVal;
    }

    private void ApplyMoveWithGround()
    {
        var oldgrounded = grounded;
        grounded = false;
         
        var groundSpotHit = new RaycastHit();
        var groundDisance = size.y;



        if (FindGroundInDirection(-body.up, groundSpotHit) < size.y)
        {
            FindFloorAngle(-body.up, size.y * 2);
            MoveOverGround();
            oldGroundSpot = groundSpotHit.point;
            
            momentum = new Vector3();
           
            grounded = true;
            Debug.Log("Gorunded");
        }
        else
        {
            if(oldGroundSpot != null && size.y * 1.5 > (oldGroundSpot - body.position).magnitude)
            {
                MoveOverGround();
              
                momentum = new Vector3();
               
                grounded = true;
                Debug.Log("grounded (just)");
            }
            else
            {
                if(oldgrounded)
                {
                    Debug.Log("leaving ground");
                    
                    momentum += ((Quaternion.AngleAxis(floorAngle.x, body.right) * body.forward) * moveWithGround.y + (Quaternion.AngleAxis(floorAngle.y, body.forward) * body.right) * moveWithGround.x);
                    jetCoolDownCountDown = 0;
                }
                

            }
            
        }
     
    }

    private void ApplyMoveInSpace()
    {
        if(jetCoolDownCountDown > jetCooldown && !grounded)
        {
            this.momentum += (((head.forward *supplies.DrawJetSupplies(defaultJetStrength * Time.deltaTime * moveWithGround.y )) + (head.right *supplies.DrawJetSupplies(defaultJetStrength * Time.deltaTime * moveWithGround.x ))) );
        }

        jetCoolDownCountDown += Time.deltaTime;
    }


    private void MoveOverGround()
    {
        var moveY = moveWithGround.y;
        var moveX = moveWithGround.x;
        if (magnoboots)
        {
            moveY = moveY / 2;
            moveX = moveX / 2;
        }
        cc.Move(body.forward * moveY * Time.deltaTime);
        cc.Move(body.right * moveX * Time.deltaTime);
        
    }

    public void  MoveWithGround(Vector2 movement)
    {
        moveWithGround = movement;
    }

    public void MoveDirection(Vector2 direction)
    {
        lookDirection = direction;
    
    }

    private void ResetGravity()
    {
        gravity = new Vector3();
    }

    private void ResetMoveGround()
    {
        moveWithGround = new Vector2();
    }
   


    private void FindFooting()
    {
      
        var move = Quaternion.FromToRotation(body.up, -gravity.normalized);

        var newForward = move * body.forward;

        var targetRot = Quaternion.LookRotation(newForward, -gravity.normalized);

        body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 3);

     
    }

    private void RotateBody()
    {
        body.transform.Rotate(Vector3.up, lookDirection.x * Time.deltaTime);
    }

    private void RotateHead()
    {
        if (lookDirection.y != 0)
        {
            headTilt += -lookDirection.y * Time.deltaTime * rotateSpeed;
            Debug.Log("head tilt = " + headTilt);
            headTilt = Mathf.Clamp(headTilt, -90, 90);
            head.localRotation = Quaternion.AngleAxis(headTilt, Vector3.right);


            //head.transform.eulerAngles = new Vector3(head.transform.eulerAngles.x, head.transform.eulerAngles.y ,floorAngle.x);

        }
    }

    private void SpinInSpace()
    {
        /*
        if (lookDirection.y != 0)
        {
            headTilt += -lookDirection.y * Time.deltaTime * rotateSpeed;
            Debug.Log("head tilt = " + headTilt);
            headTilt = Mathf.Clamp(headTilt, -90, 90);
            head.localRotation = Quaternion.AngleAxis(headTilt, Vector3.right);
        }

            var move = Quaternion.FromToRotation(footing.up, head.up);

            var newForward = move * footing.forward;

            var targetRot = Quaternion.LookRotation(newForward, head.up);

            footing.rotation = Quaternion.Lerp(footing.rotation, targetRot, Time.deltaTime * 3);
            head.localRotation = Quaternion.Euler(transform.eulerAngles.x, Mathf.Lerp(transform.eulerAngles.y, 0, Time.deltaTime * 3), transform.eulerAngles.z);
        */

        head.transform.localRotation = Quaternion.Lerp(head.transform.localRotation, Quaternion.identity, Time.deltaTime * 3);

        body.transform.Rotate(Vector3.right, -lookDirection.y * Time.deltaTime);
        body.transform.Rotate(Vector3.up, lookDirection.x * Time.deltaTime);
    }
}
