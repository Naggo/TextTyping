#if TEXTTYPING_UNITASK_SUPPORT

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace TextTyping {
    public static class TextTyperAsyncExtensions {
        public static async UniTask PlayAsync(this TextTyper typer, CancellationToken token = default) {
            await typer.PlayAsync(false, token);
        }

        public static async UniTask PlayAsync(this TextTyper typer, bool ignoreStopping, CancellationToken token = default) {
            typer.StartTyping();
            if (ignoreStopping) {
                await UniTask.WaitUntil(() => (typer == null) || typer.isCompleted, PlayerLoopTiming.Update, token);
            } else {
                await UniTask.WaitUntil(() => (typer == null) || typer.isCompleted || !typer.isUpdating, PlayerLoopTiming.Update, token);
            }
        }
    }
}

#endif
