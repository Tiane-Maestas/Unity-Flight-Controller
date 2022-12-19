using UnityEngine;
using Nebula;
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
    [SerializeField] private int _fighterEngineMass = 500;
    [SerializeField] private int _fighterEnginePower = 10000;

    [Space, Header("Wing Info")]
    [SerializeField] private float _fighterForwardWingMass = 500;
    [SerializeField] private float _fighterForwardWingSize = 100;
    [SerializeField] private WingOrientation _fighterForwardWingOrientation = WingOrientation.Normal;
    [SerializeField] private AnimationCurve _fighterForwardWingLiftCurve;
    [Space]
    [SerializeField] private float _fighterRearWingMass = 200;
    [SerializeField] private float _fighterRearWingSize = 50;
    [SerializeField] private WingOrientation _fighterRearWingOrientation = WingOrientation.Normal;
    [SerializeField] private AnimationCurve _fighterRearWingLiftCurve;
    [Space]
    [SerializeField] private float _fighterRudderWingMass = 100;
    [SerializeField] private float _fighterRudderWingSize = 25;
    [SerializeField] private WingOrientation _fighterRudderWingOrientation = WingOrientation.Rudder;
    [SerializeField] private AnimationCurve _fighterRudderWingLiftCurve;

    public void WriteFighterPartsList() // 4
    {
        fighterParts.Add(new AircraftEngine(_fighterEngineMass, _fighterEnginePower));
        fighterParts.Add(new AircraftWing(_fighterForwardWingMass, _fighterForwardWingSize, new LiftCurve(_fighterForwardWingLiftCurve), _fighterForwardWingOrientation));
        fighterParts.Add(new AircraftWing(_fighterRearWingMass, _fighterRearWingSize, new LiftCurve(_fighterRearWingLiftCurve), _fighterRearWingOrientation));
        fighterParts.Add(new AircraftWing(_fighterRudderWingMass, _fighterRudderWingSize, new LiftCurve(_fighterRudderWingLiftCurve), _fighterRudderWingOrientation));
    }

    public List<AircraftPart> GetFighterParts() // 5
    {
        return fighterParts;
    }

    #endregion
}

// This is the general AircraftPart all specific parts should inherit from.
public abstract class AircraftPart
{
    public string id;
}

#region Engine
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
public struct LiftCurve
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
#endregion