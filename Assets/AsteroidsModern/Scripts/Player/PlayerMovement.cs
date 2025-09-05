using AsteroidsModern.Core;
using UnityEngine;

namespace AsteroidsModern.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private Animator anim;
        
        private float _moveSpeed;
        private float _rotationSpeed;
        private bool _canMove;

        private void Awake()
        {
            rigid.gravityScale = 0f;
            rigid.linearDamping = 1f;
        }

        private void OnEnable()
        {
            Stop();
            _canMove = true;
        }

        private void Update()
        {
            if(_canMove)
            {
                HandleInput();
            }
        }

        private void OnDisable()
        {
            _canMove = false;
            Stop();
        }
        
        internal void Initialize(GameSettings settings)
        {
            transform.position = Vector3.zero;
            _moveSpeed = settings.playerMoveSpeed;
            _rotationSpeed = settings.playerRotationSpeed;
        }
        
        private void HandleInput()
        {
            bool isMoving = false;
            float rotationInput = Input.GetAxis("Horizontal");
            if (rotationInput != 0)
            {
                float rotationAmount = -rotationInput * _rotationSpeed * Time.deltaTime;
                transform.Rotate(0, 0, rotationAmount);
                isMoving = true;
            }
            
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Vector2 thrustDirection = transform.up;
                Move(thrustDirection, _moveSpeed);
                isMoving = true;
            }
            
            anim.SetBool(IsMoving, isMoving);
        }

        private void Move(Vector2 direction, float speed)
        {
            Vector2 force = direction.normalized * speed;
            rigid.AddForce(force, ForceMode2D.Force);
            
            if (rigid.linearVelocity.magnitude > speed)
            {
                rigid.linearVelocity = rigid.linearVelocity.normalized * speed;
            }
        }

        private void Stop()
        {
            rigid.linearVelocity = Vector2.zero;
            rigid.angularVelocity = 0f;
            rigid.totalForce = Vector2.zero;
        }
    }
}