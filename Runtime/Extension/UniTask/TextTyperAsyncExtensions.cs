#if TEXTTYPING_UNITASK_SUPPORT

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace TextTyping {
    public static class TextTyperAsyncExtensions {
        public static async UniTask PlayAsync(this TextTyper typer, CancellationToken token = default) {
            var typerToken = typer.GetCancellationTokenOnDestroy();
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, typerToken)) {
                var linkedToken = linkedCts.Token;
                typer.StartTyping();
                linkedToken.Register(() => typer.StopTyping());
                await UniTask.WaitUntil(() => typer.isCompleted, PlayerLoopTiming.Update, linkedToken);
            }
        }
    }
}

#endif
