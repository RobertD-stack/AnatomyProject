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

namespace getReal3D.Examples
{
    /// <summary>
    /// This script clones a user into as many as configured in the getReal3D configuration file.
    /// </summary>
    public class MultiUserCloning : MonoBehaviour
    {

        [Tooltip("Positions where the new users are cloned.")]
        public Transform[] m_cloningPositions = null;

        [Tooltip("getReal3D user to clone.")]
        public getRealUser m_userToClone = null;

        [Tooltip("Avatar layer prefix. The cloned user ID is added to that layer name.")]
        public string m_avatarLayerPrefix = "Avatar_";

        void Start()
        {
            uint userCount = getReal3D.Plugin.getUserCount();
            uint cloningPositionsCount = (uint)m_cloningPositions.Length;
            if (userCount > cloningPositionsCount)
            {
                getReal3D.Plugin.error("Not enough cloning position defined in MultiUserCloning instance.");
            }
            for (uint i = 0; i < userCount - 1 && i < cloningPositionsCount; ++i)
            {
                doClone(i + 1, m_cloningPositions[i].position, m_cloningPositions[i].rotation);
            }
        }

        getRealUser doClone(uint userId, Vector3 position, Quaternion rotation)
        {
            getRealUser clone = UnityEngine.Object.Instantiate(m_userToClone, position, rotation) as getRealUser;
            clone.transform.parent = m_userToClone.transform.parent;
            clone.userId = userId;

            string postfix = getPostfix(userId);

            clone.gameObject.name = m_userToClone.name + postfix;

            var shadowUI = clone.gameObject.GetComponentInChildren<ShadowUI>(true);
            if (shadowUI != null)
            {
                shadowUI.enabled = false;
            }

            var vRSettingsUI = clone.gameObject.GetComponentInChildren<getRealVRSettingsUI>(true);
            if (vRSettingsUI != null)
            {
                vRSettingsUI.enabled = false;
            }

            var cameraUpdater = clone.gameObject.GetComponentInChildren<getRealCameraUpdater>(true);
            if (cameraUpdater != null)
            {
                cameraUpdater.enabled = true;
            }

            string firstAvatarLayer = m_avatarLayerPrefix + "1";
            int firstAvatarLayerId = LayerMask.NameToLayer(firstAvatarLayer);

            string avatarLayer = m_avatarLayerPrefix + (userId + 1).ToString();
            int avatarLayerId = LayerMask.NameToLayer(avatarLayer);
            Camera[] cameras = clone.gameObject.GetComponentsInChildren<Camera>(true);
            foreach (var camera in cameras)
            {
                if (firstAvatarLayerId >= 0)
                {
                    camera.cullingMask |= 1 << firstAvatarLayerId;
                }
                if (avatarLayerId >= 0)
                {
                    camera.cullingMask &= ~(1 << avatarLayerId);
                }
            }

            if (firstAvatarLayerId >= 0 && avatarLayerId >= 0)
            {
                replaceLayer(clone.gameObject, firstAvatarLayerId, avatarLayerId);
            }

            getRealSensorUpdater[] sensorUpdaters = clone.gameObject.GetComponentsInChildren<getRealSensorUpdater>(true);

            foreach (getRealSensorUpdater sensorUpdater in sensorUpdaters)
            {
                sensorUpdater.sensorName += postfix;
            }

            return clone;
        }

        string getPostfix(uint userId)
        {
            return "_" + (userId + 1).ToString();
        }

        private void replaceLayer(GameObject go, int layerFrom, int layerTo)
        {
            if (go.layer == layerFrom)
            {
                go.layer = layerTo;
            }
            foreach (Transform child in go.transform)
            {
                replaceLayer(child.gameObject, layerFrom, layerTo);
            }
        }
    }
}
