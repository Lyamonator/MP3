//Author: Lyam Katz
//File Name: Player.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: A player

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    public class Player
    {
        //Store the 3 player textures
        public Texture2D textureStanding;
        public Texture2D textureJumping;

        //Store the player's location and boundary
        public Rectangle rectangle;
        public Vector2 position;

        //Store the stop moving speed
        public
        const float STOP = 0f;

        //Store the old x position of the player
        public float oldX;

        //Store if the player has stopped moving on the x axis
        bool xDone = false;

        //Store the new y value
        public float newY;

        //Store the normal friction and jumping friction
        public
        const float FRICTION = 0.033f;
        public
        const float JUMPING_FRICTION = 0.05f;

        //Store the texture data of the player textures
        public Color[] textureDataStanding;
        public Color[] textureDataJumping;

        //Store the possible player orientations
        public
        const int STANDING = 2;
        public
        const int JUMPING = 3;
        public
        const int MOVING = 1;

        //Store whether the player is moving left or not
        public bool left;

        //Store the number of collectables
        public int numKeys;
        public int numGems;

        //Store the screen height
        public
        const int SCREEN_HEIGHT = 1080;

        //Store the current player orientation
        static public int currentPlayerOrientation = STANDING;

        //Store gravity
        public
        const float GRAVITY = 9.8f / 60;

        //Store the base speed
        public static float baseSpeed = 7f;

        //Store the player's score
        public static int score;

        //Store the size of the command for calculating the score
        public static int commandSize;

        //Store the level scores
        public static double[] levelScores = new double[Game1.fileNames.Length];

        //Store the movements
        public
        const int DEFAULT = 1;
        public
        const int MOVE_RIGHT = 2;
        public
        const int MOVE_LEFT = 3;
        public
        const int JUMP_RIGHT = 4;
        public
        const int JUMP_LEFT = 5;
        public
        const int JUMP_ON_RIGHT = 6;
        public
        const int JUMP_ON_LEFT = 7;
        public int action = DEFAULT;

        //Store the ground heights
        float onGroundYJumping;
        float onGroundYStanding;

        //Store the player speeds
        public static Vector2 playerSpeed = new Vector2(STOP, STOP);

        //Pre: A new position
        //Post: None
        //Desc: Create a new player
        public Player(Vector2 position)
        {
            this.position = position;
            oldX = position.X;
            numGems = 0;
            numKeys = 0;
            score = 0;
        }

        //Pre: Content manager and graphics device
        //Post: None
        //Desc: Loads game resourses, sets position, and finds y value where player lands
        public void Load(ContentManager content, GraphicsDevice newGraphics)
        {
            //Artwork by me
            //Load the standing texture and store the pixel colors
            textureStanding = content.Load<Texture2D>("standingsmaller");
            textureDataStanding = new Color[textureStanding.Width * textureStanding.Height];
            textureStanding.GetData(textureDataStanding);

            //Load the jumping texture and store the pixel colors
            textureJumping = content.Load<Texture2D>("jumpingsmaller");
            textureDataJumping = new Color[textureJumping.Width * textureJumping.Height];
            textureJumping.GetData(textureDataJumping);

            //Calculate the ground heights
            onGroundYJumping = SCREEN_HEIGHT - textureJumping.Height - (SCREEN_HEIGHT - 9 * Game1.OBJECT_SIDE_LENGTH);
            onGroundYStanding = SCREEN_HEIGHT - textureStanding.Height - (SCREEN_HEIGHT - 9 * Game1.OBJECT_SIDE_LENGTH);

        }

        //Pre: None
        //Post: None
        //Desc: Update the player
        public void Update()
        {

            //Add gravity to the y speed
            playerSpeed.Y += GRAVITY;

            //Add friction to the x speed
            if (playerSpeed.X != STOP)
            {
                if (currentPlayerOrientation == JUMPING)
                {
                    playerSpeed.X += JUMPING_FRICTION * -Math.Sign(playerSpeed.X);
                }
                else
                {
                    playerSpeed.X += FRICTION * -Math.Sign(playerSpeed.X);
                }
            }

            //Update the player based on the movement or the default
            switch (action)
            {
                case DEFAULT:
                    //Check for a collision against a door
                    if (currentPlayerOrientation == JUMPING)
                    {
                        //Loop through the doors
                        for (int i = 0; i < Game1.doors.Count; i++)
                        {
                            //If the player collides with a door and have a key, delete it and remove 1 key from the player
                            if (Collision.IntersectsPixel(new Rectangle((int)Game1.doors[i].X, (int)Game1.doors[i].Y, Game1.OBJECT_SIDE_LENGTH, Game1.OBJECT_SIDE_LENGTH), Game1.doorData, rectangle, textureDataJumping) && numKeys > 0)
                            {
                                numKeys -= 1;
                                Game1.numLevelKeys--;
                                int x = (int)Math.Round(Game1.doors[i].X / Game1.OBJECT_SIDE_LENGTH);
                                int y = (int)Math.Round(Game1.doors[i].Y / Game1.OBJECT_SIDE_LENGTH);
                                if (Game1.levels[Game1.currentLevelIndex, y, x] == '4')
                                {
                                    Game1.levels[Game1.currentLevelIndex, y, x] = ' ';
                                }
                                Game1.doors.RemoveAt(i);

                            }
                        }

                    }
                    else
                    {
                        //Loop through the doors
                        for (int i = 0; i < Game1.doors.Count; i++)
                        {
                            //If the player collides with a door and have a key, delete it and remove 1 key from the player
                            if (Collision.IntersectsPixel(new Rectangle((int)Game1.doors[i].X, (int)Game1.doors[i].Y, Game1.OBJECT_SIDE_LENGTH, Game1.OBJECT_SIDE_LENGTH), Game1.doorData, rectangle, textureDataStanding) && numKeys > 0)
                            {
                                numKeys -= 1;
                                Game1.numLevelKeys--;
                                int x = (int)Math.Round(Game1.doors[i].X / Game1.OBJECT_SIDE_LENGTH);
                                int y = (int)Math.Round(Game1.doors[i].Y / Game1.OBJECT_SIDE_LENGTH);
                                if (Game1.levels[Game1.currentLevelIndex, y, x] == '4')
                                {
                                    Game1.levels[Game1.currentLevelIndex, y, x] = ' ';
                                }
                                Game1.doors.RemoveAt(i);
                            }
                        }
                    }
                    //Store whether or not the player collided
                    bool isCollided = false;

                    //Update the player rectangle based on the player's position
                    rectangle = new Rectangle((int)position.X, (int)(position.Y + 2.7), Game1.OBJECT_SIDE_LENGTH, Game1.OBJECT_SIDE_LENGTH);

                    //Calculate if the player collided or not using the updated rectangle
                    if (currentPlayerOrientation == JUMPING)
                    {
                        if (position.Y >= onGroundYJumping || Collision.ListToOne(Game1.walls, Game1.wallData, rectangle, textureDataJumping) || Collision.ListToOne(Game1.crates, Game1.crateData, rectangle, textureDataJumping) || (Collision.ListToOne(Game1.doors, Game1.doorData, rectangle, textureDataJumping) && numKeys <= 0))
                        {
                            isCollided = true;
                        }
                    }
                    else if (position.Y >= onGroundYStanding || Collision.ListToOne(Game1.walls, Game1.wallData, rectangle, textureDataStanding) || Collision.ListToOne(Game1.crates, Game1.crateData, rectangle, textureDataStanding) || (Collision.ListToOne(Game1.doors, Game1.doorData, rectangle, textureDataStanding) && numKeys <= 0))
                    {
                        isCollided = true;
                    }

                    //Store if the player has stopped moving on the y axis
                    bool yDone = false;

                    //Move the player on the y axis if it hasn't collided with anything and update yDone
                    if (!isCollided)
                    {
                        position.Y += playerSpeed.Y / 1.1f;
                    }
                    else
                    {

                        yDone = true;
                        playerSpeed.Y = STOP;

                    }

                    //Change the x speed to 0 if the player has traveled to the next x location and correct the location and update xDone
                    if (Math.Abs(position.X - oldX) > 89 && !xDone)
                    {
                        playerSpeed.X = STOP;
                        if (!left)
                        {
                            position.X = oldX + Game1.OBJECT_SIDE_LENGTH;
                        }
                        else
                        {
                            position.X = oldX - Game1.OBJECT_SIDE_LENGTH;
                        }
                        xDone = true;
                    }

                    //If the player has stopped moving, reset the player
                    if (xDone && yDone)
                    {
                        position.Y = newY;
                        xDone = false;
                        currentPlayerOrientation = STANDING;
                    }

                    //Move the player on the x axis by the x speed
                    position.X += playerSpeed.X / 4;

                    break;
                case JUMP_LEFT:
                    //Launch the player
                    playerSpeed.X = -baseSpeed / 1.11f;
                    playerSpeed.Y = -baseSpeed;
                    position.Y += playerSpeed.Y / 1.1f;//So that the player doesn't immediately intersect with the ground

                    //Change orientation
                    currentPlayerOrientation = JUMPING;

                    //Go back to default action next update
                    action = DEFAULT;

                    //Save the current x position which will become the old x position
                    oldX = position.X;

                    //The player is jumping left
                    left = true;
                    break;
                case JUMP_RIGHT:
                    //Launch the player
                    playerSpeed.X = baseSpeed / 1.11f;
                    playerSpeed.Y = -baseSpeed;
                    position.Y += playerSpeed.Y / 1.1f;//So that the player doesn't immediately intersect with the ground

                    //Change orientation
                    currentPlayerOrientation = JUMPING;

                    //Go back to default action next update
                    action = DEFAULT;

                    //Save the current x position which will become the old x position
                    oldX = position.X;

                    //The player is not jumping left
                    left = false;
                    break;
                case MOVE_LEFT:
                    //Launch the player
                    playerSpeed.X = -baseSpeed + 1;

                    //Change orientation
                    currentPlayerOrientation = MOVING;

                    //Go back to default action next update
                    action = DEFAULT;

                    //Save the current x position which will become the old x position
                    oldX = position.X;

                    //The player is moving left
                    left = true;
                    break;
                case MOVE_RIGHT:
                    //Launch the player
                    playerSpeed.X = baseSpeed - 1;

                    //Change orientation
                    currentPlayerOrientation = MOVING;

                    //Go back to the default action next update
                    action = DEFAULT;

                    //Save the current x position which will become the old x position
                    oldX = position.X;

                    //The player is not moving left
                    left = false;
                    break;
                case JUMP_ON_LEFT:
                    //Launch the player
                    playerSpeed.X = -baseSpeed;
                    playerSpeed.Y = -baseSpeed * 1.02f;
                    position.Y += playerSpeed.Y / 1.1f;//So that the player doesn't immediately intersect with the ground

                    //Change orientation
                    currentPlayerOrientation = JUMPING;

                    //Go back to the default action next update
                    action = DEFAULT;

                    //Save the current x position which will become the old x position
                    oldX = position.X;

                    //The player is jumping left
                    left = true;
                    break;
                case JUMP_ON_RIGHT:
                    //Launch the player
                    playerSpeed.X = baseSpeed;
                    playerSpeed.Y = -baseSpeed * 1.02f;
                    position.Y += playerSpeed.Y / 1.1f;//So that the player doesn't immediately intersect with the ground

                    //Change orientation
                    currentPlayerOrientation = JUMPING;

                    //Go back to the default action next update
                    action = DEFAULT;

                    //Save the current x position which will become the old x position
                    oldX = position.X;

                    //The player is jumping left
                    left = false;
                    break;
            }
        }
        //Pre: A spritebatch
        //Post: None
        //Desc: Draw the player
        public void Draw(SpriteBatch sprite)
        {
            //Draw the player using the current orientation's texture
            switch (currentPlayerOrientation)
            {

                case JUMPING:
                    //Draw the player jumping
                    sprite.Draw(
                        textureJumping, position, Color.Black);
                    break;
                default:
                    //Draw the player standing or moving
                    sprite.Draw(
                        textureStanding, position, Color.Black);
                    break;
            }
        }
    }
}