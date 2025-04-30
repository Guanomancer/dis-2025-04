using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(UnityEngine.Vector3.forward, UnityEngine.Vector3.up);
    }
}

