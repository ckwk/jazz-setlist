
using System.Collections.Generic;
using UnityEngine;

public class PlaylistLibraryPage : MonoBehaviour
{
    public Transform playlistList;
    public GameObject EntryPrefab;
    private int _numEntries = 0;
    private float _offset = 0.065f * Screen.height;
    private List<RectTransform> playlistEntries;

    private void Start()
    {
        playlistEntries = new List<RectTransform>();
        _offset = 0.065f * Screen.height;
    }

    private void OnEnable()
    {
        _numEntries = 0;
        playlistEntries = new List<RectTransform>();
        foreach (Transform entry in playlistList)
        {
            Destroy(entry.gameObject);
        }

        foreach (var playlist in Library.GetPlaylists())
        {
            var newEntry = Instantiate(EntryPrefab, playlistList).GetComponent<PlaylistEntry>();
            var entryTrans = newEntry.gameObject.GetComponent<RectTransform>();
            var anchoredPos = entryTrans.anchoredPosition;

            // position the new entry below the previous ones
            playlistEntries.Add(entryTrans);
            anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y - _numEntries * _offset);
            entryTrans.anchoredPosition = anchoredPos;
            _numEntries++;

            // fill out the entry
            newEntry.CreatePlaylistEntry(playlist);
        }
    }

    public void Remove(PlaylistEntry playlistEntry)
    {
        var index = Library.GetPlaylists().IndexOf(Library.GetPlaylist(playlistEntry.playlist));
        Library.RemovePlaylist(playlistEntry.playlist);

        for (var i = index; i < Library.GetPlaylists().Count; i++)
        {
            var entry = playlistEntries[i + 1];
            var anchoredPos = entry.anchoredPosition;
            anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y + _offset);
            entry.anchoredPosition = anchoredPos;
        }
        print(index);
        playlistEntries.Remove(playlistEntries[index]);
        Destroy(playlistEntry.gameObject);
    }
}
