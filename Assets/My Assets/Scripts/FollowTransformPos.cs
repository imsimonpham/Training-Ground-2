using UnityEngine;

public class FollowTransformPos : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Update()
    {
        transform.position = _target.position;
    }
}
