using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

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

public class ImageUploader : MonoBehaviour
{
    public void UploadImage(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToPNG();
        UnityWebRequest request = UnityWebRequest.PostWwwForm("your_api_endpoint", "POST");
        request.uploadHandler = new UploadHandlerRaw(imageBytes);
        request.uploadHandler.contentType = "image/png";
        StartCoroutine(SendRequest(request));
    }

    IEnumerator SendRequest(UnityWebRequest request)
    {
        yield return request.SendWebRequest();
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
            }
            else
            {
                Debug.LogError("Error parsing coordinate response.");
            }
        }
    }
}