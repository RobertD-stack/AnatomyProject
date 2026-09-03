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
    /// Generic interface for retrieving player inputs.
    /// </summary>
    public interface PlayerInputs
    {
        /// Behaviour this PlayerInputs is attached to
        MonoBehaviour behaviour { get; }

        /// Get value of the Yaw axis
        float YawAxis { get; }

        /// Get value of the Pitch axis
        float PitchAxis { get; }

        /// Return true if the WandLook button was pushed during this frame
        bool WandLookButtonDown { get; }

        /// Return true if the WandLook button was released during this frame
        bool WandLookButtonUp { get; }

        /// Return true if the WandLook button is currently pushed
        bool WandLookButton { get; }

        /// Return true if the WandDrive button was pushed during this frame
        bool WandDriveButtonDown { get; }

        /// Return true if the WandDrive button was released during this frame
        bool WandDriveButtonUp { get; }

        /// Return true if the WandDrive button is currently pushed
        bool WandDriveButton { get; }

        /// Return the value of the strafe axis
        float StrafeAxis { get; }

        /// Return the value of the forward axis
        float ForwardAxis { get; }

        /// Return the value of the up/down axis
        float UpDownAxis { get; }

        /// Return the value of the treadmill right axis
        float TreadmillRightAxis { get; }

        /// Return the value of the treadmill forward axis
        float TreadmillForwardAxis { get; }

        /// Return true if the NavSpeed button was pushed during this frame
        bool NavSpeedButtonDown { get; }

        /// Return true if the NavSpeed button was released during this frame
        bool NavSpeedButtonUp { get; }

        /// Return true if the NavSpeed button is currently pushed
        bool NavSpeedButton { get; }

        /// Return true if the Jump button was pushed during this frame
        bool JumpButtonDown { get; }

        /// Return true if the Jump button was released during this frame
        bool JumpButtonUp { get; }

        /// Return true if the Jump button is currently pushed
        bool JumpButton { get; }

        /// Return true if the Wand button was pushed during this frame
        bool WandButtonDown { get; }

        /// Return true if the Wand button was released during this frame
        bool WandButtonUp { get; }

        /// Return true if the Wand button is currently pushed
        bool WandButton { get; }

        /// Return true if the ChangeWand button was pushed during this frame
        bool ChangeWandButtonDown { get; }

        /// Return true if the ChangeWand button was released during this frame
        bool ChangeWandButtonUp { get; }

        /// Return true if the ChangeWand button is currently pushed
        bool ChangeWandButton { get; }

        /// Return true if the Reset button was pushed during this frame
        bool ResetButtonDown { get; }

        /// Return true if the Reset button was released during this frame
        bool ResetButtonUp { get; }

        /// Return true if the Reset button is currently pushed
        bool ResetButton { get; }

        /// Return the Wand sensor
        Sensor Wand { get; }

        /// Return the Head sensor
        Sensor Head { get; }

        /// Return the Treadmill sensor
        Sensor Treadmill { get; }
    }
}
