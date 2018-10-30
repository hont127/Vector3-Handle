using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Hont
{
    public class Vector3EditorHandle
    {
        bool mEnabledHandleMode;
        SerializedProperty mSerializeProperty;
        Editor mEditor;
        Vector3 mTargetVector3;
        bool mSceneViewIsDirty = false;

        public event Action<bool> OnHandleToggle;


        public Vector3EditorHandle(SerializedProperty serializeProperty, Editor editor)
        {
            mEditor = editor;
            mSerializeProperty = serializeProperty;

            SceneView.onSceneGUIDelegate -= onSceneGUICallback;
            SceneView.onSceneGUIDelegate += onSceneGUICallback;
        }

        public void Dispose()
        {
            SceneView.onSceneGUIDelegate -= onSceneGUICallback;
        }

        public void DrawPropertyField()
        {
            if (mSceneViewIsDirty)
            {
                mSerializeProperty.vector3Value = mTargetVector3;
                GUI.changed = true;
                mSceneViewIsDirty = false;
            }
            if (Screen.width > 340f)
            {
                var rect = EditorGUILayout.GetControlRect(true);
                var showHandleWidth = 90;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - showHandleWidth, rect.height), mSerializeProperty);

                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    mEnabledHandleMode = GUI.Toggle(new Rect(rect.xMax - showHandleWidth, rect.y, showHandleWidth, rect.height), mEnabledHandleMode, "Show Handle", EditorStyles.miniButton);

                    if (changeScope.changed)
                    {
                        SceneView.RepaintAll();

                        if (OnHandleToggle != null)
                            OnHandleToggle(mEnabledHandleMode);
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(mSerializeProperty);
            }
        }

        void onSceneGUICallback(SceneView sceneView)
        {
            if (!mEnabledHandleMode) return;

            Tools.current = Tool.Move;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var targetTransform = (mSerializeProperty.serializedObject.targetObject as MonoBehaviour).transform;

            var cacheVector3Value = mSerializeProperty.vector3Value;
            var result = targetTransform.worldToLocalMatrix.MultiplyPoint3x4(Handles.PositionHandle(targetTransform.localToWorldMatrix.MultiplyPoint3x4(mSerializeProperty.vector3Value), targetTransform.rotation));

            if (cacheVector3Value != result)
            {
                const float ERROR = 0.00001f;
                if (Mathf.Abs(result.x) < ERROR) result.x = 0;
                if (Mathf.Abs(result.y) < ERROR) result.y = 0;
                if (Mathf.Abs(result.z) < ERROR) result.z = 0;

                mTargetVector3 = result;
                mSceneViewIsDirty = true;
                mEditor.Repaint();
            }
        }
    }
}
