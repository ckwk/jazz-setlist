using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StickTypeButton : MonoBehaviour
{
    public Image stickImage;
    public List<Sprite> stickTypes;
    public Sprite stickType;

    private void Start()
    {
        stickImage = gameObject.GetComponent<Image>();
        stickImage.sprite = stickTypes[0];
        stickType = stickImage.sprite;
    }

    public void SwitchStickType()
    {
        stickType =
            stickType != stickTypes.Last()
                ? stickTypes[stickTypes.IndexOf(stickType) + 1]
                : stickTypes[0];
        stickImage.sprite = stickType;
    }
}
