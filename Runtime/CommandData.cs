using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextTyping {
    public readonly struct CommandData {
        public readonly Command cmd;
        public readonly string arg;

        public CommandData(Command cmd, string arg) {
            this.cmd = cmd;
            this.arg = arg;
        }

        public void OnCursorEnter(TextTyper typer) {
            cmd.OnCursorEnter(typer, arg);
        }

        public void OnCursorExit(TextTyper typer) {
            cmd.OnCursorExit(typer, arg);
        }
    }
}
