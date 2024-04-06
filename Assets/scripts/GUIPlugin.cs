using System.IO;
using UnityEngine;
using UnityEditor;

public class GUIPlugin : EditorWindow
{
    string myString = "Hello";
    Texture2D uploadedImage; // Переменная для хранения загруженного изображения
    ImageUploader imageUploader; // Экземпляр класса ImageUploader

    [MenuItem("Window/GUI")]
    public static void ShowWindow ()
    {
        GetWindow<GUIPlugin>("GUI");
    }

    void OnGUI ()
    {
        GUILayout.Label("This is a label.", EditorStyles.boldLabel);

        myString = EditorGUILayout.TextField("Name", myString);

        if (GUILayout.Button("Press me"))
        {
            Debug.Log ("Button was pressed");
        }

        // Кнопка для загрузки изображения
        if (GUILayout.Button("Upload Image"))
        {
            // Вызываем метод для открытия файла изображения
            string imagePath = EditorUtility.OpenFilePanel("Select Image", "", "png");
            if (!string.IsNullOrEmpty(imagePath))
            {
                // Загружаем изображение из файла
                byte[] fileData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);

                // Сохраняем загруженное изображение
                uploadedImage = texture;

                // Инициализируем экземпляр ImageUploader, если он еще не был инициализирован
                if (imageUploader == null)
                {
                    imageUploader = new ImageUploader();
                }

                // Загружаем изображение с помощью ImageUploader
                imageUploader.UploadImage(uploadedImage);
            }
        }
    }
}
