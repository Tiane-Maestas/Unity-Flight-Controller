using UnityEngine;

public class FighterBuilder : AircraftBuilder
{
    #region Wing Creation Info

    #region Stalling Percentages

    #endregion

    #endregion

    public FighterBuilder(Rigidbody aircraftBody) : base(aircraftBody)
    {
        this._parts = PartManager.Instance.GetFighterParts();
    }

    public override void AttachParts()
    {
        // Always construct the aircraft body before the rest of the parts.
        this._aircraft = new Fighter(this._aircraftBody);

        base.AttachParts();
    }

    public override Aircraft Build()
    {
        return this._aircraft;
    }
}