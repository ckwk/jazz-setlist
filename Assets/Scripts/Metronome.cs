using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    public InputField bpm;
    public Dropdown timeSig;
    public List<Transform> timeSignatures;
    public List<Image> beats;
    public bool playing;
    public List<float> _beatDeltas;
    public float _timer;
    public Image _currentBeat;

    private Color _alpha;

    private void Start()
    {
        UpdateBeatDeltas();
        _alpha = beats.Last().color;
    }

    private void Update()
    {
        if (!playing)
            return;

        _timer += Time.deltaTime;
        if (_timer > _beatDeltas.Last())
            _timer = 0;

        foreach (var beatTime in _beatDeltas.Where(beatTime => _timer < beatTime))
        {
            _currentBeat = beats[_beatDeltas.IndexOf(beatTime) - 1];
            _currentBeat.color = Color.white;
            foreach (var beat in beats.Where(beat => beat != _currentBeat))
            {
                beat.color = _alpha;
            }
            break;
        }
    }

    public void ResetTimer()
    {
        _timer = 0;
    }

    public void UpdateBeatDeltas()
    {
        _beatDeltas.Clear();
        _beatDeltas.Add(0);

        foreach (var beat in beats)
        {
            if (timeSig.value == 3)
                _beatDeltas.Add(30f / float.Parse(bpm.text) + _beatDeltas.Last()); // double time for 6/8 (index 3 in the TimeSignatureDropdown)
            else
                _beatDeltas.Add(60f / float.Parse(bpm.text) + _beatDeltas.Last());
        }
    }

    public void UpdateTimeSignature()
    {
        foreach (var timeSignature in timeSignatures)
        {
            if (timeSignatures.IndexOf(timeSignature) == timeSig.value)
            {
                timeSignature.gameObject.SetActive(true);
                beats.Clear();
                foreach (var beat in timeSignature.GetComponentsInChildren<Image>())
                {
                    beats.Add(beat);
                }
                UpdateBeatDeltas();
                continue;
            }
            timeSignature.gameObject.SetActive(false); // deactivate non-selected time signatures
        }
    }
}
