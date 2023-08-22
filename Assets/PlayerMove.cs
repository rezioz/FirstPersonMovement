using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{


    [SerializeField] float MoveSmoothTime;
    [SerializeField] float GravityStrength;
    [SerializeField] float JumpStrength;
    [SerializeField] float WalkSpeed;
    [SerializeField] float RunSpeed;

    [SerializeField] float slopeForceRayLength;

    CharacterController Controller;

    Vector3 CurrentMoveVelocity;
    Vector3 MoveDampVelocity;

    Vector3 CurrentForceVelocity;


    // Start is called before the first frame update
    void Start()
    {

        Time.fixedDeltaTime = 0.002f;

        Controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {



        //mouvement qui sert juste a maintenir la collision du character controller activée en permanence (evite les bugs de teleportation quand collision avec un kinematic rigidbody)
        Controller.Move(new Vector3(0, 0.000001f, 0));
        Controller.Move(new Vector3(0, -0.000001f, 0));
        


        Movement();

        Gravity();

        CheckMovableGround();



    }


    void Movement()
    {

        Vector3 PlayerInput = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = (0),
            z = Input.GetAxisRaw("Vertical"),
        };

        if (PlayerInput.magnitude > 1)
        {
            PlayerInput.Normalize();
        }

        Vector3 MoveVector = transform.TransformDirection(PlayerInput);
        float CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : WalkSpeed;

        CurrentMoveVelocity = Vector3.SmoothDamp(
            CurrentMoveVelocity,
            MoveVector * CurrentSpeed,
            ref MoveDampVelocity,
            MoveSmoothTime
        );


        
        Controller.Move(CurrentMoveVelocity * Time.deltaTime);

        if(IsOnSlope() && PlayerInput != Vector3.zero)
        {
            Controller.Move(Vector3.down);
        }
        else
        {
            print("no slope adaptation");
        }

        

    }

    void Gravity()
    {


        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(groundCheckRay, .25f ,LayerMask.NameToLayer("Player")))
        {
            if(CurrentForceVelocity.y<0)
                CurrentForceVelocity.y = 0;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CurrentForceVelocity.y = JumpStrength;
            }
            
        }
        else
        {
            CurrentForceVelocity.y -= GravityStrength * Time.deltaTime;
        }

        Controller.Move(CurrentForceVelocity * Time.deltaTime);
        //rb.AddForce(CurrentForceVelocity);
    }

    bool IsOnGround()
    {
        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        return CurrentForceVelocity.y == 0 && (Physics.Raycast(groundCheckRay, .25f, LayerMask.NameToLayer("Player")));
    }

    bool IsOnSlope()
    {
        if (!IsOnGround())
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.up * -1, out hit, slopeForceRayLength, LayerMask.NameToLayer("Player")))
            if (hit.normal != Vector3.up)
                return true;


        return false;

    }

    void CheckMovableGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.up*-1,out hit, .25f))
        {
            if (hit.collider.gameObject.CompareTag("Movable"))
            {
                Controller.Move(hit.collider.gameObject.GetComponent<moveStuff>().movement*Time.deltaTime);
            }
        }
    }



}
