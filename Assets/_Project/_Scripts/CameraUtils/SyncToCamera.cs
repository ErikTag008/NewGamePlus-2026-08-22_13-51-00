using Unity.Cinemachine;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{
    public class SyncToCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
        }

        private void OnDisable()
        {
            CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
        }
        private void OnCameraUpdated(CinemachineBrain brain)
        {
            if (brain.OutputCamera == null) return;
            transform.rotation = brain.OutputCamera.transform.rotation;
        }
    }
}
