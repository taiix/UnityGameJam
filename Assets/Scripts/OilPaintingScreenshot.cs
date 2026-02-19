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

        RenderTexture renderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        paintingCamera.targetTexture = renderTexture;
        paintingCamera.Render();
        RenderTexture.active = renderTexture;

        screenshotPainting = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false,
            QualitySettings.activeColorSpace == ColorSpace.Linear);
        screenshotPainting.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height),0,0);
        screenshotPainting.Apply();
        GetComponent<Renderer>().material.mainTexture = screenshotPainting;

        RenderTexture.active = null;
        paintingCamera.targetTexture = null;
        paintingCamera.gameObject.SetActive(false);
    }
}
