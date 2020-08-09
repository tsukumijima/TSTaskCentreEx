
# TSTaskCentreEx

[TSTask](https://github.com/DBCTRADO/TSTask) へコマンドメッセージを送信するコマンドラインツールです。

[RecTaskCentreEx](https://github.com/nullpohoge/RecTaskCentreEx) を C# に変換した上で、TSTask 対応やオプションの追加など様々な改良を加えています。  
もともとが簡易的なツールだったので作りが甘い箇所があると思いますが、大目に見てください…

## Option

- PID        ( -p )
  - 接続する TSTask の PID (プロセス ID)
  - PID と TaskID はどちらか必須です。
- TaskID     ( -t )
  - 接続する TSTask の TaskID (タスク ID)
  - PID と TaskID はどちらか必須です。
- TSTaskName ( -m )
  - 接続する TSTask のファイル名
  - 初期値は TSTask.exe です。接続する TSTask のファイル名を変更している場合に利用します。
- ServerName ( -n )
  - 接続する TSTask が稼働している PC のホスト名
  - 指定しない場合は TSTaskCentreEx を実行しているローカル PC に接続します。
  - IP アドレスでの指定はうまく動かないみたいです。
- Command    ( -c )
  - TSTask に送信するコマンド
    - 例：`-c Hello`
  - list と指定すると現在起動中の TSTask の PID と TaskID の一覧を表示できます。
    - Command に list を指定した場合のみ、PID または TaskID が指定されていなくても動作します。
- Option     ( -o )
  - TSTask に送信するコマンドのオプションプロパティ
  - OpenTuner など、コマンドによってはオプションがないものもあります。
  - 複数指定する場合は | で区切ってください。
    - 例：`-o "Port:1234|Address:127.0.0.1"`
  - オプション内で使用するダブルクオート (") に限り、`-o "FilePath:'BonDriver_Proxy_T.dll'"` のようにシングルクオート (') で記述することができます。
    - PowerShell で入れ子になっているダブルクオートがうまく渡せない問題への回避策です。
- RecTask    ( -r )
  - TSTask の代わりに RecTask に接続する
  - 指定した場合、TSTask の代わりに RecTask へ接続を試みます。
  - TSTaskName は明示的に指定しない限り自動で RecTask.exe に変更されます。
- Details    ( -d )
  - 受け取ったコマンドの詳細を表示する
- Version    ( -v )
  - バージョンを表示する

## License
[MIT License](LICENSE.txt)
