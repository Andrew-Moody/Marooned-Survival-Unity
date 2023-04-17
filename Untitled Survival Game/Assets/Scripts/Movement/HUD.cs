using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private TextMeshProUGUI _fpsText;

    private float _timeSinceRefresh;

    private float _timesSinceFPS;

    private int _frames;

    public static HUD Instance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
		{
            Instance = this;
		}
    }


	private void Update()
	{
        _timeSinceRefresh += Time.deltaTime;

        _timesSinceFPS += Time.deltaTime;
        _frames++;
        if (_timesSinceFPS >= 1f)
		{
            float fps = _frames / _timesSinceFPS;
            _fpsText.text = "FPS: " + fps.ToString("#.##");
            _timesSinceFPS = 0f;
            _frames = 0;
		}
	}

	public void SetText(string text)
	{
        _text.text = text;
	}


    public void UpdateStats(DisplayData data)
	{
        _text.text = "Update Time: " + data.Time.ToString("#.###") + " Ticks: " + data.Ticks + "\n";
        //_text.text += "Ticks: " + data.Ticks + "\n";
        _text.text += "Reconciles: " + data.Reconciles + "   Desyncs: " + data.Desyncs + "\n";
        _text.text += "Moves: " + data.Moves + "        Replays: " + data.Replays + "\n";

        float avgDesync;

        if (data.Desyncs == 0)
		{
            avgDesync = 0;
		}
		else
		{
            avgDesync = data.CumulativeDesync / data.Desyncs;
        }
         
        _text.text += "MaxDesync: " + data.MaxDesync.ToString("#.######") + "   AvgDesync: " + avgDesync.ToString("#.######") + "\n";


        _timeSinceRefresh = 0f;
	}
}
