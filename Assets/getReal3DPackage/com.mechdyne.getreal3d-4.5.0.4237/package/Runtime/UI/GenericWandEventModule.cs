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
    /// GenericWandEventModule is used to send generic inputs to Unity UI.
    /// </summary>
    /// <remarks>
    /// A generic PlayerInputs must be set in order for this class to retrieve the submit button.
    /// This is usually set via the VRToolkitChoice script.
    /// </remarks>
    public class GenericWandEventModule : WandEventModuleBase
    {
        /// The player inputs that are used to manipulate the UI.
        public PlayerInputs playerInputs
        {
            get { return m_playerInputs; }
            set { m_playerInputs = value; }
        }

        private PlayerInputs m_playerInputs;

        protected override bool GetButtonDown(ButtonFunction function)
        {
            switch (function)
            {
                case ButtonFunction.WandSubmit:
                    return playerInputs != null ? playerInputs.WandButtonDown : false;
                case ButtonFunction.NavSubmit:
                    return playerInputs != null ? playerInputs.ChangeWandButtonDown : false;
            }
            return false;
        }

        protected override bool GetButtonUp(ButtonFunction function)
        {
            switch (function)
            {
                case ButtonFunction.WandSubmit:
                    return playerInputs != null ? playerInputs.WandButtonUp : false;
            }
            return false;
        }

        protected override Vector2 GetRawMoveVector()
        {
            return playerInputs != null ? new Vector2(playerInputs.YawAxis, playerInputs.ForwardAxis) :
                Vector2.zero;
        }
    }
}
