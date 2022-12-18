using UnityEngine;
using System;

public enum AircraftType { Fighter };

public class AircraftFactory
{
    private AircraftBuilder[] aircraftBuilders = new AircraftBuilder[Enum.GetNames(typeof(AircraftType)).Length];

    public AircraftFactory()
    {
        aircraftBuilders[(int)AircraftType.Fighter] = new FighterBuilder();
    }

    public Aircraft CreateAircraft(AircraftType aircraft, Rigidbody aircraftBody)
    {
        switch (aircraft)
        {
            case AircraftType.Fighter:
                return Fighter(aircraftBody);
            default:
                return null;
        }
    }

    private Aircraft Fighter(Rigidbody aircraftBody)
    {
        aircraftBuilders[(int)AircraftType.Fighter].AttachParts(aircraftBody);
        return aircraftBuilders[(int)AircraftType.Fighter].Build();
    }
}