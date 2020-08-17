using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(CRotationObject))]
public class CRotationObjectEditor : Editor
{
    private SerializedProperty offset;
    private CRotationObject t;
    private CRotationNode[] nodes;
    private List<SerializedObject> nodesSerialized = new List<SerializedObject>();
    private List<SerializedObject> nodeTransforms = new List<SerializedObject>();
    private List<SerializedProperty> nodeRotations = new List<SerializedProperty>();
    private List<SerializedProperty> nodeWeights = new List<SerializedProperty>();
    private float previewCyclePos;
    private GUIStyle editNodesButtonStyle;
    private Quaternion currentRotationOfNodeHandle;

    void OnEnable()
    {
        t = target as CRotationObject;
        offset = serializedObject.FindProperty("m_offset");
        FindNodes();
        Undo.undoRedoPerformed += FindNodes;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= FindNodes;
    }

    void FindNodes()
    {
        t.UpdateNodes();
        nodes = t.GetComponentsInChildren<CRotationNode>();
        nodesSerialized.Clear();
        nodeTransforms.Clear();
        nodeRotations.Clear();
        nodeWeights.Clear();
        foreach (var node in nodes)
        {
            var nodeSO = new SerializedObject(node);
            var transformSO = new SerializedObject(node.transform);
            nodesSerialized.Add(nodeSO);
            nodeTransforms.Add(transformSO);
            nodeRotations.Add(nodeSO.FindProperty("localRotationHint"));
            nodeWeights.Add(nodeSO.FindProperty("m_weight"));
        }
        if (t.nodeSelectedForEditing >= nodes.Length)
        {
            t.nodeSelectedForEditing = -1;
        }
    }

    private bool ApproximatelyEqualToClosestInt(float f)
    {
        return Mathf.Approximately(f, Mathf.Round(f));
    }

    public override void OnInspectorGUI()
    {
        // Styles
        if (editNodesButtonStyle == null)
        {
            editNodesButtonStyle = "IN EditColliderButton";
        }

        serializedObject.Update();
        EditorGUIUtility.wideMode = true;
        EditorGUILayout.PropertyField(offset);
        EditorGUI.BeginChangeCheck();
        var showNodes = EditorGUILayout.BeginFoldoutHeaderGroup(t.showNodesInInspector, "Nodes");
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Toggle node visibility");
            t.showNodesInInspector = showNodes;
        }
        if (t.showNodesInInspector)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                var newNode = new GameObject("Node");
                Undo.RegisterCreatedObjectUndo(newNode, "Add node");
                newNode.AddComponent<CRotationNode>();
                newNode.transform.SetParent(t.transform, false);
                FindNodes();
            }
            if (GUILayout.Button("-"))
            {
                if (nodes.Length > 0)
                {
                    Undo.DestroyObjectImmediate(nodes[nodes.Length - 1].gameObject);
                    FindNodes();
                }
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodesSerialized[i].Update();
                nodeTransforms[i].Update();
                var transform = nodes[i].transform;

                EditorGUILayout.BeginHorizontal();
                var editing = (t.nodeSelectedForEditing == i);
                if (GUILayout.Toggle(editing, "Edit", editNodesButtonStyle))
                {
                    t.nodeSelectedForEditing = i;
                    currentRotationOfNodeHandle = nodes[i].transform.localRotation;
                }
                else if (editing)
                {
                    t.nodeSelectedForEditing = -1;
                }
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(nodeRotations[i]);
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(nodeWeights[i]);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                if (nodesSerialized[i].hasModifiedProperties)
                {
                    nodesSerialized[i].ApplyModifiedProperties();
                    t.UpdateNodes();
                }
                nodeTransforms[i].ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUIUtility.labelWidth = 0;
        previewCyclePos = EditorGUILayout.Slider("Preview cycle position", previewCyclePos, 0, 1);
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        if (t.showNodesInInspector && t.nodeSelectedForEditing != -1)
        {
            var node = nodes[t.nodeSelectedForEditing];
            EditorGUI.BeginChangeCheck();
            currentRotationOfNodeHandle = Handles.RotationHandle(currentRotationOfNodeHandle, node.transform.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(node, "Rotating node");
                node.localRotationHint = currentRotationOfNodeHandle.eulerAngles;
                node.transform.localRotation = currentRotationOfNodeHandle;
            }
        }
        if (!Application.isPlaying)
        {
            t.UpdateCyclePosition(previewCyclePos);
            EditorHelper.ManualPhysicsStep();
        }
    }
}