using UnityEngine;

class FSBDWheelClass
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public Transform suspensionParent;
    public bool useRotation = false;
    public bool useSuspension = false;

    public float screechCountdown = 0f;
    public Firespitter.FSBDparticleFX smokeFX;
    public GameObject fxLocation = new GameObject();

    private float deltaRPM = 0f;
    private float oldRPM = 0f;
    public bool oldIsGrounded = true;

    public FSBDWheelClass(WheelCollider _wheelCollider, Transform _wheelMesh, Transform _suspensionParent)
    {
        wheelCollider = _wheelCollider;
        wheelMesh = _wheelMesh;
        suspensionParent = _suspensionParent;
        SetupFxLocation();
    }

    public FSBDWheelClass(WheelCollider _wheelCollider)
    {
        wheelCollider = _wheelCollider;
        useRotation = false;
        useSuspension = false;
    }

    public void SetupFxLocation()
    {
        fxLocation.transform.parent = suspensionParent;
        fxLocation.transform.localPosition = new Vector3(0f, 0, -wheelCollider.radius); //
    }

    public float GetDeltaRPM()
    {        
        deltaRPM = oldRPM - wheelCollider.rpm;
        oldRPM = wheelCollider.rpm;        
        return deltaRPM;
    }
}