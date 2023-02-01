using static GameManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingActionButton : MonoBehaviour
{
    public Metronome metronome;
    public GameManager gm;
    public List<Sprite> controls;
    public Image btnImage;

    void PlayPauseMetronome()
    {
        metronome.ResetTimer();
        metronome.playing = !metronome.playing;
        btnImage.sprite = metronome.playing ? controls[2] : controls[1];
    }

    public void ChangeControls(int newControlIndex)
    {
        btnImage.sprite = controls[newControlIndex];
    }

    public void PressFAB()
    {
        switch (currentPage)
        {
            case "playlistLibrary":
                gm.cameToPlaylistFromFAB = true;
                gm.SwitchPage("playlist", false);
                break;
            case "playlist":
                gm.SwitchPage("library", false);
                break;
            case "library":
                gm.SwitchPage("song", false);
                break;
            case "song":
                PlayPauseMetronome();
                break;
        }
    }
}
