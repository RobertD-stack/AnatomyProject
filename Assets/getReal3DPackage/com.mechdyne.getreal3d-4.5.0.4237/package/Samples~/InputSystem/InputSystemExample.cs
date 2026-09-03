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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace getReal3D.Examples
{
    /// <summary>
    /// This examples script spawns a prefab at the hand location when the spawn object input action is
    /// triggered.
    /// </summary>
    public class InputSystemExample : MonoBehaviour
    {
        [Tooltip("Object to instanciate when the action is triggered")]
        public GameObject objectToSpawn;

        [Tooltip("The actions")]
        public InputSystemExampleActions actions;

        [Tooltip("Where the object is going to be spawned. Set it to the user hand object.")]
        public Transform hand;

        private List<GameObject> spawnedObjects = new List<GameObject>();
        private float m_cubeScale = 1.0f;

        void Awake()
        {
            actions = new InputSystemExampleActions();
            actions.Gameplay.AddCubes.performed += ctx => { OnSpawnObject(ctx); };
            actions.Gameplay.ClearCubes.performed += ctx => { OnClearObjects(ctx); };
            actions.Gameplay.ChangeCubeScale.performed += ctx => { OnChangeCubeSize(ctx); };
        }

        void OnSpawnObject(InputAction.CallbackContext context)
        {
            var obj = Instantiate(objectToSpawn, hand.transform.position, Quaternion.identity);
            obj.SetActive(true);
            obj.transform.localScale *= m_cubeScale;
            spawnedObjects.Add(obj);
        }

        void OnClearObjects(InputAction.CallbackContext context)
        {
            foreach (var obj in spawnedObjects)
            {
                Destroy(obj);
            }
            spawnedObjects.Clear();
        }

        void OnChangeCubeSize(InputAction.CallbackContext ctx)
        {
            m_cubeScale = (ctx.ReadValue<float>() + 1.5f);
        }

        void OnEnable()
        {
            actions.Gameplay.Enable();
        }

        void OnDisable()
        {
            actions.Gameplay.Disable();
        }
    }
}

#endif
