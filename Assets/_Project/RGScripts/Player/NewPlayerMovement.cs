using Unity.Mathematics;
using UnityEngine;

namespace _Project.RGScripts.Player
{
    [RequireComponent(typeof(PlayerAnimation), typeof(PlayerInput), typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class NewPlayerMovement : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] private float speed;
        [SerializeField] private float accelation;
        [SerializeField] private float deceleration;
        [SerializeField] private float jumpForce;

        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private Transform _playerFoot;
        private PlayerInput _playerInput;
        private PlayerAnimation _playerAnimation;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        [SerializeField] private BoxCollider2D _boxCollider2D;

        private PlayerStateType currentPlayerState = PlayerStateType.Idle;
        
        private Vector2 moveVelocity;
        private bool isGrounded;
        private bool isFacingRight;

        private float fallTime = 0;
        private float groundOffTime = 0;
        private int jumpCount = 0;
        private float defaultGravity;
        private bool jumpPressedInAir;
        private float jumpPressedInAirTimer;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();

            defaultGravity = _rigidbody2D.gravityScale;
        }

        private void Update()
        {
            GroundCheck();
            
            switch (currentPlayerState)
            {
                case PlayerStateType.Idle:
                    HandleMovement(accelation, deceleration, Vector2.zero);

                    if (math.abs(_playerInput.MovementInput.x) > 0.1f)
                    {
                        currentPlayerState = PlayerStateType.Move;
                    }
                    else if (_playerInput.JumpPressed)
                    {
                        Jump();
                        currentPlayerState = PlayerStateType.Jump;
                    }
                    break;
                case PlayerStateType.Move:
                    HandleMovement(accelation, deceleration, _playerInput.MovementInput);

                    if (math.abs(_playerInput.MovementInput.x) < 0.1f)
                    {
                        currentPlayerState = PlayerStateType.Idle;
                    }
                    else if (_playerInput.JumpPressed)
                    {
                        Jump();
                        currentPlayerState = PlayerStateType.Jump;
                    }
                    else if (!isGrounded)
                    {
                        currentPlayerState = PlayerStateType.Fall;
                    }
                    break;
                case PlayerStateType.Jump:
                    HandleMovement(accelation / 2, deceleration / 2, _playerInput.MovementInput);

                    if (_rigidbody2D.velocity.y < 0)
                    {
                        currentPlayerState = PlayerStateType.Fall;
                    }
                    else if (_playerInput.JumpPressed)
                    {
                        Jump();
                        currentPlayerState = PlayerStateType.DoubleJump;
                    }
                    break;
                case PlayerStateType.DoubleJump:
                    HandleMovement(accelation / 2, deceleration / 2, _playerInput.MovementInput);

                    if (_rigidbody2D.velocity.y < 0)
                    {
                        currentPlayerState = PlayerStateType.Fall;
                    }
                    break;
                case PlayerStateType.Fall:
                    
                    HandleMovement(accelation / 2, deceleration / 2, _playerInput.MovementInput);

                    if (jumpPressedInAir)
                    {
                        jumpPressedInAirTimer += Time.deltaTime;
                    }
                    
                    fallTime += Time.deltaTime;
                    
                    if (fallTime < 0.15f && jumpCount > 0)
                        _rigidbody2D.gravityScale = 0;
                    else
                        _rigidbody2D.gravityScale = defaultGravity * 1.5f;

                    if (isGrounded)
                    {
                        if (jumpPressedInAir && jumpPressedInAirTimer < 0.15f)
                        {
                            Jump();
                            currentPlayerState = PlayerStateType.Jump;
                        }
                        else if (fallTime > 1)
                        {
                            currentPlayerState = PlayerStateType.Land;
                        }
                        else
                        {
                            currentPlayerState = PlayerStateType.Idle;
                        }
                        
                        jumpPressedInAir = false;
                        jumpPressedInAirTimer = 0;
                        fallTime = 0;
                        
                        _rigidbody2D.gravityScale = defaultGravity;
                        jumpCount = 0;
                    }
                    else if (_playerInput.JumpPressed) // Jump Forgiveness
                    {
                        jumpPressedInAir = true;

                        if (groundOffTime < 0.1f)
                        {
                            _rigidbody2D.gravityScale = defaultGravity;
                            Jump();
                            currentPlayerState = PlayerStateType.DoubleJump;
                        }
                        else if (jumpCount < 2)
                        {
                            _rigidbody2D.gravityScale = defaultGravity;
                            Jump();
                            currentPlayerState = PlayerStateType.DoubleJump;
                        }
                    }
                    break;
                case PlayerStateType.Land:
                    jumpCount = 0;
                    _rigidbody2D.velocity = Vector2.zero;
                    fallTime += Time.deltaTime;

                    if (fallTime > 0.5f)
                    {
                        fallTime = 0;
                        currentPlayerState = PlayerStateType.Idle;
                    }
                    break;
            }
            
            _playerAnimation.PlayAnimation(currentPlayerState);
            
            if(_playerInput.MovementInput != Vector2.zero)
                FlipPlayer(_playerInput.MovementInput.x > 0);
        }

        private void HandleMovement(float acceleration, float deceleration, Vector2 moveInput)
        {
            if (moveInput != Vector2.zero)
            {
                TurnCheck(moveInput);
                moveVelocity = Vector2.Lerp(moveVelocity, new Vector2(moveInput.x, 0) * speed, acceleration * Time.deltaTime);
            }
            else
            {
                moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.deltaTime);
            }
            
            _rigidbody2D.velocity = new Vector2(moveVelocity.x, _rigidbody2D.velocity.y);
        }

        private void Jump()
        {
            jumpCount++;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
            _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void TurnCheck(Vector2 moveInput)
        {
            if (isFacingRight && moveInput.x > 0)
                FlipPlayer(true);
            else if (!isFacingRight && moveVelocity.x < 0)
                FlipPlayer(false);
        }
        
        private void FlipPlayer(bool isFacingRight)
        {
            _spriteRenderer.flipX = !isFacingRight;
            this.isFacingRight = isFacingRight;
        }

        private void GroundCheck()
        {
            float rayDetectionLenght = 0.1f;
            Vector2 boxCastOrigin = new Vector2(_boxCollider2D.bounds.center.x, _boxCollider2D.bounds.min.y);
            Vector2 boxCastSize = new Vector2(_boxCollider2D.bounds.size.x, rayDetectionLenght);

            var groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, rayDetectionLenght,
                _whatIsGround);

            if (groundHit.collider != null)
            {
                groundOffTime = 0;
                isGrounded = true;
            }
            else
            {
                groundOffTime += Time.deltaTime;
                isGrounded = false;
            }
            
            Debug.DrawLine(boxCastOrigin, boxCastOrigin + Vector2.down * rayDetectionLenght);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Vector2 boxCastOrigin = new Vector2(_boxCollider2D.bounds.center.x, _boxCollider2D.bounds.min.y);
            Vector2 boxCastSize = new Vector2(_boxCollider2D.bounds.size.x, 0.3f);
            
            Gizmos.color = Color.green;
            Gizmos.DrawCube(boxCastOrigin, boxCastSize);
        }
    }
}