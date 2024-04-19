using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace TextTyping {
    [RequireComponent(typeof(TextTyper))]
    public class TextTyperSound : MonoBehaviour {

        public AudioClip audioClip;
        public AudioSource audioSource;

        int lastFrame;

        void OnEnable() {
            TextTyper typer = GetComponent<TextTyper>();
            if (typer) typer.onType.AddListener(PlaySound);
        }

        void OnDisable() {
            TextTyper typer = GetComponent<TextTyper>();
            if (typer) typer.onType.RemoveListener(PlaySound);
        }

        public void PlaySound(TextTyper typer) {
            // - 音を再生する。
            // フレームチェック
            if (Time.frameCount == lastFrame) return;

            // 空白以外の文字で音を再生する。
            TMP_TextInfo textInfo = typer.textComponent.textInfo;
            TMP_CharacterInfo chInfo = textInfo.characterInfo[typer.currentCharIndex];
            if (!char.IsWhiteSpace(chInfo.character)) {
                audioSource.PlayOneShot(audioClip);
                lastFrame = Time.frameCount;
            }
        }
    }
}
