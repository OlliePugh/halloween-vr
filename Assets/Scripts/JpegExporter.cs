using System.IO;
using UnityEngine;

public class JpegExporter : MonoBehaviour
{

    public int TicksPerSecond = 10;
    public int FileCounter = 0;

    private float _t;
 
    private void LateUpdate()
    {
        float dur = 1f / this.TicksPerSecond;
        _t += Time.deltaTime;
        while(_t >= dur)
        {
            _t -= dur;
            CamCapture();
        }
    }
 
    void CamCapture()
    {
        Camera Cam = GetComponent<Camera>();
 
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;
 
        Cam.Render();
 
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToJPG();
        Destroy(Image);
 
        File.WriteAllBytes(Application.dataPath + "/../CameraFeed/" + FileCounter + ".jpg", Bytes);
        FileCounter++;
    }
}
