using System;
using System.Collections.Generic;
using System.Text;

namespace Day4
{
    public class PuzzleOne
    {

        public int ComputeValidPasswords(int StartAt, int EndAt)
        {
            List<int> ValidNumbers = new List<int>();

            for(int i = StartAt; i <= EndAt; i++)
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

            for(int i = 0; i < sNumber.Length; i++)
            {
                if(i > 0)
                {
                    // make sure current digit is at least the same or greater than the digit to the left of it
                    if (sNumber[i] < sNumber[i - 1])
                        return false;

                    if (sNumber[i] == sNumber[i - 1])
                        wasSameAdjacentNumberFound = true;
                }
            }

            if (wasSameAdjacentNumberFound)
                return true;
            else
                return false;
        }
    }
}
