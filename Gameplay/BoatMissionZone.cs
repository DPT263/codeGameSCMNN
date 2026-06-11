using UnityEngine;

public class BoatMissionZone : MonoBehaviour
{
    public enum ZoneKind
    {
        Pickup,
        Dropoff
    }

    [Header("Zone")]
    [SerializeField] private ZoneKind zoneKind = ZoneKind.Pickup;

    [Header("References")]
    [SerializeField] private Transform boatTarget;
    [SerializeField] private BoatMissionUI missionUI;

    [Header("Message")]
    [SerializeField] private string messageWhenInside = "Nhấn E để đón khách";
    [SerializeField] private bool hideMessageOnExit = true;

    public bool BoatInside { get; private set; }

    public ZoneKind Kind
    {
        get { return zoneKind; }
    }

    private void Reset()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Start()
    {
        if (missionUI == null)
        {
            missionUI = FindObjectOfType<BoatMissionUI>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsBoat(other))
        {
            return;
        }

        BoatInside = true;

        if (missionUI != null)
        {
            missionUI.ShowMessage(messageWhenInside);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsBoat(other))
        {
            return;
        }

        BoatInside = false;

        if (hideMessageOnExit && missionUI != null)
        {
            missionUI.HideMessage();
        }
    }

    private bool IsBoat(Collider other)
    {
        if (boatTarget == null)
        {
            return false;
        }

        Transform hitRoot = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform.root;

        if (hitRoot == boatTarget)
        {
            return true;
        }

        if (other.transform.IsChildOf(boatTarget))
        {
            return true;
        }

        return false;
    }

    private void OnDisable()
    {
        BoatInside = false;

        if (hideMessageOnExit && missionUI != null)
        {
            missionUI.HideMessage();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = zoneKind == ZoneKind.Pickup
            ? Color.green
            : Color.cyan;

        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}