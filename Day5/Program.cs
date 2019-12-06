using System;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            /*
            PuzzleOne puzzleOne = new PuzzleOne();
            int Answer = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleOneData.txt")
                .ParseData()
                .RunIntCodeComputer(1);
            */

            PartTwo.PuzzleTwo puzzleTwo= new PartTwo.PuzzleTwo();
            int Answer = puzzleTwo.LoadData(currentWorkingDirectory + "\\PuzzleOneData.txt")
                .ParseData()
                .RunIntCodeComputer(5);

            Console.WriteLine("Answer:" + Answer);
            Console.ReadKey();
        }
    }
}
