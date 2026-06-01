using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FUtility
{
    internal static class TextureLoadCompat
    {
        private static MethodInfo loadImageByteArrayMethod;

        public static bool LoadImage(Texture2D texture, byte[] data)
        {
            if (texture == null || data == null)
                return false;

            if (loadImageByteArrayMethod == null)
            {
                loadImageByteArrayMethod = ResolveLoadImageByteArrayMethod();
            }

            if (loadImageByteArrayMethod == null)
            {
                Log.Warning("Could not find UnityEngine.ImageConversion.LoadImage(Texture2D, byte[])");
                return false;
            }

            return (bool)loadImageByteArrayMethod.Invoke(null, new object[] { texture, data });
        }

        private static MethodInfo ResolveLoadImageByteArrayMethod()
        {
            var type = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");

            if (type == null)
            {
                Log.Warning("Could not find UnityEngine.ImageConversion type.");
                return null;
            }

            return type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(method =>
                {
                    if (method.Name != "LoadImage")
                        return false;

                    var parameters = method.GetParameters();

                    return parameters.Length >= 2
                        && parameters[0].ParameterType == typeof(Texture2D)
                        && parameters[1].ParameterType == typeof(byte[]);
                });
        }
    }
}
