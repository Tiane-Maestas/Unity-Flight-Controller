using UnityEngine;

public class FlightController : MonoBehaviour
{
    public AircraftType aircraftType = AircraftType.Fighter;
    private Aircraft _aircraft;

    private void Awake()
    {
        AircraftFactory aircraftFactory = new AircraftFactory();
        this._aircraft = aircraftFactory.CreateAircraft(this.aircraftType, GetComponent<Rigidbody>());
    }
}