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
        this._engines.Add(engine);
    }

    public void AttachWing(AircraftWing wing)
    {
        this._wings.Add(wing);
    }

    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {
        this.AddThrust();
        this.AddLift();
    }

    // Note: This simply adds thrust in the forward direction. No torques i.e. The position of the engine plays no role.
    protected virtual void AddThrust()
    {
        float thrustMagnitude = 0;

        foreach (AircraftEngine engine in this._engines)
        {
            thrustMagnitude += engine.power * engine.throttle * Time.fixedDeltaTime;
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
            if (wing.orientation != AircraftWing.WingOrientation.Normal)
                continue;

            float angleOfAttack = Vector3.SignedAngle(towardsForwardHorizon, forward, this._aircraftBody.transform.right);
            float curveModifier = wing.liftCurve.Evaluate(angleOfAttack);
            liftMagnitude += wing.size * this._aircraftBody.velocity.magnitude * curveModifier; // Add Air density modifier here.
        }

        this._lift = (liftMagnitude < Physics.gravity.magnitude) ? this._aircraftBody.transform.up * liftMagnitude : this._aircraftBody.transform.up * Physics.gravity.magnitude;
        this._aircraftBody.AddForce(this._lift);
    }
}

public class AircraftEngine : AircraftPart
{
    public float mass;
    public float power;
    [RangeAttribute(0, 1)] public float throttle = 0;
    public AircraftEngine(float mass, float power)
    {
        this.id = "AircraftEngine";
        this.mass = mass;
        this.power = power;
    }

    public void adjustThrottle(float newThrottle)
    {
        this.throttle = newThrottle;
    }
}

public class AircraftWing : AircraftPart
{
    public float mass;
    public float size;
    public LiftCurve liftCurve; // Test to see how velocity values greater than 1 work.
    public enum WingOrientation { Normal, Rudder };
    public WingOrientation orientation;

    public AircraftWing(float mass, float size, LiftCurve liftCurve, WingOrientation orientation)
    {
        this.id = "AircraftWing" + orientation;
        this.mass = mass;
        this.size = size;
        this.liftCurve = liftCurve;
        this.orientation = orientation;
    }
}

// This just wraps an animation curve to a "lift curve" so I can the editor functionality.
public class LiftCurve
{
    AnimationCurve animationCurveToLiftCurve;

    public LiftCurve(AnimationCurve animationCurve)
    {
        this.animationCurveToLiftCurve = animationCurve;
    }
    public float Evaluate(float angleOfAttack)
    {
        return this.animationCurveToLiftCurve.Evaluate(angleOfAttack);
    }
}

/* Developer Notes:
    Neither engines or wings take into acount physical positions in their force calcualtions. This
    means that a single engine at the end of a wing won't cause any torques around the center of mass
    and a single wing on the right and pointed directly up won't change the flight behaviour. The way 
    I am modeling these aircrafts is for only semi-realistic behaviour. It would be easier to implement 
    positional dependence if an aircraft was made of multiple game objects I assume.
*/