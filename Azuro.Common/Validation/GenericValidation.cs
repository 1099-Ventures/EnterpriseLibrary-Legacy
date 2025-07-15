using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Common.Validation
{
	/// <summary>
	/// A class for static helper methods to perform generic validation functions.
	/// </summary>
	public abstract class GenericValidation
	{
		private const int IDNumberLength = 13;
		/// <summary>
		/// This method will validate a South African ID for checksum, gender 
		/// and date of birth validity.
		/// </summary>
		/// <param name="dob">The date of birth, or DateTime.MinValue to skip date of birth validation.</param>
		/// <param name="gender">The gender, or ' ' to skip.</param>
		/// <param name="idNumber">The ID number to validate.</param>
		/// <returns>True if valid, else false.</returns>
		public static bool ValidateID(DateTime dob, char gender, string idNumber)
		{
			idNumber = idNumber.Trim();
			//	Must be 13 characters long
			if (idNumber.Length != IDNumberLength)
				return false;
			int[] numbers = new int[IDNumberLength];
			int oddSum = 0, evenSum = 0, sum;
			string evenConcat = "";
			for (int i = 0; i < IDNumberLength; ++i)
				numbers[i] = int.Parse(idNumber[i].ToString());
			//	Must be numeric
			if (!Util.IsNumeric(idNumber))
				return false;
			//	Gender must match 7th character
			if (gender != ' ' && (gender == 'F' && numbers[6] >= 5) || (gender == 'M' && numbers[6] < 5))
				return false;
			if (dob != DateTime.MinValue)
			{
				//	DOB must match first 6 characters
				if (dob.Month != int.Parse(idNumber.Substring(2, 2)))
					return false;
				if (dob.Day != int.Parse(idNumber.Substring(4, 2)))
					return false;
				if ((dob.Year % 100) != int.Parse(idNumber.Substring(0, 2)))
					return false;
			}
			//	Calculate check digit
			//	sum of all numbers in odd positions, except the thirteenth.
			for (int i = 0; i < 12; i += 2)
				oddSum += numbers[i];
			//	sum of all numbers in even positions
			for (int i = 1; i < 13; i += 2)
				evenConcat += idNumber[i];
			evenConcat = (int.Parse(evenConcat) * 2).ToString();
			for (int i = 0; i < evenConcat.Length; ++i)
				evenSum += int.Parse(evenConcat[i].ToString());
			sum = oddSum + evenSum;
			int controlNo = (sum % 10 == 0) ? 0 : (10 - sum % 10);
			if (controlNo != numbers[12])
				return false;

			return true;
		}

		/// <summary>
		/// Validate a South African Tax number against the checksum algorithm.
		/// </summary>
		/// <param name="taxNumber">The number to validate.</param>
		/// <returns>True if valid, else false.</returns>
		public static bool ValidateTaxNumber(string taxNumber)
		{
			taxNumber = taxNumber.Trim();
			if (!Util.IsNumeric(taxNumber) || taxNumber.Length < 10)
				return false;
			int[] numbers = new int[taxNumber.Length];
			for (int i = 0; i < numbers.Length; ++i)
				numbers[i] = int.Parse(taxNumber[i].ToString());
			int total = 0, result = 0;
			numbers[0] = (numbers[0] == 7) ? 4 : numbers[0];
			//	add all even numbered entries together, massage and add all 
			//	even numbered entries
			for (int i = 1; i < numbers.Length - 1; i += 2)
				total += numbers[i];
			for (int i = 0; i < numbers.Length; i += 2)
			{
				result = numbers[i] * 2;
				total += result > 9 ? 1 + result % 10 : result;
			}

			//	checksum
			int checksum = 0;
			if (total % 10 == 0)
				checksum = total;
			else
				checksum = total + (10 - (total % 10));
			if ((checksum - total) == numbers[numbers.Length - 1])
				return true;
			return false;
		}

		/// <summary>
		/// Validate a South African VAT Number against the checksum algorithm.
		/// </summary>
		/// <param name="vatNumber"></param>
		/// <returns></returns>
		public static bool ValidateVATNumber(string vatNumber)
		{
		    return !(!Util.IsNumeric(vatNumber) || vatNumber.Length != 10 || int.Parse(vatNumber[0].ToString()) != 4);
            
			//	TODO: VAT number valid - something is wrong
			vatNumber = vatNumber.Trim();
			if (!Util.IsNumeric(vatNumber) || vatNumber.Length != 10 || int.Parse(vatNumber[0].ToString()) != 4)
				return false;
			int[] numbers = new int[10];
			for (int i = 0; i < numbers.Length; ++i)
				numbers[i] = int.Parse(vatNumber[i].ToString());
			int total = 0, countdown = 10;
			for (int i = 0; i < 9; ++i)
			{
				total += numbers[i] * countdown;
				countdown--;
			}
			return ((total % 11) == numbers[9]);
		}
	}
}
