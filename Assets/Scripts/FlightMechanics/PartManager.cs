using Nebula;
using System.Collections.Generic;

public class PartManager : Singleton<PartManager>
{
    public List<AircraftPart> GetFighterParts()
    {
        List<AircraftPart> fighterParts = new List<AircraftPart>();
        return fighterParts;
    }
}

public abstract class AircraftPart
{
    public string id;
}
