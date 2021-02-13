//Author: Lyam Katz
//File Name: Crate.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: A crate

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    public class Crate
    {
        //Store the crate's position
        public Vector2 cratePosition;

        //Store the old x position
        public float oldX;

        //Store the position of the crate last update to see if it moved
        public Vector2 oldPosition;

        //Store the crate speeds
        public Vector2 crateSpeeds = new Vector2(Player.STOP, Player.STOP);

        //Store the crate boundary rectangle
        public Rectangle crateRectangle;

        //Store if the current crate being updates is itself for collision purposes
        public bool isSelf = false;

        //Pre: A new position
        //Post: None
        //Desc: Create a new crate
        public Crate(Vector2 cratePosition)
        {
            this.cratePosition = cratePosition;
            oldX = cratePosition.X;
            oldPosition = cratePosition;
        }

        //Pre: A bool representing if the crate is being pushed right or left
        //Post: None
        //Desc: Update the crate
        public void UpdateCrate(bool isRight)
        {
            //This crate is itself
            isSelf = true;

            //The old position is the current position before the position gets updated
            oldPosition = cratePosition;

            //Add gravity to the y speed
            crateSpeeds.Y += Player.GRAVITY;

            //Check if the crate should stop moving on the x axis based on which direction it's moving
            if (isRight)
            {
                //If the crate has passed the right x value, reset it and stop the crate from moving
                if (cratePosition.X - oldX > 83)
                {
                    cratePosition.X = oldX + 83;
                    crateSpeeds.X = Player.STOP;
                }
            }
            else
            {
                //If the crate has passed the right x value, reset it and stop the crate from moving
                if (oldX - cratePosition.X > 83)
                {
                    cratePosition.X = oldX - 83;
                    crateSpeeds.X = Player.STOP;
                }
            }
            //Update the crate rectangle
            crateRectangle = new Rectangle((int)cratePosition.X, (int)cratePosition.Y, Game1.OBJECT_SIDE_LENGTH, Game1.OBJECT_SIDE_LENGTH);

            //Check if the crate collided
            if (cratePosition.Y >= Player.SCREEN_HEIGHT - Game1.crateTexture.Height - (Player.SCREEN_HEIGHT - 9 * Game1.OBJECT_SIDE_LENGTH) || Collision.ListToOne(Game1.walls, crateRectangle) || Collision.ListToOne(Game1.doors, crateRectangle) || Collision.ListToOne(Game1.crates, Game1.crateData, crateRectangle, Game1.crateData) || Collision.ListToOne(Game1.spikes, crateRectangle))
            {
                //Make the crate 1 pixel inside the object it collided with to prevent the crate from going up and down
                cratePosition.Y = (int)Math.Round(cratePosition.Y / Game1.OBJECT_SIDE_LENGTH) * Game1.OBJECT_SIDE_LENGTH + 1;

                //Reset the y speed
                crateSpeeds.Y = Player.STOP;
            }
            else
            {
                //Move down the crate using gravity
                cratePosition.Y += crateSpeeds.Y / 1.1f;
            }

            //Move the crate on the x axis
            cratePosition.X += crateSpeeds.X / 4;

            //Collect any gems the crate touches
            for (int i = 0; i < Game1.gems.Count; i++)
            {
                if (crateRectangle.Intersects(new Rectangle((int)Game1.gems[i].X, (int)Game1.gems[i].Y, Game1.OBJECT_SIDE_LENGTH, Game1.OBJECT_SIDE_LENGTH)))
                {
                    //Delete the gem from the array if it's not already replaced and increase the number of gems the user collected by one
                    int x = (int)Math.Round(Game1.gems[i].X / Game1.OBJECT_SIDE_LENGTH);
                    int y = (int)Math.Round(Game1.gems[i].Y / Game1.OBJECT_SIDE_LENGTH);
                    if (Game1.levels[Game1.currentLevelIndex, y, x] == '6')
                    {
                        Game1.levels[Game1.currentLevelIndex, y, x] = ' ';
                    }
                    Game1.player.numGems++;
                    Game1.gems.RemoveAt(i);
                }
            }

            //Collect any keys the crate touches
            for (int i = 0; i < Game1.keys.Count; i++)
            {
                if (crateRectangle.Intersects(new Rectangle((int)Game1.keys[i].X, (int)Game1.keys[i].Y, Game1.OBJECT_SIDE_LENGTH, Game1.OBJECT_SIDE_LENGTH)))
                {
                    //Delete the key from the array if it's not already replaced and increase the number of keys the user collected by one
                    int x = (int)Math.Round(Game1.gems[i].X / Game1.OBJECT_SIDE_LENGTH);
                    int y = (int)Math.Round(Game1.gems[i].Y / Game1.OBJECT_SIDE_LENGTH);
                    if (Game1.levels[Game1.currentLevelIndex, y, x] == '7')
                    {
                        Game1.levels[Game1.currentLevelIndex, y, x] = ' ';
                    }
                    Game1.player.numKeys++;
                    Game1.keys.RemoveAt(i);
                }
            }

            //Launch the crate if it intersects with the player
            if (Player.currentPlayerOrientation == Player.MOVING && Collision.IntersectsPixel(Game1.player.rectangle, Game1.player.textureDataStanding, crateRectangle, Game1.crateData) && Math.Abs(cratePosition.Y - Game1.player.position.Y) < 5)
            {
                //Re-adjust the x value and launch the crate
                if (isRight)
                {
                    cratePosition.X += 7;
                    crateSpeeds.X = Player.baseSpeed - 1;
                }
                else
                {
                    cratePosition.X -= 7;
                    crateSpeeds.X = -Player.baseSpeed + 1;
                }
                //Store the current x value which will become the old x value
                oldX = cratePosition.X;
            }
            //This is no longer the crate being updated
            isSelf = false;
        }
        //Pre: A spritebatch
        //Post: None
        //Desc: Draw the crate
        public void DrawCrate(SpriteBatch sprite)
        {
            sprite.Draw(
                Game1.crateTexture, cratePosition, Color.White);
        }
    }
}