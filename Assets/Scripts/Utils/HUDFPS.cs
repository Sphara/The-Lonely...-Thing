using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// FPS count & various GUI stuff (player position tracker atm)
/// </summary>

public class HUDFPS : MonoBehaviour 
{
	public  float updateInterval = 0.5F;
	public GameObject player;

	private float accum   = 0;
	private int   frames  = 0;
	private float timeleft;
	
	string format;

	public GUIStyle FPSControllerStyle;

	void Start()
	{
		timeleft = updateInterval;  
	}

	void Update () {

		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		if( timeleft <= 0.0 )
		{
			float fps = accum/frames;
			format = System.String.Format("{0:F2} FPS",fps);
			
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}

	void OnGUI()
	{
		GUI.Label (new Rect (Screen.width / 20, Screen.height / 20, 50, 20), format, FPSControllerStyle);
		GUI.Label (new Rect(Screen.width / 15, Screen.height / 12, 50, 20), "Position = [" + player.transform.position.x.ToString("0.00") + "," + player.transform.position.y.ToString("0.00") + "]", FPSControllerStyle);
	}
}