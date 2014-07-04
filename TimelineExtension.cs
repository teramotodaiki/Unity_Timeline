using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 拡張メソッド"tl()"を定義します。
/// </summary>
static class TimelineExtension
{
    /// <summary>
    /// 複数の処理をキューで順番に実行する簡単なメソッドチェインを提供します。
    /// </summary>
    /// <param name="target">処理の対象</param>
    /// <returns>メソッドチェインの始点</returns>
    public static Timeline tl(this GameObject target)
    {
        return new Timeline(target);
    }

    /// <summary>
    /// 複数の処理をキューで順番に実行する簡単なメソッドチェインを提供します。
    /// </summary>
    /// <param name="target">処理の対象</param>
    /// <returns>メソッドチェインの始点</returns>
    public static Timeline tl(this Transform target)
    {
        return new Timeline(target.gameObject);
    }

    /// <summary>
    /// 複数の処理をキューで順番に実行する簡単なメソッドチェインを提供します。
    /// </summary>
    /// <param name="target">処理の対象</param>
    /// <returns>メソッドチェインの始点</returns>
    public static Timeline tl(this MonoBehaviour target)
    {
        return new Timeline(target.gameObject);
    }

}

/// <summary>
/// 複数の処理をキューで順番に実行する簡単なメソッドチェインを提供します。
/// </summary>
public partial class Timeline
{

    private GameObject target;
    private bool beginFlag;

    private MonoBehaviour component;
    private Queue<DoWhile> routines;
    struct DoWhile
    {
        public Action<float> Do;
        public float While;
    }

    /// <summary>
    /// 新しいキューを生成します
    /// </summary>
    /// <param name="gameObject">処理の対象</param>
    public Timeline(GameObject gameObject)
    {
        this.target = gameObject;
        this.beginFlag = false;
    }

    /// <summary>
    /// 戻り値がvoidの関数、またはデリゲートを呼び出します。
    /// </summary>
    /// <param name="routine">関数またはデリゲート</param>
    /// <param name="time">呼び出す時間</param>
    /// <returns>メソッドチェイン</returns>
    public Timeline Then(Action<float> routine, float time = 0)
    {
        // キューのメモリ確保
        if (this.routines == null)
            this.routines = new Queue<DoWhile>();
        // 新しいルーチンをエンキュー
        this.routines.Enqueue(new DoWhile { Do = routine, While = time });
        // ルーチンがひとつも実行されていなければ、デキューしてコール
        if (this.beginFlag == false)
            this.DequeueToCall();
        // メソッドチェイン
        return this;
    }

    private void DequeueToCall()
    {
        if (this.routines.Count > 0)
        {
            // デキュー
            DoWhile dw = this.routines.Dequeue();
            // （なければ）新しいコンポーネントを用意
            if (this.component == null)
                this.component = this.target.AddComponent<MonoBehaviour>();
            // 新しいコルーチンを開始
            this.component.StartCoroutine(one_routine(dw.Do, dw.While));
            this.beginFlag = true;
        }
        else
        {
            // 破棄
            GameObject.Destroy(this.component);
            this.routines = null;
        }
    }

    /// <summary>
    /// time秒間、actionを呼び出し続けるコルーチン
    /// </summary>
    /// <param name="action">処理</param>
    /// <param name="time">実行時間</param>
    /// <returns>yieldで返すので、単体では呼び出せない</returns>
    private IEnumerator one_routine(Action<float> action, float time)
    {
        float spend = 0;
        while (true)
        {
            action(spend);
            if ((spend += Time.deltaTime) >= time)
                break;
            yield return null;
        }
        // 次のルーチンを呼び出す
        this.DequeueToCall();
    }

}