using UnityEngine;

/// <summary>
/// Holds metadata for a city node and ensures a trigger collider with the given radius.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class CityNodeData : MonoBehaviour
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private string cityId = string.Empty;

    private SphereCollider sphere;

    private void Awake()
    {
        ApplyRadius();
    }

    private void OnValidate()
    {
        ApplyRadius();
    }

    private void ApplyRadius()
    {
        if (!sphere)
        {
            sphere = GetComponent<SphereCollider>();
        }
        sphere.isTrigger = true;
        sphere.radius = radius;
    }

    /// <summary>
    /// Identifier for the city represented by this node.
    /// </summary>
    public string CityId => cityId;
}
