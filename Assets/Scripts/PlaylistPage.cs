using static GameManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistPage : MonoBehaviour
{
    public List<RectTransform> songEntries;
    public float offset = Screen.height * 0.065f;
    public InputField playlistNameInput;
    public GameObject EntryPrefab;
    public Transform songList;
    public FloatingActionButton fab;

    private GameManager gm;
    private int _numEntries;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        fab.ChangeControls(0);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm.cameToPlaylistFromFAB)
        {
            print("Hello");
            gm.cameToPlaylistFromFAB = false;
            ResetPage();
            return;
        }
        _numEntries = 0;
        songEntries = new List<RectTransform>();
        foreach (Transform entry in songList)
        {
            Destroy(entry.gameObject);
        }

        foreach (var song in currentPlaylist.list)
        {
            var newEntry = Instantiate(EntryPrefab, songList).GetComponent<SongEntry>();
            var entryTrans = newEntry.gameObject.GetComponent<RectTransform>();
            var anchoredPos = entryTrans.anchoredPosition;

            // position the new entry below the previous ones
            songEntries.Add(entryTrans);
            anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y - _numEntries * offset);
            entryTrans.anchoredPosition = anchoredPos;
            _numEntries++;

            // fill out the entry
            newEntry.CreateSongEntry(song);
        }
        print(currentPlaylist.name);
        playlistNameInput.text = currentPlaylist.name;
        var oldPlaylist = Library
            .GetPlaylists()
            .Find(playlist => playlist.name == currentPlaylist.name);
        if (oldPlaylist != null)
        {
            Library.RemovePlaylist(oldPlaylist);
            Library.AddPlaylist(currentPlaylist);
            return;
        }

        Library.AddPlaylist(currentPlaylist);
    }

    public void ResetPage()
    {
        playlistNameInput.text = "";
        foreach (Transform entry in songList)
        {
            Destroy(entry.gameObject);
        }
    }

    public void Remove(SongEntry songEntry)
    {
        var index = currentPlaylist.list.IndexOf(songEntry.mySong);
        currentPlaylist.RemoveSong(songEntry.mySong);

        for (var i = index; i < currentPlaylist.list.Count; i++)
        {
            var entry = songEntries[i + 1];
            var anchoredPos = entry.anchoredPosition;
            anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y + offset);
            entry.anchoredPosition = anchoredPos;
        }
        songEntries.Remove(songEntries[index]);
        Destroy(songEntry.gameObject);
    }

    public void InputTitle()
    {
        if (
            Library.GetPlaylists().Find(playlist => playlist.name == playlistNameInput.text) != null
        )
            return;
        currentPlaylist = new Playlist(playlistNameInput.text);
        Library.AddPlaylist(currentPlaylist);
    }
}
