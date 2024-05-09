# TextTyping

TextMeshProの文章を一文字ずつ表示するためのパッケージです。


# 機能

* TextMeshProを一文字ずつ表示
* テキスト内の記号による表示スピード等の制御


# 導入方法

UnityのPackage Managerから`Add package from git URL...`を選択し、
```
https://github.com/Naggo/TextTyping.git
```
または
```
https://github.com/Naggo/TextTyping.git#v2.0.1
```
を入力してください。


# 使用方法

TextMeshPro等のコンポーネントを持ったオブジェクトに`TextTyper`コンポーネントを設定してください。
`Source Text`に表示するテキストを入力し、スクリプトから`StartTyping()`を呼び出すことで文字が表示されます。


# 対応パッケージ

* [RubyTextMeshPro](https://github.com/jp-netsis/RubyTextMeshPro)

TextMeshProの代わりにRubyTextMeshProが使用できます。
  
* [UniTask](https://github.com/Cysharp/UniTask)

`TextTyper.PlayAsync(CancellationToken token = default)`および
`TextTyper.PlayAsync(bool ignoreStopping, CancellationToken token = default)`が追加されます。


# 変更点（v2.0.0）

* TextTyper.AddTime()の削除

internalになりました。

* TextTyper.isSetupped、isUpdatingの追加

２つとも読み取り専用です。

* TextTyper.SkipCharacters()をTextTyper.SkipText()に改名

* TextTyper.SkipText(int length, bool ignoreStopping = false)について

`isUpdating`がfalseの間はメソッドを呼んでもスキップ処理を行わず、
スキップ処理の途中でisUpdatingがfalseになった場合（コマンドからStopTyping()を呼んだ時など）にも
処理を中断するようになりました。
ignoreStoppingがtrueの場合、従来通りの動作になります。

* TextTyper.PlayAsync()の変更

TextTyper.PlayAsync(bool ignoreStopping, CancellationToken token = default)を追加しました。
SkipTextと同様、待機中にTextTyper.StopTyping()が呼ばれた場合には処理を完了するようになりました。
ignoreStoppingがtrueの場合、従来通りisCompletedがtrueになるまで待機し続けます。

* 細かい動作変更

TextTyper.PlayAsync()の待機中にTextTyperが破壊された場合、今まではタスクがキャンセルされるようになっていましたが、
タスクを完了するようにしました。
