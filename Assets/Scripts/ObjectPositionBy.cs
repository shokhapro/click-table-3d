using UnityEngine;

public class ObjectPositionBy : MonoBehaviour
{
    [SerializeField] private Transform byTransform;

    private Transform _t;

    private void Awake()
    {
        _t = transform;
    }

    private void Update()
    {
        _t.position = byTransform.position;
    }
}
