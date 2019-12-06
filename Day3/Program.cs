using System;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            
            /*
            PuzzleOne puzzleOne = new PuzzleOne();

            int answer = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .FindIntersections();

            Console.WriteLine(answer);
            */
            Day3.partTwo.PuzzleTwo puzzleTwo = new Day3.partTwo.PuzzleTwo();

            int puzzleTwoAnswer = puzzleTwo.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .FindIntersections();

            Console.WriteLine(puzzleTwoAnswer);

        }
    }
}
