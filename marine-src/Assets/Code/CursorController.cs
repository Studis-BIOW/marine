using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite _pointing;
    [SerializeField] private Sprite _clicking;

    [SerializeField] private float _cursorSensivity = 1f;

    private Canvas _canvas;
    private RectTransform _canvasRect;

    public Vector3 ViewportPosition
    {
        get
        {
            float scale = this._canvas.scaleFactor;
            Vector2 res = this._canvas.renderingDisplaySize;
            Vector2 pos = this.transform.localPosition;
            return ((pos * scale) + (res * .5f)) / res;
        }
    }

    private Image _cursorImage;
    private bool _mouseFollow;

    private void Start()
    {
        this._canvas = this.GetComponentInParent<Canvas>();
        this._canvasRect = this._canvas.GetComponent<RectTransform>();
        this._cursorImage = this.GetComponentInChildren<Image>();
        this._mouseFollow = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        this.UpdateCursorPosition();
    }

    public void SetLock(bool shouldLock)
    {
        if (shouldLock) { this._cursorImage.sprite = this._clicking; }
        else { this._cursorImage.sprite = this._pointing; }

        this._mouseFollow = !shouldLock;
    }

    private void UpdateCursorPosition()
    {
        if (!this._mouseFollow) return;

        Vector3 clamp = this._canvasRect.rect.size / 2f;
        Vector3 delta = Input.mousePositionDelta * this._cursorSensivity;
        float x = Mathf.Clamp(this.transform.localPosition.x + delta.x, -clamp.x, clamp.x);
        float y = Mathf.Clamp(this.transform.localPosition.y + delta.y, -clamp.y, clamp.y);
        this.transform.localPosition = new Vector3(x, y, 0f);
    }
}
