using UnityEngine;
using Nebula;
using System.Collections.Generic;

public abstract class Aircraft
{
    // Components
    protected Rigidbody _rigidBody;
    protected List<AircraftEngine> _engines;
    protected List<AircraftWing> _wings;

    // Attributes
    protected Vector3 _thrust = new Vector3();
    protected Vector3 _lift = new Vector3();

    public Aircraft(Rigidbody rigidBody, List<AircraftEngine> engines, List<AircraftWing> wings)
    {
        this._rigidBody = rigidBody;
        this._engines = engines;
        this._wings = wings;
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
        this._thrust = this._rigidBody.transform.forward * thrustMagnitude;
        this._rigidBody.AddForce(this._thrust);
    }

    protected virtual void AddLift()
    {
        Vector3 forward = this._rigidBody.transform.forward;
        Vector3 towardsForwardHorizon = new Vector3(forward.x, 0, forward.z);
        foreach (AircraftWing wing in this._wings)
        {
            Vector3 wingWorldOrientation = Utils.RotateVector3ByDeg(forward, wing.orientation);
            float angleOfAttack = Vector3.SignedAngle(towardsForwardHorizon, wingWorldOrientation, this._rigidBody.transform.right);
        }
        // this._lift = this._rigidBody.transform.up * liftMagnitude;
    }
}

public class AircraftEngine
{
    public float mass;
    public float power;
    [RangeAttribute(0, 1)] public float throttle = 0;
    public AircraftEngine(float mass, float power)
    {
        this.mass = mass;
        this.power = power;
    }

    public void adjustThrottle(float newThrottle)
    {
        this.throttle = newThrottle;
    }
}

public struct AircraftWing
{
    public float mass;
    public float size;
    public LiftCurve liftCurve; // Test to see how velocity values greater than 1 work.


    // JUST MAKE THIS A RUDDER OR FLAT WING
    public Vector3 orientation; // This is how you angle wings from the defualt flat wing position.
                                // Note: Rotate from forward position in degrees. Think from right, flat wing. 
                                // (Ex. Rudder -> (90, 0, 0) i.e around (forward, up, right))

    public AircraftWing(float mass, float size, LiftCurve liftCurve, Vector3 orientation)
    {
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