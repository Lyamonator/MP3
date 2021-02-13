//Author: Lyam Katz
//File Name: Queue.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: A custom command queue

using System.Collections.Generic;
namespace PASS4
{
    class CommandQueue
    {
        //Store the collection of Commands
        List<Command> queue = new List<Command>();

        public CommandQueue()
        {
        }

        //Pre: Valid command
        //Post: None
        //Description: Add the command to the back of the queue
        public void Enqueue(Command newCommand)
        {
            queue.Add(newCommand);
        }

        //Pre: None
        //Post: Returns the front element of the queue
        //Description: Returns and removes the element at the front of the queue, or returns null if there is no element
        public Command Dequeue()
        {
            //Store the result
            Command result = null;

            //Remove a command if possible
            if (queue.Count > 0)
            {
                result = queue[0];
                queue.RemoveAt(0);
            }
            return result;
        }

        //Pre: None
        //Post: Returns the current size of the queue
        //Description: Returns the current number of commands in the queue
        public int Size()
        {
            return queue.Count;
        }

        //Pre: None
        //Post: Returns true if the size of the queue is 0, false otherwise
        //Description: Compare the size of the queue against 0 to determine if it's empty
        public bool IsEmpty()
        {
            return queue.Count == 0;
        }
    }
}