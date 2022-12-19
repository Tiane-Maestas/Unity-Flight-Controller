using UnityEngine;
using System;

public enum AircraftType { Fighter };

public class AircraftFactory
{
    private AircraftBuilder[] aircraftBuilders = new AircraftBuilder[Enum.GetNames(typeof(AircraftType)).Length];

    public AircraftFactory(Rigidbody aircraftBody)
    {
        aircraftBuilders[(int)AircraftType.Fighter] = new FighterBuilder(aircraftBody);
    }

    public Aircraft CreateAircraft(AircraftType aircraft)
    {
        switch (aircraft)
        {
            case AircraftType.Fighter:
                return Fighter();
            default:
                return null;
        }
    }

    private Aircraft Fighter()
    {
        aircraftBuilders[(int)AircraftType.Fighter].AttachParts();
        return aircraftBuilders[(int)AircraftType.Fighter].Build();
    }
}