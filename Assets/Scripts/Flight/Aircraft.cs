using UnityEngine;
using System.Collections.Generic;

public abstract class Aircraft
{
    protected Rigidbody _rigidBody;
    protected List<AircraftEngine> _engines;
    protected List<AircraftWing> _wings;
}

public struct AircraftEngine
{
    public float mass;
    public float power;
    public float acceleration;
    public float deceleration;
}

public struct AircraftWing
{
    public float mass;
    public float size;
    public LiftCurve liftCurve; // Test to see how velocity values greater than 1 work.
}

// This just wraps an animation curve to a "lift curve" so I can the editor functionality.
public class LiftCurve
{
    AnimationCurve animationCurveToLiftCurve;

    public LiftCurve(AnimationCurve animationCurve)
    {
        this.animationCurveToLiftCurve = animationCurve;
    }
    public float Evaluate(float velocity)
    {
        return this.animationCurveToLiftCurve.Evaluate(velocity);
    }
}