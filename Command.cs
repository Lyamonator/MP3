//Author: Lyam Katz
//File Name: Command.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: A command to be stored in a command queue

namespace PASS4
{

    class Command
    {
        //Store the command
        private char command;

        public Command(char command)
        {
            this.command = command;
        }

        //Pre: None
        //Post: Returns the command
        //Description: Retrieves the command
        public char GetChar()
        {
            return command;
        }
    }
}
