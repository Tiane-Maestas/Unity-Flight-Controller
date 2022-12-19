using UnityEngine;
using System.Collections.Generic;

public class Fighter : Aircraft
{
    public Fighter(Rigidbody rigidBody) : base(rigidBody)
    {
    }

    public override void Throttle(float input)
    {
        input = Mathf.Clamp(input, 0, 1);
        this._engines[0].throttle = input;
    }

    public override void Roll(float input)
    {

    }
    public override void Pitch(float input)
    {

    }
    public override void Yaw(float input)
    {

    }
}