using UnityEngine;

public class AltF4Toggle : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F4))
        {
            if (targetObject != null)
            {
                targetObject.SetActive(!targetObject.activeSelf);
            }
        }
    }
}