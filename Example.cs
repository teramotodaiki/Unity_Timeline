using UnityEngine;
using System.Collections;

/// <summary>
/// TimelineExtension.csを使ってみるテスト
/// </summary>
public class Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.tl().MoveBy(Vector3.right*5, 0.5f).Delay(1f).MoveTo(new Vector3(0, 5, 0), 2f);
	}
}
