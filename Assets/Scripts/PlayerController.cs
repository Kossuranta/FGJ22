using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
    /// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        // Public for external hooks
        public Vector3 Velocity { get; private set; }
        public FrameInput Input { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded { get { return colDown; } }

        [SerializeField, FormerlySerializedAs("_animatorController")]
        PlayerAnimationController animatorController;

        Vector3 lastPosition;
        float currentHorizontalSpeed, currentVerticalSpeed;

        // This is horrible, but for some reason colliders are not fully established when update starts...
        bool active;

        void Awake()
        {
            Invoke(nameof(Activate), 0.5f);
        }

        void Activate()
        {
            active = true;
        }

        void Update()
        {
            if (!active) return;
            // Calculate velocity
            Velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            GatherInput();
            RunCollisionChecks();

            if (rolling)
                CalculateRoll(); // Faster Horizontal movement
            else
                CalculateWalk(); // Horizontal movement

            CalculateJumpApex(); // Affects fall speed, so calculate before gravity
            CalculateGravity(); // Vertical movement
            CalculateJump(); // Possibly overrides vertical

            FlipCharacterSpriteAsNecessary(); //flips character facing based on horizontal speed
            DoAirAnimations(); //Turns air animations on and off as needed
            MoveCharacter(); // Actually perform the axis movement
        }


        #region Gather Input

        bool inputIsDisabled = false;

        public void DisableInput()
        {
            inputIsDisabled = true;
        }

        public void EnableInput()
        {
            inputIsDisabled = false;
        }

        void GatherInput()
        {
            if (!inputIsDisabled)
            {
                Input = new FrameInput
                {
                    JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
                    JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
                    StartRolling = UnityEngine.Input.GetButtonDown("StartRolling"),
                    X = UnityEngine.Input.GetAxisRaw("Horizontal")
                };
            }
            //when input is disabled
            else
            {
                Input = new FrameInput
                {
                    JumpDown = false,
                    JumpUp = false,
                    StartRolling = false,
                    X = 0f
                };
            }


            if (Input.JumpDown)
            {
                lastJumpPressed = Time.time;
            }
            //starts rolling if speed is high enough
            else if (Input.StartRolling && Mathf.Abs(currentHorizontalSpeed) > speedWhenRollingStops)
            {
                rolling = true;
                animatorController.StartRolling();
            }
        }

        #endregion

        #region Collisions

        [FormerlySerializedAs("_characterBounds"),Header("COLLISION"), SerializeField] 
        Bounds characterBounds;

        [FormerlySerializedAs("_groundLayer"),SerializeField]
        LayerMask groundLayer;

        [FormerlySerializedAs("_detectorCount"),SerializeField]
        int detectorCount = 3;

        [FormerlySerializedAs("_detectionRayLength"),SerializeField]
        float detectionRayLength = 0.1f;

        [FormerlySerializedAs("_rayBuffer"),SerializeField, Range(0.1f, 0.3f)] 
        float rayBuffer = 0.1f; // Prevents side detectors hitting the ground

        [FormerlySerializedAs("_airTimeNeededForDeath"),SerializeField]
        float airTimeNeededForDeath = 6;

        RayRange raysUp, raysRight, raysDown, raysLeft;
        bool colUp, colRight, colDown, colLeft;

        float timeLeftGrounded;

        public void PlayerRespawned()
        {
            airTime = 0f;
        }

        // We use these raycast checks for pre-collision information
        void RunCollisionChecks()
        {
            // Generate ray ranges. 
            CalculateRayRanged();

            // Ground
            LandingThisFrame = false;
            bool groundedCheck = RunDetection(raysDown);
            if (colDown && !groundedCheck)
            {
                timeLeftGrounded = Time.time; // Only trigger when first leaving
            }
            else if (!colDown && groundedCheck)
            {
                coyoteUsable = true; // Only trigger when first touching
                LandingThisFrame = true;

                if (airTime >= airTimeNeededForDeath) GetComponent<KillPlayer>().PlayerDies();

                airTime = 0;
            }

            colDown = groundedCheck;

            // The rest
            colUp = RunDetection(raysUp);
            colLeft = RunDetection(raysLeft);
            colRight = RunDetection(raysRight);

            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, detectionRayLength, groundLayer));
            }
        }

        void CalculateRayRanged()
        {
            // This is crying out for some kind of refactor. 
            Bounds b = new Bounds(transform.position, characterBounds.size);

            raysDown = new RayRange(b.min.x + rayBuffer, b.min.y, b.max.x - rayBuffer, b.min.y, Vector2.down);
            raysUp = new RayRange(b.min.x + rayBuffer, b.max.y, b.max.x - rayBuffer, b.max.y, Vector2.up);
            raysLeft = new RayRange(b.min.x, b.min.y + rayBuffer, b.min.x, b.max.y - rayBuffer, Vector2.left);
            raysRight = new RayRange(b.max.x, b.min.y + rayBuffer, b.max.x, b.max.y - rayBuffer, Vector2.right);
        }


        IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
        {
            for (int i = 0; i < detectorCount; i++)
            {
                float t = (float) i / (detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        void OnDrawGizmos()
        {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + characterBounds.center, characterBounds.size);

            // Rays
            if (!Application.isPlaying)
            {
                CalculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (RayRange range in new List<RayRange> {raysUp, raysRight, raysDown, raysLeft})
                {
                    foreach (Vector2 point in EvaluateRayPositions(range)) Gizmos.DrawRay(point, range.Dir * detectionRayLength);
                }
            }

            if (!Application.isPlaying) return;

            // Draw the future position. Handy for visualizing gravity
            Gizmos.color = Color.red;
            Vector3 move = new Vector3(currentHorizontalSpeed, currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + move, characterBounds.size);
        }

        #endregion

        #region Walk

        [FormerlySerializedAs("_acceleration"),Header("WALKING"), SerializeField] 
        float acceleration = 90;

        [FormerlySerializedAs("_moveClamp"),SerializeField]
        float moveClamp = 13;

        [FormerlySerializedAs("_deAcceleration"),SerializeField]
        float deAcceleration = 60f;

        [FormerlySerializedAs("_apexBonus"),SerializeField]
        float apexBonus = 2;

        void CalculateWalk()
        {
            if (Input.X != 0)
            {
                // Set horizontal move speed
                currentHorizontalSpeed += Input.X * acceleration * Time.deltaTime;

                // clamped by max frame movement
                currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClamp, moveClamp);

                // Apply bonus at the apex of a jump
                float apexBonus = Mathf.Sign(Input.X) * this.apexBonus * apexPoint;
                currentHorizontalSpeed += apexBonus * Time.deltaTime;

                animatorController.StartMoving();
            }
            else
            {
                // No input. Let's slow the character down
                currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, deAcceleration * Time.deltaTime);
            }

            if (currentHorizontalSpeed > 0 && colRight || currentHorizontalSpeed < 0 && colLeft) // Don't walk through walls
                currentHorizontalSpeed = 0;

            if (currentHorizontalSpeed == 0)
            {
                animatorController.StopMoving();
                animatorController.StopRolling();
            }
        }

        #endregion

        #region Roll

        [FormerlySerializedAs("_accelerationRolling"),Header("ROLLING"), SerializeField] 
        float accelerationRolling = 90;

        [FormerlySerializedAs("_moveClampRolling"),SerializeField]
        float moveClampRolling = 13;

        [FormerlySerializedAs("_deAccelerationRolling"),SerializeField]
        float deAccelerationRolling = 10f;

        [FormerlySerializedAs("_apexBonusRolling"),SerializeField]
        float apexBonusRolling = 2;

        [FormerlySerializedAs("_speedWhenRollingStops"),SerializeField]
        float speedWhenRollingStops = 10f;

        [SerializeField]
        float sidewaysSpeedNeededForDeath = 15;

        bool rolling = false;

        void CalculateRoll()
        {
            if (Input.X != 0)
            {
                // Set horizontal move speed
                currentHorizontalSpeed += Input.X * accelerationRolling * Time.deltaTime;

                // clamped by max frame movement
                currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClampRolling, moveClampRolling);

                // Apply bonus at the apex of a jump
                float apexBonus = Mathf.Sign(Input.X) * apexBonusRolling * apexPoint;
                currentHorizontalSpeed += apexBonusRolling * Time.deltaTime;
            }
            else
            {
                // No input. Let's slow the character down
                currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, deAccelerationRolling * Time.deltaTime);

                if (Mathf.Abs(currentHorizontalSpeed) < speedWhenRollingStops)
                {
                    rolling = false;
                    animatorController.StopRolling();
                }
            }

            if (currentHorizontalSpeed > 0 && colRight || currentHorizontalSpeed < 0 && colLeft)
            {
                // Kill player when rolling into wall too fast.
                if (Mathf.Abs(currentHorizontalSpeed) >= sidewaysSpeedNeededForDeath) GetComponent<KillPlayer>().PlayerDies();

                //stops player from going through walls
                currentHorizontalSpeed = 0;
            }

            if (currentHorizontalSpeed == 0)
            {
                animatorController.StopMoving();
                animatorController.StopRolling();
            }
        }

        #endregion

        #region Gravity

        [FormerlySerializedAs("_fallClamp"),Header("GRAVITY"), SerializeField] 
        float fallClamp = -40f;

        [FormerlySerializedAs("_minFallSpeed"),SerializeField]
        float minFallSpeed = 80f;

        [FormerlySerializedAs("_maxFallSpeed"),SerializeField]
        float maxFallSpeed = 120f;

        float fallSpeed;

        void CalculateGravity()
        {
            if (colDown)
            {
                // Move out of the ground
                if (currentVerticalSpeed < 0) currentVerticalSpeed = 0;
            }
            else
            {
                // Add downward force while ascending if we ended the jump early
                float fallSpeed = endedJumpEarly && currentVerticalSpeed > 0 ? this.fallSpeed * jumpEndEarlyGravityModifier : this.fallSpeed;

                // Fall
                currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                // Clamp
                if (currentVerticalSpeed < fallClamp) currentVerticalSpeed = fallClamp;
            }
        }

        #endregion

        #region Jump

        [FormerlySerializedAs("_jumpHeight"),Header("JUMPING"), SerializeField] 
        float jumpHeight = 30;

        [FormerlySerializedAs("_jumpApexThreshold"),SerializeField]
        float jumpApexThreshold = 10f;

        [FormerlySerializedAs("_coyoteTimeThreshold"),SerializeField]
        float coyoteTimeThreshold = 0.1f;

        [FormerlySerializedAs("_jumpBuffer"),SerializeField]
        float jumpBuffer = 0.1f;

        [FormerlySerializedAs("_jumpEndEarlyGravityModifier"),SerializeField]
        float jumpEndEarlyGravityModifier = 3;

        bool coyoteUsable;
        bool endedJumpEarly = true;
        float apexPoint; // Becomes 1 at the apex of a jump
        float lastJumpPressed;
        float airTime = 0;

        bool CanUseCoyote { get { return coyoteUsable && !colDown && timeLeftGrounded + coyoteTimeThreshold > Time.time; } }

        bool HasBufferedJump { get { return colDown && lastJumpPressed + jumpBuffer > Time.time; } }

        void CalculateJumpApex()
        {
            if (!colDown)
            {
                // Gets stronger the closer to the top of the jump
                apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);

                airTime = airTime + Time.deltaTime;
            }
            else
            {
                apexPoint = 0;
            }
        }

        void CalculateJump()
        {
            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump)
            {
                currentVerticalSpeed = jumpHeight;
                endedJumpEarly = false;
                coyoteUsable = false;
                timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!colDown && Input.JumpUp && !endedJumpEarly && Velocity.y > 0) // _currentVerticalSpeed = 0;
                endedJumpEarly = true;
            //hits roof
            if (colUp)
                if (currentVerticalSpeed > 0)
                    currentVerticalSpeed = 0;
        }

        #endregion

        #region Move

        [FormerlySerializedAs("_freeColliderIterations"),Header("MOVE"), SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")] 
        int freeColliderIterations = 10;

        void FlipCharacterSpriteAsNecessary()
        {
            if (currentHorizontalSpeed < 0) animatorController.SetSpriteFlipX(true);
            else if (currentHorizontalSpeed > 0) animatorController.SetSpriteFlipX(false);
        }

        void DoAirAnimations()
        {
            if (currentVerticalSpeed > 0 && !colDown)
            {
                animatorController.PlayerFlyingUp(true);
                animatorController.PlayerFlyingDown(false);
            }
            else if (currentVerticalSpeed < 0 && !colDown)
            {
                animatorController.PlayerFlyingUp(false);
                animatorController.PlayerFlyingDown(true);
            }
            else
            {
                animatorController.PlayerFlyingUp(false);
                animatorController.PlayerFlyingDown(false);
            }
        }

        // We cast our bounds before moving to avoid future collisions
        void MoveCharacter()
        {
            Vector3 pos = transform.position;
            RawMovement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed); // Used externally
            Vector3 move = RawMovement * Time.deltaTime;
            Vector3 furthestPoint = pos + move;

            // check furthest movement. If nothing hit, move and don't do extra checks
            Collider2D hit = Physics2D.OverlapBox(furthestPoint, characterBounds.size, 0, groundLayer);
            if (!hit)
            {
                transform.position += move;
                return;
            }
            /*
            else if (hit.gameObject.tag == "MovingPlatform") {
                hit.gameObject.GetComponent<FloatingPlatform>().LinkToPlayer(gameObject);
            }
            */

            // otherwise increment away from current pos; see what closest position we can move to
            Vector3 positionToMoveTo = transform.position;
            for (int i = 1; i < freeColliderIterations; i++)
            {
                // increment to check all but furthestPoint - we did that already
                float t = (float) i / freeColliderIterations;
                Vector2 posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, characterBounds.size, 0, groundLayer))
                {
                    transform.position = positionToMoveTo;

                    // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                    if (i == 1)
                    {
                        if (currentVerticalSpeed < 0) currentVerticalSpeed = 0;
                        Vector3 dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }

                    return;
                }

                positionToMoveTo = posToTry;
            }
        }

        #endregion
    }
}