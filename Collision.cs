//Author: Lyam Katz
//File Name: Collision.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: A class of collision methods

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PASS4
{
    class Collision
    {
        //https://youtu.be/asU7afngQ8U
        //Pre: Boundaries and color data of both objects
        //Post: True/false boolean representing if there is a collision or not
        //Desc: Check for a pixel precise collision
        //Compare all overlapping pixels. If one of the sets of pixels being compared are both not transparent, there is a collision. If not, there is no collision
        public static bool IntersectsPixel(Rectangle rect1, Color[] data1, Rectangle rect2, Color[] data2)
        {
            int top = MathHelper.Max(rect1.Top, rect2.Top);
            int bottom = MathHelper.Min(rect1.Bottom, rect2.Bottom);
            int left = MathHelper.Max(rect1.Left, rect2.Left);
            int right = MathHelper.Min(rect1.Right, rect2.Right);

            for (int y = top; y < bottom; y++)
                for (int x = left; x < right; x++)
                {
                    Color colour1 = data1[(x - rect1.Left) + (y - rect1.Top) * rect1.Width];
                    Color colour2 = data2[(x - rect2.Left) + (y - rect2.Top) * rect2.Width];

                    if (colour1.A != 0 && colour2.A != 0) return true;
                }
            return false;
        }
        //Pre: A valid list of crates, the crate texture data, a rectangle, and the recatangle's texture data
        //Post: A bool representing if there is a collision or not
        //Desc: Check if there is a collision between a crate and another object
        public static bool ListToOne(List<Crate> rects, Color[] textureDataList, Rectangle rect, Color[] data)
        {
            //Loop through the crates
            for (int i = 0; i < rects.Count; i++)
            {
                //Check if a crate is intersecting with another crate.
                if(!rects[i].isSelf && IntersectsPixel(new Rectangle((int)rects[i].cratePosition.X, (int)rects[i].cratePosition.Y, 90, 90), textureDataList, rect, data))
                {
                    return true;
                }
            }
            return false;
        }
        //Pre: A valid list of rectangles, the rectangles texture data, a rectangle, and the recatangle's texture data
        //Post: A bool representing if there is a collision or not
        //Desc: Check if there is a collision between an object and another object
        public static bool ListToOne(List<Vector2> rects, Color[] textureDataList, Rectangle rect, Color[] data)
        {
            //Loop through the list of objects
            for (int i = 0; i < rects.Count; i++)
            {
                //Check if an object from the list is colliding with the single object
                if (IntersectsPixel(new Rectangle((int)rects[i].X, (int)rects[i].Y, 90, 90), textureDataList, rect, data))
                {
                    return true;
                }
            }
            return false;
        }
        //Pre: A valid list of rectangles and a rectangle
        //Post: A bool representing if there is a collision or not
        //Desc: Check if there is a boundary collision between an object and another object
        public static bool ListToOne(List<Vector2> rects, Rectangle rect)
        {
            //Loop through the list of objects
            for (int i = 0; i < rects.Count; i++)
            {
                //Check if an object from the list is colliding with the single object
                if (new Rectangle((int)rects[i].X, (int)rects[i].Y, 90, 90).Intersects(rect))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
