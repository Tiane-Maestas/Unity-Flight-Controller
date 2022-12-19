using UnityEngine;

public class FlightController : MonoBehaviour
{
    // Key-binds
    public bool throttleUp => Input.GetKey(KeyCode.Mouse0);
    public bool throttleDown => Input.GetKey(KeyCode.Mouse1);
    public bool yawLeft => Input.GetKey(KeyCode.Q);
    public bool yawRight => Input.GetKey(KeyCode.E);
    public bool aileronLock => Input.GetKey(KeyCode.LeftShift);
    public bool boost => Input.GetKey(KeyCode.LeftControl);

    // Movement Data for pitch(x), yaw(y), and roll(z) in that order
    private Vector3 activeMovement;
    private float aileronMultUp = 1.5f;
    private float aileronMultDown = 0.5f;

    // Aircraft Info
    public AircraftType aircraftType = AircraftType.Fighter;
    private Aircraft _aircraft;

    private void Awake()
    {
        AircraftFactory aircraftFactory = new AircraftFactory(GetComponent<Rigidbody>());
        this._aircraft = aircraftFactory.CreateAircraft(this.aircraftType);
    }

    private void Update()
    {
        HandleInput();
        this._aircraft.Throttle(1);
        this._aircraft.Pitch(activeMovement.x);
        this._aircraft.Yaw(activeMovement.y);
        this._aircraft.Roll(activeMovement.z);
    }

    private void FixedUpdate()
    {
        this._aircraft.FixedUpdate();
    }

    void HandleInput()
    {
        // Updates inputs unless aileron lock is active
        if (!aileronLock)
        {
            activeMovement = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));
        }

        // Controlls yaw rotation and makes sure they cancel if both are active
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

        // Allows extra pitch and roll by locking forward aileron
        if (aileronLock)
        {
            float tmpPitch = Input.GetAxisRaw("Vertical");
            float tmpRoll = -Input.GetAxisRaw("Horizontal");
            // Handles Pitch up acc/dcc
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
}