//Author: Lyam Katz
//File Name: Game1.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: Allow the user to play a game of path-finder

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;

namespace PASS4
{
    public class Game1 : Game
    {
        //Store the steam writer and reader
        private StreamReader inFile;
        private StreamWriter outFile;

        //Store the game states
        const int MENU = 0;
        const int HIGHSCORES = 1;
        const int INSTRUCTIONS = 2;
        const int GAME_ENTRY = 3;
        const int GAME_EXECUTION = 4;
        const int RESULTS = 5;
        const int NAME_ENTRY = 6;

        //Store the side length of each object
        public
        const int OBJECT_SIDE_LENGTH = 90;

        //Store the user entered input
        private string userInput = "";

        //Store whether or not the play is on a collectable or goal
        private bool onGoal = false;
        private bool onGem = false;
        private bool onKey = false;

        //Store the calculated list of spaces the pointer needs to be under
        private List<int> space = new List<int>();

        //Store whether or not the high score lists are sorted
        private bool isSorted = false;

        //Store the feedback
        private string feedBack = "";

        //Store whether or not the level was cleared
        private bool levelCleared;

        //Store whether or not the crate is being pushed to the right
        private bool isRight;

        //Store the level file names
        public static string[] fileNames = new string[] {
            "Level1.txt",
            "Level2.txt",
            "Level3.txt",
            "Level4.txt"
        };

        //Store the calculated current level index
        public static int currentLevelIndex = -1;

        //Store the ascii offset
        const int CHAR_OFFSET = 48;

        //Store the levels
        public static char[,,] levels;

        //Store the user entered command
        private string command = "";

        //Store the max bar width
        const int MAX_BAR_WIDTH = 700;

        //Store the calculated prograss bar multiple
        private int progressBarMultiple;

        //Store the calculated number of collectables of the current level
        private int numLevelGems = 0;
        public static int numLevelKeys = 0;

        //Store the locations of everything except for crates and the player
        public static List<Vector2> walls = new List<Vector2>();
        public static List<Vector2> doors = new List<Vector2>();
        public static List<Vector2> gems = new List<Vector2>();
        private Vector2 goal;
        public static List<Vector2> keys = new List<Vector2>();
        public static List<Vector2> spikes = new List<Vector2>();

        //Store the crates
        public static List<Crate> crates = new List<Crate>();

        //Store the sorted high scores
        private List<HighScore> numSort = new List<HighScore>();
        private List<HighScore> nameSort = new List<HighScore>();

        //Store the found high score
        private string search;

        //Store the fonts
        private SpriteFont arial;
        private SpriteFont monospace;

        //Store the keyboard states
        private KeyboardState oldKeyboardState;
        private KeyboardState currentKeyboardState;

        //Store whether or not the user is pushing a crate
        private bool isPushing;

        //Store the command queue
        private CommandQueue commands;

        //Store the menu options
        const int MNU_PLAY = 1;
        const int MNU_INSTRUCTIONS = 3;
        const int MNU_EXIT = 4;
        const int MNU_HIGH_SCORES = 2;

        //Store if the user is clicking shift or not
        private bool shift = false;

        //Store the number of menu options
        const int NUM_OPTIONS = 4;

        //Store the offset of the pointer
        const int POINTER_MOVE_OFFSET = 10;

        //Store the current menu option, play
        public int menuChoice = MNU_PLAY;

        //Store the current game state
        private int gameState = MENU;

        //Store the graphics device manager
        private GraphicsDeviceManager _graphics;

        //Store the spritebatch
        private SpriteBatch _spriteBatch;

        //Store the play location and texture
        private Vector2 playLocation;
        private Texture2D playTexture;

        //Store the exit location and texture
        private Vector2 exitLocation;
        private Texture2D exitTexture;

        //Store the instructions location and texture
        private Vector2 instructionsLocation;
        private Texture2D instructionsTexture;

        //Store the pointer location and texture
        private Vector2 pointerLocation;
        private Texture2D pointerTexture;

        //Store the high score location and texture
        private Vector2 highScoresLocation;
        private Texture2D highScoresTexture;

        //Store the bar texture
        private Texture2D barTexture;

        //Store whether or not the legend should be visible
        private bool showLegend = true;

        //Store how many spaces should be placed before the action pointer
        private string spaces = "";

        //Store the command legend
        private string commandLegend = @"
        Commands:
            d = move right, a = move left, c = collect object (on the space occupied),
            e = jump right, q = jump left, + = push right, - = push left, s # = Start a loop that will iterate # times,
            f = finish of loop Example Loop sequence: s3edcf: which(jumps right, moves right, collects object) 3 times ";

        //Store all the colors of the objects in arrays
        public static Color[] wallData;
        public static Color[] crateData;
        public static Color[] doorData;
        public static Color[] spikesData;

        //Store the textures of game objects
        private Texture2D brickWallTexture;
        public static Texture2D crateTexture;
        private Texture2D doorTexture;
        private Texture2D gemTexture;
        private Texture2D goalTexture;
        private Texture2D keyTexture;
        private Texture2D spikesTexture;

        //Store the player
        public static Player player;

        //Store the progress bar rectangles
        private Rectangle bar1;
        private Rectangle bar2;

        //Pre: None
        //Post: None
        //Desc: Loads content folder, sets mouse visibility, and defines graphics device manager
        public Game1()
        {
            //Define graphics device manager and content manager
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //Set mouse visibility to false
            IsMouseVisible = false;
        }

        //Pre: None
        //Post: None
        //Desc: Sets resolution and initializes game
        protected override void Initialize()
        {
            //Set the resolution
            //80:18 + 6 = 80:24 but character height is double character width and pixels are 1:1 height/width ratio so 80:24*2 = 48 = 80:48 = 5:3 ratio. I settled on the 5:3 ratio resolution 1800 * 1080
            //2 by 4 in characters is really 4 by 4 in pixels. 1080/48 and 1080/80 = 22.5. 22.5* 4 is OBJECT_SIDE_LENGTH, so my sprites need to be OBJECT_SIDE_LENGTH by OBJECT_SIDE_LENGTH pixels
            _graphics.PreferredBackBufferWidth = 1800;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            //Initialize game
            base.Initialize();
        }

        //Pre: None
        //Post: None
        //Desc: Loads more game resourses
        protected override void LoadContent()
        {
            //Define the array of levels
            levels = new char[fileNames.Length, 9, 20];

            //Define the spritebatch
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Define the command queue
            commands = new CommandQueue();

            //Define the player
            player = new Player(new Vector2());

            //Define the play texture and set a new location
            playTexture = Content.Load<Texture2D>("play");
            playLocation = new Vector2(_graphics.PreferredBackBufferWidth / 5 * 1 - playTexture.Width / 2, _graphics.PreferredBackBufferHeight / 2 - playTexture.Height);

            //Define the play texture and set a new location
            highScoresTexture = Content.Load<Texture2D>("High Scores");
            highScoresLocation = new Vector2(_graphics.PreferredBackBufferWidth / 5 * 2 - playTexture.Width / 2 - 60, _graphics.PreferredBackBufferHeight / 2 - playTexture.Height + 40);

            //Define the instructions texture and set a new location
            instructionsTexture = Content.Load<Texture2D>("instructions");
            instructionsLocation = new Vector2(_graphics.PreferredBackBufferWidth / 5 * 3 - instructionsTexture.Width / 2, _graphics.PreferredBackBufferHeight / 2 - instructionsTexture.Height);

            //Define the exit texture and set a new location
            exitTexture = Content.Load<Texture2D>("exit");
            exitLocation = new Vector2(_graphics.PreferredBackBufferWidth / 5 * 4 - exitTexture.Width / 2, _graphics.PreferredBackBufferHeight / 2 - exitTexture.Height);

            //Define the pointer texture and set a new location
            pointerTexture = Content.Load<Texture2D>("pointer");
            pointerLocation = new Vector2(_graphics.PreferredBackBufferWidth / 5 * 1 - playTexture.Width, playLocation.Y + playTexture.Height + pointerTexture.Height);

            //Define the bar texture
            barTexture = Content.Load<Texture2D>("bar");

            //Define the bar rectangles
            bar1 = new Rectangle(350, 950, 0, 50);
            bar2 = new Rectangle(350, 950, MAX_BAR_WIDTH, 50);

            //Define the color datas
            brickWallTexture = Content.Load<Texture2D>("brick_wall");
            wallData = new Color[brickWallTexture.Width * brickWallTexture.Height];
            brickWallTexture.GetData(wallData);
            crateTexture = Content.Load<Texture2D>("crate");
            crateData = new Color[crateTexture.Width * crateTexture.Height];
            crateTexture.GetData(crateData);
            doorTexture = Content.Load<Texture2D>("door");
            doorData = new Color[doorTexture.Width * doorTexture.Height];
            doorTexture.GetData(doorData);
            gemTexture = Content.Load<Texture2D>("gem");
            goalTexture = Content.Load<Texture2D>("goal");
            keyTexture = Content.Load<Texture2D>("key");
            spikesTexture = Content.Load<Texture2D>("spikes2");
            spikesData = new Color[spikesTexture.Width * spikesTexture.Height];
            spikesTexture.GetData(spikesData);

            //Define the fonts
            arial = Content.Load<SpriteFont>("Arial");
            monospace = Content.Load<SpriteFont>("monospace");

            //Load the levels
            for (int i = 0; i < fileNames.Length; i++)
            {
                StoreLevel(fileNames[i]);
            }
            currentLevelIndex = 0;
            LoadLevel(true);

            // TODO: use this.Content to load your game content here
        }

        //Pre: The game time
        //Post: None
        //Desc: Update the game
        protected override void Update(GameTime gameTime)
        {
            //Update the current gamestate
            switch (gameState)
            {
                case MENU:
                    UpdateMenu();
                    break;
                case HIGHSCORES:
                    UpdateHighScores();
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions();
                    break;
                case GAME_ENTRY:
                    UpdateGameEntry(gameTime);
                    break;
                case GAME_EXECUTION:
                    UpdateGameExecution(gameTime);
                    break;
                case RESULTS:
                    UpdateResults(gameTime);
                    break;
                case NAME_ENTRY:
                    UpdateNameEntry();
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        //Pre: The game time
        //Post: None
        //Desc: Draw the game
        protected override void Draw(GameTime gameTime)
        {
            //Set the background colour to tan
            GraphicsDevice.Clear(Color.Tan);

            //Draw the current gamestate
            switch (gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case HIGHSCORES:
                    DrawHighScores();
                    break;
                case INSTRUCTIONS:
                    DrawInstructions();
                    break;
                case GAME_ENTRY:
                    DrawGameEntry();
                    break;
                case GAME_EXECUTION:
                    DrawGameExecution();
                    break;
                case RESULTS:
                    DrawResults();
                    break;
                case NAME_ENTRY:
                    DrawNameEntry();
                    break;
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        //Pre: None
        //Post: None
        //Desc: Update the menu
        protected void UpdateMenu()
        {
            //Get the current state of the keyboard
            Keyboard.GetState();

            //Move the pointer or go to whichever game state the user has chosen
            if (Keyboard.HasBeenPressed(Keys.Right) && menuChoice < NUM_OPTIONS)
            {
                //Increment the current menu choice
                menuChoice++;
                //Move the pointer to the next menu option
                pointerLocation.X += _graphics.PreferredBackBufferWidth / 5 + POINTER_MOVE_OFFSET;
            }
            else if (Keyboard.HasBeenPressed(Keys.Left) && menuChoice > 1)
            {
                //De-increment the current menu choice
                menuChoice--;
                //Move the pointer to the previous menu choice
                pointerLocation.X -= _graphics.PreferredBackBufferWidth / 5 + POINTER_MOVE_OFFSET;
            }
            else if (Keyboard.HasBeenPressed(Keys.Enter))
            {
                //Go the the gamestate the user chose, or exit the game
                switch (menuChoice)
                {
                    case MNU_PLAY:
                        //Go to gameplay
                        gameState = GAME_ENTRY;
                        break;
                    case MNU_INSTRUCTIONS:
                        //Go to instructions
                        gameState = INSTRUCTIONS;
                        break;
                    case MNU_EXIT:
                        //Exit the game
                        Exit();
                        break;
                    case MNU_HIGH_SCORES:
                        //If the high scores aren't sorted, sort them. Go to high scores.
                        if (!isSorted)
                        {
                            //Store the temporary high score
                            HighScore temp;

                            //Clear the high score lists
                            numSort.Clear();
                            nameSort.Clear();

                            //Read in the data from the high score text file and bubble sort it based on the score
                            inFile = File.OpenText("High Scores.txt");
                            //Store the data array
                            string[] data;
                            //Loop through all the lines in the text file
                            while (!inFile.EndOfStream)
                            {
                                //Store the contents of the current line in the array
                                data = inFile.ReadLine().Split(',');

                                //Store the data in the number sort list
                                numSort.Insert(0, new HighScore(data[0], Convert.ToInt32(data[1]), null));

                                //Sort the newly added high score by comparing it against all the existing high scores until it finds one that's larger
                                for (int j = 0; j < numSort.Count - 1; j++)
                                {
                                    //If the current high score is higher then the next high score, swap them. Otherwise, break out of the loop
                                    if (numSort[j].score > numSort[j + 1].score)
                                    {
                                        //Swap the current high score with the next high score
                                        temp = numSort[j];
                                        numSort[j] = numSort[j + 1];
                                        numSort[j + 1] = temp;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            //Close the high score text file
                            inFile.Close();

                            //Loop through all the number sorted high scores
                            for (int i = 0; i < numSort.Count; i++)
                            {
                                //Set the place to one higher then the index, and then add the highscore to the name sorted list
                                numSort[i].place = i + 1;
                                nameSort.Insert(0, new HighScore(numSort[i].name, numSort[i].score, numSort[i].place));
                                //Sort the newly added high score by comparing it against all the existing high scores until it finds one that's higher alphabetically
                                for (int j = 0; j < nameSort.Count - 1; j++)
                                {
                                    //If the current high score is higher alphabetically then the next high score, swap them. Otherwise, break out of the loop
                                    if (nameSort[j].name.CompareTo(nameSort[j + 1].name) > 0)
                                    {
                                        //Swap the current high score with the next high score
                                        temp = nameSort[j];
                                        nameSort[j] = nameSort[j + 1];
                                        nameSort[j + 1] = temp;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            //The high scores are now sorted
                            isSorted = true;
                        }
                        //Set the current gamestate to highscores
                        gameState = HIGHSCORES;
                        break;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Update High Scores
        protected void UpdateHighScores()
        {
            //Get the keyboard's state
            Keyboard.GetState();

            //Leave the high score gamestate and go back to menu
            if (Keyboard.HasBeenPressed(Keys.Escape))
            {
                //Reset the search and what the user is in the middle of searching and go back to the menu
                userInput = "";
                search = null;
                gameState = MENU;
            }
            //Search for the name
            if (Keyboard.HasBeenPressed(Keys.Enter) && userInput.Length > 0)
            {
                //Set the top and bottom indexes of the search range to their first values
                int bottom = 0;
                int top = nameSort.Count - 1;

                while (true)
                {
                    //Set the middle to the current middle of the search range
                    int middle = (bottom + top) / 2;

                    //If the middle is on the right name, store the right high score, and break out of the loop 
                    if (userInput.Equals(nameSort[middle].name))
                    {
                        search = $"{nameSort[middle].place}. {nameSort[middle].name}: {nameSort[middle].score}";
                        break;
                    }
                    else if (userInput.CompareTo(nameSort[middle].name) < 0)
                    {
                        top = middle - 1;
                    }
                    else
                    {
                        bottom = middle + 1;
                    }
                    //If the search range has now become negative, store a sorry message and break out of the loop
                    if (top < bottom)
                    {
                        search = "Sorry, not found";
                        break;
                    }
                }

                //Reset the user input
                userInput = "";
            }
            //Update the user input
            UpdateInput();
        }

        //Pre: The game time
        //Post: None
        //Desc: Update the instructions
        protected void UpdateInstructions()
        {
            //If the user clicked escape, go back to the menu
            Keyboard.GetState();
            if (Keyboard.HasBeenPressed(Keys.Escape))
            {
                gameState = MENU;
            }
        }

        //Pre: The game time
        //Post: None
        //Desc: Update the game entry
        protected void UpdateGameEntry(GameTime gameTime)
        {
            //Add the time since the last update to the user's level score
            Player.levelScores[currentLevelIndex] += gameTime.ElapsedGameTime.TotalMilliseconds;

            //Get the keyboard's current state;
            Keyboard.GetState();

            //Toggle the legend
            if (Keyboard.HasBeenPressed(Keys.L))
            {
                showLegend = !showLegend;
            }

            //Allow the user to exit or type to the command, provided the command isn't already too long
            if (Keyboard.HasBeenPressed(Keys.X))
            {
                //Reset the game and go back to the main menu
                currentLevelIndex = -1;
                for (int i = 0; i < fileNames.Length; i++)
                {
                    StoreLevel(fileNames[i]);
                }
                currentLevelIndex = 0;
                LoadLevel(true);
                Player.score = 0;
                Player.levelScores = new double[fileNames.Length];
                showLegend = true;
                feedBack = "";
                gameState = MENU;
            }
            else if (command.Length < 68)
            {
                //If the user types in a valid character, add it to the end of the string
                if (Keyboard.HasBeenPressed(Keys.D0))
                {
                    command = command.Insert(command.Length, "0");
                }
                if (Keyboard.HasBeenPressed(Keys.D1))
                {
                    command = command.Insert(command.Length, "1");
                }
                if (Keyboard.HasBeenPressed(Keys.D2))
                {
                    command = command.Insert(command.Length, "2");
                }
                if (Keyboard.HasBeenPressed(Keys.D3))
                {
                    command = command.Insert(command.Length, "3");
                }
                if (Keyboard.HasBeenPressed(Keys.D4))
                {
                    command = command.Insert(command.Length, "4");
                }
                if (Keyboard.HasBeenPressed(Keys.D5))
                {
                    command = command.Insert(command.Length, "5");
                }
                if (Keyboard.HasBeenPressed(Keys.D6))
                {
                    command = command.Insert(command.Length, "6");
                }
                if (Keyboard.HasBeenPressed(Keys.D7))
                {
                    command = command.Insert(command.Length, "7");
                }
                if (Keyboard.HasBeenPressed(Keys.D8))
                {
                    command = command.Insert(command.Length, "8");
                }
                if (Keyboard.HasBeenPressed(Keys.D9))
                {
                    command = command.Insert(command.Length, "9");
                }
                if (Keyboard.HasBeenPressed(Keys.D))
                {
                    command = command.Insert(command.Length, "d");
                }
                if (Keyboard.HasBeenPressed(Keys.A))
                {
                    command = command.Insert(command.Length, "a");
                }
                if (Keyboard.HasBeenPressed(Keys.C))
                {
                    command = command.Insert(command.Length, "c");
                }
                if (Keyboard.HasBeenPressed(Keys.E))
                {
                    command = command.Insert(command.Length, "e");
                }
                if (Keyboard.HasBeenPressed(Keys.Q))
                {
                    command = command.Insert(command.Length, "q");
                }
                if (Keyboard.HasBeenPressed(Keys.Add))
                {
                    command = command.Insert(command.Length, "+");
                }
                if (Keyboard.HasBeenPressed(Keys.Subtract))
                {
                    command = command.Insert(command.Length, "-");
                }
                if (Keyboard.HasBeenPressed(Keys.S))
                {
                    command = command.Insert(command.Length, "s");
                }
                if (Keyboard.HasBeenPressed(Keys.F))
                {
                    command = command.Insert(command.Length, "f");
                }
            }

            //If there is a letter to delete at the front of the string, delete it
            if (command.Length > 0 && Keyboard.HasBeenPressed(Keys.Back))
            {
                command = command.Remove(command.Length - 1);
            }

            //If the user clicks enter, check if the command is valid
            if (Keyboard.HasBeenPressed(Keys.Enter))
            {
                //Store whether the command is valid and if a loop has been started
                bool commandValid = true;
                bool loopStarted = false;

                //Loop through the characters in the string command
                for (int i = 0; i < command.Length; i++)
                {
                    //If a loop has been started, check if the loop structure is valid
                    if (command[i] == 's')
                    {
                        //If the loop structure is invalid, set feedback to the proper feedback and break. Otherwise, a loop has been started, and the number in front of the s is accounted for
                        if (loopStarted || i >= command.Length - 3 || !Int32.TryParse(Convert.ToString(command[i + 1]), out _) || command[i + 2] == 'f' || Int32.TryParse(Convert.ToString(command[i + 2]), out _))
                        {
                            commandValid = false;
                            feedBack = $"Error at command letter number {i + 1}. Invalid loop. Take a look at the legend example.";
                            break;
                        }
                        else
                        {
                            loopStarted = true;
                            i++;
                        }
                    }
                    else if (command[i] == 'f')
                    {
                        //If a loop has been ended preemptively, set feedback to the proper feedback and break. Otherwise, the loop has been ended
                        if (loopStarted)
                        {
                            loopStarted = false;
                        }
                        else
                        {
                            commandValid = false;
                            feedBack = $"Error at command letter number {i + 1}. Did you mean to start a loop first?";
                            break;
                        }
                    }
                    else if (Int32.TryParse(Convert.ToString(command[i]), out _))
                    {
                        //Since numbers in front of an s are skipped, there should never be a number. If there is, the commannd is invalid, so set feedback to the proper feedback and break
                        commandValid = false;
                        feedBack = $"Error at command letter number {i + 1}. Number not expected unless right after start of loop.";
                        break;
                    }
                }

                //If the command ended without a loop being finished, set feedback to the proper feedback and the command is invalid.
                if (loopStarted)
                {
                    feedBack = "End your last loop";
                    commandValid = false;
                }

                //If the command is valid, queue it up
                if (commandValid)
                {
                    //Save the command size. If it is successful, it will be used to calculate the score
                    Player.commandSize = command.Length;
                    //Loop through the command
                    for (int i = 0; i < command.Length; i++)
                    {
                        //If a loop isn't started yet just queue the command and add 1 to space
                        if (command[i] != 's')
                        {
                            commands.Enqueue(new Command(command[i]));
                            space.Add(1);
                        }
                        else
                        {
                            //Add 2 twos to space for the s and number
                            space.Add(2);
                            space.Add(2);

                            //Store the loop sequence and length
                            string command = "";
                            int numCommands = 2;

                            //Calculate the sequence and add 1s to spaces for each command in the sequence
                            while (this.command[i + numCommands] != 'f')
                            {
                                command = command.Insert(command.Length, Convert.ToString(this.command[i + numCommands]));
                                numCommands++;
                                space.Add(1);
                            }
                            i += numCommands + 1;

                            //Add the number of iterations of the loop to spaces
                            space.Add(-(Convert.ToInt32(this.command[i - numCommands]) - CHAR_OFFSET) + 1);

                            //If there are 0 iterations make the whole loop into 2s so tha arrow skips it
                            if (space[space.Count - 1] == 1)
                            {
                                int j = space.Count - 1;
                                while (space[j] == 1)
                                {
                                    space[j] = 2;
                                    j--;
                                }
                            }

                            //Queue the loop sequence however many times there are iterations
                            for (int j = 0; j < Convert.ToInt32(this.command[i - numCommands]) - CHAR_OFFSET; j++)
                            {
                                for (int k = 0; k < command.Length; k++)
                                {
                                    commands.Enqueue(new Command(command[k]));
                                }
                            }

                            //Deincrement i
                            i--;
                        }
                    }

                    //Make proper changes if the first command isn't a loop
                    if (command[0] != 's')
                    {
                        command = command.Insert(0, " ");
                        space.Insert(0, 1);
                    }

                    //Set bar widths
                    progressBarMultiple = MAX_BAR_WIDTH / commands.Size();
                    bar2.Width = progressBarMultiple * commands.Size();

                    //Reset feedback and go to game execution
                    feedBack = "";
                    gameState = GAME_EXECUTION;
                }
            }

        }

        //Pre: The game time
        //Post: None
        //Desc: Update the game execution
        protected void UpdateGameExecution(GameTime gameTime)
        {
            //Add the time since the last update to the user's level score
            Player.levelScores[currentLevelIndex] += gameTime.ElapsedGameTime.TotalMilliseconds;

            //Check if the player died to spikes
            if (Player.currentPlayerOrientation == Player.JUMPING)
            {
                if (Collision.ListToOne(spikes, spikesData, player.rectangle, player.textureDataJumping))
                {
                    //Reset and go to results
                    Reset();
                }
            }
            else
            {
                if (Collision.ListToOne(spikes, spikesData, player.rectangle, player.textureDataStanding))
                {
                    //Reset and go to results
                    Reset();
                }
            }

            //Only dequeue next command if both player and crates have stopped moving
            if (Player.currentPlayerOrientation == Player.STANDING)
            {
                //Check if the crates are all done moving
                bool doneMoving = true;
                if (isPushing)
                {
                    for (int i = 0; i < crates.Count; i++)
                    {
                        if (crates[i].cratePosition != crates[i].oldPosition)
                        {
                            doneMoving = false;
                        }
                    }
                }
                //If both the player and crates are done moving, and the quese is not empty, dequeue and execute the next action
                if (!commands.IsEmpty() && doneMoving)
                {
                    //Dequeue the next action
                    char nextAction = commands.Dequeue().GetChar();

                    //Increment the progress bar width by the progress bar multiple
                    bar1.Width += progressBarMultiple;

                    //Update the location of the pointer
                    while (true)
                    {
                        //Only stay on the next character if it is a one. If it's a two, skip it, and if it's a negative number, go back to the start of the loop and increment the negative number
                        if (space[spaces.Length + 1] == 1)
                        {
                            //Stay on the next character
                            spaces += " ";
                            break;
                        }
                        else if (space[spaces.Length + 1] == 2)
                        {
                            //Make the next character the current character
                            spaces += " ";
                        }
                        else
                        {
                            //Either go back to the start of the loop, or if all iterations have finished, continue
                            if (space[spaces.Length + 1] != 0)
                            {
                                //Go back to the start of the loop
                                space[spaces.Length + 1]++;
                                while (space[spaces.Length] != 2)
                                {
                                    spaces = spaces.Remove(spaces.Length - 1);
                                }
                            }
                            else
                            {
                                //Make the next character the current character
                                spaces += " ";
                            }

                        }
                    }
                    //If the command is to push, change the command to move right or left, but set isPushing to true. Otherwise, set isPushing to false
                    switch (nextAction)
                    {
                        case '+':
                            isPushing = true;
                            isRight = true;
                            nextAction = 'd';
                            break;
                        case '-':
                            isPushing = true;
                            isRight = false;
                            nextAction = 'a';
                            break;
                        default:
                            isPushing = false;
                            break;
                    }
                    int y = (int)player.position.Y / OBJECT_SIDE_LENGTH;
                    int x = (int)player.position.X / OBJECT_SIDE_LENGTH;
                    //Execute the next action (if it is valid)
                    switch (nextAction)
                    {
                        case 'd':
                            //If it is valid to push right, push right
                            if (levels[currentLevelIndex, y, x] == '0' && x + 1 <= 19 && (levels[currentLevelIndex, y, x + 1] == ' ' || levels[currentLevelIndex, y, x + 1] == '5' || levels[currentLevelIndex, y, x + 1] == '6' || levels[currentLevelIndex, y, x + 1] == '3' || levels[currentLevelIndex, y, x + 1] == '7' || (levels[currentLevelIndex, y, x + 1] == '4' && player.numKeys > 0) || (levels[currentLevelIndex, y, x + 1] == '2' && isPushing)))
                            {
                                if (isPushing)
                                {
                                    //Update the crate's location, or do nothing if there is no crate, or break if you can't push it
                                    if (x + 2 >= 0 && levels[currentLevelIndex, y, x + 1] == '2' && levels[currentLevelIndex, y, x + 2] == ' ' && ((y - 1 < 0) || levels[currentLevelIndex, y - 1, x + 1] == ' ' || levels[currentLevelIndex, y - 1, x + 1] == '1' || levels[currentLevelIndex, y - 1, x + 1] == '5'))
                                    {
                                        //Update the crate's location
                                        int temp = y;
                                        y++;
                                        while (y <= 8 && (levels[currentLevelIndex, y, x + 2] == ' ' || levels[currentLevelIndex, y, x + 2] == '6' || levels[currentLevelIndex, y, x + 2] == '3' || levels[currentLevelIndex, y, x + 2] == '7'))
                                        {
                                            y++;
                                        }
                                        levels[currentLevelIndex, y - 1, x + 2] = '2';
                                        y = temp;
                                    }
                                    else if (levels[currentLevelIndex, y, x + 1] == '2')
                                    {
                                        break;
                                    }
                                }
                                //Update the player's old spot and new spot
                                if (onGem)
                                {
                                    levels[currentLevelIndex, y, x] = '6';
                                }
                                else if (onGoal)
                                {
                                    levels[currentLevelIndex, y, x] = '3';
                                }
                                else if (onKey)
                                {
                                    levels[currentLevelIndex, y, x] = '7';
                                }
                                else
                                {
                                    levels[currentLevelIndex, y, x] = ' ';
                                }
                                onGem = false;
                                onGoal = false;
                                onKey = false;

                                //Figure out where the player will land and update their location
                                y++;
                                while (y <= 8 && (levels[currentLevelIndex, y, x + 1] == ' ' || levels[currentLevelIndex, y, x + 1] == '6' || levels[currentLevelIndex, y, x + 1] == '3' || levels[currentLevelIndex, y, x + 1] == '7' || (levels[currentLevelIndex, y, x + 1] == '4' && player.numKeys > 0)))
                                {
                                    y++;
                                }
                                switch (levels[currentLevelIndex, y - 1, x + 1])
                                {
                                    case '6':
                                        onGem = true;
                                        break;
                                    case '3':
                                        onGoal = true;
                                        break;
                                    case '7':
                                        onKey = true;
                                        break;
                                }
                                levels[currentLevelIndex, y - 1, x + 1] = '0';

                                //Set the player's new y value to the y index's value times OBJECT_SIDE_LENGTH
                                player.newY = OBJECT_SIDE_LENGTH * (y - 1);

                                //Move the player
                                player.action = Player.MOVE_RIGHT;

                            }
                            break;
                        case 'a':
                            //If it is valid to push left, push left
                            if (levels[currentLevelIndex, y, x] == '0' && x - 1 >= 0 && (levels[currentLevelIndex, y, x - 1] == ' ' || levels[currentLevelIndex, y, x - 1] == '5' || levels[currentLevelIndex, y, x - 1] == '6' || levels[currentLevelIndex, y, x - 1] == '3' || levels[currentLevelIndex, y, x - 1] == '7' || (levels[currentLevelIndex, y, x - 1] == '4' && player.numKeys > 0) || (levels[currentLevelIndex, y, x - 1] == '2' && isPushing)))
                            {
                                //Update the crate's location, or do nothing if there is no crate, or break if you can't push it
                                if (isPushing)
                                {
                                    if (x + 2 >= 0 && levels[currentLevelIndex, y, x - 1] == '2' && levels[currentLevelIndex, y, x - 2] == ' ' && ((y - 1 < 0) || levels[currentLevelIndex, y - 1, x - 1] == ' ' || levels[currentLevelIndex, y - 1, x - 1] == '1' || levels[currentLevelIndex, y - 1, x - 1] == '5'))
                                    {
                                        //Update the crate's location
                                        int temp = y;
                                        y++;
                                        while (y <= 8 && (levels[currentLevelIndex, y, x - 2] == ' ' || levels[currentLevelIndex, y, x - 2] == '6' || levels[currentLevelIndex, y, x - 2] == '3' || levels[currentLevelIndex, y, x - 2] == '7'))
                                        {
                                            y++;
                                        }
                                        levels[currentLevelIndex, y - 1, x - 2] = '2';
                                        y = temp;
                                    }
                                    else if (levels[currentLevelIndex, y, x - 1] == '2')
                                    {
                                        break;
                                    }
                                }
                                //Update the player's old spot and new spot
                                if (onGem)
                                {
                                    levels[currentLevelIndex, y, x] = '6';
                                }
                                else if (onGoal)
                                {
                                    levels[currentLevelIndex, y, x] = '3';
                                }
                                else if (onKey)
                                {
                                    levels[currentLevelIndex, y, x] = '7';
                                }
                                else
                                {
                                    levels[currentLevelIndex, y, x] = ' ';
                                }
                                onGem = false;
                                onGoal = false;
                                onKey = false;

                                //Figure out where the player will land and update their location
                                y++;
                                while (y <= 8 && (levels[currentLevelIndex, y, x - 1] == ' ' || levels[currentLevelIndex, y, x - 1] == '6' || levels[currentLevelIndex, y, x - 1] == '3' || levels[currentLevelIndex, y, x - 1] == '7' || (levels[currentLevelIndex, y, x - 1] == '4' && player.numKeys > 0)))
                                {
                                    y++;
                                }
                                switch (levels[currentLevelIndex, y - 1, x - 1])
                                {
                                    case '6':
                                        onGem = true;
                                        break;
                                    case '3':
                                        onGoal = true;
                                        break;
                                    case '7':
                                        onKey = true;
                                        break;
                                }
                                levels[currentLevelIndex, y - 1, x - 1] = '0';

                                //Set the player's new y value to the y index's value times OBJECT_SIDE_LENGTH
                                player.newY = OBJECT_SIDE_LENGTH * (y - 1);

                                //Move the player
                                player.action = Player.MOVE_LEFT;

                            }
                            break;
                        case 'e':
                            //If it is valid to jump right, jump right
                            if (y > 0 && levels[currentLevelIndex, y, x] == '0' && (levels[currentLevelIndex, y - 1, x] == ' ' || levels[currentLevelIndex, y - 1, x] == '5' || (levels[currentLevelIndex, y - 1, x] == '4' && player.numKeys > 0)) && x + 1 <= 19 && (levels[currentLevelIndex, y - 1, x + 1] == ' ' || levels[currentLevelIndex, y - 1, x + 1] == '5' || levels[currentLevelIndex, y - 1, x + 1] == '6' || levels[currentLevelIndex, y - 1, x + 1] == '3' || levels[currentLevelIndex, y - 1, x + 1] == '7' || (levels[currentLevelIndex, y - 1, x + 1] == '4' && player.numKeys > 0)))
                            {
                                //Update the player's old spot and new spot
                                if (onGem)
                                {
                                    levels[currentLevelIndex, y, x] = '6';
                                }
                                else if (onGoal)
                                {
                                    levels[currentLevelIndex, y, x] = '3';
                                }
                                else if (onKey)
                                {
                                    levels[currentLevelIndex, y, x] = '7';
                                }
                                else
                                {
                                    levels[currentLevelIndex, y, x] = ' ';
                                }
                                onGem = false;
                                onGoal = false;
                                onKey = false;

                                //Move the player
                                if (levels[currentLevelIndex, y, x + 1] == ' ' || levels[currentLevelIndex, y, x + 1] == '6' || levels[currentLevelIndex, y, x + 1] == '3' || levels[currentLevelIndex, y, x + 1] == '7')
                                {
                                    player.action = Player.JUMP_RIGHT;
                                }
                                else
                                {
                                    player.action = Player.JUMP_ON_RIGHT;
                                }

                                //Figure out where the player will land and update their location
                                while (y <= 8 && (levels[currentLevelIndex, y, x + 1] == ' ' || levels[currentLevelIndex, y, x + 1] == '6' || levels[currentLevelIndex, y, x + 1] == '3' || levels[currentLevelIndex, y, x + 1] == '7' || (levels[currentLevelIndex, y, x + 1] == '4' && player.numKeys > 0)))
                                {
                                    y++;
                                }
                                switch (levels[currentLevelIndex, y - 1, x + 1])
                                {
                                    case '6':
                                        onGem = true;
                                        break;
                                    case '3':
                                        onGoal = true;
                                        break;
                                    case '7':
                                        onKey = true;
                                        break;
                                }
                                levels[currentLevelIndex, y - 1, x + 1] = '0';

                                //Set the player's new y value to the y index's value times OBJECT_SIDE_LENGTH
                                player.newY = OBJECT_SIDE_LENGTH * (y - 1);
                            }
                            break;
                        case 'q':
                            //If it is valid to jump left, jump left
                            if (y > 0 && levels[currentLevelIndex, y, x] == '0' && (levels[currentLevelIndex, y - 1, x] == ' ' || levels[currentLevelIndex, y - 1, x] == '5' || (levels[currentLevelIndex, y - 1, x] == '4' && player.numKeys > 0)) && x - 1 >= 0 && (levels[currentLevelIndex, y - 1, x - 1] == ' ' || levels[currentLevelIndex, y - 1, x - 1] == '5' || levels[currentLevelIndex, y - 1, x - 1] == '6' || levels[currentLevelIndex, y - 1, x - 1] == '3' || levels[currentLevelIndex, y - 1, x - 1] == '7' || (levels[currentLevelIndex, y - 1, x + 1] == '4' && player.numKeys > 0)))
                            {
                                //Update the player's old spot and new spot
                                if (onGem)
                                {
                                    levels[currentLevelIndex, y, x] = '6';
                                }
                                else if (onGoal)
                                {
                                    levels[currentLevelIndex, y, x] = '3';
                                }
                                else if (onKey)
                                {
                                    levels[currentLevelIndex, y, x] = '7';
                                }
                                else
                                {
                                    levels[currentLevelIndex, y, x] = ' ';
                                }
                                onGem = false;
                                onGoal = false;
                                onKey = false;

                                //Move the player
                                if (levels[currentLevelIndex, y, x - 1] == ' ' || levels[currentLevelIndex, y, x - 1] == '6' || levels[currentLevelIndex, y, x - 1] == '3' || levels[currentLevelIndex, y, x - 1] == '7')
                                {
                                    player.action = Player.JUMP_LEFT;
                                }
                                else
                                {
                                    player.action = Player.JUMP_ON_LEFT;
                                }

                                //Figure out where the player will land and update their location
                                while (y <= 8 && (levels[currentLevelIndex, y, x - 1] == ' ' || levels[currentLevelIndex, y, x - 1] == '6' || levels[currentLevelIndex, y, x - 1] == '3' || levels[currentLevelIndex, y, x - 1] == '7' || (levels[currentLevelIndex, y, x - 1] == '4' && player.numKeys > 0)))
                                {
                                    y++;
                                }
                                switch (levels[currentLevelIndex, y - 1, x - 1])
                                {
                                    case '6':
                                        onGem = true;
                                        break;
                                    case '3':
                                        onGoal = true;
                                        break;
                                    case '7':
                                        onKey = true;
                                        break;
                                }
                                levels[currentLevelIndex, y - 1, x - 1] = '0';

                                //Set the player's new y value to the y index's value times OBJECT_SIDE_LENGTH
                                player.newY = OBJECT_SIDE_LENGTH * (y - 1);
                            }

                            break;
                        case 'c':
                            //If the player is on a collectible, collect it 
                            if (onKey)
                            {
                                //Loop through the keys
                                for (int i = 0; i < keys.Count; i++)
                                {
                                    //When the key is found, remove it and break out of the loop
                                    if (keys[i] == player.position)
                                    {
                                        onKey = false;
                                        player.numKeys++;
                                        keys.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else if (onGem)
                            {
                                //Loop through the gems
                                for (int i = 0; i < gems.Count; i++)
                                {
                                    //When the gem is found, remove it and break out of the loop
                                    if (gems[i] == player.position)
                                    {
                                        onGem = false;
                                        player.numGems++;
                                        gems.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            break;

                    }
                }
                else if (doneMoving)
                {
                    //If the player won, stop increasing the score, factor in the number of characters used, and set level cleared to true
                    if (numLevelGems == player.numGems && onGoal)
                    {

                        Player.levelScores[currentLevelIndex] += 100 * Player.commandSize;
                        Player.score += (int)Player.levelScores[currentLevelIndex];
                        levelCleared = true;
                    }
                    //Reset and go to results
                    Reset();
                }

            }
            //Update the player
            player.Update();

            //Update the crates
            for (int i = 0; i < crates.Count; i++)
            {
                crates[i].UpdateCrate(isRight);
            }
        }

        //Pre: The game time
        //Post: None
        //Desc: Update the results
        protected void UpdateResults(GameTime gameTime)
        {
            //Get the keyboard's state
            Keyboard.GetState();

            //Increase the score if the user hasn't won yet
            if (!levelCleared)
            {
                Player.levelScores[currentLevelIndex] += gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            //Load the next level or reset the current level
            if (Keyboard.HasBeenPressed(Keys.Enter))
            {
                //In case the player died to a spike, reset their orientation and speed
                Player.currentPlayerOrientation = Player.STANDING;
                Player.playerSpeed.X = Player.STOP;
                Player.playerSpeed.Y = Player.STOP;

                //Load the next level, or reset the current one
                if (levelCleared)
                {
                    //Load the next level
                    levelCleared = false;
                    currentLevelIndex++;
                    if (LoadLevel(false))
                    {
                        gameState = GAME_ENTRY;
                    }
                    else
                    {
                        //Reset the game and go to name entry
                        currentLevelIndex = -1;
                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            StoreLevel(fileNames[i]);
                        }
                        currentLevelIndex = 0;
                        LoadLevel(true);
                        gameState = NAME_ENTRY;
                    }
                }
                else
                {
                    //Reset the level
                    LoadLevel(true);
                    gameState = GAME_ENTRY;

                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Update the name entry
        protected void UpdateNameEntry()
        {
            //Add the new high score to the high score text file
            Keyboard.GetState();
            if (userInput.Length > 0 && Keyboard.HasBeenPressed(Keys.Enter))
            {
                outFile = File.AppendText("High Scores.txt");
                outFile.Write($"\n{userInput},{Player.score}");
                outFile.Close();
                userInput = "";
                isSorted = false;
                Player.score = 0;
                Player.levelScores = new double[fileNames.Length];
                showLegend = true;
                gameState = MENU;
            }

            //Update the user input
            UpdateInput();

        }

        //Pre: None
        //Post: None
        //Desc: Draw the menu
        protected void DrawMenu()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(playTexture, playLocation, Color.White);
            _spriteBatch.Draw(instructionsTexture, instructionsLocation, Color.White);
            _spriteBatch.Draw(exitTexture, exitLocation, Color.White);
            _spriteBatch.Draw(pointerTexture, pointerLocation, Color.White);
            _spriteBatch.Draw(highScoresTexture, highScoresLocation, Color.White);
            _spriteBatch.DrawString(arial, "Path Finder", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 66, 0), Color.Black); //-50 represents half the width
            _spriteBatch.End();
        }

        //Pre: None
        //Post: None
        //Desc: Draw the high scores
        protected void DrawHighScores()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(arial, $"Search for a name by typing it and then clicking enter: {userInput}", new Vector2(0, 900), Color.Black);
            _spriteBatch.DrawString(arial, $"Click escape to return to the menu", new Vector2(0, 950), Color.Black);
            for (int i = 0; i < 10; i++)
            {
                _spriteBatch.DrawString(arial, $"{numSort[i].place}. {numSort[i].name}: {numSort[i].score}", new Vector2(0, i * 45), Color.Black);
            }
            if (search != null)
            {
                _spriteBatch.DrawString(arial, $"Result:", new Vector2(0, 650), Color.Black);
                _spriteBatch.DrawString(arial, search, new Vector2(0, 700), Color.Black);
            }
            _spriteBatch.End();

        }

        //Pre: None
        //Post: None
        //Desc: Draw the instructions
        protected void DrawInstructions()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(arial, "Collect all the gems and finish on the goal. A falling crate will collect gems on it's path.", new Vector2(0, _graphics.PreferredBackBufferHeight / 2), Color.Black);
            _spriteBatch.DrawString(arial, "Collect keys to open doors. Click escape to go back to the main menu.", new Vector2(0, _graphics.PreferredBackBufferHeight / 2 + 50), Color.Black);
            _spriteBatch.End();
        }

        //Pre: None
        //Post: None
        //Desc: Draw the game entry
        protected void DrawGameEntry()
        {
            DrawLevel();
            _spriteBatch.Begin();
            _spriteBatch.DrawString(arial, $"Level {currentLevelIndex + 1}       {feedBack}", new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(monospace, $"Command: {command}", new Vector2(0, 850), Color.Black);
            if (showLegend)
            {
                _spriteBatch.DrawString(arial, commandLegend, new Vector2(0, 860), Color.Black);
            }
            _spriteBatch.DrawString(arial, "X To Quit", new Vector2(1550, 900), Color.Black);
            _spriteBatch.DrawString(arial, "L To Toggle", new Vector2(1550, 960), Color.Black);
            _spriteBatch.DrawString(arial, "Command Legend", new Vector2(1550, 1000), Color.Black);
            _spriteBatch.End();
        }

        //Pre: None
        //Post: None
        //Desc: Draw the game execution
        protected void DrawGameExecution()
        {
            DrawLevel();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(arial, $"Level {currentLevelIndex + 1}", new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(monospace, $"Command: {command}", new Vector2(0, 850), Color.Black);
            _spriteBatch.DrawString(monospace, $"Action:  {spaces}^", new Vector2(0, 900), Color.Black);
            _spriteBatch.DrawString(arial, $"Progress:", new Vector2(200, 950), Color.Black);
            _spriteBatch.DrawString(arial, $"Gems: {player.numGems}/{numLevelGems}    Keys: {player.numKeys}/{numLevelKeys}", new Vector2(0, 1000), Color.Black);

            _spriteBatch.Draw(barTexture, bar2, Color.Red);
            _spriteBatch.Draw(barTexture, bar1, Color.Black);
            _spriteBatch.End();
        }

        //Pre: None
        //Post: None
        //Desc: Draw the results
        protected void DrawResults()
        {
            DrawLevel();
            _spriteBatch.Begin();
            _spriteBatch.DrawString(arial, $"Level {currentLevelIndex + 1}", new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(arial, "Click Enter To Continue", new Vector2(0, 850), Color.Black);

            //Draw the correct message based on if the user cleared the level or not
            if (levelCleared)
            {
                _spriteBatch.DrawString(arial, $"Level Cleared! Score: {(int)Player.levelScores[currentLevelIndex]}    Time: {(int)Player.levelScores[currentLevelIndex] - 100 * Player.commandSize}ms", new Vector2(340, 850), Color.Black);
                if (currentLevelIndex + 1 == fileNames.Length)
                {
                    _spriteBatch.DrawString(arial, $"Game Cleared! Score: {Player.score}", new Vector2(1000, 850), Color.Black);

                    //Loop through the level scores
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        //Calculate the location of the level score
                        int y = i;
                        int x = 0;
                        while (y > 2)
                        {
                            x += 350;
                            y -= 3;
                        }
                        y *= 50;
                        y += 900;

                        //Draw the level score
                        _spriteBatch.DrawString(arial, $"Level {i + 1} Score: {(int)Player.levelScores[i]}", new Vector2(x, y), Color.Black);
                    }
                }

            }
            else
            {
                _spriteBatch.DrawString(arial, $"Sorry, try again.", new Vector2(340, 850), Color.Black);
            }
            _spriteBatch.End();
        }

        //Pre: None
        //Post: None
        //Desc: Draw the name entry
        protected void DrawNameEntry()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(arial, $"Enter your name and then click enter: {userInput}", new Vector2(0, 850), Color.Black);
            _spriteBatch.End();
        }

        //Pre: A bool representing wether or not to reset the level
        //Post: A bool representing if there was another level to load or not.
        //Desc: Load a level
        protected bool LoadLevel(bool resetLevel)
        {
            //Reset the current level
            if (resetLevel)
            {
                currentLevelIndex--;
                StoreLevel(fileNames[currentLevelIndex + 1]);
            }
            //Load the next/current level
            if (currentLevelIndex < fileNames.Length)
            {
                //Reset the variables and lists
                numLevelGems = 0;
                numLevelKeys = 0;
                onGem = false;
                onGoal = false;
                onKey = false;
                command = "";
                walls.Clear();
                crates.Clear();
                doors.Clear();
                spikes.Clear();
                gems.Clear();
                keys.Clear();
                Vector2 currentBlock = new Vector2(0, 0);

                //Add the data from the levels array to the lists and variables
                //Loop through the y values
                for (int i = 0; i < 9; i++)
                {
                    //Loop through the x values
                    for (int j = 0; j < 20; j++)
                    {
                        //Add the data based on which is the next character in the array
                        switch (levels[currentLevelIndex, i, j])
                        {
                            case '0':
                                //Load in the player
                                player.position = currentBlock;
                                player.oldX = currentBlock.X;
                                player.rectangle = new Rectangle((int)player.position.X, (int)(player.position.Y + 2.7), OBJECT_SIDE_LENGTH, OBJECT_SIDE_LENGTH);
                                player.Load(Content, GraphicsDevice);

                                break;

                            case '1':
                                //Load in a wall
                                walls.Add(currentBlock);
                                break;
                            case '2':
                                //Load in a crate
                                crates.Add(new Crate(currentBlock));
                                break;
                            case '3':
                                //Load in a goal
                                goal = currentBlock;
                                break;
                            case '4':
                                //Load in a door
                                doors.Add(currentBlock);
                                break;
                            case '5':
                                //Load in spikes
                                spikes.Add(currentBlock);
                                break;
                            case '6':
                                //Load in a gem
                                gems.Add(currentBlock);
                                numLevelGems++;
                                break;
                            case '7':
                                //Load in a key
                                keys.Add(currentBlock);
                                numLevelKeys++;
                                break;
                        }
                        //Increment the current black on the x axis by one object
                        currentBlock.X += OBJECT_SIDE_LENGTH;
                    }
                    //Change the x axis to 0 and lower the hiight by one object
                    currentBlock.Y += OBJECT_SIDE_LENGTH;
                    currentBlock.X = 0;
                }

                return true;
            }
            return false;
        }

        //Pre: None
        //Post: None
        //Desc: Draw the level
        protected void DrawLevel()
        {
            //Draw the objects in the current level
            _spriteBatch.Begin();
            _spriteBatch.Draw(goalTexture, goal, Color.White);
            for (int i = 0; i < walls.Count; i++)
            {
                _spriteBatch.Draw(brickWallTexture, walls[i], Color.White);
            }
            for (int i = 0; i < doors.Count; i++)
            {
                _spriteBatch.Draw(doorTexture, doors[i], Color.White);
            }
            for (int i = 0; i < gems.Count; i++)
            {
                _spriteBatch.Draw(gemTexture, gems[i], Color.White);
            }
            for (int i = 0; i < keys.Count; i++)
            {
                _spriteBatch.Draw(keyTexture, keys[i], Color.White);
            }
            for (int i = 0; i < spikes.Count; i++)
            {
                _spriteBatch.Draw(spikesTexture, spikes[i], Color.White);
            }
            for (int i = 0; i < crates.Count; i++)
            {
                crates[i].DrawCrate(_spriteBatch);
            }
            player.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        //Pre: A valid text file name that exists in the right folder
        //Post: None
        //Desc: Store a level
        protected void StoreLevel(string textFile)
        {
            //Store the next level
            currentLevelIndex++;
            inFile = File.OpenText(textFile);
            //Loop through the y values
            for (int i = 0; i < 9; i++)
            {
                //Loop through the x values
                for (int j = 0; j < 20; j++)
                {
                    //Add the current character in the text file to the array
                    levels[currentLevelIndex, i, j] = (char)inFile.Read();
                }
                //Go to the next line of the file
                inFile.ReadLine();
            }
            inFile.Close();
        }


        //Pre: None
        //Post: None
        //Desc: Update the user input
        protected void UpdateInput()
        {
            //Store the keyboard states
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            Keys[] pressedKeys = currentKeyboardState.GetPressedKeys();

            //Loop through the pressed keys for the current frame
            for (int i = 0; i < pressedKeys.Length; i++)
            {
                //If a key has been pressed this frame, try to add it to the end of the user input string
                if (oldKeyboardState.IsKeyUp(pressedKeys[i]))
                {
                    if (pressedKeys[i] == Keys.Back && userInput.Length > 0)
                    {
                        userInput = userInput.Remove(userInput.Length - 1);
                    }
                    else if (pressedKeys[i] == Keys.Space && userInput.Length < 68)
                    {
                        userInput = userInput.Insert(userInput.Length, " ");
                    }
                    else if (userInput.Length < 68 && (pressedKeys[i].ToString().Length == 1 || (pressedKeys[i].ToString()[0] == 'D' && pressedKeys[i].ToString().Length == 2)))
                    {
                        //Captitalize the input if the user is holding down shift
                        if (shift)
                        {
                            userInput += pressedKeys[i].ToString()[pressedKeys[i].ToString().Length - 1];
                        }
                        else
                        {
                            userInput += char.ToLower(pressedKeys[i].ToString()[pressedKeys[i].ToString().Length - 1]);
                        }
                    }
                }
            }
            //If the user is clicking shift, set shift to true
            if (pressedKeys.Length > 0 && (pressedKeys[pressedKeys.Length - 1] == Keys.LeftShift || pressedKeys[pressedKeys.Length - 1] == Keys.RightShift))
            {
                shift = true;
            }
            else
            {
                shift = false;
            }
        }

        //Pre: None
        //Post: None
        //Desc: Reset the level counts and change the gamestate to results
        protected void Reset()
        {
            spaces = "";
            space.Clear();
            player.numKeys = 0;
            player.numGems = 0;
            bar1.Width = 0;
            gameState = RESULTS;
        }
    }
}