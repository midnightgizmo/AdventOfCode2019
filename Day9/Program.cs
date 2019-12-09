using System;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            /*
            PuzzleOne puzzleOne = new PuzzleOne();
            long Answer = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .RunIntCodeComputer(1);

            Console.WriteLine("Answer:" + Answer);
            Console.ReadKey();*/

            PuzzleTwo puzzTwo = new PuzzleTwo();
            long Answer = puzzTwo.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .RunIntCodeComputer(2);

            Console.WriteLine("Answer:" + Answer);
            Console.ReadKey();
        }
    }
}
