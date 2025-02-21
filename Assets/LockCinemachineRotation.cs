using UnityEngine;
using Cinemachine;

public class LockCinemachineRotation : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Quaternion initialRotation;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
            initialRotation = virtualCamera.transform.rotation;
    }

    void LateUpdate()
    {
        if (virtualCamera != null)
            virtualCamera.transform.rotation = initialRotation;
    }
}