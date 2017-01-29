// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ScreenShotUploader : MonoBehaviour
{
    public SmoothCamera2D InputCamera;
    public NetworkManager myNetworkManager;
    public bool isVisible = true;
    private float myDebounceTimer = 0.0f;

    void Awake()
    {
        // We're going to hide these objects in the windows player, but show them in the editor.
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            isVisible = false;
        }
        else
        {
            isVisible = true;
        }
        myNetworkManager = FindObjectOfType<NetworkManager>();
    }

    void Update()
    {
        float myTargetAlpha = 0.0f;
        if (isVisible && myNetworkManager.IsClientConnected())
        {
            myTargetAlpha = 1.0f;
        }

        myDebounceTimer += Time.deltaTime;
        if(myDebounceTimer > 60.0f)
        {
            myDebounceTimer = 0.0f;
        }

        SpriteBehavior.FadeAlphaToTarget(this.gameObject, 1f, myTargetAlpha);
    }

    IEnumerator SwitchToDisconnectedDisplay()
    {
        isVisible = false;
        PlayerPaintPaletteHandler[] palettes = FindObjectsOfType<PlayerPaintPaletteHandler>();
        foreach(var palette in palettes)
        {
            palette.isVisible = false;
        }
        InputCamera.isTakingScreenShot = true;
        yield return new WaitForSeconds(4);
    }

    IEnumerator SwitchToConnectedDisplay()
    {
        isVisible = true;
        PlayerPaintPaletteHandler[] palettes = FindObjectsOfType<PlayerPaintPaletteHandler>();
        foreach (var palette in palettes)
        {
            palette.isVisible = true;
        }
        InputCamera.isTakingScreenShot = false;
        yield return new WaitForSeconds(4);
    }

    // Uploads a png file to katesnow.work
    IEnumerator UploadPNG()
    {
        print("Generating screen shot!");

        //Animator animator = GetComponent<Animator>();
        //animator.SetBool("UploadingScreenShot", true);

        StartCoroutine(SwitchToDisconnectedDisplay());
        yield return new WaitForSeconds(1);

        // We should only read the screen after all rendering is complete 
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToPNG();
        Destroy(tex);

        // Create a Web Form
        var fileName = SystemInfo.deviceUniqueIdentifier.ToString() + ".png";
        var form = new WWWForm();
        form.AddField("frameCount", Time.frameCount.ToString());
        form.AddBinaryData("file", bytes, fileName, "image/png");

        // Upload to a cgi script
        var w = new WWW("http://katesnow.work/wp-content/communities_gallery/images/screenshot.php", form);
        yield return w;
        if (w.error != null)
        {
            print(w.error);
            Application.ExternalCall("debug", w.error);
        }
        else
        {
            print("Finished Uploading Screenshot");
        }

        StartCoroutine(SwitchToConnectedDisplay());

        //animator.SetBool("UploadingScreenShot", false);

        Application.OpenURL("http://katesnow.work/wp-content/communities_gallery/");
    }

    void OnTouchUp()
    {
        if(myDebounceTimer >= 2.0f)
        {
            myDebounceTimer = 0.0f;
            StartCoroutine(UploadPNG());
        }
    }

}