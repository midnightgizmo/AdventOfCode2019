using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day1
{
    public class PuzzleOne
    {
        private string _fileData;
        private List<int> _ModuleMassList;

        public PuzzleOne LoadData(string fileLocation)
        {

            if (File.Exists(fileLocation))
            {
                this._fileData = File.ReadAllText(fileLocation);
                return this;
            }
            else
                return null;


        }

        public PuzzleOne ParseData()
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
                TotalFuleRequierments += this.CaculateFuelForModule(moduleMass);

            return TotalFuleRequierments;
        }

        private int CaculateFuelForModule(int ModuleMass)
        {
            return (int)((ModuleMass / 3) - 2);
        }
    }
}
