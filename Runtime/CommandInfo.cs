using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextTyping {
    public readonly struct CommandInfo {
        public readonly Command cmd;
        public readonly int argLength;

        public CommandInfo(Command cmd, int argLength) {
            this.cmd = cmd;
            this.argLength = argLength;
        }
    }
}
