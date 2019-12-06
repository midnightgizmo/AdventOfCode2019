using System;
using System.IO;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            /*
            PuzzleOne puzzleOne = new PuzzleOne();
            string fixedData = puzzleOne.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .FixData();

            Console.WriteLine(fixedData);
            Console.ReadKey();
            */

            PuzzleTwo puzzleTwo = new PuzzleTwo();
            int expectedOutput = 19690720;

            Console.WriteLine($"Searching for noun & verb that produce expected output of {expectedOutput}");
            Console.WriteLine("...");
            Console.WriteLine("");
            int[] inputCodes = puzzleTwo.LoadData(currentWorkingDirectory + "\\PuzzleData.txt")
                .ParseData()
                .FindInputCodes(expectedOutput);

            Console.WriteLine($"Match found:noun={inputCodes[0]}  verb={inputCodes[1]}");
            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        }
    }
}
