using UnityEngine;

public class OilPaintingScreenshot : MonoBehaviour
{
    Camera paintingCamera;
    Texture2D screenshotPainting;

    void Start()
    {
        paintingCamera = transform.GetChild(0).GetComponent<Camera>();
        PaintingScreenshotUpdate();
    }

    public void PaintingScreenshotUpdate()
    {
        if(!paintingCamera.gameObject.activeSelf)
            paintingCamera.gameObject.SetActive(true);

        RenderTexture renderTexture = new RenderTexture(1280, 720, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        paintingCamera.targetTexture = renderTexture;
        paintingCamera.Render();
        RenderTexture.active = renderTexture;

        screenshotPainting = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshotPainting.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height),0,0);
        screenshotPainting.Apply();

        int size = 720;

        Texture2D cropped = new Texture2D(size, size, TextureFormat.RGB24, false);

        cropped.SetPixels(screenshotPainting.GetPixels((1280 - size) / 2,0,size,size));
        cropped.Apply();

        GetComponent<Renderer>().material.mainTexture = cropped;

        RenderTexture.active = null;
        paintingCamera.targetTexture = null;
        paintingCamera.gameObject.SetActive(false);
    }
}
