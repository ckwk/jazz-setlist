using static GameManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SongEntry : MonoBehaviour, IDragHandler
{
    private RectTransform myTransform,
        buttonTransform;
    private PlaylistPage _playlistPage;
    private LibraryPage _libraryPage;
    private Rect startRect;
    private Vector2 startPosition;

    public bool libraryEntry;
    public GameManager gm;
    public Song mySong;
    public Text title,
        bpm;
    public Image stickType;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        myTransform = gameObject.GetComponent<RectTransform>();
        buttonTransform = transform.GetChild(1).GetComponent<RectTransform>();
        startRect = buttonTransform.rect;
        startPosition = buttonTransform.anchoredPosition;
        if (!libraryEntry)
            _playlistPage = GameObject.Find("PlaylistPage").GetComponent<PlaylistPage>();
        else
            _libraryPage = GameObject.Find("LibraryPage").GetComponent<LibraryPage>();
    }

    public void CreateSongEntry(Song song)
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        mySong = song;
        title.text = mySong.title;
        bpm.text = mySong.bpm + " BPM";
        stickType.sprite = gm.stickTypes[mySong.stickType];
    }

    public void BtnMove(int dir)
    {
        var myIndex = Library.GetPlaylist(currentPlaylist).list.IndexOf(mySong);
        var moveToIndex = myIndex + dir;

        if (moveToIndex == -1 || moveToIndex == Library.GetPlaylist(currentPlaylist).list.Count)
            return;

        Library.GetPlaylist(currentPlaylist).list.Remove(mySong);
        Library.GetPlaylist(currentPlaylist).list.Insert(moveToIndex, mySong);

        var aboveEntry = _playlistPage.songEntries[moveToIndex];
        var anchoredPos = myTransform.anchoredPosition;
        aboveEntry.anchoredPosition = anchoredPos;
        anchoredPos = new Vector2(anchoredPos.x, anchoredPos.y - (_playlistPage.offset * dir));
        myTransform.anchoredPosition = anchoredPos;

        _playlistPage.songEntries.Remove(myTransform);
        _playlistPage.songEntries.Insert(moveToIndex, myTransform);
    }

    public void BtnSong()
    {
        gm.SwitchPage(mySong, true);
    }

    public void BtnEdit()
    {
        if (currentPage == "song")
        {
            gm.SwitchPage(mySong, true);
        }

        if (!libraryEntry)
        {
            gm.SwitchPage(mySong, false);
            return;
        }

        if (!currentPlaylist.list.Contains(mySong))
        {
            currentPlaylist.list.Add(mySong);
        }
    }

    public void BtnDelete()
    {
        if (libraryEntry)
            _libraryPage.Remove(this);
        else
            _playlistPage.Remove(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var distance = eventData.position.x - eventData.pressPosition.x;
        distance = Mathf.Clamp(distance, -Screen.width / 10f, 0);
        buttonTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            startRect.width + distance
        );
        buttonTransform.anchoredPosition = new Vector2(
            startPosition.x + distance / 2,
            buttonTransform.anchoredPosition.y
        );
    }
}
