using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hont
{
    [CustomEditor(typeof(Vector3EditorHandleFoo))]
    public class Vector3EditorHandleFooInspector : Editor
    {
        Vector3EditorHandle mVector3EditorHandle;


        public override void OnInspectorGUI()
        {
            var testField = serializedObject.FindProperty("testField");

            if (mVector3EditorHandle == null)
                mVector3EditorHandle = new Vector3EditorHandle(testField, this);

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                mVector3EditorHandle.DrawPropertyField();

                if (changeCheck.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                }
            }
        }

        void OnDestroy()
        {
            if (mVector3EditorHandle != null)
                mVector3EditorHandle.Dispose();
        }
    }
}
