using UnityEngine;
using System.Collections.Generic;

public abstract class AircraftBuilder
{
    protected Aircraft _aircraft;
    protected List<AircraftPart> _parts;
    public abstract void AttachParts(Rigidbody aircraftBody);
    public abstract Aircraft Build();
}