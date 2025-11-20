using UnityEngine;

public class ToggleFollower : MonoBehaviour
{
    public GameObject target;

    private void OnEnable()
    {
        if (target != null) target.SetActive(true);
    }

    private void OnDisable()
    {
        if (target != null) target.SetActive(false);
    }
}
