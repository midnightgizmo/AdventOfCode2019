using System;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            PuzzleOne puzzleOne = new PuzzleOne();
            int Answer = puzzleOne.ComputeValidPasswords(168630, 718098);
            */
            DateTime nowTime = DateTime.Now;
            PuzzleTwo puzzleTwo = new PuzzleTwo();
            int Answer = puzzleTwo.ComputeValidPasswords(168630, 718098);
            DateTime afterTime = DateTime.Now;

            Console.WriteLine(afterTime.Subtract(nowTime));

            Console.WriteLine("Answer to question:" + Answer);
            Console.ReadKey();
        }
    }
}
