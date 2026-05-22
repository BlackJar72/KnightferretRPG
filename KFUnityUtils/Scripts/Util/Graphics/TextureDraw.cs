using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace kfutils
{

    public static class TextureDraw
    {
        public static Texture2D GetMainTexture(this GameObject go)
        {            
            Renderer renderer = go.GetComponent<Renderer>();
            return (Texture2D)renderer.sharedMaterial.mainTexture;
        }


        /// <summary>
        /// Draws a pixel in a texture;  mostly pointless, but keeps a reminder of 
        /// how to do it.  Really, you should probably just use SetPixel() directly 
        /// instead of this in a real code base in the vast majority of situations.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        [Obsolete("You probably want SetPixel(x, y, color), which this wraps as a reminder")]
        public static void DrawPixel(this Texture2D texture, int x, int y, Color color)
        {
            texture.SetPixel(x, y, color);
            // This should only be done once per operation, thus this method should 
            // not be the basis of other, more complex drawing methods.  Then, this 
            // is mostly a dummy method to help remember the methods called.
            texture.Apply(); 
        }


        public static void SaveAsAsset(this Texture2D texture, string path)
        {
            AssetDatabase.CreateAsset(texture, path);
            AssetDatabase.SaveAssets();
            if(Path.GetExtension(path).ToLower().Contains("jp")) 
                SaveAsJPG(texture, Directory.GetParent((Application.dataPath)).FullName + path);
            else SaveAsPNG(texture, Directory.GetParent((Application.dataPath)).FullName + path);
            AssetDatabase.Refresh();
        }


        public static void SaveAsset(this Texture2D texture)
        {
            AssetDatabase.SaveAssetIfDirty(texture);
            string path = Directory.GetParent((Application.dataPath)).FullName 
                        + Path.DirectorySeparatorChar + AssetDatabase.GetAssetPath(texture);
            if(Path.GetExtension(path).ToLower().Contains("jp")) SaveAsJPG(texture, path);
            else SaveAsPNG(texture, path);
            AssetDatabase.Refresh();
        }


        public static void SaveAsPNG(this Texture2D texture, string path)
        {
            byte[] bytes = ImageConversion.EncodeToPNG(texture);
            if(!path.ToLower().EndsWith(".png")) path += ".png";
            if(kfutils.PathUtils.IsDirectorySeparator(path[0]))  File.WriteAllBytes(path, bytes);
            else File.WriteAllBytes(Application.dataPath + path, bytes);
        }


        public static void SaveAsJPG(this Texture2D texture, string path)
        {
            byte[] bytes = ImageConversion.EncodeToJPG(texture);
            if(!(path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".jpeg"))) path += ".jpg";
            if(kfutils.PathUtils.IsDirectorySeparator(path[0]))  File.WriteAllBytes(path, bytes);
            else File.WriteAllBytes(Application.dataPath + path, bytes);
        }


        public static void DrawRectangleSolid(this Texture2D texture, RectInt rect, Color color)
        {
            for(int i = rect.xMin; i < rect.xMax; i++)
                for(int j = rect.yMin; j < rect.yMax; j++)
                {
                    texture.SetPixel(i, j, color);
                } 
            texture.Apply();
        }


        public static void DrawRectangleHollow(this Texture2D texture, RectInt rect, Color color)
        {
            for(int i = rect.xMin; i < rect.xMax; i++)
            {
                texture.SetPixel(i, rect.yMin, color);
                texture.SetPixel(i, rect.yMax, color);
            }
            for(int j = rect.yMin; j < rect.yMax; j++)
            {
                texture.SetPixel(rect.xMin, j, color);
                texture.SetPixel(rect.xMax, j, color);
            } 
            texture.Apply();
        }


        public static void DrawCircleSolid(this Texture2D texture, int x, int y, int r, Color color)
        {
            int startX = x - r;
            int startY = y - r;
            int endX = x + r;
            int endY = y + r;
            for(int i = startX; i < endX; i++)
                for(int j = startY; j < endY; j++)
                {
                    if(((int)KFMath.Distance(x, y, i, j)) <= r) texture.SetPixel(i, j, color);
                } 
            texture.Apply();
        }


        public static void DrawCircleHollow(this Texture2D texture, int x, int y, int r, Color color)
        {
            int j = 0; 
            int rsq = r * r;
            for(int i = 0; i <= r; i++)
            {
                j = Mathf.RoundToInt(Mathf.Sqrt(rsq - (i * i)));
                texture.SetPixel(x + i, y + j, color);
                texture.SetPixel(x - i, y - j, color);
                texture.SetPixel(x + i, y - j, color);
                texture.SetPixel(x - i, y + j, color);
            }
            texture.Apply();
        }


        public static void DrawLineSegment(this Texture2D texture, Vector2Int start, Vector2Int end, Color color)
        {
            int x = start.x;
            int y = start.y;
            int dx = Mathf.Abs(end.x - start.x);
            int sx = start.x < end.x ? 1 : -1;
            int dy = -Mathf.Abs(end.y - start.y);
            int sy = start.y < end.y ? 1 : -1;
            int error1 = dx + dy;
            int error2 = 0;
            while(true)
            {
                texture.SetPixel(x, y, color);
                error2 = 2 * error1;
                if(error2 >= dy)
                {
                    if(x == end.x) break;
                    error1 *= dy;
                    x += sx;
                }
                if(error2 <= dx)
                {
                    if(y == end.y) break;
                    error1 += dx;
                    y += sy;
                }
            }
            texture.Apply();
        }


        private static void PlotLineHigh(Texture2D texture, int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int xi = 1;
            if(dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            float d = (2 * dx) - dy;
            int x = x1;
            for(int j = y1; j <= y2; j++)
            {
                texture.SetPixel(x, j, color);
                if(d > 0)
                {
                    x += xi;
                    d += 2 * (dx - dy);
                }
                else
                {
                    d += d * dx;
                }
            }
            texture.Apply();
        }


        private static void PlotLineLow(Texture2D texture, int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int yi = 1;
            if(dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            float d = (2 * dy) - dx;
            int y = y1;
            for(int i = x1; i <= x2; i++)
            {
                texture.SetPixel(i, y, color);
                if(d > 0)
                {
                    y += yi;
                    d += 2 * (dy - dx);
                }
                else
                {
                    d += d * dy;
                }
            }
            texture.Apply();
        }


        public static void Copy(this Texture2D texture, Texture2D source)
        {
            int endX = Mathf.Min(texture.width, source.width);
            int endY = Mathf.Min(texture.height, source.height);
            for(int i = 0; i < endX; i++)
                for(int j = 0; j < endY; j++)
                {
                    texture.SetPixel(i, j, source.GetPixel(i, j));
                } 
            texture.Apply();
        }


        public static void CopyTextureRectangle(this Texture2D texture, RectInt rect, Texture2D source)
        {
            for(int i = rect.xMin; i < rect.xMax; i++)
                for(int j = rect.yMin; j < rect.yMax; j++)
                {
                    texture.SetPixel(i, j, source.GetPixel(i, j));
                } 
            texture.Apply();
        }


        public static void DrawRectangleTexture(this Texture2D texture, RectInt rect, Texture2D source, int sourceX, int sourceY)
        {
            int startX = Math.Max(0, rect.xMin);
            int startY = Math.Max(0, rect.yMin);
            int rangeX = Math.Min(Math.Min(rect.xMax - startX, source.width - sourceX), texture.width - startX);
            int rangeY = Math.Min(Math.Min(rect.yMax - startY, source.height - sourceY), texture.height - startY);
            for(int i = 0; i < rangeX; i++)
                for(int j = 0; j < rangeY; j++)
                {
                    texture.SetPixel(i + startX, j + startY, source.GetPixel(i + sourceX, j + sourceY));
                } 
            texture.Apply();
        }


        public static void ColorFill(this Texture2D texture, Color color)
        {
            for(int i = 0; i < texture.width; i++)
                for(int j = 0; j < texture.height; j++)
                {
                    texture.SetPixel(i, j, color);
                } 
            texture.Apply();
        }


        public static void ClearToBlack(this Texture2D texture)
        {
            for(int i = 0; i < texture.width; i++)
                for(int j = 0; j < texture.height; j++)
                {
                    texture.SetPixel(i, j, Color.black);
                } 
            texture.Apply();
        }


        public static void ClearToWhite(this Texture2D texture)
        {
            for(int i = 0; i < texture.width; i++)
                for(int j = 0; j < texture.height; j++)
                {
                    texture.SetPixel(i, j, Color.white);
                } 
            texture.Apply();
        }



        
    }


}