using UnityEngine;

public class FollowTransformRot : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Update()
    {
        transform.rotation = _target.rotation;
    }
}
