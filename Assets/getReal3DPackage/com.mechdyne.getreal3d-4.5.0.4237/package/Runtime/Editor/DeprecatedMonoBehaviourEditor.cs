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

#if UNITY_EDITOR
using UnityEditor;

namespace getReal3D.Editor
{
    [CanEditMultipleObjects]
    class DeprecatedEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This script is deprecated.", MessageType.Warning, true);
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(DeprecatedGetRealUserScript), true)]
    class DeprecatedGetRealUserScriptEditor : DeprecatedEditor { }

    [CustomEditor(typeof(DeprecatedMonoBehaviour), true)]
    class DeprecatedMonoBehaviourEditor : DeprecatedEditor { }
}
#endif
