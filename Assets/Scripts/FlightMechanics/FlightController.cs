using UnityEngine;

public class FlightController : MonoBehaviour
{
    public AircraftType aircraftType = AircraftType.Fighter;
    private Aircraft _aircraft;

    private void Awake()
    {
        AircraftFactory aircraftFactory = new AircraftFactory(GetComponent<Rigidbody>());
        this._aircraft = aircraftFactory.CreateAircraft(this.aircraftType);
    }

    private void Update()
    {
        this._aircraft.Throttle(1);
    }

    private void FixedUpdate()
    {
        this._aircraft.FixedUpdate();
    }
}