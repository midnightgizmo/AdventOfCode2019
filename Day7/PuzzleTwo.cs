using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Day7.PartTwo
{
    public class PuzzleTwo
    {
        private string _fileData;
        private List<int> _IntcodeList;



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
            // list of all Thruster signal values
            List<int> ThrustersValue = new List<int>();

            int[] PhaseSettingSequence = new int[] { 5, 6, 7, 8, 9 };

            int OutputParameter = -10;

            // find all combinations of the phaseSquences
            List<int[]> AllCombinations = FindAllCombinations(PhaseSettingSequence);


            int lastOutput = InputParameter;

            // run through each phaseSequence
            foreach (int[] aPhaseSettingsSequence in AllCombinations)
            {
                // each time a new phase sequence starts we have to reset the lastOuput parameter to the input parameter
                lastOutput = InputParameter;
                
                // we need to keep track of each Amplifier because they will not finish in one go 
                // (they will pause and wait for new input)
                IntCodeComputer[] _IntCodeComputerArray = new IntCodeComputer[aPhaseSettingsSequence.Length];

                // the first time around each Amplifier runs we have to pass in its Phase setting
                for (int eachPhaseSettingIndex = 0; eachPhaseSettingIndex < aPhaseSettingsSequence.Length; eachPhaseSettingIndex++)
                {
                    List<int> InputParameters = new List<int>();
                    // get the phase settings (eaither 5,6,7,8 or 9)
                    int phaseSetting = aPhaseSettingsSequence[eachPhaseSettingIndex];
                    IntCodeComputer _IntCodeComputer = null;
                    if (_IntCodeComputerArray[eachPhaseSettingIndex] == null)
                    {
                        _IntCodeComputerArray[eachPhaseSettingIndex] = new IntCodeComputer();
                        _IntCodeComputer = _IntCodeComputerArray[eachPhaseSettingIndex];

                        // add the phase settings as the input
                        InputParameters.Add(phaseSetting);
                    }
                    
                    

                    // construct the parameters that we will need for for the intCodeComputer we are about to trun
                    // add the last ouput from the previouse amplifier
                    InputParameters.Add(lastOutput);
                    // take the output from this run and pass it as a parameter on the next run (of this current phase sequence)
                    OutputParameter = _IntCodeComputer.RunIntCodeComputer(InputParameters, _IntcodeList.ToList());
                    lastOutput = OutputParameter;
                }

                // each ammplifier has now run once and passed in there phase setting.
                // from now on all parameters that are passed in which be the output from the previouse amplifier.
                bool IsStillRunning = true;
                // keep going through each amplifier grabbing there output and pass that output into the next amplifier.
                // we will keep doing this until all amplifiers return op code 99.
                while(IsStillRunning)
                {
                    for(int eachComputerIndex = 0; eachComputerIndex < _IntCodeComputerArray.Length; eachComputerIndex++)
                    {
                        List<int> InputParameters = new List<int>();
                        IntCodeComputer computer = _IntCodeComputerArray[eachComputerIndex];

                        InputParameters.Add(lastOutput);
                        lastOutput = computer.ContinueFromPausedState(InputParameters);

                        // check if this is the last amplifer in the array.
                        if (eachComputerIndex == _IntCodeComputerArray.Length -1)
                        {// does this amplifier need to go and call the first apmplfier again or has it finished

                            // if amplifier has finished we can exit the while loop
                            if (computer.HasFinished == true)
                                IsStillRunning = false;
                        }
                    }
                }
                // finished running a phase sequence (eg..g 0,1,2,3,4) and feedback loop
                // store the final value of this sequnce and move onto the next phase sequence
                ThrustersValue.Add(lastOutput);


            }

            // sort the numbers from lowest to highest
            ThrustersValue.Sort();


            // retuns the highest signal that can be sent to the thrusters
            return ThrustersValue[ThrustersValue.Count - 1];
        }




        /// <summary>
        /// Finds all possible combinations of the supplied number list
        /// </summary>
        /// <param name="list">The list of numbers to use to find all possible combinations</param>
        /// <returns>List of all possible combinations</returns>        
        public List<int[]> FindAllCombinations(int[] list)
        {
            // the place to store the combinations
            List<int[]> permucationsList = new List<int[]>();

            void swapTwoNumber(ref int a, ref int b)
            {
                int temp = a;
                a = b;
                b = temp;
            }

            List<int[]> CombinationsCalculator(int[] list, int k, int m)
            {
                int i;
                if (k == m)
                {
                    int[] numbers = new int[m + 1];
                    for (i = 0; i <= m; i++)
                        numbers[i] = list[i];

                    permucationsList.Add(numbers);


                }
                else
                    for (i = k; i <= m; i++)
                    {
                        swapTwoNumber(ref list[k], ref list[i]);
                        CombinationsCalculator(list, k + 1, m);
                        swapTwoNumber(ref list[k], ref list[i]);
                    }

                return permucationsList;
            }

            CombinationsCalculator(list, 0, list.Length - 1);
            return permucationsList;
        }
    }

    class IntCodeComputer
    {
        private List<int> _IntcodeList;
        private int _LastOutPutCodeComputed = -2;
        private int _StartPosition = 0;

        private bool _HasFinished = false;

        public bool HasFinished
        {
            get { return _HasFinished; }
            
        }


        public int RunIntCodeComputer(List<int> InputParameters, List<int> IntCodeList)
        {
            int IndexStartPosition = this._StartPosition;
            //List<int> InputParameters = new List<int>();
            int OutputParameter;
            bool ShouldPauseComputer;

            this._IntcodeList = IntCodeList;

            //InputParameters.Add(InputParameter);
            //InputParameters.Add(3);
            do
            {
                IndexStartPosition = this.RunNextSection(IndexStartPosition, InputParameters, out OutputParameter, out ShouldPauseComputer);

            } while (IndexStartPosition != -1 && ShouldPauseComputer == false);

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
        public int ContinueFromPausedState(List<int> InputParameters)
        {
            return this.RunIntCodeComputer(InputParameters, this._IntcodeList);
        }





        private int RunNextSection(int startIndexPosition, List<int> InputParameters, out int OutputParameter, out bool ShouldPauseComputer)
        {
            int NewValueToCompute = 0;

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

                    this.Input(this._IntcodeList, startIndexPosition, InputParameters[0]);
                    InputParameters.RemoveAt(0);
                    NextIndexStartPosition = startIndexPosition + 2;
                    break;

                case OpCodeMode.Output:
                    OutputParameter = this.Output(this._IntcodeList, opCodeParser, startIndexPosition);
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
                    NextIndexStartPosition = this.JumpIfTrue(this._IntcodeList, opCodeParser, startIndexPosition);
                    break;


                case OpCodeMode.JumpIfFalse:
                    NextIndexStartPosition = this.JumpIfFalse(this._IntcodeList, opCodeParser, startIndexPosition);
                    break;

                case OpCodeMode.LessThan:
                    this.LessThan(this._IntcodeList, opCodeParser, startIndexPosition);
                    NextIndexStartPosition = startIndexPosition + 4;
                    break;

                case OpCodeMode.Equals:
                    this.Equals(this._IntcodeList, opCodeParser, startIndexPosition);
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
        /// Add 2 numbers together.
        /// </summary>
        /// <param name="IntcodeList">data for the IntCodeComputer to use</param>
        /// <param name="opCodeData">information about how each parameter should operate (Position mode or Immediate mode)</param>
        /// <param name="startIndexPosition">the index position in IntcodeList the OpCode starts at that we are currently looking at</param>
        private void Addision(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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
        private void Multiplication(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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

        private void Input(List<int> IntcodeList, int startIndexPosition, int InputValue)
        {
            IntcodeList[IntcodeList[startIndexPosition + 1]] = InputValue;
        }

        private int Output(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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
        private int JumpIfTrue(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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

        private int JumpIfFalse(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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

        private void LessThan(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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



            if (paramOneValue < paramTwoValue)
            {
                IntcodeList[IntcodeList[startIndexPosition + 3]] = 1;
            }
            else
            {
                IntcodeList[IntcodeList[startIndexPosition + 3]] = 0;
            }


        }

        private void Equals(List<int> IntcodeList, OpCodeParser opCodeData, int startIndexPosition)
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
        Halt = 99
    }


}
