using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day5.PartTwo
{
    public class PuzzleTwo
    {
        private string _fileData;
        private List<int> _IntcodeList;

        private IntCodeComputer _IntCodeComputer = new IntCodeComputer();

        private int _LastOutPutCodeComputed = -2;
        public int LastOutPutCodeComputed
        {
            get
            {
                return _LastOutPutCodeComputed;
            }
        }
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

        public int RunIntCodeComputer(int InputParameter)
        {
            int IndexStartPosition = 0;
            int OutputParameter;
            do
            {
                IndexStartPosition = this.RunNextSection(IndexStartPosition, InputParameter, out OutputParameter);

            } while (IndexStartPosition != -1);



            return OutputParameter;
        }

        private int RunNextSection(int startIndexPosition, int InputParameter, out int OutputParameter)
        {
            int NewValueToCompute = 0;

            // the value that will be returned from this function (-2 should never be returned)
            int NextIndexStartPosition = -2;

            OutputParameter = -1;

            //if (startIndexPosition > ((this._IntcodeList.Count - 1) - 4))
            //    return -1;

            OpCodeParser opCodeParser = new OpCodeParser(this._IntcodeList[startIndexPosition]);


            //switch (this._IntcodeList[startIndexPosition])
            switch (opCodeParser.FunctionMode)
            {
                case OpCodeMode.Addision:
                    //NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] + this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    _IntCodeComputer.Addision(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Multiplication:
                    //NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] * this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    _IntCodeComputer.Multiplication(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Input:
                    _IntCodeComputer.Input(this._IntcodeList, startIndexPosition, InputParameter);
                    NextIndexStartPosition = startIndexPosition + 2;
                    break;

                case OpCodeMode.Output:
                    OutputParameter = _IntCodeComputer.Output(this._IntcodeList, opCodeParser, startIndexPosition);
                    this._LastOutPutCodeComputed = OutputParameter;

                    NextIndexStartPosition = startIndexPosition + 2;
                    if (this._IntcodeList[NextIndexStartPosition] == (int)OpCodeMode.Halt)
                    {
                        return -1;
                    }
                    break;

                case OpCodeMode.JumpIfTrue:
                    NextIndexStartPosition  = _IntCodeComputer.JumpIfTrue(this._IntcodeList, opCodeParser, startIndexPosition);
                    break;


                case OpCodeMode.JumpIfFalse:
                    NextIndexStartPosition = _IntCodeComputer.JumpIfFalse(this._IntcodeList, opCodeParser, startIndexPosition);
                    break;

                case OpCodeMode.LessThan:
                    _IntCodeComputer.LessThan(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Equals:
                    _IntCodeComputer.Equals(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;



                case OpCodeMode.Halt:
                    OutputParameter = -1;
                    return -1;

                default:
                    throw new Exception("unknown code");

            }

            // where do we need to put the new value
            //this._IntcodeList[this._IntcodeList[startIndexPosition + 3]] = NewValueToCompute;

            // return new start index for next section or -1 if finished
            return NextIndexStartPosition;
        }


        /// <summary>
        /// A test function, not needed in the main program
        /// </summary>
        /// <returns></returns>
        private string IntCodeListToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in this._IntcodeList)
                sb.Append(i.ToString() + ",");

            return sb.ToString();
        }
    }


    class IntCodeComputer
    {
        /// <summary>
        /// Add 2 numbers together.
        /// </summary>
        /// <param name="IntcodeList">data for the IntCodeComputer to use</param>
        /// <param name="opCodeData">information about how each parameter should operate (Position mode or Immediate mode)</param>
        /// <param name="startIndexPosition">the index position in IntcodeList the OpCode starts at that we are currently looking at</param>
        public void Addision(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            int paramOneValue = 0;
            int paramTwoValue = 0;

            int NewValueToCompute;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];


            // do the sum
            NewValueToCompute = paramOneValue + paramTwoValue;

            // where do we need to put this new value
            IntcodeList[IntcodeList[startIndexPosition + 3]] = NewValueToCompute;
        }

        /// <summary>
        /// multiplys 2 numbers
        /// </summary>
        /// <param name="IntcodeList">data for the IntCodeComputer to use</param>
        /// <param name="opCodeData">information about how each parameter should operate (Position mode or Immediate mode)</param>
        /// <param name="startIndexPosition">the index position in IntcodeList the OpCode starts at that we are currently looking at</param>
        public void Multiplication(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            int paramOneValue = 0;
            int paramTwoValue = 0;

            int NewValueToCompute;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];


            // do the sum
            NewValueToCompute = paramOneValue * paramTwoValue;

            // where do we need to put this new value
            IntcodeList[IntcodeList[startIndexPosition + 3]] = NewValueToCompute;
        }

        public void Input(List<int> IntcodeList, int startIndexPosition, int InputValue)
        {
            IntcodeList[IntcodeList[startIndexPosition + 1]] = InputValue;
        }

        public int Output(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            

            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                return IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                return IntcodeList[startIndexPosition + 1];

            throw new Exception("uknown Parameter Mode");

        }

        /// <summary>
        /// Determins where the next StartIndexPosition should be
        /// </summary>
        /// <param name="IntcodeList"></param>
        /// <param name="opCodeData"></param>
        /// <param name="startIndexPosition"></param>
        /// <returns>The next start index position</returns>
        public int JumpIfTrue(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            int paramOneValue = 0;
            int paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];



            if (paramOneValue != 0)
            {
                return paramTwoValue;
            }
            else
            {
                return startIndexPosition + 3;
            }



        }

        public int JumpIfFalse(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            int paramOneValue = 0;
            int paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];



            if (paramOneValue == 0)
            {
                return paramTwoValue;
            }
            else
            {
                return startIndexPosition + 3;
            }
        }

        public void LessThan(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            int paramOneValue = 0;
            int paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];



            if(paramOneValue < paramTwoValue)
            {
                IntcodeList[IntcodeList[startIndexPosition + 3]] = 1;
            }
            else
            {
                IntcodeList[IntcodeList[startIndexPosition + 3]] = 0;
            }


        }

        public void Equals(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            int paramOneValue = 0;
            int paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];



            if (paramOneValue == paramTwoValue)
            {
                IntcodeList[IntcodeList[startIndexPosition + 3]] = 1;
            }
            else
            {
                IntcodeList[IntcodeList[startIndexPosition + 3]] = 0;
            }


        }





    }
    class OpCodeParser
    {
        public static int ParameterPositionMode = 0;
        public static int ParameterImmediateMode = 1;

        /**
         * Should be a value of 1,2,3 or 4
         * 1 = addision (takes 2 input values)
         * 2 = multiplication (takes 2 input values)
         * 3 = write value of input (parameter) to the address input is pointing to (takes 1 input value)
         * 4 = read value (output to the user) of where input value is pointing to
         */
        public int opCode;
        /**
         * Same as opCode just expressed as an enum
         */
        public OpCodeMode FunctionMode;

        // set all parameterModes to a default of zero
        public int ParameterModeOne = 0;
        public int ParameterModeTwo = 0;
        public int ParameterModeThree = 0;
        public int ParameterModeFour = 0;

        public OpCodeParser(int opCode)
        {
            string sOpCode = opCode.ToString();

            if (sOpCode.Length > 2)
            {// there are parameter modes
                this.ParseOpCodeMode(sOpCode.Substring(sOpCode.Length - 2));

                // find the parameter modes
                int parameterCount = 1;
                for (int i = sOpCode.Length - 3; i >= 0; i--)
                {
                    switch (parameterCount)
                    {
                        case 1:

                            this.ParameterModeOne = int.Parse(sOpCode.Substring(i, 1));
                            break;

                        case 2:

                            this.ParameterModeTwo = int.Parse(sOpCode.Substring(i, 1));
                            break;

                        case 3:

                            this.ParameterModeThree = int.Parse(sOpCode.Substring(i, 1));
                            break;

                        case 4:

                            this.ParameterModeFour = int.Parse(sOpCode.Substring(i, 1));
                            break;
                    }
                    parameterCount++;

                }
            }
            else
            { // there are no parameter modes
                this.ParseOpCodeMode(sOpCode);

                // no parameter modes specified so default to positionMode
                this.ParameterModeOne = OpCodeParser.ParameterPositionMode;
                this.ParameterModeTwo = OpCodeParser.ParameterPositionMode;
                this.ParameterModeThree = OpCodeParser.ParameterPositionMode;
                this.ParameterModeFour = OpCodeParser.ParameterPositionMode;
            }
        }

        public int GetParameterMode(int ParameterNumber)
        {
            switch (ParameterNumber)
            {
                case 1:
                    return this.ParameterModeOne;

                case 2:
                    return this.ParameterModeTwo;

                case 3:
                    return this.ParameterModeThree;

                case 4:
                    return this.ParameterModeFour;
            }

            // not sure why we would get hear but return 0 which indicates Position Mode
            return 0;
        }

        private void ParseOpCodeMode(string opCode)
        {
            int OpCode = int.Parse(opCode);
            switch (OpCode)
            {
                case 1:

                    this.FunctionMode = OpCodeMode.Addision;
                    this.opCode = OpCode;
                    break;

                case 2:

                    this.FunctionMode = OpCodeMode.Multiplication;
                    this.opCode = OpCode;
                    break;

                case 3:

                    this.FunctionMode = OpCodeMode.Input;
                    this.opCode = OpCode;
                    break;

                case 4:

                    this.FunctionMode = OpCodeMode.Output;
                    this.opCode = OpCode;
                    break;

                case 5:

                    this.FunctionMode = OpCodeMode.JumpIfTrue;
                    this.opCode = OpCode;
                    break;

                case 6:

                    this.FunctionMode = OpCodeMode.JumpIfFalse;
                    this.opCode = OpCode;
                    break;

                case 7:

                    this.FunctionMode = OpCodeMode.LessThan;
                    this.opCode = OpCode;
                    break;

                case 8:

                    this.FunctionMode = OpCodeMode.Equals;
                    this.opCode = OpCode;
                    break;


                case 99:

                    this.FunctionMode = OpCodeMode.Halt;
                    this.opCode = OpCode;
                    break;

            }
        }




    }
    enum OpCodeMode { Addision = 1, 
                      Multiplication = 2, 
                      Input = 3, 
                      Output = 4, 
                      JumpIfTrue = 5,
                      JumpIfFalse = 6,
                      LessThan = 7,
                      Equals = 8,
                      Halt = 99 }


}
