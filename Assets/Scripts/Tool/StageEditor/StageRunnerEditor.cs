using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FMLib.Extensions;
using Rogue.Ingame.Data;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using StageRunner = Rogue.Ingame.Stage.StageRunner;


namespace Rogue.Tool.StageEditor
{
    [CustomEditor(typeof(StageRunner))]
    public class StageRunnerEditor : OdinEditor
    {
        private static readonly Vector3[] baseRect = new Vector3[]
        {
            new Vector3(-100f, 0f, -100f),
            new Vector3(-100f, 0f, 100f),
            new Vector3(100f, 0f, 100f),
            new Vector3(100f, 0f, -100f)
        };

        private List<Transform> selectedPoints = new List<Transform>();

        public void OnEnable()
        {
            Tools.hidden = true;
        }

        public void OnDisable()
        {
            Tools.hidden = false;
        }

        public void OnSceneGUI()
        {
            DrawBaseRect();
            DrawWaveSlider();
            DrawWave();
            DrawAllSpawnPoints();
            if (GetKeyDown(KeyCode.A))
            {
                var pos = GetMouseWorldPosition();
                AddSpawnPoint(pos);

            }

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        private int selectedWave = 0;
        private void DrawWaveSlider()
        {
            var stage = target as StageRunner;
            Handles.BeginGUI();
            EditorGUI.LabelField(new Rect(5, 5, 60, 18), "Wave", new GUIStyle());
            selectedWave = EditorGUI.IntSlider(new Rect(70, 5, 240, 18), selectedWave, -1, stage.Waves.Count - 1);
            Handles.EndGUI();
        }

        private void DrawWave()
        {
            var stage = target as StageRunner;
            if (selectedWave < 0 || selectedWave >= stage.Waves.Count)
                return;

            var wave = stage.Waves[selectedWave];

            foreach (var mon in wave.Monsters)
            {
                DrawMonster(mon);
            }
        }

        private void DrawMonster(MonsterSpawnData monster)
        {
            var anchor = GetPositionAnchor(monster);
            var min = anchor + monster.PosMin;
            var max = anchor + monster.PosMax;
            var center = (min + max) / 2f;

            DrawRect(min, max, Color.yellow);


            if (monster.PositionType == SpawnPositionType.Player)
            {
                Handles.color = Color.red;
                Handles.DrawLine(Vector3.zero, center);
            }
            Handles.color = new Color(1f, 0.2f, 0.2f, 0.3f);
            Handles.DrawSolidDisc(center, Vector3.up, 2f);
            var str = $"[{monster.Frame}] {(monster.MonsterPrefab != null ? monster.MonsterPrefab.name : "null")} * {monster.Count}";
            var style = new GUIStyle();
            GUI.contentColor = Color.red;
            Handles.Label(center, str);
            GUI.contentColor = Color.black;
            Handles.color = Color.white;
        }

        private void DrawRect(Vector3 min, Vector3 max, Color color)
        {
            var p1 = new Vector3(min.x, 0f, max.z);
            var p2 = new Vector3(max.x, 0f, min.z);
            var verts = new Vector3[] { min, p1, max, p2 };
            var faceColor = new Color(color.r, color.g, color.b, 0.2f);
            var outlineColor = new Color(color.r, color.g, color.b, 1f);
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);
        }


        private Vector3 GetPositionAnchor(MonsterSpawnData data)
        {
            var stage = target as StageRunner;
            var type = data.PositionType;
            switch (type)
            {
                case SpawnPositionType.Player:
                case SpawnPositionType.World:
                    return Vector3.zero;
                case SpawnPositionType.Spawner:
                    return stage.transform.position;
                case SpawnPositionType.SpawnPoint:
                    return data.SpawnPoint != null ? data.SpawnPoint.position : Vector3.zero;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void DrawBaseRect()
        {
            if (EditorWindow.focusedWindow == SceneView.currentDrawingSceneView)
                Handles.DrawSolidRectangleWithOutline(baseRect, new Color(1f, 1f, .5f, 0.2f), Color.white);
        }

        private void DrawAllSpawnPoints()
        {
            var stage = target as StageRunner;
            var tms = stage.GetComponentsInChildren<Transform>();
            foreach (var tm in tms)
            {
                if (tm != stage.transform)
                    DrawSpawnPoint(tm);
            }
        }

        private void DrawSpawnPoint(Transform tm)
        {
            var size = HandleUtility.GetHandleSize(tm.transform.position);

            var pos = Handles.FreeMoveHandle(tm.transform.position, Quaternion.identity, size * .3f, Vector3.zero, Handles.SphereHandleCap).ToVec3XZ();
            var posStr = $"({tm.position.x:0.0},{tm.position.z:0.0})";
            Handles.Label(tm.position, tm.name + posStr, new GUIStyle());

            if (pos != tm.transform.position)
            {
                selectedPoints.Clear();
                selectedPoints.Add(tm);
                if (Event.current.control)
                {
                    var snap = 0.5f;
                    pos.x = Mathf.RoundToInt(pos.x / snap) * snap;
                    pos.z = Mathf.RoundToInt(pos.z / snap) * snap;
                }
                Undo.RecordObject(tm, $"Move {tm.name}");
                tm.transform.position = pos;
            }
        }


        private static readonly string nameRegex = "^P([0-9]+)$";

        private void AddSpawnPoint(Vector3 position)
        {
            var stage = target as StageRunner;
            var tms = stage.GetComponentsInChildren<Transform>();
            var newIdx = 1;
            foreach (var tm in tms)
            {
                var match = Regex.Match(tm.name, nameRegex);
                if (match.Success)
                {
                    var curIdx = int.Parse(match.Groups[1].Value);
                    newIdx = Mathf.Max(newIdx, curIdx + 1);
                }
            }

            var name = $"P{newIdx}";
            var obj = new GameObject(name);
            obj.transform.SetParent(stage.transform);
            obj.transform.position = position;

            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
        }


        private bool GetKeyDown(KeyCode key)
        {
            var e = Event.current;

            return e.type == EventType.KeyDown && e.keyCode == key;
        }

        private Vector3 GetMouseWorldPosition()
        {
            var ray = GetMouseRay();
            var plane = new Plane(Vector3.up, Vector3.zero);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        private Ray GetMouseRay()
        {
            return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        }
    }
}
