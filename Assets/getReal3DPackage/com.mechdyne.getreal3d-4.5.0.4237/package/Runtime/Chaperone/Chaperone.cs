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

using System.Collections;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// This component shows a chaperone grid for a single screen. @see getReal3D.ChaperoneManager.
    /// </summary>
    [ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Chaperone : MonoBehaviour
    {
        private Vector3 m_lastScale = Vector3.zero;

        private MeshRenderer m_meshRenderer;

        private MeshFilter meshFilter
        {
            get
            {
                return GetComponent<MeshFilter>();
            }
        }

        private bool m_fading = false;

        private void Awake()
        {
            m_lastScale = Vector3.zero;
            m_meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (m_lastScale != transform.localScale)
            {
                BuildMesh();
                m_lastScale = transform.localScale;
            }
        }

        private void BuildMesh()
        {
            var mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            mesh.name = "Chaperone " + gameObject.name;

            mesh.vertices = new Vector3[] {
            new Vector3(-0.5f, -0.5f),
            new Vector3(0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f),
            new Vector3(0.5f, 0.5f)
        };

            mesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(transform.localScale.x, 0),
            new Vector2(0, transform.localScale.y),
            new Vector2(transform.localScale.x, transform.localScale.y)
        };

            mesh.triangles = new int[]
            {
            0, 2, 1,
            2, 3, 1
            };

            mesh.RecalculateNormals();
        }

        /// Hide the chaperone grid, fading out over @p time seconds.
        public void FadeOut(float time)
        {
            if (m_fading || !m_meshRenderer.enabled)
            {
                return;
            }
            StartCoroutine(FadeCoroutine(time, false));
        }

        /// Show the chaperone grid, fading in over @p time seconds.
        public void FadeIn(float time)
        {
            if (m_fading || m_meshRenderer.enabled)
            {
                return;
            }
            StartCoroutine(FadeCoroutine(time, true));
        }

        private IEnumerator FadeCoroutine(float time, bool fadeIn)
        {
            m_fading = true;
            var colorBackup = m_meshRenderer.material.color;

            var startColor = new Color(colorBackup.r, colorBackup.g, colorBackup.b, fadeIn ? 0.0f : colorBackup.a);
            var endColor = new Color(colorBackup.r, colorBackup.g, colorBackup.b, fadeIn ? colorBackup.a : 0.0f);

            m_meshRenderer.enabled = true;

            float startTime = Time.time;
            while (Time.time <= startTime + time)
            {
                float t = (Time.time - startTime) / time;
                m_meshRenderer.material.color = Color.Lerp(startColor, endColor, t);
                yield return 0;
            }

            m_meshRenderer.material.color = colorBackup;
            m_meshRenderer.enabled = fadeIn;

            m_fading = false;
        }

        /// Set the chaperone grid color.
        public void SetColor(Color color)
        {
            m_meshRenderer.material.color = color;
        }
    }
}
