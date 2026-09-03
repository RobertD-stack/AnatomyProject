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
    /// This script handles grabbing objects with a wand.
    /// </summary>
    [RequireComponent(typeof(PlayerInputs))]
    public class GenericGrabbingWand
 : MonoBehaviour
    {
        private GameObject grabObject = null;
        private PlayerInputs m_playerInputs;
        private PlayerInputs playerInputs
        {
            get
            {
                if (m_playerInputs == null)
                {
                    m_playerInputs = GetComponent<PlayerInputs>();
                }
                return m_playerInputs;
            }
        }

        [Tooltip("The raycast layer used for object to grab.")]
        public LayerMask grabLayerMask = -1;

        [Tooltip("If stealing an object is allowed.")]
        public bool allowGrabSteal = false;

        [Tooltip("The wand transform.")]
        public Transform Wand;

        [Tooltip("The parent used to hold the object.")]
        public Transform ReparentTransform;

        void OnDisable()
        {
            DropObject();
        }

        void DropObject()
        {
            if (grabObject != null)
            {
                // If the object has the GrabbedObject behavior, tell it to drop
                GrabbedObject grabbedObject = grabObject.GetComponent<GrabbedObject>();
                if (grabbedObject)
                    grabbedObject.dropObject(ReparentTransform);

                grabObject = null;
            }
        }

        void Update()
        {
            if (!Wand || !Wand.gameObject.activeInHierarchy)
            {
                return;
            }

            Debug.DrawRay(Wand.parent.position, Wand.parent.forward * 2f, Color.yellow);

            // If the wand button is released, drop the object
            if (playerInputs.WandButtonUp)
            {
                DropObject();
            }
            // If the wand button was pressed and we're not already grabbing something, test for objects to grab
            else if (grabObject == null && playerInputs.WandButtonDown)
            {
                // Raycast test for objects to grab
                RaycastHit hit = new RaycastHit();
                bool hitTest = Physics.Raycast(Wand.parent.position, Wand.parent.forward, out hit, 2.0f, grabLayerMask);
                if (hitTest)
                {
                    Rigidbody rb = hit.rigidbody;
                    Transform tf = hit.transform.parent;
                    while (rb == null && tf && tf.parent != null)
                    {
                        tf = tf.parent;
                        rb = tf.GetComponent<Rigidbody>();
                    }

                    // If the object doesn't have a rigidbody, don't do anything
                    if (!rb)
                        return;

                    grabObject = rb.gameObject;

                    // Add the GrabbedObject behavior(script) to the object if it hasn't already been grabbed by someone else
                    GrabbedObject grabbedObject = grabObject.GetComponent<GrabbedObject>();
                    if (!grabbedObject)
                    {
                        grabbedObject = grabObject.AddComponent<GrabbedObject>();
                    }

                    // Grab the object
                    grabbedObject.grabObject(ReparentTransform, allowGrabSteal);
                }
            }
        }
    }
}
