using System;
using System.Collections.Generic;
using System.Text;

namespace Day4
{
    public class PuzzleTwo
    {
        public int ComputeValidPasswords(int StartAt, int EndAt)
        {
            List<int> ValidNumbers = new List<int>();

            for (int i = StartAt; i <= EndAt; i++)
            {
                if (IsValidNumber(i))
                    ValidNumbers.Add(i);
            }

            return ValidNumbers.Count;
        }

        private bool IsValidNumber(int number)
        {
            string sNumber = number.ToString();
            bool wasSameAdjacentNumberFound = false;

            for (int i = 0; i < sNumber.Length; i++)
            {

                if (i > 0)
                {
                    // make sure current digit is at least the same or greater than the digit to the left of it
                    if (sNumber[i] < sNumber[i - 1])
                        return false;

                    if (sNumber[i] == sNumber[i - 1])
                    {
                        // make sure this number is not repeated more than twice in one go.
                        // check the numbers at positions sNumber[i+1] and sNumber[i-2]

                        // make sure we dont go outside the index range
                        if (i - 2 >= 0)
                            if (sNumber[i - 2] == sNumber[i])
                                continue; // we found 3 same numbers together so this is not a match for 2 adjacent numbers

                        if (i + 1 < sNumber.Length)
                            if (sNumber[i + 1] == sNumber[i])
                                continue; // we found 3 same numbers together so this is not a match for 2 adjacent numbers

                        wasSameAdjacentNumberFound = true;
                    }


                }
            }

            if (wasSameAdjacentNumberFound)
                return true;
            else
                return false;
        }
    }

 
}
