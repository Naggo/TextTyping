using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace TextTyping {
    public abstract class Command {
        public static bool isPreview { get; internal set; } = false;

        public Command() {}

        public virtual void OnCursorEnter(TextTyper typer, string arg) {}

        public virtual void OnCursorExit(TextTyper typer, string arg) {}
    }
}
