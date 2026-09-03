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

#if ENABLE_INPUT_SYSTEM

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace getReal3D
{
    /// <summary>
    /// An AxisControl that can also function as two individual directional buttons.
    /// </summary>
    class AxisWithButtons : AxisControl
    {
        /// The actual axis
        [InputControl(offset = 0)]
        public AxisControl axis { get; private set; }

        /// Button activated when the axis is >= 1
        [InputControl(useStateFrom = "axis", processors = "axisDeadzone", parameters = "clamp=2,clampMin=0,clampMax=1", synthetic = true)]
        public ButtonControl positive { get; private set; }

        /// Button activated when the axis is <= -1
        [InputControl(useStateFrom = "axis", processors = "axisDeadzone", parameters = "clamp=2,clampMin=-1,clampMax=0,invert", synthetic = true)]
        public ButtonControl negative { get; private set; }

        /// Perform final initialization tasks after the control hierarchy has been put into place.
        protected override void FinishSetup()
        {
            base.FinishSetup();
            axis = GetChildControl<AxisControl>("axis");
            positive = GetChildControl<ButtonControl>("positive");
            negative = GetChildControl<ButtonControl>("negative");
        }
    }
}

#endif
