using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;


namespace TextTyping {
    public static class Parser {
        public static char cmdHead = '$';
        public static char cmdFlag = '¦';

        static Dictionary<char, CommandInfo> commandInfoMap = new();

        internal static void LogCommands() {
            string text = "";
            foreach (var kvp in commandInfoMap) {
                text += kvp.Key + " : " + kvp.Value.argLength + "\n";
            }
            Debug.Log(text);
        }

        public static void RegisterCommand(
            char key, int argLength, Command cmd, bool forceOverride = false)
        {
            // コマンドの登録
            if (!forceOverride && commandInfoMap.ContainsKey(key)) {
                throw new ArgumentException("Exists key");
            }
            if (cmd is null) {
                throw new ArgumentException("cmd is null");
            }
            var cmdInfo = new CommandInfo(cmd, argLength);
            commandInfoMap[key] = cmdInfo;
        }


        public static string ParseCommand(string text, List<CommandData> commands) {
            // テキスト解析
            commands.Clear();
            if (string.IsNullOrEmpty(text)) {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(text.Length);
            ReadOnlySpan<char> span = text.AsSpan();

            int sliceStart = 0;
            int i = text.IndexOf(cmdHead);
            while (i >= 0) {
                i++;
                // i >> キーの位置
                char c;
                try {
                    // キー取得
                    c = text[i];
                } catch (IndexOutOfRangeException) {
                    break;
                }

                if (c == cmdHead) {
                    // ヘッダーエスケープ処理、ヘッダまで書き込んでひとつ飛ばす
                    builder.Append(span.Slice(sliceStart, i - sliceStart));
                    i++;

                } else if (commandInfoMap.TryGetValue(c, out CommandInfo cmdInfo)) {
                    // コマンド判定、ヘッダ直前まで書き込む
                    builder.Append(span.Slice(sliceStart, i - sliceStart - 1));
                    i++;
                    // i >> 引数の位置
                    try {
                        CommandData cmdData;
                        if (cmdInfo.argLength > 0) {
                            // 引数あり
                            string arg = span.Slice(i, cmdInfo.argLength).ToString();
                            cmdData = new CommandData(cmdInfo.cmd, arg);
                            i += cmdInfo.argLength;
                        } else {
                            // 引数なし
                            cmdData = new CommandData(cmdInfo.cmd, null);
                        }
                        // コマンドフラグの書き込み
                        builder.Append(cmdFlag);
                        commands.Add(cmdData);
                    } catch (Exception) {
                        // エラーが出たらコマンド部分も書き込む
                        builder.Append(span.Slice(i - 2, 2));
                    }

                } else {
                    // キー未定義、ヘッダまで書き込む
                    builder.Append(span.Slice(sliceStart, i - sliceStart));
                }

                sliceStart = i;
                i = text.IndexOf(cmdHead, i);
            }

            // 残ってる文字の書き込み
            int remain = text.Length - sliceStart;
            if (remain > 0) {
                builder.Append(span.Slice(sliceStart, remain));
            }

            return builder.ToString();
        }

#if TEXTTYPING_RUBYTMP_SUPPORT
        public static string SetupForRuby(string text, List<CommandData> commands) {
            // - ルビ表示用のタグ処理を行う。
            // コマンド用のテキスト解析では、ルビタグの消去処理をあらかじめこちらで行い、
            // そこでスキップコマンドの追加処理をする。
            StringBuilder builder = new StringBuilder(text, text.Length);
            MatchCollection matches = RubyTextMeshProDefinitions.RUBY_REGEX.Matches(text);
            CommandData skipCmd = new CommandData(commandInfoMap['+'].cmd, null);
            CommandData skipEndCmd = new CommandData(commandInfoMap['-'].cmd, null);

            // 各コマンドのインデックスを取得
            int[] commandIndexes = new int[commands.Count];
            int i = text.IndexOf(cmdFlag);
            int j = 0;
            while (i >= 0) {
                commandIndexes[j++] = i;
                i = text.IndexOf(cmdFlag, i + 1);
            }

            // ルビタグを正規表現で探す
            int addCount = 0;
            foreach (Match match in matches) {
                if (match.Groups.Count != 5) {
                    continue;
                }

                string fullMatch = match.Groups[0].ToString();
                string rubyText = match.Groups["ruby"].ToString();
                string baseText = match.Groups["val"].ToString();
                string replace = $"{baseText}{cmdFlag}{rubyText}{cmdFlag}";

                // 追加するスキップコマンドの位置を特定
                int insertPos = addCount;
                int tagEndIndex = match.Index + match.Length;
                foreach (int cmdIdx in commandIndexes) {
                    if (cmdIdx < tagEndIndex) {
                        insertPos++;
                    } else {
                        break;
                    }
                }

                // スキップコマンドの追加
                commands.Insert(insertPos, skipEndCmd);
                commands.Insert(insertPos, skipCmd);
                addCount += 2;

                builder.Replace(fullMatch, replace);
            }
            return builder.ToString();
        }
#endif

    }
}
