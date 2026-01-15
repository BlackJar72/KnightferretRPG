using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursorOnHover : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorImege;
    [SerializeField] private int _heatPointX = 0;
    [SerializeField] private int _heatPointY = 0;
    // Start is called before the first frame update
   
    public void ChangeCursor()
    {
        Cursor.SetCursor(_cursorImege, new Vector2(_heatPointX, _heatPointY), CursorMode.Auto);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
