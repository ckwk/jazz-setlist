using UnityEngine;
using UnityEngine.UI;

public class Section : MonoBehaviour
{
    public Dropdown instrumentDropdown;
    public Image sectionImage;
    public Color bassColour,
        drumsColour,
        guitarColour,
        pianoColour,
        saxColour,
        vocalsColour;
    public SongPage parent;
    public InputField sectionTitle,
        length,
        sauce;

    private const int NONE = 6,
        NONE_2 = 7;

    public void ChangeSectionColour()
    {
        sectionImage.color = instrumentDropdown.value switch
        {
            0 => bassColour,
            1 => drumsColour,
            2 => guitarColour,
            3 => pianoColour,
            4 => saxColour,
            5 => vocalsColour,
            _ => sectionImage.color
        };
    }

    public void SetDataTo(Part part)
    {
        sectionTitle.text = part.name;
        instrumentDropdown.value = part.instrument;
        length.text = part.length;
        sauce.text = part.sauce;
        ChangeSectionColour();
    }

    public void InputSectionTitle(bool save)
    {
        if (parent.songTitle.text.Trim() == "")
            return;
        parent.currentSong.parts[parent.sections.IndexOf(this)].name = sectionTitle.text;
        if (save)
            SaveSystem.PublishSongToLibrary(parent.currentSong);
    }

    public void InputSectionInstrument(bool save)
    {
        if (parent.songTitle.text.Trim() == "")
            return;
        parent.currentSong.parts[parent.sections.IndexOf(this)].instrument =
            instrumentDropdown.value;
        if (save)
            SaveSystem.PublishSongToLibrary(parent.currentSong);
    }

    public void InputSectionLength(bool save)
    {
        if (parent.songTitle.text.Trim() == "")
            return;
        parent.currentSong.parts[parent.sections.IndexOf(this)].length = length.text;
        if (save)
            SaveSystem.PublishSongToLibrary(parent.currentSong);
    }

    public void InputSectionSauce(bool save)
    {
        if (parent.songTitle.text.Trim() == "")
            return;
        parent.currentSong.parts[parent.sections.IndexOf(this)].sauce = sauce.text;
        if (save)
            SaveSystem.PublishSongToLibrary(parent.currentSong);
    }
}
