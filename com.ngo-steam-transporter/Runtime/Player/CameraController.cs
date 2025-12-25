using Player;
using Unity.Cinemachine;
using UnityEngine;
using Utils;

namespace Player
{
    public class CameraController : Singleton<CameraController>
    {
        private PlayerController _playerController;
        private Transform _target;
        [SerializeField] private CinemachineCamera _vCam;

        private void Start()
        {
            if (!_vCam)
            {
                _vCam = FindAnyObjectByType<CinemachineCamera>();
            }
        }

        public void SetPlayerController(PlayerController playerController)
        {
            _playerController = playerController;
            _target = playerController.transform;
            _vCam.Target.TrackingTarget = _target;
            _vCam.OnTargetObjectWarped(_target, _target.position - _vCam.transform.position);
        }
        
        
    }
}