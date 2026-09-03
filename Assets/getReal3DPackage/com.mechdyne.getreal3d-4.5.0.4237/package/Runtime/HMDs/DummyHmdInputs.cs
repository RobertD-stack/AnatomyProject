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
    /// PlayerInputs implementation that does nothing.
    /// </summary>
    /// All buttons are released, all axes are 0 and sensors are identity.
    public class DummyHmdInputs : MonoBehaviour, PlayerInputs
    {
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
                return 0;
            }
        }

        public float PitchAxis
        {
            get
            {
                return 0;
            }
        }

        public bool WandLookButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool WandLookButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool WandLookButton
        {
            get
            {
                return false;
            }
        }

        public bool WandDriveButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool WandDriveButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool WandDriveButton
        {
            get
            {
                return false;
            }
        }

        public float StrafeAxis
        {
            get
            {
                return 0;
            }
        }

        public float ForwardAxis
        {
            get
            {
                return 0;
            }
        }

        public float TreadmillRightAxis
        {
            get
            {
                return 0;
            }
        }

        public float TreadmillForwardAxis
        {
            get
            {
                return 0;
            }
        }

        public bool NavSpeedButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool NavSpeedButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool NavSpeedButton
        {
            get
            {
                return false;
            }
        }

        public bool JumpButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool JumpButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool JumpButton
        {
            get
            {
                return false;
            }
        }

        public bool WandButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool WandButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool WandButton
        {
            get
            {
                return false;
            }
        }

        public bool ChangeWandButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool ChangeWandButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool ChangeWandButton
        {
            get
            {
                return false;
            }
        }

        public bool ResetButtonDown
        {
            get
            {
                return false;
            }
        }

        public bool ResetButtonUp
        {
            get
            {
                return false;
            }
        }

        public bool ResetButton
        {
            get
            {
                return false;
            }
        }

        private Sensor identity = new Sensor();

        public Sensor Wand
        {
            get
            {
                return identity;
            }
        }

        public Sensor Head
        {
            get
            {
                return identity;
            }
        }

        public Sensor Treadmill
        {
            get
            {
                return identity;
            }
        }

        public float UpDownAxis
        {
            get
            {
                return 0;
            }
        }
    }
}
