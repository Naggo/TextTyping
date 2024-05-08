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
            var typerToken = typer.GetCancellationTokenOnDestroy();
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, typerToken)) {
                var linkedToken = linkedCts.Token;
                typer.StartTyping();
                linkedToken.Register(() => typer.StopTyping());
                if (ignoreStopping) {
                    await UniTask.WaitUntil(() => typer.isCompleted, PlayerLoopTiming.Update, linkedToken);
                } else {
                    await UniTask.WaitUntil(() => typer.isCompleted || !typer.isUpdating, PlayerLoopTiming.Update, linkedToken);
                }
            }
        }
    }
}

#endif
