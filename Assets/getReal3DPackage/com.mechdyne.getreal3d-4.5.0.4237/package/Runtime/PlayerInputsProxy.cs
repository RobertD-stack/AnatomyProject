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
    /// This class implements the PlayerInputs interface and is used as a proxy over an another
    /// PlayerInputs instance.
    /// </summary>
    /// <remarks>
    /// The way the target is set depends on the initialization mode initializationMode.
    /// </remarks>
    public class PlayerInputsProxy : MonoBehaviour, PlayerInputs
    {
        private PlayerInputs m_target;

        /// Return the target the proxy is forwaring inputs from.
        public PlayerInputs target
        {
            get
            {
                if (m_target == null && m_targetBehaviour != null)
                {
                    m_target = m_targetBehaviour.GetComponent<PlayerInputs>();
                }
                return m_target;
            }
            set
            {
                m_targetBehaviour = (value != null) ? value.behaviour : null;
            }
        }

        [SerializeField]
        private MonoBehaviour m_targetBehaviour; //< Used for serialization only

        /// Proxy initialization mode
        public enum InitializationMode
        {
            /// No initialization, the target has to be manually set.
            None,
            /// Search for a PlayerInputs component in parents of the object this script is attached to.
            SearchInParents,
            /// Search for a PlayerInputs in the given object (see searchInThisObject).
            SearchInObject
        }

        [Tooltip("How the proxy is initialized. It can either search for a target proxy in its parents or in a specified object.")]
        public InitializationMode initializationMode = InitializationMode.None;

        [Tooltip("The object on which to search for the target PlayerInputs.")]
        public Transform searchInThisObject;

        private void Start()
        {
            if (initializationMode == InitializationMode.SearchInParents && transform.parent)
            {
                target = transform.parent.GetComponentInParent<PlayerInputs>();
            }
            else if (initializationMode == InitializationMode.SearchInObject && searchInThisObject)
            {
                target = searchInThisObject.GetComponent<PlayerInputs>();
            }
        }

        private getReal3D.Sensor m_identitySensor = new getReal3D.Sensor();

        public MonoBehaviour behaviour
        {
            get
            {
                return this;
            }
        }

        public float YawAxis
        {
            get
            {
                return target != null ? target.YawAxis : 0;
            }
        }

        public float PitchAxis
        {
            get
            {
                return target != null ? target.PitchAxis : 0;
            }
        }

        public bool WandLookButtonDown
        {
            get
            {
                return target != null ? target.WandLookButtonDown : false;
            }
        }

        public bool WandLookButtonUp
        {
            get
            {
                return target != null ? target.WandLookButtonUp : false;
            }
        }

        public bool WandLookButton
        {
            get
            {
                return target != null ? target.WandLookButton : false;
            }
        }

        public bool WandDriveButtonDown
        {
            get
            {
                return target != null ? target.WandDriveButtonDown : false;
            }
        }

        public bool WandDriveButtonUp
        {
            get
            {
                return target != null ? target.WandDriveButtonUp : false;
            }
        }

        public bool WandDriveButton
        {
            get
            {
                return target != null ? target.WandDriveButton : false;
            }
        }

        public float StrafeAxis
        {
            get
            {
                return target != null ? target.StrafeAxis : 0;
            }
        }

        public float ForwardAxis
        {
            get
            {
                return target != null ? target.ForwardAxis : 0;
            }
        }

        public float UpDownAxis
        {
            get
            {
                return target != null ? target.UpDownAxis : 0;
            }
        }

        public float TreadmillRightAxis
        {
            get
            {
                return target != null ? target.TreadmillRightAxis : 0;
            }
        }

        public float TreadmillForwardAxis
        {
            get
            {
                return target != null ? target.TreadmillForwardAxis : 0;
            }
        }

        public bool NavSpeedButtonDown
        {
            get
            {
                return target != null ? target.NavSpeedButtonDown : false;
            }
        }

        public bool NavSpeedButtonUp
        {
            get
            {
                return target != null ? target.NavSpeedButtonUp : false;
            }
        }

        public bool NavSpeedButton
        {
            get
            {
                return target != null ? target.NavSpeedButton : false;
            }
        }

        public bool JumpButtonDown
        {
            get
            {
                return target != null ? target.JumpButtonDown : false;
            }
        }

        public bool JumpButtonUp
        {
            get
            {
                return target != null ? target.JumpButtonUp : false;
            }
        }

        public bool JumpButton
        {
            get
            {
                return target != null ? target.JumpButton : false;
            }
        }

        public bool WandButtonDown
        {
            get
            {
                return target != null ? target.WandButtonDown : false;
            }
        }

        public bool WandButtonUp
        {
            get
            {
                return target != null ? target.WandButtonUp : false;
            }
        }

        public bool WandButton
        {
            get
            {
                return target != null ? target.WandButton : false;
            }
        }

        public bool ChangeWandButtonDown
        {
            get
            {
                return target != null ? target.ChangeWandButtonDown : false;
            }
        }

        public bool ChangeWandButtonUp
        {
            get
            {
                return target != null ? target.ChangeWandButtonUp : false;
            }
        }

        public bool ChangeWandButton
        {
            get
            {
                return target != null ? target.ChangeWandButton : false;
            }
        }

        public bool ResetButtonDown
        {
            get
            {
                return target != null ? target.ResetButtonDown : false;
            }
        }

        public bool ResetButtonUp
        {
            get
            {
                return target != null ? target.ResetButtonUp : false;
            }
        }

        public bool ResetButton
        {
            get
            {
                return target != null ? target.ResetButton : false;
            }
        }

        getReal3D.Sensor PlayerInputs.Wand
        {
            get
            {
                return target != null ? target.Wand : m_identitySensor;
            }
        }

        getReal3D.Sensor PlayerInputs.Head
        {
            get
            {
                return target != null ? target.Head : m_identitySensor;
            }
        }

        getReal3D.Sensor PlayerInputs.Treadmill
        {
            get
            {
                return target != null ? target.Treadmill : m_identitySensor;
            }
        }
    }
}
