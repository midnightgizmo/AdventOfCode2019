using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day2
{
    public class PuzzleTwo
    {



        private string _fileData;
        private List<int> _IntcodeList;

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
            string[] DataSplitByCommas;
            this._IntcodeList = new List<int>();


            DataSplitByCommas = this._fileData.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (string aLine in DataSplitByCommas)
                _IntcodeList.Add(int.Parse(aLine));

            return this;
        }

        public int[] FindInputCodes(int expectedOutput)
        {
            int IndexStartPosition = 0;
            CopyList(this._IntcodeList, this._OriginalData);

          
            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    this._IntcodeList[1] = noun;
                    this._IntcodeList[2] = verb;

                    do
                    {
                        IndexStartPosition = this.FixSectionOfData(IndexStartPosition);

                    }while(IndexStartPosition != -1);

                    if (this._IntcodeList[0] == expectedOutput)
                        return new int[]{ noun, verb };

                    IndexStartPosition = 0;
                    ResetData();
                }
            }

            return null;
        }

        private List<int> _OriginalData = new List<int>();
        private void CopyList(List<int> from, List<int> too)
        {
            foreach (int number in from)
                too.Add(number);
        }
        private void ResetData()
        {
            this._IntcodeList.Clear();
            this.CopyList(this._OriginalData, this._IntcodeList);
        }

        private int FixSectionOfData(int startIndexPosition)
        {
            int NewValueToCompute = 0;

            if (startIndexPosition > ((this._IntcodeList.Count - 1) - 4))
                return -1;


            switch (this._IntcodeList[startIndexPosition])
            {
                case 1:
                    NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] + this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    break;

                case 2:
                    NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] * this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    break;

                case 99:
                    //Console.WriteLine(this._IntcodeList[this._IntcodeList[startIndexPosition - 1]]);
                    //Console.WriteLine(this._IntcodeList[0]);
                    return -1;

                default:
                    throw new Exception("unknown code");
            }

            // where do we need to put the new value
            this._IntcodeList[this._IntcodeList[startIndexPosition + 3]] = NewValueToCompute;

            /*if (this._IntcodeList[0] == 19690720)
            {
                int i = 0;
            }*/

            // return new start index for next section or -1 if finished
            return startIndexPosition + 4;
        }





    }
}
    
