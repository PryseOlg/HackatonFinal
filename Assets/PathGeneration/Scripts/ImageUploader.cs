using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

[System.Serializable]
public class Coordinate
{
    public float x;
    public float y;

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}

[System.Serializable]
public class CoordinateResponse
{
    public List<Coordinate> coordinates;
}

public static class ImageUploader
{
    private static byte[] GetBytes(Texture2D image)
    {
        Texture2D savedTexture = image;
        Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.ARGB32, false);

        newTexture.SetPixels(0, 0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
        newTexture.Apply();
        return newTexture.EncodeToPNG();
    }

    public static List<Vector2> UploadImage(Texture2D image)
    {
        byte[] imageBytes = GetBytes(image);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "image.png", "image/png");
        UnityWebRequest request = UnityWebRequest.Post("http://localhost/upload-file", form);
     
        // Отправляем запрос синхронно
        request.SendWebRequest();

        // Проверяем результат запроса
        var time = DateTime.Now;

        while (!request.isDone && (DateTime.Now - time).Seconds < 10)
        {
            
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error uploading image: " + request.result);
            return new List<Vector2>();
        }
        
        string jsonResponse = request.downloadHandler.text;
        CoordinateResponse response = JsonUtility.FromJson<CoordinateResponse>(jsonResponse);
        
        if (response != null && response.coordinates != null)
        {
            foreach (Coordinate coordinate in response.coordinates)
            {
                Vector2 vector2 = coordinate.ToVector2();
                Debug.Log("Vector2: " + vector2);
            }
            return response.coordinates.Select(c => c.ToVector2()).ToList();
        }
        else
        {
            Debug.LogError("Error parsing coordinate response.");
            return new List<Vector2>();
        }
    }
}
