# TextTyping

TextMeshProの文章を一文字ずつ表示するためのパッケージです。


# 機能

* TextMeshProを一文字ずつ表示
* テキスト内の記号による表示スピード等の制御


# 使用方法

TextMeshPro等のコンポーネントを持ったオブジェクトに`TextTyper`コンポーネントを設定してください。
`Source Text`に表示するテキストを入力し、スクリプトから`StartTyping()`を呼び出すことで文字が表示されます。


# 対応パッケージ

* [RubyTextMeshPro](https://github.com/jp-netsis/RubyTextMeshPro)
TextMeshProの代わりにRubyTextMeshProを使用できます。
  
* [UniTask](https://github.com/Cysharp/UniTask)
`TextTyper.PlayAsync()`が追加されます。
