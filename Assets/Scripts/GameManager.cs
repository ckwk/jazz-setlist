using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string currentPage;
    public static Playlist currentPlaylist;
    public static List<string> pageBackCatalog = new List<string>();
    public List<Sprite> stickTypes;
    public List<GameObject> pages;
    public GameObject backButton;
    public bool cameToPlaylistFromFAB = false;

    private float saveTimer = 1f,
        saveCount = 0f;

    private void Awake()
    {
        currentPage = "playlistLibrary";
        backButton.SetActive(false);
        if (PlayerPrefs.HasKey("path"))
            Library.LoadLibrary(PlayerPrefs.GetString("path"));
    }

    private void RecordPreviousPage()
    {
        if (pageBackCatalog.Contains(currentPage) || currentPage == "song")
            return;
        pageBackCatalog.Add(currentPage);
    }

    public void SwitchPage(string targetPage, bool returning)
    {
        backButton.SetActive(true);
        if (!returning)
            RecordPreviousPage();
        currentPage = targetPage;

        var pageName = targetPage[0].ToString().ToUpper() + targetPage.Substring(1) + "Page";
        foreach (var p in pages)
        {
            p.SetActive(p.name == pageName);
            if (targetPage == "song" && p.name == pageName)
            {
                var songPage = p.GetComponent<SongPage>();
                songPage.ResetPage();
                songPage.nextSong.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (saveCount > saveTimer)
        {
            SaveSystem.Save();
            saveCount = 0;
        }

        saveCount += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.D))
        {
            var playlists = "";
            foreach (var playlist in Library.GetPlaylists())
            {
                playlists += playlist.name + "{ ";
                foreach (var song in playlist.list)
                {
                    playlists += song.title + ", ";
                }

                playlists += "}";
            }
            print(playlists);
        }
    }

    public void SwitchPage(Song targetSong, bool comingFromPlaylist)
    {
        backButton.SetActive(true);
        RecordPreviousPage();
        currentPage = "song";

        var songPage = pages[0];
        var songPageScript = songPage.GetComponent<SongPage>();
        foreach (var p in pages)
        {
            p.SetActive(false);
        }
        songPage.SetActive(true);
        songPageScript.LoadSongOntoPage(targetSong);

        var currentIndex = currentPlaylist.list.IndexOf(targetSong);
        var onLastSong = currentIndex == currentPlaylist.list.Count - 1;
        songPageScript.nextSong.gameObject.SetActive(comingFromPlaylist && !onLastSong);
        if (!onLastSong)
            songPageScript.nextSong.CreateSongEntry(currentPlaylist.list[currentIndex + 1]);
    }

    public void ReturnToPreviousPage()
    {
        var previousPage = pageBackCatalog.Count - 1;
        SwitchPage(pageBackCatalog[previousPage], true);
        pageBackCatalog.RemoveAt(previousPage);
        backButton.SetActive(pageBackCatalog.Count > 0);
    }
}

[Serializable]
public class Part
{
    public string name,
        length,
        sauce;
    public int instrument;
}

[Serializable]
public class Song
{
    public string title,
        bpm;
    public int timeSig,
        stickType;
    public List<Part> parts;

    public Song() { }

    public Song(string title)
    {
        this.title = title;
        parts = new List<Part>();
        for (var i = 0; i < 9; i++)
        {
            parts.Add(new Part());
        }
    }

    public void SetTitle(string title, bool save)
    {
        this.title = title;
        if (save)
            SaveSystem.PublishSongToLibrary(this);
    }

    public void SetStickType(int stickType, bool save)
    {
        this.stickType = stickType;
        if (save)
            SaveSystem.PublishSongToLibrary(this);
    }

    public void SetTimeSignature(int index, bool save)
    {
        this.timeSig = index;
        if (save)
            SaveSystem.PublishSongToLibrary(this);
    }

    public void SetBPM(string bpm, bool save)
    {
        this.bpm = bpm;
        if (save)
            SaveSystem.PublishSongToLibrary(this);
    }
}

[Serializable]
public class Playlist
{
    public string name;
    public List<Song> list = new List<Song>();

    public Playlist(string name)
    {
        this.name = name;
        list = new List<Song>();
    }

    public void AddSongs(List<Song> list)
    {
        this.list = list;
    }

    public bool RemoveSong(Song song) => list.Remove(song);
}

[Serializable]
public class LibraryObject
{
    private List<Playlist> _playlistLibrary;
    private List<Song> _songLibrary;

    public LibraryObject(List<Playlist> playlists, List<Song> songs)
    {
        _playlistLibrary = playlists;
        _songLibrary = songs;
    }

    public List<Playlist> GetPlaylists() => _playlistLibrary;

    public List<Song> GetSongs() => _songLibrary;
}

public static class Library
{
    private static List<Playlist> _playlistLibrary = new List<Playlist>();
    private static List<Song> _songLibrary = new List<Song>();

    public static void AddSong(Song song)
    {
        _songLibrary.Add(song);
        _songLibrary.Sort((s1, s2) => string.CompareOrdinal(s1.title, s2.title));
    }

    public static bool RemoveSong(Song song)
    {
        foreach (var playlist in _playlistLibrary.Where(playlist => playlist.list.Contains(song)))
        {
            playlist.list.Remove(song);
        }
        return _songLibrary.Remove(song);
    }

    public static bool RemovePlaylist(Playlist playlist)
    {
        var pl = _playlistLibrary.Find(p => p.name == playlist.name);
        return _playlistLibrary.Remove(pl);
    }

    public static void AddPlaylist(Playlist playlist)
    {
        _playlistLibrary.Add(playlist);
        _playlistLibrary.Sort((p1, p2) => string.CompareOrdinal(p1.name, p2.name));
    }

    public static void AddPlaylist(string playlistName)
    {
        _playlistLibrary.Add(new Playlist(playlistName));
    }

    public static Playlist GetPlaylist(Playlist playlist)
    {
        return _playlistLibrary.Find(p => p.name == playlist.name);
    }

    public static List<Song> GetSongs() => _songLibrary;

    public static List<Playlist> GetPlaylists() => _playlistLibrary;

    public static bool UpdateSong(Song song)
    {
        var songToUpdate = _songLibrary.Find(s => s.title == song.title);
        if (songToUpdate == null)
            return false;
        _songLibrary.Remove(songToUpdate);
        _songLibrary.Add(song);
        _songLibrary.Sort((s1, s2) => string.CompareOrdinal(s1.title, s2.title));
        return true;
    }

    public static void UpdatePlaylist(string playlistName, Playlist oldPlaylist)
    {
        var newPlaylist = new Playlist(playlistName);
        newPlaylist.AddSongs(oldPlaylist.list);
        _playlistLibrary.Add(newPlaylist);
        _playlistLibrary.Sort((p1, p2) => string.CompareOrdinal(p1.name, p2.name));
    }

    public static void LoadLibrary(string path)
    {
        var library = SaveSystem.Load(path);
        _playlistLibrary = library.GetPlaylists();
        _songLibrary = library.GetSongs();
    }
}

public static class SaveSystem
{
    public static void Save()
    {
        var library = new LibraryObject(Library.GetPlaylists(), Library.GetSongs());
        var bf = new BinaryFormatter();
        var fName = DateTime.Now.ToString(new CultureInfo("en-GB"));
        var directory = Application.persistentDataPath + "/Save/";

        fName = fName.Replace('/', '-').Replace(' ', '-').Replace(':', '-');
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        var path = directory + "/" + fName + ".sav";
        var filesInDir = Directory.GetFiles(directory);

        foreach (var f in filesInDir)
        {
            File.Delete(f);
        }

        var file = new FileStream(path, FileMode.Create);
        //Debug.Log("Saving library to " + path);
        bf.Serialize(file, library);
        file.Close();
        PlayerPrefs.SetString("path", path);
    }

    public static LibraryObject Load(string path)
    {
        if (File.Exists(path))
        {
            var bf = new BinaryFormatter();
            var file = new FileStream(path, FileMode.Open);
            var library = (LibraryObject)bf.Deserialize(file);
            file.Close();
            return library;
        }
        Debug.LogError("Save file not found in " + path);
        return null;
    }

    public static void PublishSongToLibrary(Song song)
    {
        if (!Library.UpdateSong(song))
            Library.AddSong(song);
        foreach (var s in Library.GetSongs())
        {
            Debug.Log(s.title);
        }
    }
}
