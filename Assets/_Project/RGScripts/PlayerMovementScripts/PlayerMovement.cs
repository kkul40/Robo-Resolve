
using System;
using System.Collections;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Platformer.Mechanics
{
    public class PlayerMovement : MonoBehaviour
    {
        public UiScript script;
        public bool isCharging = false;
        public int rechargeSpeed = -5;

        public float maxSpeed = 4;
        public float modifiedSpeed = 0;
        public float jumpTakeOffSpeed = 4;
        float jumpCooldown;

        public bool IsGrounded = true;

        public float gravityModifier = 1f;
        public float dashSpeed;

        public JumpState jumpState = JumpState.Grounded;
        public bool stopJump;
        public Collider2D collider2d;
        public bool controlEnabled = true;
        public int baseDrainSpeed = 1;
        public int numberOfJumps = 2;

        private Rigidbody2D rigidbody2d;
        protected Rigidbody2D body;
        public float minGroundNormalY = .65f;

        public bool jump;
        public int doubleJumpBatteryDrain;
        public bool dash;
        public float dashTimer;
        public int dashBatteryDrain;
        public bool canDash;
        Vector2 move;
        public Vector2 velocity;
        protected Vector2 targetVelocity;
        protected Vector2 groundNormal;

        protected const float minMoveDistance = 0.001f;

        protected const float shellRadius = 0.01f;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        protected ContactFilter2D contactFilter;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        public float jumpModifier = 1.5f;
        public float jumpDeceleration = 0.5f;

        // For Restart
        Vector3 baseLocation = Vector3.zero;
        JumpState baseJumpState = JumpState.Grounded;
        bool baseIsCharging = false;
        bool baseStopJump = false;
        bool baseControlEnabled = false;
        bool baseJump = false;
        Vector2 baseMove = Vector2.zero;
        bool baseSRFlipX = false;
        bool baseSRFlipY = false;
        Vector2 baseVelocity = Vector2.zero;
        bool baseIsGrounded = false;
        Vector2 baseTargetVelocity = Vector2.zero;
        Vector2 baseGroundNormal = Vector2.zero;

        void Awake()
        {
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rigidbody2d = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            StartCoroutine(playFootSteps());
            GetBaseStatus();
        }

        protected virtual void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();
            body.isKinematic = true;
        }

        protected virtual void OnDisable()
        {
            body.isKinematic = false;
        }

        protected void Update()
        {
            if (GameManager.Instance.IsGamePause())
            {
                SoundManager.Instance.SetPauseSEFootstepChannel();
                return;
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                modifiedSpeed += 1;

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                modifiedSpeed -= 1;
                if (modifiedSpeed < -3)
                    modifiedSpeed = -3;
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
                modifiedSpeed = 0;

            if (controlEnabled)
            {
                if (Input.GetButtonDown("BurstDash") && numberOfJumps > 0 && velocity.x >= 0 && canDash)
                {
                    print("dash right");
                    dash = true;
                    jumpState = JumpState.Dash;
                    script.batteryPercent -= dashBatteryDrain;
                    StartCoroutine(DashTimer());
                    SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Jump);
                } 
                else if (Input.GetButtonDown("BurstDash") && numberOfJumps > 0 && velocity.x < 0 && canDash)
                {
                    print("dash left");
                    dash = true;
                    jumpState = JumpState.Dash;
                    script.batteryPercent -= dashBatteryDrain;
                    StartCoroutine(DashTimer());
                    SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Jump);
                }



                move.x = Input.GetAxis("Horizontal");
                if (move.x != 0)
                {
                    animator.SetBool("isWalking", true);
                } else animator.SetBool("isWalking", false);




                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                    animator.SetBool("isJumping", true);
                    SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Jump);
                }
                else if (jumpState == JumpState.InFlight && numberOfJumps == 2 && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                    animator.SetBool("isJumping", true);
                    animator.Play("JUMP");
                    SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Jump);
                }
                else if (jumpState == JumpState.InFlight && numberOfJumps == 1 && Input.GetButtonDown("Jump"))
                {
                    
                    jumpState = JumpState.PrepareToJump;
                    //animator.SetBool("isDoubleJump", true);
                    animator.Play("DOUBLEJUMP");
                    SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE01, SoundManager.eSoundEffect.Jump_Reverb);
                    script.batteryPercent -= doubleJumpBatteryDrain;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                }
            }
            else
            {
                move.x = 0;
            }

            UpdateJumpState();

            targetVelocity = Vector2.zero;

            ComputeVelocity();

            CalculateDrainSpeed();
        }

        void CalculateDrainSpeed()
        {
            if (isCharging)
                this.script.drainSpeed = rechargeSpeed;
            else { this.script.drainSpeed = baseDrainSpeed; }
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;

                    animator.SetBool("isGrounded", false);
                    if (numberOfJumps > 0)
                    {
                        jump = true;
                        stopJump = false;
                    }
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        jumpState = JumpState.InFlight;

                        //animator.SetBool("isJumping", false);
                        //animator.SetBool("isDoubleJump", false);
                        animator.SetBool("isFalling", true);
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        jumpState = JumpState.Landed;

                        animator.SetBool("isGrounded", true);
                        animator.SetBool("isJumping", false);
                        animator.SetBool("isDoubleJump", false);
                        animator.SetBool("isFalling", false);
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    numberOfJumps = 2;
                    break;
                case JumpState.Dash:
                    if (!dash)
                    {
                        jumpState = JumpState.InFlight;
                    }
                    break;
            }
        }

        IEnumerator DashTimer()
        {
            print("Dashing");
            yield return new WaitForSeconds(dashTimer);
            print("Dash end");
            dash = false;
        }

        protected void ComputeVelocity()
        {
            if (dash)
            {
                jump = false;
                stopJump = false;
                print("Dashing");
            }
            else if (jump && numberOfJumps > 0)
            {
                numberOfJumps -= 1;
                velocity.y = jumpTakeOffSpeed * jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y *= jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            /*animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / (maxSpeed + modifiedSpeed));*/

            targetVelocity = move * (maxSpeed + modifiedSpeed);
            if (dash && velocity.x > 0)
            {
                targetVelocity.x += dashSpeed;
                targetVelocity.y = 0;
            } else if (dash && velocity.x < 0)
            {
                targetVelocity.x += -dashSpeed;
                targetVelocity.y = 0;
            }

        }

        protected virtual void FixedUpdate()
        {
            if (GameManager.Instance.IsGamePause())
                return;

            //if already falling, fall faster than the jump speed, otherwise use normal gravity.
            if (velocity.y < 0)
                velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            else if (!dash)
                velocity += Physics2D.gravity * Time.deltaTime;

            velocity.x = targetVelocity.x;

            IsGrounded = false;

            var deltaPosition = velocity * Time.deltaTime;

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            PerformMovement(move, false);

            move = Vector2.up * deltaPosition.y;

            PerformMovement(move, true);

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                //check if we hit anything in current direction of travel
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                if (count == 0)
                {
                    animator.SetBool("isGrounded", false);
                    animator.SetBool("isFalling", true);
                    jumpState = JumpState.InFlight;
                }
                for (var i = 0; i < count; i++)
                {
                    var currentNormal = hitBuffer[i].normal;

                    //is this surface flat enough to land on?
                    if (currentNormal.y > minGroundNormalY)
                    {
                        IsGrounded = true;
                        // if moving up, change the groundNormal to new surface normal.
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    if (IsGrounded)
                    {
                        //how much of our velocity aligns with surface normal?
                        var projection = Vector2.Dot(velocity, currentNormal);
                        if (projection < 0)
                        {
                            //slower velocity if moving against the normal (up a hill).
                            velocity = velocity - projection * currentNormal;
                        }
                    }
                    else
                    {
                        //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                        velocity.x *= 0;
                        velocity.y = Mathf.Min(velocity.y, 0);
                    }
                    //remove shellDistance from actual move distance.
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            body.position += move.normalized * distance;
        }


        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Dash,
            Jumping,
            InFlight,
            Landed
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "ChargingStation")
            {
                script.numberOfBatteries = script.maxSolarCell;
                isCharging = true;
                SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE_Charge, SoundManager.eSoundEffect.SolarCharge);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "ChargingStation")
            {
                isCharging = false;
                SoundManager.Instance.StopSESound(SoundManager.eSoundChannel.SE_Charge);
            }
        }

        /// <summary>
        /// TO_DO : Temporary Function (Resource loading issues)
        /// </summary>
        public void SetFootstep()
        {
            SoundManager.Instance.PlaySEFootstepSound();
            SoundManager.Instance.SetPauseSEFootstepChannel();
        }

        IEnumerator playFootSteps()
        {
            while (true)
            {
                yield return new WaitForSeconds(.1f);
                if (IsGrounded && velocity.x != 0)
                {
                    SoundManager.Instance.SetUnPauseSEFootstepChannel();
                }
                else
                {
                    SoundManager.Instance.SetPauseSEFootstepChannel();
                }

            }
            
        }

        void GetBaseStatus()
        {
            baseLocation = transform.position;
            baseJumpState = this.jumpState;
            baseIsCharging = this.isCharging;
            baseStopJump = this.stopJump;
            baseControlEnabled = this.controlEnabled;
            baseJump = this.jump;
            baseMove = this.move;
            baseSRFlipX = this.spriteRenderer.flipX;
            baseSRFlipY = this.spriteRenderer.flipY;
            baseVelocity = this.velocity;
            baseIsGrounded = this.IsGrounded;
            baseTargetVelocity = this.targetVelocity;
            baseGroundNormal = this.groundNormal;
        }
        void SetBaseStatus()
        {
            transform.position = baseLocation;
            this.jumpState = baseJumpState;
            this.isCharging = baseIsCharging;
            this.stopJump = baseStopJump;
            this.controlEnabled = baseControlEnabled;
            this.jump = baseJump;
            this.move = baseMove;
            this.spriteRenderer.flipX = baseSRFlipX;
            this.spriteRenderer.flipY = baseSRFlipY;
            this.velocity = baseVelocity;
            this.targetVelocity = baseTargetVelocity;
            this.groundNormal = baseGroundNormal;
            this.body.position = baseLocation;
        }
        public void ResetPlayer()
        {
            SetBaseStatus();
        }

    }
}