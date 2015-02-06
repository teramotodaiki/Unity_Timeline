# Unity_Timeline
複数の処理をメソッドチェインで記述するUnityプラグインです。enchant.jsのTimelineが羨ましくて作りました。
C#のみ対応しています。

たとえば1秒かけて(1,0,0)へ移動したいときは、
this.tl().moveTo(new Vector3(1,0,0),1);

というように記述します。そのあとに(0,1,1)へ移動したいときはメソッドチェインで記述できます。
this.tl().moveTo(new Vector3(1,0,0),1).moveTo(new Vector3(1,1,0),1);

tl()　というメソッドは拡張メソッドで実装しています。
GameObject, Transform, Component
のインスタンスから呼び出せるため、自分で作ったコンポーネントからは、this.tl()と呼び出すこともできますし、
hoge.gameObject.tl()と呼び出すこともできます。


もうfloat time = 0; time += Time.deltatime; ...なんて書く必要はないね！
面倒だったアニメーションがTimelineで簡単に！

書き易さがウリなので、パフォーマンスは保障できません...。
