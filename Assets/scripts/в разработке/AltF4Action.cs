using UnityEngine;

public class AltF4Action : MonoBehaviour
{
    public GameObject obj;

    void OnEnable()
    {
        AltF4Blocker.OnAltF4 += ActivateObject;
    }

    void OnDisable()
    {
        AltF4Blocker.OnAltF4 -= ActivateObject;
    }

    private void ActivateObject()
    {
        obj.SetActive(true);
        Debug.Log("Alt+F4 нажато — объект включён!");
    }
}
