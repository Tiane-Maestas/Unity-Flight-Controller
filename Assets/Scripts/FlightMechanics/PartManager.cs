using UnityEngine;
using Nebula;
using System;
using System.Collections.Generic;

// This class is what an AircraftBuilder uses to find and attach parts for an AircraftType
public class PartManager : Singleton<PartManager>
{
    // For creating other plane types I labeled the steps below 1-5.
    private void Awake()
    {
        WriteFighterPartsList(); // 1
    }

    // 2
    #region Fighter 

    List<AircraftPart> fighterParts = new List<AircraftPart>(); // 3

    [Header("Fighter"), Space, Header("Engine Info")]
    [SerializeField] private AircraftEngine _fighterEngine;

    [Space, Header("Wing Info")]
    [SerializeField] private AircraftWing _fighterForwardWing;

    [Space]
    [SerializeField] private AircraftWing _fighterRearWing;

    [Space]
    [SerializeField] private AircraftWing _fighterRudder;

    public void WriteFighterPartsList() // 4
    {
        fighterParts.Add(_fighterEngine);
        fighterParts.Add(_fighterForwardWing);
        fighterParts.Add(_fighterRearWing);
        fighterParts.Add(_fighterRudder);
    }

    public List<AircraftPart> GetFighterParts() // 5
    {
        return fighterParts;
    }

    #endregion
}

// This is the general AircraftPart all specific parts should inherit from.
[Serializable]
public abstract class AircraftPart
{
    public string id;
}

#region Engine
[Serializable]
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
#endregion

#region Wing
public enum WingOrientation { Normal, Rudder };

[Serializable]
public class AircraftWing : AircraftPart
{
    public float mass;
    public float size;
    public LiftCurve liftCurve; // Test to see how velocity values greater than 1 work.
    public WingOrientation orientation;

    public AircraftWing(float mass, float size, LiftCurve liftCurve, WingOrientation orientation)
    {
        this.id = "AircraftWing";
        this.mass = mass;
        this.size = size;
        this.liftCurve = liftCurve;
        this.orientation = orientation;
    }
}

// This just wraps an animation curve to a "lift curve" so I can the editor functionality.
[Serializable]
public struct LiftCurve
{
    [SerializeField] private AnimationCurve animationCurveToLiftCurve;

    public LiftCurve(AnimationCurve animationCurve)
    {
        this.animationCurveToLiftCurve = animationCurve;
    }
    public float Evaluate(float angleOfAttack)
    {
        return this.animationCurveToLiftCurve.Evaluate(angleOfAttack);
    }
}
#endregion