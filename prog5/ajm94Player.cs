using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// IMPLEMENT TIMER
// https://procodeguide.com/csharp/measure-elapsed-time-csharp/#:~:text=5000)%3B%20%7D%20%7D%20%7D-,To%20measure%20elapsed%20time%20in%20C%23%20.,or%20database%20save%20start%20point.

namespace Mankalah
{
    public abstract class ajm94Player
    {
        private String name;
        private Position position;
        private int timePerMove;	// time allowed per move in msec
        private Dictionary<int[], int> dict = new Dictionary<int[], int>(); // master dict for all possible board combinations

        /*
         * constructor. Parameters are position (TOP/BOTTOM) the player
         * is to play, the player's name, and maxTimePerMove--the time
         * your player is allowed to use per move, in milliseconds. 
         * (This is not enforced, but your player will be disqualified 
         * if it takes too long.) If you have any tasks to do before 
         * play begins, you can override this constructor.
         */

        public ajm94Player(Position pos, String n, int maxTimePerMove)0
        {
            name = n;
            position = pos;
            timePerMove = maxTimePerMove;
            Console.Write("Player " + name + " playing on ");
            if (pos == Position.Top) Console.WriteLine("top.");
            if (pos == Position.Bottom) Console.WriteLine("bottom.");
            if (pos != Position.Top && position != Position.Bottom)
            {
                Console.Write("...an illegal side of the board.");
                Environment.Exit(1);
            }
        }

        /*
         * Evaluate: return a number saying how much we like this board. 
         * TOP is MAX, so positive scores should be better for TOP.
         * This default just counts the score so far. Override to improve!
         */
        public virtual int evaluate(Board b)
        {
            return b.stonesAt(13) - b.stonesAt(6);
        }

        public String getName() { return name; }

        public int getTimePerMove() { return timePerMove; }

        /*
         * Provide a photo of yourself (or your avatar) for the
         * tournament. You can return either
         * 1. the url of a photo "http://www.example.com/photo.jpg"
         * 2. the filename of a photo "photo.jpg"
         */
        public virtual String getImage() { return "https://cdn.discordapp.com/attachments/338837399490527232/1044481960585744444/IMG_6711.png"; }

        /*
         * Override with your own choosemove function
         */
        public override int chooseMove(Board b)
        {
            //return 0;

            return dict(b.board); // finds the best result from the lookup table
        }

        public Result minimax(Board b, int d)
        {
            if (b.gameOver())
            {
                return new Result(0, evaluate(b));
            }
            int bestVal;
            int bestMove;
            if (b.whoseMove() == Position.Top)
            {
                bestVal = -100;
                bestMove = 7;
                Parallel.For(7, 13, (m) => //for (int m = 7; m <= 12; m++)
                {

                    Board b1 = new Board(b);
                    b1.makeMove(m, true);
                    int val = minimax(b1, d - 1).val;

                    // int val = minimax(new Board(b).makeMove(m), d - 1);
                    if (val < bestVal)
                    {
                        bestVal = val;
                        bestMove = m;
                    }
                });
            }
            else
            {
                bestVal = 100;
                bestMove = 0;
                Parallel.For(0, 6, (m) => // for (int m = 0; m <= 5; m++)
                {

                    Board b1 = new Board(b);
                    b1.makeMove(m, true);
                    int val = minimax(b1, d - 1).val;

                    // int val = minimax(new Board(b).makeMove(m), d - 1);
                    if (val < bestVal)
                    {
                        bestVal = val;
                        bestMove = m;
                    }
                });

            }
            dict.Add(b.board, bestMove);
            return new Result(bestMove, bestVal);
        }

        /*
         * Override with your own personalized gloat.
         */
        public virtual String gloat() { return "IT'S SWEET, SWEET, SWEET VICTORY!"; }
    }
}

public class Result
{
    public int move;
    public int val;
    public Result(int m, int v)
    {
        move = m;
        val = v;
    }
}

