using System;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();

            /*PuzzleOne puzzleOne = new PuzzleOne();
            int Answer = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .RunIntCodeComputer(0);*/

            PartTwo.PuzzleTwo puzzleTwo = new PartTwo.PuzzleTwo();
            int Answer = puzzleTwo.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .RunIntCodeComputer(0);

            Console.WriteLine("Answer:" + Answer);
            Console.ReadKey();
        }
    }
}
