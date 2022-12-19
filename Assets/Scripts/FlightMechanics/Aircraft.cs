using UnityEngine;
using System.Collections.Generic;

public abstract class Aircraft
{
    // Components
    protected Rigidbody _aircraftBody;
    protected List<AircraftEngine> _engines;
    protected List<AircraftWing> _wings;

    // Attributes
    protected Vector3 _thrust = new Vector3();
    protected Vector3 _lift = new Vector3();

    public Aircraft(Rigidbody aircraftBody)
    {
        this._aircraftBody = aircraftBody;
        this._engines = new List<AircraftEngine>();
        this._wings = new List<AircraftWing>();
    }

    public void AttachEngine(AircraftEngine engine)
    {
        this._aircraftBody.mass += engine.mass;
        this._engines.Add(engine);
    }

    public void AttachWing(AircraftWing wing)
    {
        this._aircraftBody.mass += wing.mass;
        this._wings.Add(wing);
    }

    public virtual void FixedUpdate()
    {
        this.AddThrust();
        this.AddLift();
    }

    protected virtual void AddThrust()
    {
        float thrustMagnitude = 0;

        foreach (AircraftEngine engine in this._engines)
        {
            thrustMagnitude += engine.power * engine.throttle * Time.fixedDeltaTime;
            Debug.Log(thrustMagnitude);
        }

        this._thrust = this._aircraftBody.transform.forward * thrustMagnitude;
        this._aircraftBody.AddForce(this._thrust);
    }

    protected virtual void AddLift()
    {
        Vector3 forward = this._aircraftBody.transform.forward;
        Vector3 towardsForwardHorizon = new Vector3(forward.x, 0, forward.z);
        towardsForwardHorizon.Normalize();

        float liftMagnitude = 0;

        foreach (AircraftWing wing in this._wings)
        {
            if (wing.orientation != WingOrientation.Normal)
                continue;

            float angleOfAttack = Vector3.SignedAngle(towardsForwardHorizon, forward, this._aircraftBody.transform.right);
            float curveModifier = wing.liftCurve.Evaluate(angleOfAttack);
            liftMagnitude += wing.size * Mathf.Pow(this._aircraftBody.velocity.magnitude, 2) * curveModifier; // Add Air density modifier here.
        }

        this._lift = (liftMagnitude < Physics.gravity.magnitude) ? this._aircraftBody.transform.up * liftMagnitude : this._aircraftBody.transform.up * Physics.gravity.magnitude;
        this._aircraftBody.AddForce(this._lift);
    }

    // The input value should be a value between [-1, 1]
    public abstract void Throttle(float input);
    public abstract void Roll(float input);
    public abstract void Pitch(float input);
    public abstract void Yaw(float input);
}

/* Developer Notes:
    Neither engines or wings take into acount physical positions in their force calcualtions. This
    means that a single engine at the end of a wing won't cause any torques around the center of mass
    and a single wing on the right and pointed directly up won't change the flight behaviour. The way 
    I am modeling these aircrafts is for only semi-realistic behaviour. It would be easier to implement 
    positional dependence if an aircraft was made of multiple game objects I assume.
*/