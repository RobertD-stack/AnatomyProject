/*************************************************************************
 *
 * Copyright 2026, Mechdyne Corporation
 * ALL RIGHTS RESERVED
 *
 * UNPUBLISHED -- Rights reserved under the copyright laws of the United
 * States. Use of a copyright notice is precautionary only and does not
 * imply publication or disclosure.
 *
 * THE CONTENT OF THIS WORK CONTAINS CONFIDENTIAL AND PROPRIETARY
 * INFORMATION OF MECHDYNE CORPORATION. ANY DUPLICATION, MODIFICATION,
 * DISTRIBUTION, OR DISCLOSURE IN ANY FORM, IN WHOLE, OR IN PART, IS
 * STRICTLY PROHIBITED WITHOUT THE PRIOR EXPRESS WRITTEN PERMISSION OF
 * MECHDYNE CORPORATION.
 *
 * Version 4.5.0.4237
 *
 ************************************************************************/

using System;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// NavigationHelper is the core of the getReal3D user navigation system. 
    /// </summary>
    /// Using this class involves giving it some kind of playerInputs then calling both the
    /// Update and FixedUpdate methods from a MonoBehaviour. 
    [Serializable]
    public class NavigationHelper : System.Object
    {

        private Transform m_transform = null;
        private CharacterController m_controller = null;
        private CharacterMotorC m_motor = null;
        private Vector3 m_lastGroundedPosition = Vector3.zero;
        private Vector3 m_initialWand = Vector3.zero;
        private Quaternion m_initialWandRotation = new Quaternion();
        private Quaternion m_initialRotation = new Quaternion();

        [Tooltip("Maximum speed of motion in m/s.")]
        public float TranslationSpeed = 1.0f;

        [Tooltip("The maximum rotation speed in degree/s")]
        public float RotationSpeed = 30.0f;

        [Tooltip("If the wand's orientation is within DeadZone degrees of its initial, do not apply the difference.")]
        public float WandLookDeadZone = 5f;

        [Tooltip("Is using fixed update?")]
        public bool UseFixedUpdate = false;

        /// Type of navigation reference frame
        public enum NavFollows {
            Wand,        //!< Navigation follows the wand
            Head,        //!< Navigtation follows the head
            Reference    //!< Navigation follows the navReference field
        };

        [Tooltip("The navigation reference frame. If set to Reference, then the reference transform can be set.")]
        public NavFollows navFollows = NavFollows.Wand;

        [Tooltip("Reference frame.")]
        public Transform navReference = null;

        [Tooltip("Enumeration selecting the Transform defining frame of the rotation: Head, Wand, Reference. If set to Reference, then the reference transform can be set.")]
        public NavFollows rotationAround = NavFollows.Head;

        [Tooltip("Reference frame.")]
        public Transform rotationAroundReference = null;

        [Tooltip("Enumeration selecting the Transform at the center of the applied rotation: Head, Wand, Reference. If set to Reference, then the reference transform can be set.")]
        public NavFollows rotationFollows = NavFollows.Wand;

        [Tooltip("Reference frame.")]
        public Transform rotationFollowsReference = null;

        /// Rotation axes combinations
        public enum RotationAxes { JoyX = 0, JoyY, JoyXY, JoyZ, JoyXZ, JoyYZ }

        [Tooltip("The axes updated by JoyLook rotation.")]
        public RotationAxes joylookRotationAxes = RotationAxes.JoyXY;

        /// Wand axes combinations
        public enum WandAxes { WandX = 0, WandY, WandXY, WandZ, WandXZ, WandYZ, WandXYZ }

        [Tooltip("The axes updated by WandLook rotation.")]
        public WandAxes wandlookRotation = WandAxes.WandY;

        [Tooltip("If enabled, apply the relative orientation difference (current – initial) of the wand continuously. If not enabled, the orientation changes only if the wand orientation changes.")]
        public bool wandLookContinuousDrive = false;

        [Tooltip("If enabled, the capsule is moved at each frame to follow the user head position.")]
        public bool capsuleFollowsHeadPosition = true;

        private PlayerInputs m_inputs;

        /// Set the inputs used for the navigation
        public PlayerInputs playerInputs
        {
            set { m_inputs = value; }
        }

        /// The different navigation methods
        public enum NavigationMethod
        {
            WalkThrough,
            AimAndGo,
            WandDrive,
            Treadmill,
            Orbit
        }

        [Tooltip("The navigation type used (can be changed at runtime).")]
        public NavigationMethod m_navigationMethod = NavigationMethod.WalkThrough;

        /// Must be called from the Unity Start callback
        public void Start()
        {
            Input.NavOptions.TranslationSpeed = TranslationSpeed;
            Input.NavOptions.RotationSpeed = RotationSpeed;
            Input.NavOptions.WandLookDeadZone = WandLookDeadZone;
        }


        /// Apply the navigation (fixed update part)
        public void Update(Transform transform)
        {
            if (!UseFixedUpdate)
            {
                Plugin.debug($"Navigation time {Time.time} frame {Time.frameCount} delta {Time.deltaTime} sdt {Time.smoothDeltaTime}");
                Navigation(transform, Config.UseSmoothDeltaTime ? Time.smoothDeltaTime : Time.deltaTime);
            }
        }

        /// Apply the navigation
        public void FixedUpdate(Transform transform)
        {
            if (UseFixedUpdate)
            {
                Navigation(transform, Time.fixedDeltaTime);
            }
        }

        /// Apply the navigation
        private void Navigation(Transform transform, float elapsed)
        {
            TranslationSpeed = Input.NavOptions.TranslationSpeed;
            RotationSpeed = Input.NavOptions.RotationSpeed;
            WandLookDeadZone = Input.NavOptions.WandLookDeadZone;

            if (m_inputs == null)
            {
                return;
            }
            Sensor wand = m_inputs.Wand;
            Sensor head = m_inputs.Head;
            if (transform != m_transform)
            {
                SetTransform(transform);
            }
            switch (m_navigationMethod)
            {
                case NavigationMethod.WalkThrough:
                    DoWalkThrough(wand, head, elapsed);
                    break;
                case NavigationMethod.AimAndGo:
                    DoAimAndGo(wand, head, elapsed);
                    break;
                case NavigationMethod.WandDrive:
                    DoWandDrive(wand, head, elapsed);
                    break;
                case NavigationMethod.Treadmill:
                    DoTreadmill(m_inputs.Treadmill,
                                m_inputs.TreadmillRightAxis, m_inputs.TreadmillForwardAxis);
                    break;
                case NavigationMethod.Orbit:
                    DoOrbit(elapsed);
                    break;
            }

            bool isOrbit = m_navigationMethod == NavigationMethod.Orbit;

            if (!isOrbit)
            {
                DoJoyLook(wand, head, elapsed);
                DoWandLook(wand, head, elapsed);

                if (capsuleFollowsHeadPosition)
                {
                    DoCapsuleFollow(head);
                }
            }

            if (m_inputs.ResetButtonDown)
            {
                DoReset();
            }
        }

        /// Stores information for the orbit navigation
        [System.Serializable]
        public class OrbitNavData {
            /// Yaw angle in degree
            [System.NonSerialized]
            public float yaw = 45;
            /// Pitch angle in degree
            [System.NonSerialized]
            public float pitch = 45;
            /// Radius
            [System.NonSerialized]
            public float radius = 5;
            [Tooltip("The center of rotation for orbit navigation. If not set, the scene center is used instead.")]
            public Transform center = null;
            [Tooltip("Initial distance from orbit center to camera.")]
            public float initialRadius = 5;
        }

        /// Stores information for the orbit navigation
        public OrbitNavData orbitNavData = new OrbitNavData();

        private void DoOrbit(float elapsed)
        {
            EnsureBehaviourDisabled(m_controller);
            EnsureBehaviourDisabled(m_motor);

            if (orbitNavData.center == null)
            {
                DoOrbitReset();
            }

            float pitchAxis = m_inputs.ForwardAxis;
            float yawAxis = m_inputs.StrafeAxis;
            float dollyAxis = m_inputs.UpDownAxis;

            orbitNavData.yaw += elapsed * yawAxis * RotationSpeed;
            orbitNavData.pitch += elapsed * pitchAxis * RotationSpeed;
            orbitNavData.radius -= elapsed * dollyAxis * TranslationSpeed;

            orbitNavData.pitch = Mathf.Clamp(orbitNavData.pitch, -90f, 90f);
            orbitNavData.radius = Mathf.Max(orbitNavData.radius, 1f);

            var rotation = Quaternion.Euler(-orbitNavData.pitch, -orbitNavData.yaw, 0f);
            m_transform.position = orbitNavData.center.position + rotation *
                (orbitNavData.radius * Vector3.forward);
            m_transform.LookAt(orbitNavData.center, rotation * Vector3.up);
        }

        private void DoOrbitReset()
        {
            orbitNavData.yaw = 0;
            orbitNavData.pitch = 45;
            orbitNavData.radius = orbitNavData.initialRadius;
            if (orbitNavData.center == null)
            {
                var centerObject = new GameObject("OrbitNavigationCenter");
                orbitNavData.center = centerObject.transform;
            }
            orbitNavData.center.position = Vector3.zero;

            var b = new Bounds();
            foreach (Renderer r in Plugin.FindObjectsByTypeUnsorted<Renderer>()){
                if (r.transform.root != m_transform.root) // We don't want our player wand there
                {
                    b.Encapsulate(r.bounds);
                }
            }
            orbitNavData.center.position = b.center;
        }

        /// Try to move the capsule to the head position.
        private void DoCapsuleFollow(Sensor head)
        {
            var localPositionBak = m_controller.transform.localPosition;

            m_controller.detectCollisions = false;

            Vector3 capsuleCenter = m_controller.center;
            Vector3 capsuleCenterAtHeadHeight = new Vector3(capsuleCenter.x, head.position.y, capsuleCenter.z);
            Vector3 offset = head.position - capsuleCenterAtHeadHeight;

            m_controller.Move(offset);

            var movement = m_controller.transform.localPosition - localPositionBak;

            m_controller.center += new Vector3(movement.x, 0, movement.z);

            m_controller.detectCollisions = true;
            m_controller.transform.localPosition = localPositionBak;
        }

        /// Walk using treadmill. Right and forward are the speeds in m/s.
        private void DoTreadmill(Sensor treadmill, float right, float forward)
        {
            EnsureBehaviourEnabled(m_controller);
            EnsureBehaviourEnabled(m_motor);
            m_motor.useFixedUpdate = UseFixedUpdate;
            //Debug.Log(string.Format("{0} {1}", right, forward));
            var dir = treadmill.rotation * new Vector3(right, 0, forward);
            dir = m_transform.TransformDirection(dir);
            m_motor.inputMoveDirection = dir;
            m_motor.inputJump = m_inputs.JumpButtonDown;
            m_lastGroundedPosition = m_transform.position;
        }

        private void UpdateWandLookRotation(Transform transform, Sensor wand, Sensor head,
            Quaternion initialWand, Quaternion currentWand, float elapsed)
        {
            Quaternion diffOrn = Quaternion.Inverse(initialWand) * currentWand;
            diffOrn = wand.rotation * diffOrn * Quaternion.Inverse(wand.rotation);
            diffOrn = head.rotation * diffOrn * Quaternion.Inverse(head.rotation);
            float angle;
            Vector3 axis;
            diffOrn.ToAngleAxis(out angle, out axis);
            float sign = angle < 0 ? -1f : 1f;
            angle = Mathf.Abs(angle);

            if (angle < WandLookDeadZone) return;
            if (wandLookContinuousDrive)
            {
                angle = Mathf.Clamp01((angle - WandLookDeadZone) /
                    (RotationSpeed - WandLookDeadZone)) * RotationSpeed;
                diffOrn = Quaternion.AngleAxis(sign * angle, axis);
            }
            switch (wandlookRotation)
            {
                case WandAxes.WandX:
                    diffOrn = Quaternion.Euler
                        (new Vector3(diffOrn.eulerAngles.x, 0, 0)); break;
                case WandAxes.WandY:
                    diffOrn = Quaternion.Euler
                        (new Vector3(0, diffOrn.eulerAngles.y, 0)); break;
                case WandAxes.WandXY:
                    diffOrn = Quaternion.Euler
                        (new Vector3(diffOrn.eulerAngles.x, diffOrn.eulerAngles.y, 0)); break;
                case WandAxes.WandZ:
                    diffOrn = Quaternion.Euler
                        (new Vector3(0, 0, diffOrn.eulerAngles.z)); break;
                case WandAxes.WandXZ:
                    diffOrn = Quaternion.Euler
                        (new Vector3(diffOrn.eulerAngles.x, 0, diffOrn.eulerAngles.z)); break;
                case WandAxes.WandYZ:
                    diffOrn = Quaternion.Euler
                        (new Vector3(0, diffOrn.eulerAngles.y, diffOrn.eulerAngles.z)); break;
                case WandAxes.WandXYZ: break;
            }
            Vector3 up = m_transform.up;
            Vector3 forward = head.rotation * Vector3.forward;
            Vector3 right = Vector3.Cross(forward, up);
            forward = Vector3.Cross(right, up);
            Quaternion frame = Quaternion.LookRotation(forward, up);
            diffOrn = Quaternion.Inverse(frame) * diffOrn * frame;
            Vector3 about = m_transform.position;
            switch (rotationAround)
            {
                case NavFollows.Head:
                    about =
                        m_transform.TransformPoint(head.position); break;
                case NavFollows.Wand:
                    about =
                        m_transform.TransformPoint(wand.position); break;
                case NavFollows.Reference:
                    if (rotationAroundReference != null)
                        about = rotationAroundReference.position; break;
            }
            about = m_transform.worldToLocalMatrix * (about - m_transform.position);
            if (m_controller == null || !m_controller.enabled)
                m_transform.Translate(about, Space.Self);
            if (wandLookContinuousDrive)
            {
                m_transform.rotation = Quaternion.Slerp
                    (m_initialRotation, m_initialRotation * diffOrn, elapsed);
                m_initialRotation = m_transform.rotation;
            }
            else
            {
                m_transform.rotation = m_initialRotation * diffOrn;
            }
            if (m_controller == null || !m_controller.enabled)
                m_transform.Translate(-about, Space.Self);
        }

        private void DoJoyLook(Sensor wand, Sensor head, float elapsed)
        {
            Vector2 joy = new Vector2(m_inputs.YawAxis, m_inputs.PitchAxis);
            if (joy.sqrMagnitude > 0f)
            {
                UpdateRotation(joy, wand, head, elapsed);
            }
        }

        private void DoWandLook(Sensor wand, Sensor head, float elapsed)
        {
            if (m_inputs.WandLookButtonDown)
            {
                m_initialWandRotation = wand.rotation;
                m_initialRotation = m_transform.rotation;
            }
            else if (m_inputs.WandLookButton)
            {
                UpdateWandLookRotation(m_transform, wand, head, m_initialWandRotation,
                    wand.rotation, elapsed);
            }
        }

        private void SetTransform(Transform transform)
        {
            m_transform = transform;
            m_controller = transform.GetComponent<CharacterController>();
            m_motor = transform.GetComponent<CharacterMotorC>();
        }

        private void DoWandDrive(Sensor wand, Sensor head, float elapsed)
        {
            if (m_inputs.WandDriveButtonDown)
            {
                m_initialWand = wand.position;
            }
            else if (m_inputs.WandDriveButtonUp)
            {
                m_initialWand = Vector3.zero;
                if (m_motor != null)
                {
                    m_motor.inputMoveDirection = Vector3.zero;
                    m_motor.inputJump = false;
                }
            }
            else if (m_inputs.WandDriveButton)
            {
                Vector3 directionVector = wand.position - m_initialWand;
                directionVector = m_transform.TransformDirection(directionVector);
                directionVector -= Vector3.Dot(directionVector, Physics.gravity.normalized) *
                    Physics.gravity.normalized;

                DoWalk(directionVector, elapsed);
            }
        }

        private void DoAimAndGo(Sensor wand, Sensor head, float elapsed)
        {
            EnsureBehaviourEnabled(m_controller);
            EnsureBehaviourDisabled(m_motor);

            Vector3 joy = new Vector3(m_inputs.StrafeAxis, m_inputs.UpDownAxis, m_inputs.ForwardAxis);

            // Get the input vector from keyboard or analog stick
            Vector3 directionVector = joy;
            switch (navFollows)
            {
                case NavFollows.Wand:
                    directionVector = wand.rotation * directionVector;
                    break;
                case NavFollows.Head:
                    directionVector = head.rotation * directionVector;
                    break;
                case NavFollows.Reference:
                    if (navReference != null)
                        directionVector = (navReference.localToWorldMatrix *
                            m_transform.worldToLocalMatrix).MultiplyVector(directionVector);
                    break;
            }

            if (directionVector != Vector3.zero)
            {
                // Get the length of the directon vector and then normalize it
                // Dividing by the length is cheaper than normalizing when we already have
                // the length anyway
                float directionLength = directionVector.magnitude;
                directionVector = directionVector / directionLength;

                // Make sure the length is no bigger than 1
                directionLength = Mathf.Min(1, directionLength);

                // Make the input vector more sensitive towards the extremes and less sensitive in
                // the middle
                // This makes it easier to control slow speeds when using analog sticks
                directionLength = directionLength * directionLength;

                // Multiply the normalized direction vector by the modified length
                directionVector = directionVector * directionLength;
            }

            directionVector *= TranslationSpeed;
            if (!m_inputs.NavSpeedButton)
            {
                directionVector *= Scale.worldScale;
            }

            // Move the controller
            if (m_controller != null && m_controller.enabled)
            {
                CollisionFlags flags = m_controller.Move
                    (m_transform.TransformDirection(directionVector) * elapsed);
                bool grounded = (flags & CollisionFlags.CollidedBelow) != 0;
                if (grounded) m_lastGroundedPosition = m_transform.position;
            }
            else
            {
                m_transform.position += m_transform.TransformDirection(directionVector) * elapsed;
            }
        }

        private void DoReset()
        {
            if (m_navigationMethod == NavigationMethod.Orbit)
            {
                DoOrbitReset();
            }
            else
            {
                m_transform.position = m_lastGroundedPosition;
                Vector3 up = -Physics.gravity;
                up = (up.sqrMagnitude == 0.0f) ? Vector3.up : up.normalized;
                up.Scale(m_transform.eulerAngles);
                m_transform.rotation = Quaternion.Euler(up);
            }
        }

        private void DoWalkThrough(Sensor wand, Sensor head, float elapsed)
        {
            EnsureBehaviourEnabled(m_controller);
            EnsureBehaviourEnabled(m_motor);
            m_motor.useFixedUpdate = UseFixedUpdate;

            Vector3 joy = new Vector3(m_inputs.StrafeAxis, 0, m_inputs.ForwardAxis);

            // Get the input vector from keyboard or analog stick
            Vector3 directionVector = joy;
            switch (navFollows)
            {
                case NavFollows.Wand:
                    directionVector = wand.rotation * directionVector;
                    break;
                case NavFollows.Head:
                    directionVector = head.rotation * directionVector;
                    break;
                case NavFollows.Reference:
                    if (navReference != null)
                        directionVector = (navReference.localToWorldMatrix *
                            m_transform.worldToLocalMatrix).MultiplyVector(directionVector);
                    break;
            }

            DoWalk(directionVector, elapsed);
        }

        private void DoWalk(Vector3 directionVector, float elapsed)
        {
            if (directionVector != Vector3.zero)
            {
                // Get the length of the direction vector and then normalize it
                // Dividing by the length is cheaper than normalizing when we already have the
                // length anyway
                float directionLength = directionVector.magnitude;
                directionVector = directionVector / directionLength;

                // Make sure the length is no bigger than 1
                directionLength = Mathf.Min(1, directionLength);

                // Make the input vector more sensitive towards the extremes and less sensitive in
                // the middle
                // This makes it easier to control slow speeds when using analog sticks
                directionLength = directionLength * directionLength;

                // Multiply the normalized direction vector by the modified length
                directionVector = directionVector * directionLength;
            }

            directionVector *= TranslationSpeed;
            if (!m_inputs.NavSpeedButton)
                directionVector *= Scale.worldScale;

            // Apply the direction to the CharacterMotor, CharacterController, or Transform, as
            // available
            if (m_motor != null && m_motor.enabled && m_motor.canControl)
            {
                m_motor.inputMoveDirection = m_transform.rotation * directionVector;
                m_motor.inputJump = m_inputs.JumpButtonDown;
            }
            else if (m_controller != null && m_controller.enabled)
            {
                CollisionFlags flags = m_controller.Move
                    (m_transform.TransformDirection(directionVector) * elapsed);
                bool grounded = (flags & CollisionFlags.CollidedBelow) != 0;
                if (grounded) m_lastGroundedPosition = m_transform.position;
            }
            else
            {
                m_transform.position += m_transform.TransformDirection(directionVector) * elapsed;
            }
        }

        private void EnsureBehaviourDisabled(MonoBehaviour behaviour)
        {
            EnsureBehaviourEnabled(behaviour, false);
        }
        private void EnsureBehaviourEnabled(MonoBehaviour behaviour, bool enabled = true)
        {
            if (behaviour && behaviour.enabled != enabled)
            {
                behaviour.enabled = enabled;
            }
        }

        private void EnsureBehaviourDisabled(CharacterController behaviour)
        {
            EnsureBehaviourEnabled(behaviour, false);
        }
        private void EnsureBehaviourEnabled(CharacterController behaviour, bool enabled = true)
        {
            if (behaviour && behaviour.enabled != enabled)
            {
                behaviour.enabled = enabled;
            }
        }

        private void UpdateRotation(Vector2 joy, Sensor wand, Sensor head, float elapsed)
        {

            joy *= RotationSpeed * elapsed; // default scale ~6 seconds to spin 360 degrees

            Matrix4x4 frame = Matrix4x4.identity;
            switch (rotationFollows)
            {
                case NavFollows.Head:
                    frame = Matrix4x4.TRS(Vector3.zero, m_transform.rotation *
                    head.rotation, Vector3.one); break;
                case NavFollows.Wand:
                    frame = Matrix4x4.TRS(Vector3.zero, m_transform.rotation *
                    wand.rotation, Vector3.one); break;
                case NavFollows.Reference:
                    frame = (rotationFollowsReference != null)
                          ? Matrix4x4.TRS(Vector3.zero, rotationFollowsReference.rotation, Vector3.one)
                          : Matrix4x4.TRS(Vector3.zero, m_transform.rotation, Vector3.one); break;
            }

            Vector3 up = -Physics.gravity;
            up = (up.sqrMagnitude == 0.0f) ? Vector3.up : up.normalized;
            Vector3 forward = frame.GetColumn(2);
            Vector3 right = frame.GetColumn(0);

            Vector3 about = m_transform.position;
            switch (rotationAround)
            {
                case NavFollows.Head: about = m_transform.TransformPoint(head.position); break;
                case NavFollows.Wand: about = m_transform.TransformPoint(wand.position); break;
                case NavFollows.Reference:
                    if (rotationAroundReference != null)
                        about = rotationAroundReference.position; break;
            }

            switch (joylookRotationAxes)
            {
                case RotationAxes.JoyX: m_transform.RotateAround(about, right, joy.x); break;
                case RotationAxes.JoyY: m_transform.RotateAround(about, up, joy.x); break;
                case RotationAxes.JoyZ: m_transform.RotateAround(about, forward, joy.x); break;
                case RotationAxes.JoyXY:
                    m_transform.RotateAround(about, up, joy.x);
                    m_transform.RotateAround(about, right, joy.y);
                    break;
                case RotationAxes.JoyXZ:
                    m_transform.RotateAround(about, forward, joy.x);
                    m_transform.RotateAround(about, right, joy.y);
                    break;
                case RotationAxes.JoyYZ:
                    m_transform.RotateAround(about, up, joy.x);
                    m_transform.RotateAround(about, forward, joy.y);
                    break;
            }

        }

    }
}
