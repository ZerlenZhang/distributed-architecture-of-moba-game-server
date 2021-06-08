using UnityEngine;

public class ReporterGUI : MonoBehaviour
{
	Reporter reporter;
	void Awake()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnGUI()
	{
		reporter.OnGUIDraw();
	}
}
