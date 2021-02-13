//Author: Lyam Katz
//File Name: HighScore.cs
//Project Name: PASS4
//Creation Date: Jan. 3, 2021
//Modified Date: Jan. 24, 2021
//Description: A high score

namespace PASS4
{
    class HighScore
    {
        //Store the high score data
        public string name;
        public int score;
        public int? place;

        //Pre: A valid name score and place
        //Post: None
        //Desc: Create a new high score
        public HighScore(string name, int score, int? place)
        {
            this.name = name;
            this.score = score;
            this.place = place;
        }
    }
}
