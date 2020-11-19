using ReadyGamerOne.Common;
using UnityEngine;

public class Trigger2DMessage : MonoBehaviour
{
	//public LayerMask testLayers;
	public string message;
	public int index;
	public bool args;
	void OnTriggerEnter2D(Collider2D col)
	{
		Debug.Log("触碰");
		//if (col.IsTouchingLayers(this.testLayers))
		{
			CEventCenter.BroadMessage(this.message,index,args);
			
			//this.enabled = false;
			
			//gameObject.SetActive(false);
		}
	}
}
