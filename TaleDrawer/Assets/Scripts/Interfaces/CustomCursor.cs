using UnityEngine;
using UnityEngine.UI;
public class CustomCursor : MonoBehaviour
{
    public RectTransform cursorImage;
    Vector2 position = Vector2.zero;
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorImage.parent.GetComponent<RectTransform>(),InputManager.instance.mouseInput,null,out position);
    }
}
