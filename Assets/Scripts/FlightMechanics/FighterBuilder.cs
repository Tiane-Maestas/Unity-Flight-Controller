using UnityEngine;

public class FighterBuilder : AircraftBuilder
{
    #region Wing Creation Info

    #region Stalling Percentages

    #endregion

    #endregion

    public FighterBuilder()
    {
        this._parts = PartManager.Instance.GetFighterParts();
    }

    public override void AttachParts(Rigidbody aircraftBody)
    {
        this._aircraft = new Fighter(aircraftBody);
        // this._aircraft.AttachEngine();
        // this._aircraft.AttachWing();
    }

    public override Aircraft Build()
    {
        throw new System.NotImplementedException();
    }
}