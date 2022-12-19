using UnityEngine;
using System.Collections.Generic;

public abstract class AircraftBuilder
{
    protected Aircraft _aircraft;
    protected Rigidbody _aircraftBody;
    protected List<AircraftPart> _parts;

    public AircraftBuilder(Rigidbody aircraftBody)
    {
        this._aircraftBody = aircraftBody;
    }
    public virtual void AttachParts()
    {
        foreach (AircraftPart part in this._parts)
        {
            if (part.id.Equals("AircraftEngine"))
                this._aircraft.AttachEngine((AircraftEngine)part);

            if (part.id.Equals("AircraftWing"))
                this._aircraft.AttachWing((AircraftWing)part);
        }
    }
    public abstract Aircraft Build();
}