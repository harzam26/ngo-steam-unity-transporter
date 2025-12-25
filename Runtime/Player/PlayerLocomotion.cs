using UnityEngine;
using Unity.Netcode;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerLocomotion : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _maxSpeed = 6f;
        [SerializeField] private float _acceleration = 15f; // Hızlanma ivmesi
        [SerializeField] private float _deceleration = 10f; // Durma ivmesi (Sürtünme)
        [SerializeField] private float _turnSpeed = 12f;

        private Rigidbody _rb;
        private Vector2 _inputVector;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
            // Rigidbody Ayarları
            _rb.mass = 70f; 
            _rb.linearDamping = 0f;
            _rb.angularDamping = 0.5f;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void SetInput(Vector2 input)
        {
            _inputVector = input;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            MoveCharacter();
            RotateCharacter();
        }

        private void MoveCharacter()
        {
            Vector3 targetVel = new Vector3(_inputVector.x, 0, _inputVector.y).normalized * _maxSpeed;
            Vector3 currentVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            float accelRate = (_inputVector.magnitude > 0.01f) ? _acceleration : _deceleration;
            
            Vector3 forceDir = targetVel - currentVel;
            forceDir.y = 0;
            
            _rb.AddForce(forceDir * (accelRate * _rb.mass), ForceMode.Force);
        }

        private void RotateCharacter()
        {
            if (_inputVector.sqrMagnitude > 0.01f)
            {
                Vector3 direction = new Vector3(_inputVector.x, 0, _inputVector.y);
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
            }
        }
    }
}