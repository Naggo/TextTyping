# TextTyping

TextMeshProの文章を一文字ずつ表示するためのパッケージです。


# 機能

* TextMeshProを一文字ずつ表示
* テキスト内の記号による表示スピード等の制御


# 依存関係

## 必須
* [RubyTextMeshPro](https://github.com/jp-netsis/RubyTextMeshPro) v1.2.0

## 任意
* [UniTask](https://github.com/Cysharp/UniTask) v2.5.4


# 使用方法
TextMeshPro等のコンポーネントを持ったオブジェクトに`TextTyper`コンポーネントを設定してください。
`Source Text`に表示するテキストを入力し、スクリプトから`StartTyping()`を呼び出すことで文字が表示されます。