using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
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
    public static List<Vector2> UploadImage(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToPNG();
        UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost/uploadfile", "POST");
        request.uploadHandler = new UploadHandlerRaw(imageBytes);
        request.uploadHandler.contentType = "image/png";
        
        // Отправляем запрос синхронно
        request.SendWebRequest();

        // Проверяем результат запроса
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error uploading image: " + request.error);
        }
        else
        {
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
                
            }
        }
        return new();
    }
    
}