using UnityEngine;

public class FlightController : MonoBehaviour
{
    //key-binds
    public bool throttleUp => Input.GetKey(KeyCode.Mouse0);
    public bool throttleDown => Input.GetKey(KeyCode.Mouse1);
    public bool yawLeft => Input.GetKey(KeyCode.Q);
    public bool yawRight => Input.GetKey(KeyCode.E);
    public bool aileronLock => Input.GetKey(KeyCode.LeftShift);
    public bool boost => Input.GetKey(KeyCode.LeftControl);

    //Engine parameters
    public float enginePower;
    private float activeThrottle;
    private float groundSpeed;
    private float prevGroundSpeed;
    private float boostMult = 50;
    private float maxSpeed;
    private float maxSpeedMult = 3f;
    private float engineAcc;
    private float accMult = 0.025f;
    private float engineDcc;
    private float dccMult = 0.01f;

    //Data of pitch(x), yaw(y), and roll(z) in that order
    public Vector3 movementStrengths;
    private Vector3 activeMovement;
    private float aileronMultUp = 1.5f;
    private float aileronMultDown = 0.5f;
    private Vector3 rotationSpeed;
    public float rotSmooth;

    //Data used for lift
    Rigidbody rb;
    private Vector3 lift;
    private float fGravMag;
    private float mainWingSize;
    private float mainWingPercentage = 0.3f;
    private float rearWingSize;
    private float rearWingPercentage = 0.2f;
    private float rudderSize;
    private float rudderPercentage = 0.5f;
    private float engineSize;
    private float enginePercentage = 0.5f;
    private float addedVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //initializes flight information
        maxSpeed = enginePower * maxSpeedMult;
        engineAcc = enginePower * accMult;
        engineDcc = enginePower * dccMult;
        rotationSpeed = new Vector3(0, 0, 0);
        prevGroundSpeed = 1f;
        //initializes lift information
        rb = GetComponent<Rigidbody>();
        fGravMag = Physics.gravity.magnitude * rb.mass;
        calculateLiftSizes();
    }

    //Calculates "sizes" of wings to fit stalling percentages
    void calculateLiftSizes()
    {
        mainWingSize = fGravMag * (2 / (maxSpeed * mainWingPercentage));
        rearWingSize = (fGravMag - mainWingForce(maxSpeed * rearWingPercentage)) * (2 / (maxSpeed * rearWingPercentage));
        rudderSize = fGravMag * (2 / (maxSpeed * rudderPercentage));
        //Maybe make engine power factor in somehow but this is good for now.
        engineSize = fGravMag * (2 / (maxSpeed * enginePercentage));
    }

    //Engine Lift
    float engineForce(float velocity)
    {
        return 0.5f * engineSize * velocity;
    }

    //Main Wing Lift
    float mainWingForce(float velocity)
    {
        return 0.5f * mainWingSize * velocity;
    }

    //Rear wing force only active if aileron is engaged
    float rearWingForce(float velocity)
    {
        if (aileronLock)
        {
            float tmpPitch = Input.GetAxisRaw("Vertical");
            return 0.5f * rearWingSize * velocity * tmpPitch;
        }
        return 0;
    }

    //Verticle Rudder Force
    float rudderForce(float velocity)
    {
        return 0.5f * rudderSize * velocity;
    }

    //Update for physics calls on fixed time interval
    void FixedUpdate()
    {
        HandleMovement();
        HandleThrottle();
        HandleLift();
    }

    public void HandleLift()
    {
        //Calculates forces with projections based on plane orientation and strengths based on my formulas.
        Vector3 mainLift = Vector3.Scale(transform.up, Vector3.up) * mainWingForce(groundSpeed);
        Vector3 rearLift = Vector3.Scale(transform.up, Vector3.up) * rearWingForce(groundSpeed);
        Vector3 rudderLift = Vector3.Scale(transform.right, Vector3.up) * rudderForce(groundSpeed);
        Vector3 engineLift = Vector3.Scale(transform.forward, Vector3.up) * engineForce(groundSpeed);
        //Lift always acts against gravity
        mainLift.y = Mathf.Abs(mainLift.y);
        rearLift.y = Mathf.Abs(rearLift.y);
        rudderLift.y = Mathf.Abs(rudderLift.y);
        engineLift.y = Mathf.Abs(engineLift.y);

        lift = mainLift + rearLift + rudderLift + engineLift;

        if (lift.y >= fGravMag)
        {
            //doesn't allow lift to be greater than gravity
            lift.y = fGravMag;
            //Smooths the transition from stall to flight
            rb.velocity = new Vector3(0, Mathf.Lerp(rb.velocity.y, 0, Time.deltaTime), 0);
            groundSpeed = Mathf.Lerp(groundSpeed, groundSpeed - addedVelocity, Time.deltaTime);
            addedVelocity -= addedVelocity * Time.deltaTime;
        }
        else
        {
            //Rotates the nose of the plane downward when lift isn't sufficient. Done by feel justifying the magic number.
            rotationSpeed += new Vector3(transform.up.y, transform.up.x, transform.up.z) * -rb.velocity.y * 0.25f;
            //Adds some speed tp gound speed if the nose is down to allow for stall recovery without throttle up. Also
            //done by feel justifying the magic number.
            float velocityToAdd = Mathf.Abs(transform.forward.y) * -rb.velocity.y * 0.025f;
            if (transform.forward.y < -0.25)
            {
                groundSpeed += velocityToAdd;
                addedVelocity += velocityToAdd;
            }
        }

        //Adds the lift force to the rigidbody but only really shows when at 
        //a stalling speed based on orientation of the aircraft.
        rb.AddForce(lift);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    //Controls all aspects of throttle up and down
    void HandleThrottle()
    {
        //Increases and decreases throttle.
        if (throttleUp && groundSpeed <= maxSpeed)
        {
            groundSpeed += engineAcc;
        }
        else if (throttleDown && groundSpeed > 0)
        {
            groundSpeed -= engineDcc;
        }

        if (boost)
        {
            groundSpeed = prevGroundSpeed + engineAcc * boostMult;
        }
        else
        {
            if (groundSpeed > prevGroundSpeed + 1)
            {
                groundSpeed = prevGroundSpeed;
            }
            else
            {
                prevGroundSpeed = groundSpeed;
            }
        }
    }

    void HandleInput()
    {
        //updates inputs unless aileron lock is active
        if (!aileronLock)
        {
            activeMovement = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));
        }

        //Controlls yaw rotation and makes sure they cancel if both are active
        if (yawLeft)
        {
            activeMovement.y = -1f;
            if (yawRight)
            {
                activeMovement.y = 0f;
            }
        }
        else if (yawRight)
        {
            activeMovement.y = 1f;
        }
        else
        {
            activeMovement.y = 0f;
        }

        //allows modification of rotation speed of pitch and roll by locking forward aileron
        if (aileronLock)
        {
            float tmpPitch = Input.GetAxisRaw("Vertical");
            float tmpRoll = -Input.GetAxisRaw("Horizontal");
            //Handles Pitch up acc/dcc
            if (activeMovement.x > 0)
            {
                if (tmpPitch == 1 && activeMovement.x < aileronMultUp)
                {
                    activeMovement.x *= aileronMultUp;
                }
                else if (tmpPitch == -1 && activeMovement.x > aileronMultDown)
                {
                    activeMovement.x *= aileronMultDown;
                }
            }
            //Handles Pitch down acc/dcc
            if (activeMovement.x < 0)
            {
                if (tmpPitch == -1 && activeMovement.x > -aileronMultUp)
                {
                    activeMovement.x *= aileronMultUp;
                }
                else if (tmpPitch == 1 && activeMovement.x < -aileronMultDown)
                {
                    activeMovement.x *= aileronMultDown;
                }
            }
            //Handles Roll left acc/dcc
            if (activeMovement.z > 0)
            {
                if (tmpRoll == 1 && activeMovement.z < aileronMultUp)
                {
                    activeMovement.z *= aileronMultUp;
                }
                else if (tmpRoll == -1 && activeMovement.z > aileronMultDown)
                {
                    activeMovement.z *= aileronMultDown;
                }
            }
            //Handles Roll right acc/dcc
            if (activeMovement.z < 0)
            {
                if (tmpRoll == -1 && activeMovement.z > -aileronMultUp)
                {
                    activeMovement.z *= aileronMultUp;
                }
                else if (tmpRoll == 1 && activeMovement.z < -aileronMultDown)
                {
                    activeMovement.z *= aileronMultDown;
                }
            }
            //Sets pitch and roll to origonal value (pre-lock)
            if (tmpPitch == 0)
            {
                if (activeMovement.x < 0)
                {
                    activeMovement.x = -1f;
                }
                else if (activeMovement.x > 0)
                {
                    activeMovement.x = 1f;
                }
            }
            if (tmpRoll == 0)
            {
                if (activeMovement.z < 0)
                {
                    activeMovement.z = -1f;
                }
                else if (activeMovement.z > 0)
                {
                    activeMovement.z = 1f;
                }
            }
        }
    }

    //Controlls the orientation of the plane
    void HandleMovement()
    {
        //Adds units of position to the position vector.
        transform.position += transform.forward * groundSpeed * Time.deltaTime;
        //Controlls pitch, yaw, and roll rotations
        rotationSpeed = new Vector3(Mathf.Lerp(rotationSpeed.x, activeMovement.x * movementStrengths.x, Time.deltaTime * rotSmooth),
                                    Mathf.Lerp(rotationSpeed.y, activeMovement.y * movementStrengths.y, Time.deltaTime * rotSmooth),
                                    Mathf.Lerp(rotationSpeed.z, activeMovement.z * movementStrengths.z, Time.deltaTime * rotSmooth));
        Vector3 rotation = new Vector3(rotationSpeed.x * Time.deltaTime,
                                       rotationSpeed.y * Time.deltaTime,
                                       rotationSpeed.z * Time.deltaTime);
        transform.Rotate(rotation);
    }
}