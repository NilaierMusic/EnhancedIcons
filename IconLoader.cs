using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace EnhancedIcons
{
    public static class IconLoader
    {
        private const string ResourcePathPrefix = "EnhancedIcons.image.";

        public static Sprite LoadCustomIcon(string imagePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"{ResourcePathPrefix}{imagePath}";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        var texture = new Texture2D(2, 2);
                        texture.LoadImage(memoryStream.ToArray());

                        var customIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        Debug.Log($"Custom icon loaded successfully: {imagePath}");
                        return customIcon;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading custom icon: {imagePath}. Exception: {ex}");
                return null;
            }
        }
    }
}