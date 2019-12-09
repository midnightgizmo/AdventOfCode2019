using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Day9
{
    public class PuzzleOne
    {
        private string _fileData;
        private ComputerMemory<long> _IntcodeList;

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
            this._IntcodeList = new ComputerMemory<long>();


            DataSplitByCommas = this._fileData.Split(",", StringSplitOptions.RemoveEmptyEntries);

            
            for(int eachLineIndex = 0; eachLineIndex < DataSplitByCommas.Length; eachLineIndex++)
                _IntcodeList[eachLineIndex] = (long.Parse(DataSplitByCommas[eachLineIndex]));

            return this;
        }

        public long RunIntCodeComputer(long? InputParameter)
        {

            IntCodeComputer computer = new IntCodeComputer();
            List<long> InputParameters = new List<long>();
            long OutputParameter;

            if (InputParameter != null)
                InputParameters.Add(InputParameter.GetValueOrDefault());

            OutputParameter = computer.RunIntCodeComputer(InputParameters, _IntcodeList.Copy());

            return OutputParameter;
        }
    }

    class IntCodeComputer
    {
        private ComputerMemory<long> _IntcodeList;
        private long _LastOutPutCodeComputed = -2;
        private int _StartPosition = 0;
        private int _Relativebase = 0;

        private bool _HasFinished = false;

        public bool HasFinished
        {
            get { return _HasFinished; }

        }


        public long RunIntCodeComputer(List<long> InputParameters, ComputerMemory<long> IntCodeList)
        {
            int IndexStartPosition = this._StartPosition;
            long OutputParameter;
            bool ShouldPauseComputer;

            this._IntcodeList = IntCodeList;


            List <long> outPuts = new List<long>();
            do
            {
                IndexStartPosition = this.RunNextSection(IndexStartPosition, InputParameters, out OutputParameter, out ShouldPauseComputer);

                if (ShouldPauseComputer)
                    outPuts.Add(OutputParameter);

            } while (IndexStartPosition != -1 ); 

            this._StartPosition = IndexStartPosition;
            if (IndexStartPosition == -1)
                this._HasFinished = true;


            //return OutputParameter;
            return this._LastOutPutCodeComputed;
        }

        /// <summary>
        /// (Only call this after the function RunIntCodeComputer() has been called and the IntCodeComputer.HasFinished == false)
        /// The computer will carry on from the last position in the IntCodeList it was paused at.
        /// When calling RunIntCodeComputer, the software will pause when an output value has been found (and the next op code is not 99)
        /// Where the software finds an ouput and the next op code == 99 it will return the output and terminate the program
        /// </summary>
        /// <param name="InputParameters"></param>
        /// <returns></returns>
        public long ContinueFromPausedState(List<long> InputParameters)
        {
            return this.RunIntCodeComputer(InputParameters, this._IntcodeList);
        }





        private int RunNextSection(int startIndexPosition, List<long> InputParameters, out long OutputParameter, out bool ShouldPauseComputer)
        {
            

            // the value that will be returned from this function (-2 should never be returned)
            int NextIndexStartPosition = -2;

            OutputParameter = -1;
            ShouldPauseComputer = false;

            //if (startIndexPosition > ((this._IntcodeList.Count - 1) - 4))
            //    return -1;

            OpCodeParser opCodeParser = new OpCodeParser(this._IntcodeList[startIndexPosition]);


            //switch (this._IntcodeList[startIndexPosition])
            switch (opCodeParser.FunctionMode)
            {
                case OpCodeMode.Addision:
                    //NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] + this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    this.Addision(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Multiplication:
                    //NewValueToCompute = this._IntcodeList[this._IntcodeList[startIndexPosition + 1]] * this._IntcodeList[this._IntcodeList[startIndexPosition + 2]];
                    this.Multiplication(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Input:

                    this.Input(this._IntcodeList, opCodeParser, startIndexPosition, InputParameters[0]);
                    InputParameters.RemoveAt(0);
                    NextIndexStartPosition = startIndexPosition + 2;
                    break;

                case OpCodeMode.Output:
                    OutputParameter = this.Output(this._IntcodeList, opCodeParser, (int)startIndexPosition);
                    this._LastOutPutCodeComputed = OutputParameter;

                    NextIndexStartPosition = startIndexPosition + 2;
                    if (this._IntcodeList[NextIndexStartPosition] == (int)OpCodeMode.Halt)
                    {
                        return -1;
                    }
                    //InputParameters.Add(OutputParameter);
                    ShouldPauseComputer = true;
                    break;

                case OpCodeMode.JumpIfTrue:
                    NextIndexStartPosition = this.JumpIfTrue(this._IntcodeList, opCodeParser, (int)startIndexPosition);
                    break;


                case OpCodeMode.JumpIfFalse:
                    NextIndexStartPosition = this.JumpIfFalse(this._IntcodeList, opCodeParser, (int)startIndexPosition);
                    break;

                case OpCodeMode.LessThan:
                    this.LessThan(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Equals:
                    this.Equals(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.AdjustRelativeBase:
                    this.AdjustRelativeBase(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 2;
                    break;



                case OpCodeMode.Halt:
                    OutputParameter = -1;
                    return -1;

                default:
                    throw new Exception("unknown code");

            }



            // return new start index for next section or -1 if finished
            return NextIndexStartPosition;
        }


        /// <summary>
        /// Add 2 numbers together.
        /// </summary>
        /// <param name="IntcodeList">data for the IntCodeComputer to use</param>
        /// <param name="opCodeData">information about how each parameter should operate (Position mode or Immediate mode)</param>
        /// <param name="startIndexPosition">the index position in IntcodeList the OpCode starts at that we are currently looking at</param>
        private void Addision(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;
            long paramTwoValue = 0;

            long NewValueToCompute;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];
            

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterRelativeMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2] + this._Relativebase];

            // do the sum
            NewValueToCompute = paramOneValue + paramTwoValue;

            // where do we need to put this new value
            //IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = NewValueToCompute;

            if (opCodeData.ParameterModeThree == OpCodeParser.ParameterPositionMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = NewValueToCompute;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterImmediateMode)
                IntcodeList[startIndexPosition + 3] = NewValueToCompute;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterRelativeMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3] + this._Relativebase] = NewValueToCompute;
        }

        /// <summary>
        /// multiplys 2 numbers
        /// </summary>
        /// <param name="IntcodeList">data for the IntCodeComputer to use</param>
        /// <param name="opCodeData">information about how each parameter should operate (Position mode or Immediate mode)</param>
        /// <param name="startIndexPosition">the index position in IntcodeList the OpCode starts at that we are currently looking at</param>
        private void Multiplication(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;
            long paramTwoValue = 0;

            long NewValueToCompute;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterRelativeMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2] + this._Relativebase];


            // do the sum
            NewValueToCompute = paramOneValue * paramTwoValue;

            // where do we need to put this new value
            //IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = NewValueToCompute;

            if (opCodeData.ParameterModeThree == OpCodeParser.ParameterPositionMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = NewValueToCompute;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterImmediateMode)
                IntcodeList[startIndexPosition + 3] = NewValueToCompute;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterRelativeMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3] + this._Relativebase] = NewValueToCompute;
        }

        private void Input(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition, long InputValue)
        {
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 1]] = InputValue;
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                IntcodeList[startIndexPosition + 1] = InputValue;
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase] = InputValue;
        }

        private long Output(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {


            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                return IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                return IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                return IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            throw new Exception("uknown Parameter Mode");

        }

        /// <summary>
        /// Determins where the next StartIndexPosition should be
        /// </summary>
        /// <param name="IntcodeList"></param>
        /// <param name="opCodeData"></param>
        /// <param name="startIndexPosition"></param>
        /// <returns>The next start index position</returns>
        private int JumpIfTrue(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;
            long paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterRelativeMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2] + this._Relativebase];



            if (paramOneValue != 0)
            {
                return (int)paramTwoValue;
            }
            else
            {
                return startIndexPosition + 3;
            }



        }

        private int JumpIfFalse(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;
            long paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterRelativeMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2] + this._Relativebase];



            if (paramOneValue == 0)
            {
                return (int)paramTwoValue;
            }
            else
            {
                return startIndexPosition + 3;
            }
        }

        private void LessThan(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;
            long paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterRelativeMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2] + this._Relativebase];


            long answer;
            if (paramOneValue < paramTwoValue)
            {
                answer = 1;
                //IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = 1;
            }
            else
            {
                answer = 0;
                //IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = 0;
            }

            if (opCodeData.ParameterModeThree == OpCodeParser.ParameterPositionMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = answer;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterImmediateMode)
                IntcodeList[startIndexPosition + 3] = answer;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterRelativeMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3] + this._Relativebase] = answer;


        }

        private void Equals(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;
            long paramTwoValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            ///////////////////////////
            // get parameter Twos value
            if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterPositionMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2]];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterImmediateMode)
                paramTwoValue = IntcodeList[startIndexPosition + 2];
            else if (opCodeData.ParameterModeTwo == OpCodeParser.ParameterRelativeMode)
                paramTwoValue = IntcodeList[(int)IntcodeList[startIndexPosition + 2] + this._Relativebase];


            long answer;
            if (paramOneValue == paramTwoValue)
            {
                answer = 1;
                //IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = 1;
            }
            else
            {
                answer = 0;
                //IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = 0;
            }

            if (opCodeData.ParameterModeThree == OpCodeParser.ParameterPositionMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3]] = answer;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterImmediateMode)
                IntcodeList[startIndexPosition + 3] = answer;
            else if (opCodeData.ParameterModeThree == OpCodeParser.ParameterRelativeMode)
                IntcodeList[(int)IntcodeList[startIndexPosition + 3] + this._Relativebase] = answer;



        }

        private void AdjustRelativeBase(ComputerMemory<long> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
        {
            long paramOneValue = 0;

            ///////////////////////////
            // get parameter ones value
            if (opCodeData.ParameterModeOne == OpCodeParser.ParameterPositionMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1]];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterImmediateMode)
                paramOneValue = IntcodeList[startIndexPosition + 1];
            else if (opCodeData.ParameterModeOne == OpCodeParser.ParameterRelativeMode)
                paramOneValue = IntcodeList[(int)IntcodeList[startIndexPosition + 1] + this._Relativebase];

            this._Relativebase += (int)paramOneValue;
        }





    }
    class OpCodeParser
    {
        public static int ParameterPositionMode = 0;
        public static int ParameterImmediateMode = 1;
        public static int ParameterRelativeMode = 2;

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

        public OpCodeParser(long opCode)
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

                case 9:

                    this.FunctionMode = OpCodeMode.AdjustRelativeBase;
                    this.opCode = OpCode;
                    break;


                case 99:

                    this.FunctionMode = OpCodeMode.Halt;
                    this.opCode = OpCode;
                    break;

            }
        }




    }
    enum OpCodeMode
    {
        Addision = 1,
        Multiplication = 2,
        Input = 3,
        Output = 4,
        JumpIfTrue = 5,
        JumpIfFalse = 6,
        LessThan = 7,
        Equals = 8,
        AdjustRelativeBase = 9,
        Halt = 99
    }

    class ComputerMemory<T>
    {
        private Dictionary<int, T> _IntCode = new Dictionary<int, T>();

        public T this[int i]
        {
            get 
            {
                T returnValue;
                // check if the key exists in the dictionary
                if(this._IntCode.TryGetValue(i,out returnValue) == true)
                {
                    return returnValue;
                }
                else
                {// key does not exist. Add it and set its value to zero
                    
                    this._IntCode[i] = default(T);
                    return default(T);
                }
                //return this._IntCode[i]; 
            }
            set { this._IntCode[i] = value; }
        }

        public ComputerMemory<T> Copy()
        {
            ComputerMemory<T> theCopy = new ComputerMemory<T>();

            foreach (KeyValuePair<int, T> entry in this._IntCode)
            {
                theCopy[entry.Key] = entry.Value;
            }

            return theCopy;

        }



       

    }

}
