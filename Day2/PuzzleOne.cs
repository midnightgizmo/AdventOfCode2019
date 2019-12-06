using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day2
{
    public class PuzzleOne
    {
        private string _fileData;
        private List<int> _IntcodeList;

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
            string[] DataSplitByCommas;
            this._IntcodeList= new List<int>();


            DataSplitByCommas = this._fileData.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (string aLine in DataSplitByCommas)
                _IntcodeList.Add(int.Parse(aLine));

            return this;
        }

        public string FixData()
        {
            int IndexStartPosition = 0;

            do
            {
                IndexStartPosition = this.FixSectionOfData(IndexStartPosition);

            } while (IndexStartPosition != -1);

            StringBuilder sb = new StringBuilder();
            foreach (int aNumber in this._IntcodeList)
                sb.Append(aNumber.ToString() + ",");

            return sb.ToString();
        }

        private int FixSectionOfData(int startIndexPosition)
        {
            int NewValueToCompute = 0;

            if (startIndexPosition > ((this._IntcodeList.Count - 1) - 4))
                return -1;
            /*
            // check if the value = 99 (which means stop the program)
            if (this._IntcodeList[startIndexPosition] == 99)
                return -1;
            else if(this._IntcodeList[startIndexPosition] == 1)
            {// its an addision
                NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] + this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
            }
            else if (this._IntcodeList[startIndexPosition] == 2)
            {// its a multiplication
                NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] * this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
            }
            else
                throw new Exception("unknown code");*/

            switch(this._IntcodeList[startIndexPosition])
            {
                case 1:
                    NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] + this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    break;

                case 2:
                    NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] * this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    break;

                case 99:
                    return -1;

                default:
                    throw new Exception("unknown code");
            }

            // where do we need to put the new value
            this._IntcodeList[this._IntcodeList[startIndexPosition + 3]] = NewValueToCompute;

            // return new start index for next section or -1 if finished
            return startIndexPosition + 4;
        }
    }
}

