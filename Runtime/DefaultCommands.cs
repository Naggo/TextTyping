using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;


namespace TextTyping {

#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
    static class EditorDefaultCommandsRegister {
        static EditorDefaultCommandsRegister() {
            DefaultCommandsRegister.Register();
        }
    }
#endif

    static class DefaultCommandsRegister {
        static bool registered = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Register() {
            if (registered) return;
            Parser.RegisterCommand('s', 1, new SpeedCommand());
            Parser.RegisterCommand('w', 2, new WaitCommand());
            Parser.RegisterCommand('+', 0, new SkipCommand());
            Parser.RegisterCommand('-', 0, new SkipEndCommand());
            registered = true;
        }
    }

    public class SpeedCommand : Command {
        public override void OnCursorEnter(TextTyper typer, string arg) {
            int i = int.Parse(arg, NumberStyles.HexNumber);
            float speed = i switch {
                0 => 0.1f,
                1 => 0.25f,
                2 => 0.33f,
                3 => 0.5f,
                4 => 0.66f,
                5 => 1f,
                6 => 1.5f,
                7 => 2f,
                8 => 3f,
                9 => 4f,
                10 => 5f,
                11 => 6f,
                12 => 7f,
                13 => 8f,
                14 => 9f,
                15 => 10f,
                _ => 1f
            };
            typer.cmdi.ChangeSpeed(speed);
        }
    }

    public class WaitCommand : Command {
        public override void OnCursorEnter(TextTyper typer, string arg) {
            float time = int.Parse(arg) * 0.1f;
            typer.cmdi.Wait(time);
        }
    }

    public class SkipCommand : Command {
        public override void OnCursorEnter(TextTyper typer, string arg) {
            typer.cmdi.ToggleSkip(true);
        }
    }

    public class SkipEndCommand : Command {
        public override void OnCursorEnter(TextTyper typer, string arg) {
            typer.cmdi.ToggleSkip(false);
        }
    }
}
