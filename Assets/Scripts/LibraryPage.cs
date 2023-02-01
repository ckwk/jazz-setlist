using System.Collections.Generic;
using UnityEngine;

public class LibraryPage : MonoBehaviour
{
    public Transform songList;
    public GameObject EntryPrefab;
    public FloatingActionButton fab;
    private int _numEntries = 0;
    private float _offset = 0.065f * Screen.height;
    private List<RectTransform> songEntries;

    private void Start()
    {
        songEntries = new List<RectTransform>();
    }

    private void OnEnable()
    {
        _numEntries = 0;
        songEntries = new List<RectTransform>();
        fab.ChangeControls(0);
        foreach (Transform entry in songList)
        {
            Destroy(entry.gameObject);
        }

        foreach (var song in Library.GetSongs())
        {
            var newEntry = Instantiate(EntryPrefab, songList).GetComponent<SongEntry>();
            var entryTrans = newEntry.gameObject.GetComponent<RectTransform>();
            var anchoredPos = entryTrans.anchoredPosition;

            // position the new entry below the previous ones
            songEntries.Add(entryTrans);
            anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y - _numEntries * _offset);
            entryTrans.anchoredPosition = anchoredPos;
            _numEntries++;

            // fill out the entry
            newEntry.CreateSongEntry(song);
            newEntry.libraryEntry = true;
        }
    }

    public void Remove(SongEntry songEntry)
    {
        var index = Library.GetSongs().IndexOf(songEntry.mySong);
        Library.RemoveSong(songEntry.mySong);

        for (var i = index; i < Library.GetSongs().Count; i++)
        {
            var entry = songEntries[i + 1];
            var anchoredPos = entry.anchoredPosition;
            anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y + _offset);
            entry.anchoredPosition = anchoredPos;
        }
        songEntries.Remove(songEntries[index]);
        Destroy(songEntry.gameObject);
    }
}
