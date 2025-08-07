using UnityEngine;
using Vuplex.WebView;

public class ManualKeyboardInput : MonoBehaviour
{
    public CanvasWebViewPrefab canvasWebViewPrefab;

    void Start()
    {
        canvasWebViewPrefab.KeyboardEnabled = false;

        var listener = Vuplex.WebView.Internal.NativeKeyboardListener.Instantiate();

        listener.KeyDownReceived += (sender, args) =>
        {
            var keyboardCapable = canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
            if (keyboardCapable != null)
                keyboardCapable.KeyDown(args.Key, args.Modifiers);
            else
                canvasWebViewPrefab.WebView.SendKey(args.Key);
        };

        listener.KeyUpReceived += (sender, args) =>
        {
            var keyboardCapable = canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
            keyboardCapable?.KeyUp(args.Key, args.Modifiers);
        };
    }
}