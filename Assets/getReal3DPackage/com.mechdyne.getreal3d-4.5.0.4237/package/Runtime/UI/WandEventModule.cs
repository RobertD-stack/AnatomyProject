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


namespace getReal3D
{
    /// This class is deprecated. GenericWandEventModule should be used instead.
    public class WandEventModule : WandEventModuleBase
    {

        /// name of button to use for click/submit
        public string submitButtonName = "WandButton";

        /// name of horizontal axis used to navigate menu
        public string horizontalAxisName = "Yaw";

        /// name of veritcal axis used to navigate menu
        public string verticalAxisName = "Forward";

        protected override bool GetButtonDown(ButtonFunction function)
        {
            switch (function)
            {
                case ButtonFunction.WandSubmit:
                    return getReal3D.Input.GetButtonDown(submitButtonName);
            }
            return false;
        }

        protected override bool GetButtonUp(ButtonFunction function)
        {
            switch (function)
            {
                case ButtonFunction.WandSubmit:
                    return getReal3D.Input.GetButtonUp(submitButtonName);
            }
            return false;
        }

        protected override UnityEngine.Vector2 GetRawMoveVector()
        {
            return new UnityEngine.Vector2(getReal3D.Input.GetAxis(horizontalAxisName),
                getReal3D.Input.GetAxis(verticalAxisName));
        }
    }
}
