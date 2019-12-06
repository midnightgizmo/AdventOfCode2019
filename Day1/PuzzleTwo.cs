using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day1
{
    public class PuzzleTwo
    {
        private string _fileData;
        private List<int> _ModuleMassList;

        public PuzzleTwo LoadData(string fileLocation)
        {

            if (File.Exists(fileLocation))
            {
                this._fileData = File.ReadAllText(fileLocation);
                return this;
            }
            else
                return null;


        }

        public PuzzleTwo ParseData()
        {
            string[] DataSplitByLines;
            this._ModuleMassList = new List<int>();


            DataSplitByLines = this._fileData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string aLine in DataSplitByLines)
                _ModuleMassList.Add(int.Parse(aLine));

            return this;
        }

        public int CaculateTotalFuleRequierments()
        {
            int TotalFuleRequierments = 0;

            foreach (int moduleMass in this._ModuleMassList)
            {
                int fuelForModual = this.CaculateFuelForModule(moduleMass);
                int fuelNeededForFuel = this.CaculateAddishanalFuel(fuelForModual);

                TotalFuleRequierments += fuelForModual + fuelNeededForFuel;
            }

            return TotalFuleRequierments;
        }

        private int CaculateFuelForModule(int ModuleMass)
        {
            return (int)((ModuleMass / 3) - 2);
        }

        private int CaculateAddishanalFuel(int fuel)
        {
            int extraFuleNeeded = 0;


            

            int currentFuel = this.CaculateFuelForModule(fuel);
            extraFuleNeeded += currentFuel;

            while (currentFuel > 0)
            {
                currentFuel = this.CaculateFuelForModule(currentFuel);
                if(currentFuel > 0)
                    extraFuleNeeded += currentFuel;
            }
            return extraFuleNeeded;
        }
    }
}
