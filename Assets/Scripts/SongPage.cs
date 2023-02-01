using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongPage : MonoBehaviour
{
    public Song currentSong;
    public StickTypeButton stickType;
    public InputField bpm;

    //public InputField songTitle;
    public TMP_InputField songTitle;
    public Dropdown timeSig;
    public List<Section> sections;
    public SongEntry nextSong;
    public FloatingActionButton fab;

    private void Start()
    {
        currentSong = new Song(songTitle.text);
    }

    private void OnEnable()
    {
        GameManager.currentPage = "song";
        fab.ChangeControls(1);
    }

    public void ResetPage()
    {
        songTitle.text = "";
    }

    public void LoadSongOntoPage(Song song)
    {
        currentSong = song;
        stickType.stickImage.sprite = stickType.stickTypes[song.stickType];
        songTitle.text = song.title;
        bpm.text = song.bpm;
        timeSig.value = song.timeSig;

        foreach (var part in song.parts)
        {
            sections[song.parts.IndexOf(part)].SetDataTo(part);
        }
    }

    public void BtnStick()
    {
        if (songTitle.text.Trim() == "")
            return;
        currentSong.SetStickType(stickType.stickTypes.IndexOf(stickType.stickType), true);
    }

    public void InputTitle()
    {
        if (songTitle.text.Trim() == "")
            return;
        print(songTitle.text);
        currentSong = new Song(songTitle.text);
        currentSong.SetStickType(stickType.stickTypes.IndexOf(stickType.stickType), false);
        currentSong.SetTimeSignature(timeSig.value, false);
        currentSong.SetBPM(bpm.text, false);

        foreach (var section in sections)
        {
            section.InputSectionTitle(false);
            section.InputSectionInstrument(false);
            section.InputSectionLength(false);
            section.InputSectionSauce(false);
        }
        SaveSystem.PublishSongToLibrary(currentSong);
        print(currentSong.stickType);
    }

    public void DropdownTimeSig()
    {
        if (songTitle.text.Trim() == "")
            return;
        currentSong.SetTimeSignature(timeSig.value, true);
    }

    public void InputBPM()
    {
        if (songTitle.text.Trim() == "")
            return;
        currentSong.SetBPM(bpm.text, true);
    }

    // section functions are in the Sections script
}
