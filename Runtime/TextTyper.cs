using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using TMPro;


namespace TextTyping {

    [Serializable]
    public class TyperEvent : UnityEvent<TextTyper> {}

    // [RequireComponent(typeof(TMP_Text))]
    public class TextTyper : MonoBehaviour {

#if UNITY_EDITOR
        void OnValidate() {
            // 実行時非表示にする
            if (Application.isPlaying && !isSetupped) {
                Initialize();
                Setup();
            }
        }

        // [ContextMenu("GetLog")]
        void Logging() {
            Debug.Log(tmp.text + " : " + tmp.GetParsedText());
            Debug.Log(
                $"commands : {commands?.Count}\n"
                + $"cursor : {cursorIndex}\n"
                + $"parsedText : {parsedText}\n"
                + $"isUpdating : {isUpdating}\n"
                + $"cmdi : {cmdi.source}\n"
            );
            Parser.LogCommands();
            foreach (CommandData cmdd in commands) {
                Debug.Log(cmdd.cmd.GetType().FullName + " : " + cmdd.arg);
            }
        }
#endif

        [TextArea(5, 10)]
        public string sourceText;

        [Min(0)]
        public float typeInterval = 0.1f;
        [Min(0)]
        public float typeSpeed = 1;

        public bool skipSpace = false;

        public TyperEvent onType;
        public TyperEvent onComplete;


        TMP_Text tmp;

        int cursorIndex = 0;
        int commandIndex = 0;
        int parsedTextIndex => cursorIndex + commandIndex;

        string parsedText;
        List<CommandData> commands = new();
        Action<TextTyper> commandExitActions;

        bool isSetupped;
        bool isUpdating;
        float deltaTime;

        public int currentCharIndex => cursorIndex - 1;
        public int nextCharIndex => cursorIndex;
        public TMP_Text textComponent => tmp;
        public bool isCompleted => parsedTextIndex >= parsedText.Length;

        // コマンド関連
        public CommandInterface cmdi;

        float commandSpeed = 1;
        int skipCount;
        bool isSkipping => skipCount > 0;

        public struct CommandInterface {
            // コマンドから利用する機能。
            public TextTyper source;

            public CommandInterface(TextTyper source) {
                this.source = source;
            }

            public void ChangeSpeed(float speed) {
                source.commandSpeed = speed;
            }

            public void Wait(float time) {
                // テキスト進行を遅らせる。
                source.deltaTime -= time;
            }

            public void ToggleSkip(bool skip = true) {
                // テキスト進行をスキップする。
                if (skip) {
                    source.skipCount++;
                } else {
                    source.skipCount--;
                }
            }
        }


        void Awake() {
            Initialize();
            ResetParams();
            isSetupped = false;
            isUpdating = false;
        }

        void Update() {
            if (isUpdating) {
                UpdateTime(Time.deltaTime);
            }
        }


        [ContextMenu("Start Typing")]
        public void StartTyping() {
            // 文字送りを開始する。

            // 初期化されてなかったらする
            if (!isSetupped) {
                Setup();
            }

            isUpdating = true;
        }

        [ContextMenu("Stop Typing")]
        public void StopTyping() {
            // 文字送りを中断する。
            isUpdating = false;
        }

        [ContextMenu("Reset Typing")]
        public void ResetTyping() {
            // 文字送りをリセットする。
            StopTyping();
            ResetParams();
            isSetupped = false;
        }

        public void AddTime(float time) {
            // 手動で時間経過させる
            if (!isSetupped) {
                Setup();
            }

            if (!isCompleted) {
                UpdateTime(time);
            }
        }

        public void SkipCharacters() {
            SkipCharacters(sourceText.Length);
        }

        public void SkipCharacters(int count) {
            // 文字を指定数進める
            if (!isSetupped) {
                Setup();
            }

            for (int i = 0; i < count; i++) {
                if (isCompleted) {
                    break;
                }
                ShowNextCharacter();
            }
        }

        // 初期化関数
        internal void Initialize() {
            tmp ??= GetComponent<TMP_Text>();
            cmdi = new CommandInterface(this);
        }

        internal void Setup() {
            // データの初期化
            ResetParams();

            // コマンドが処理された中間テキストを取得する。
            string text = Parser.ParseCommand(sourceText, commands);
            // tmp の種類によって分岐。
            switch (tmp) {
            #if TEXTTYPING_RUBYTMP_SUPPORT
                case RubyTextMeshPro:
                    SetupRuby(text);
                    break;
                case RubyTextMeshProUGUI:
                    SetupRubyUI(text);
                    break;
            #endif
                default:
                    SetupTMP(text);
                    break;
            }

            isSetupped = true;
        }

        void ResetParams() {
            tmp.maxVisibleCharacters = 0;
            cursorIndex = 0;
            commandIndex = 0;
            deltaTime = 0;

            commandExitActions = null;
            commandSpeed = 1;
            skipCount = 0;
        }

#if TEXTTYPING_RUBYTMP_SUPPORT
        void SetupRuby(string text) {
            // ルビ付きｔｍｐの設定
            RubyTextMeshPro ruby = tmp as RubyTextMeshPro;

            // ルビをコマンドに置換して更新する。
            string rubyReplacedText = Parser.SetupForRuby(text, commands);

            // uneditedText 内で ForceMeshUpdate が呼ばれる。
            // tmp.text = rubyReplacedText;
            ruby.uneditedText = rubyReplacedText;

            // 1. inactive 状態での強制, 2. テキスト解析の強制
            // tmp.ForceMeshUpdate(true, true);
            parsedText = tmp.GetParsedText();

            // 最終テキストからコマンドを取り除いて表示する。
            ruby.uneditedText = text.Replace(Parser.cmdFlag.ToString(), string.Empty);
        }

        void SetupRubyUI(string text) {
            RubyTextMeshProUGUI ruby = tmp as RubyTextMeshProUGUI;
            string rubyReplacedText = Parser.SetupForRuby(text, commands);
            // tmp.text = rubyReplacedText;
            ruby.uneditedText = rubyReplacedText;
            // tmp.ForceMeshUpdate(true, true);
            parsedText = tmp.GetParsedText();

            ruby.uneditedText = text.Replace(Parser.cmdFlag.ToString(), string.Empty);
        }
#endif

        void SetupTMP(string text) {
            // 通常のｔｍｐの設定
            tmp.text = text;
            tmp.ForceMeshUpdate(true, true);
            parsedText = tmp.GetParsedText();

            tmp.text = text.Replace(Parser.cmdFlag.ToString(), string.Empty);
        }


        // 更新処理
        void UpdateTime(float time) {
            // 時間をカウントして文字を進める。
            if (isCompleted) {
                isUpdating = false;
                onComplete.Invoke(this);
                return;
            }

            deltaTime += time * typeSpeed * commandSpeed;
            while (deltaTime > typeInterval) {
                ShowNextCharacter();
                deltaTime -= typeInterval;
                if (isCompleted) {
                    break;
                }
            }
        }

        void ShowNextCharacter() {
            // １ステップ進める。
            while (true) {
                // カーソルを進める
                SeekCursor();

                // 最後まで到達したらイベント叩いて終わる
                if (isCompleted) {
                    isUpdating = false;
                    onComplete.Invoke(this);
                    break;
                }

                // コマンドのスキップ判定
                if (isSkipping) continue;

                // 空白のスキップ判定
                if (skipSpace) {
                    TMP_TextInfo textInfo = tmp.textInfo;
                    TMP_CharacterInfo chInfo = textInfo.characterInfo[cursorIndex];
                    if (char.IsWhiteSpace(chInfo.character)) continue;
                    // if (!chInfo.isVisible) continue;
                }
                break;
            }
        }

        void SeekCursor() {
            // 一文字進めてコマンドを実行する。
            if (isCompleted) {
                return;
            }

            // コマンド実行
            if (cursorIndex == 0) {
                EnterCommands();
                if (isCompleted) {
                    return;
                }
            }
            ExitCommands();

            // 文字を進める
            cursorIndex++;
            tmp.maxVisibleCharacters = cursorIndex;
            onType.Invoke(this);

            // コマンド実行
            EnterCommands();
        }

        void EnterCommands() {
            // コマンドの侵入処理を行う。
            while (!isCompleted && parsedText[parsedTextIndex] == Parser.cmdFlag) {
                CommandData cmd = commands[commandIndex];
                // Debug.Log("Run " + cmd.GetType().FullName);
                cmd.OnCursorEnter(this);
                commandExitActions += cmd.OnCursorExit;
                commandIndex++;
            }
        }

        void ExitCommands() {
            // コマンドの離脱処理を行う。
            if (commandExitActions is not null) {
                commandExitActions(this);
                commandExitActions = null;
            }
        }
    }
}
