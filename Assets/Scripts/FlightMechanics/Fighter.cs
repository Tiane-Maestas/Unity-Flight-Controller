using UnityEngine;

public class Fighter : Aircraft
{
    public Fighter(Rigidbody rigidBody) : base(rigidBody) { }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 rotation = new Vector3(_rotationSpeed.x * Time.fixedDeltaTime,
                                       _rotationSpeed.y * Time.fixedDeltaTime,
                                       _rotationSpeed.z * Time.fixedDeltaTime);
        this._aircraftBody.transform.Rotate(rotation);
    }

    public override void Throttle(float input)
    {
        this._engines[0].throttle = input;
    }

    // Member Variables for pitch, yaw, and roll.
    private float _smoothRotation = 2.5f;
    private Vector3 _rotationSpeed = new Vector3();
    private Vector3 _movementStrengths = new Vector3(90f, 25f, 100f);
    public override void Pitch(float input)
    {
        _rotationSpeed.x = Mathf.Lerp(_rotationSpeed.x, input * _movementStrengths.x, Time.deltaTime * _smoothRotation);
    }
    public override void Yaw(float input)
    {
        _rotationSpeed.y = Mathf.Lerp(_rotationSpeed.y, input * _movementStrengths.y, Time.deltaTime * _smoothRotation);
    }

    public override void Roll(float input)
    {
        _rotationSpeed.z = Mathf.Lerp(_rotationSpeed.z, input * _movementStrengths.z, Time.deltaTime * _smoothRotation);
    }
}