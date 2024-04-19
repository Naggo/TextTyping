using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using TMPro;


namespace TextTyping.Editor {

    [CustomEditor(typeof(TextTyper))]
    public class TextTyperEditor : UnityEditor.Editor {
        static bool expandEvents;

        // Update 系
        bool repaintFlag;
        bool previewFlag;
        double lastTime;

        // キャッシュやエイリアス
        SerializedProperty textProperty;
        SerializedProperty intervalProperty;
        SerializedProperty speedProperty;
        SerializedProperty skipSpaceProperty;
        SerializedProperty onTypeProperty;
        SerializedProperty onCompleteProperty;

        TextTyper typer => target as TextTyper;

        GUILayoutOption previewButtonHeight => GUILayout.Height(20);

        void FindProperties() {
            textProperty = serializedObject.FindProperty("sourceText");
            intervalProperty = serializedObject.FindProperty("typeInterval");
            speedProperty = serializedObject.FindProperty("typeSpeed");
            skipSpaceProperty = serializedObject.FindProperty("skipSpace");
            onTypeProperty = serializedObject.FindProperty("onType");
            onCompleteProperty = serializedObject.FindProperty("onComplete");
        }

        void OnEnable() {
            // Debug.Log("enable");
            FindProperties();

            if (!Application.isPlaying && typer.isActiveAndEnabled) {
                typer.Initialize();
                SetupTyper();
                SceneView.RepaintAll();
            }

            // コンポーネントの更新
            EditorApplication.update += Update;
        }

        void OnDisable() {
            // Debug.Log("disable");
            if (!Application.isPlaying && typer && typer.isActiveAndEnabled) {
                SetupTyper();
                SceneView.RepaintAll();
            }

            EditorApplication.update -= Update;
        }

        void SetupTyper(bool showText = true) {
            StopPreview();
            typer.Setup();
            int visible = showText ? typer.textComponent.GetParsedText().Length : 0;
            typer.textComponent.maxVisibleCharacters = visible;
            typer.textComponent.ForceMeshUpdate();
        }

        // 描画系
        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawText();
            DrawOtherSettings();
            DrawEvents();

            // データ更新
            if (serializedObject.ApplyModifiedProperties()) {
                SetupTyper();
            }

            // その他
            DrawPreviewButton();
        }

        void DrawText() {
            // テキスト表示
            EditorGUILayout.PropertyField(textProperty);
        }

        void DrawOtherSettings() {
            // その他設定
            EditorGUILayout.PropertyField(intervalProperty);
            EditorGUILayout.PropertyField(speedProperty);
            EditorGUILayout.PropertyField(skipSpaceProperty);
        }

        void DrawEvents() {
            // イベント系
            expandEvents = EditorGUILayout.Foldout(expandEvents, "Events", true);
            if (expandEvents) {
                EditorGUILayout.PropertyField(onTypeProperty);
                EditorGUILayout.PropertyField(onCompleteProperty);
            }
        }

        void DrawPreviewButton() {
            // プレビューのボタン
            if (previewFlag) {
                if (GUILayout.Button("Preview: Stop", previewButtonHeight)) {
                    StopPreview();
                }
            } else {
                if (GUILayout.Button("Preview: Start", previewButtonHeight)) {
                    if (typer.isCompleted) {
                        SetupTyper(false);
                    }
                    StartPreview();
                }
            }
        }

        // Update

        void Update() {
            // プレビュー表示
            if (previewFlag) {
                typer.AddTime((float)(EditorApplication.timeSinceStartup - lastTime));
                typer.textComponent.ForceMeshUpdate();
                if (typer.isCompleted) {
                    StopPreview();
                    Repaint();
                }
                repaintFlag = true;
            }

            // 画面の再描画
            if (repaintFlag) {
                SceneView.RepaintAll();
                repaintFlag = false;
            }

            lastTime = EditorApplication.timeSinceStartup;
        }

        void StartPreview() {
            previewFlag = true;
            lastTime += 0.15f;
        }

        void StopPreview() {
            previewFlag = false;
        }
    }
}
