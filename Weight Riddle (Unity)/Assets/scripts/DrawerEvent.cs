using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawerEvent : MonoBehaviour
{
    [SerializeField]
    RectTransform _parentCanvasRectTransform;
    [SerializeField]
    RectTransform _drawerRectTransform;
    [SerializeField]
    TextMeshProUGUI ButtonText;

    Vector2 _basicSizeDrawer;

    [SerializeField]
    float _borderStart;

    float speed = 1500;

    bool IsGoingUp = false;

    // Start is called before the first frame update
    void Start()
    {
        _borderStart = 300;
        _drawerRectTransform.offsetMin = new Vector2(0, -_borderStart);
        _basicSizeDrawer = new Vector2(0, 0.85f * _borderStart);
        _drawerRectTransform.sizeDelta = _basicSizeDrawer;
    }

    public void toggleDrawer()
    {
        IsGoingUp = !IsGoingUp;
        ButtonText.text = IsGoingUp ? "⌄" : "^";
        StartCoroutine(SlideDrawerMenu());
    }

    IEnumerator SlideDrawerMenu()
    {
        Vector2 posToGo = IsGoingUp ? Vector2.zero : new Vector2(0, -_borderStart);

        while (_drawerRectTransform.offsetMin != posToGo)
        {
            _drawerRectTransform.offsetMin = Vector2.MoveTowards(_drawerRectTransform.offsetMin, posToGo, speed * Time.deltaTime);
            _drawerRectTransform.sizeDelta = _basicSizeDrawer;
            yield return null;
        }
    }
}
