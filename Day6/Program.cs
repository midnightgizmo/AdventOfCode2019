using System;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            /*
            PuzzleOne puzzleOne = new PuzzleOne();
            int PuzzleOneAnswer = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                                            .ParseData().
                                            ComputeTotalDirectAndIndirectOrbits();
            */
            PartTwo.PuzzleTwo puzzleTwo = new PartTwo.PuzzleTwo();
            int PuzzleTwoAnswer = puzzleTwo.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                                            .ParseData().
                                            ComputeTotalDirectAndIndirectOrbits();
        }
    }
}
