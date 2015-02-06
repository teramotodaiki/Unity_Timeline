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
    public static Timeline tl(this Component target)
    {
        return new Timeline(target.gameObject);
    }

}

/// <summary>
/// 複数の処理をキューで順番に実行する簡単なメソッドチェインを提供します。
/// </summary>
public partial class Timeline
{
    /// <summary>
    /// Timelineメソッドを実行する母体を提供するビヘイビア
    /// </summary>
    public static MonoBehaviour TimelineProvider { get; private set; }

    private GameObject target;
    private bool beginFlag;

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
        this.target = gameObject; // 処理対象のgameObject
        this.beginFlag = false; // ルーチン実行フラグ
        this.routines = new Queue<DoWhile>(); // キューのメモリ確保

        // プロバイダの用意
        if (TimelineProvider == null)
        {
            var provider = new GameObject("Timeline Provider");
            TimelineProvider = provider.AddComponent<MonoBehaviour>(); // 実体はMonoBehaviour
        }
    }

    /// <summary>
    /// 戻り値がvoidの関数、またはデリゲートを呼び出します。
    /// </summary>
    /// <param name="routine">関数またはデリゲート</param>
    /// <param name="time">呼び出す時間</param>
    /// <returns>メソッドチェイン</returns>
    public Timeline Then(Action<float> routine, float time = 0)
    {
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
            // 新しいコルーチンを開始
            TimelineProvider.StartCoroutine(one_routine(dw.Do, dw.While));
            this.beginFlag = true;
        }
        else
            this.routines = null; // キューの破棄
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
            spend = Mathf.Min(spend + Time.deltaTime, time); // 時間経過
            action(spend);
            yield return null;
            if (spend >= time)
                break;
        }
        // 次のルーチンを呼び出す
        this.DequeueToCall();
    }

}