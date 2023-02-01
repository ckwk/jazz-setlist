using static GameManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaylistEntry : MonoBehaviour, IDragHandler
{
    public RectTransform buttonTransform;
    public Playlist playlist;
    public Text txtName;
    private PlaylistLibraryPage _playlistLibraryPage;
    private GameManager gm;
    private Vector2 startPosition;
    private Rect startRect;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        startRect = buttonTransform.rect;
        startPosition = buttonTransform.anchoredPosition;
        _playlistLibraryPage = GameObject
            .Find("PlaylistLibraryPage")
            .GetComponent<PlaylistLibraryPage>();
    }

    public void CreatePlaylistEntry(Playlist p)
    {
        playlist = p;
        txtName.text = playlist.name;
    }

    public void BtnPlaylist()
    {
        currentPlaylist = Library.GetPlaylist(playlist);
        gm.SwitchPage("playlist", false);
    }

    public void BtnDelete()
    {
        _playlistLibraryPage.Remove(this);
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
