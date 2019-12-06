using System;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();

            PuzzleOne puzzleOne = new PuzzleOne();
            int TotalFuleRequierments = puzzleOne.LoadData(currentWorkingDirectory + "\\ModuleMassList.txt")
                .ParseData()
                .CaculateTotalFuleRequierments();

            PuzzleTwo puzzleTwo = new PuzzleTwo();
            int TotalFuel = puzzleTwo.LoadData(currentWorkingDirectory + "\\ModuleMassList.txt")
                .ParseData()
                .CaculateTotalFuleRequierments();
        }


    }
}
