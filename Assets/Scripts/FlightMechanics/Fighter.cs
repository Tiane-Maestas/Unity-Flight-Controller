using UnityEngine;
using System.Collections.Generic;

public class Fighter : Aircraft
{
    //Engine Information
    public int mainEngine = 0;

    // Wing Information
    public enum fighterWing { forwardWing, rearWing, rudder };

    public Fighter(Rigidbody rigidBody) : base(rigidBody)
    {
    }
}