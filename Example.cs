using UnityEngine;
using System.Collections;

/// <summary>
/// TimelineExtension.csを使ってみるテスト
/// </summary>
public class Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        // Move object (5,0,0) -> stop -> (0,5,0)
        this.tl().MoveBy(Vector3.right * 5, 0.5f).Delay(1f).MoveTo(new Vector3(0, 5, 0), 2f);

        // Call method (Debug.Log 0, 0.001, 0.002, ... 1.0)
        this.tl().Then(LogTime, 1f);

	}

    void LogTime(float time)
    {
        Debug.Log(time + " seconds spend.");
    }
}
