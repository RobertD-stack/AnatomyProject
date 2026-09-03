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

using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// This class is used to track the velocity of the attach game object.
    /// </summary>
    public class VelocityTracking : MonoBehaviour
    {

        [Tooltip("Smoothing factor of the velocitySmoothed and angularVelocitySmoothed properties.")]
        public float smoothingFactor = 0.1f;

        /// Velocity of the tracked object.
        public Vector3 velocity
        {
            get { return m_velocity; }
        }

        /// Smoothed velocity of the tracked object.
        public Vector3 velocitySmoothed
        {
            get { return m_velocitySmoothed; }
        }

        /// Angular velocity of the tracked object.
        public Vector3 angularVelocity
        {
            get { return m_angularVelocity; }
        }

        /// Smoothed angular velocity of the tracked object.
        public Vector3 angularVelocitySmoothed
        {
            get { return m_angularVelocitySmoothed; }
        }

        private Vector3 m_lastPosition = Vector3.zero;
        private Vector3 m_velocity = Vector3.zero;
        private Vector3 m_velocitySmoothed = Vector3.zero;
        private Vector3 m_angularVelocity = Vector3.zero;
        private Quaternion m_lastOrientation = Quaternion.identity;
        private Vector3 m_angularVelocitySmoothed = Vector3.zero;

        private void Start()
        {
            ResetVelocity();
        }

        /// Reset velocity of the tracked object to zero.
        public void ResetVelocity()
        {
            m_lastPosition = transform.position;
            m_lastOrientation = transform.rotation;
            m_velocity = Vector3.zero;
            m_velocitySmoothed = Vector3.zero;
            m_angularVelocity = Vector3.zero;
            m_angularVelocitySmoothed = Vector3.zero;
        }

        private Vector3 CalculateAngularVelocity(Quaternion prev, Quaternion current)
        {
            Quaternion deltaRotation = Quaternion.Inverse(prev) * current;
            float angle = 0.0f;
            Vector3 axis = Vector3.zero;
            deltaRotation.ToAngleAxis(out angle, out axis);
            if (axis == Vector3.zero || axis.x == Mathf.Infinity || axis.x == Mathf.NegativeInfinity)
                axis = Vector3.zero;
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            return axis.normalized * angle / Time.fixedDeltaTime;
        }

        private void trackVelocity()
        {
            m_velocity = (transform.position - m_lastPosition) / Time.fixedDeltaTime;
            m_velocitySmoothed = Vector3.Lerp(m_velocitySmoothed, m_velocity, Time.fixedDeltaTime
                / smoothingFactor);

            m_angularVelocity = CalculateAngularVelocity(m_lastOrientation, transform.rotation);
            m_angularVelocitySmoothed = Vector3.Lerp(m_angularVelocitySmoothed, m_angularVelocity,
                Time.fixedDeltaTime / smoothingFactor);

            m_lastPosition = transform.position;
            m_lastOrientation = transform.rotation;
        }

        private void FixedUpdate()
        {
            trackVelocity();
        }
    }
}
