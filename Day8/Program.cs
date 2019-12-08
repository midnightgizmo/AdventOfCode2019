using System;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            /*
            PuzzleOne puzzleOne = new PuzzleOne();
            int Answer = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .CheckImage();

            Console.WriteLine("Answer:" + Answer);
            Console.ReadKey();*/


            PartTwo.PuzzleTwo puzzleTwo = new PartTwo.PuzzleTwo();
            puzzleTwo.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .CreateImage();
        }
    }
}
