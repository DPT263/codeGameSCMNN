using UnityEngine;

public class BoatSlowZone : MonoBehaviour
{
    [Header("Slow Settings")]
    [SerializeField] private float slowMultiplier = 0.75f;
    [SerializeField] private float restoreDelay = 0.25f;

    private BoatController currentBoat;
    private float lastBoatTouchTime = -999f;
    private bool isSlowingBoat;

    private void Reset()
    {
        Collider col = GetComponent<Collider>(); 

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        if (!isSlowingBoat || currentBoat == null)
        {
            return;
        }

        bool stillInZone = Time.time - lastBoatTouchTime <= restoreDelay;

        if (!stillInZone)
        {
            currentBoat.ClearEnvironmentSpeedMultiplier();
            currentBoat = null;
            isSlowingBoat = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplySlow(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryApplySlow(other);
    }

    private void TryApplySlow(Collider other)
    {
        BoatController boat = other.GetComponentInParent<BoatController>();

        if (boat == null)
        {
            return;
        }

        currentBoat = boat;
        lastBoatTouchTime = Time.time;
        isSlowingBoat = true;

        currentBoat.SetEnvironmentSpeedMultiplier(slowMultiplier);
    }

    private void OnDisable()
    {
        if (currentBoat != null)
        {
            currentBoat.ClearEnvironmentSpeedMultiplier();
        }

        currentBoat = null;
        isSlowingBoat = false;
    }
}