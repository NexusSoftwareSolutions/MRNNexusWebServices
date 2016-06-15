using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.ServiceModel;
using MRNNexus_Model;

namespace MRNNexus_Service
{
	public class UserAuthentication : UserNamePasswordValidator
	{
		private const int SaltByteLength = 24;
		private const int DerivedKeyLength = 24;

		//public override void Validate(string userName, string password)
		//{
		//	try
		//	{
		//		MRNNexus_DAL.MRNNexusTestEntities context = new MRNNexus_DAL.MRNNexusTestEntities();

		//		var outputParameter = new System.Data.Entity.Core.Objects.ObjectParameter("new_identity", typeof(int));
		//		//get user hashed pass using provided username
		//		var result = context.proc_GetUser(userName, outputParameter).Single();
		//		string pass = result.Pass;
		//		//foreach (var e in result)
		//		//{
		//		//	pass = e.Pass;
		//		//}
		//		////

		//		if (!VerifyPassword(password, pass))
		//		{
		//			throw new FaultException("Unkown Username and/or Password");
		//		}

		//	} catch(Exception ex)
		//	{
		//		throw new FaultException("No User with that name.");
		//	}
		//}

		public string CreatePasswordHASH(DTO_Employee token)
		{
			var salt = GenerateRandomSalt();
			var iterationCount = 3;
			var hashValue = GenerateHashValue(token.LastName + token.CellPhone.Substring(token.CellPhone.Length - 4), salt, iterationCount);
			var iterationCountBtyeArr = BitConverter.GetBytes(iterationCount);
			var valueToSave = new byte[SaltByteLength + DerivedKeyLength + iterationCountBtyeArr.Length];
			Buffer.BlockCopy(salt, 0, valueToSave, 0, SaltByteLength);
			Buffer.BlockCopy(hashValue, 0, valueToSave, SaltByteLength, DerivedKeyLength);
			Buffer.BlockCopy(iterationCountBtyeArr, 0, valueToSave, salt.Length + hashValue.Length, iterationCountBtyeArr.Length);
			return Convert.ToBase64String(valueToSave);
		}


		private static byte[] GenerateRandomSalt()
		{
			var csprng = new RNGCryptoServiceProvider();
			var salt = new byte[SaltByteLength];
			csprng.GetBytes(salt);
			return salt;
		}

		private static byte[] GenerateHashValue(string password, byte[] salt, int iterationCount)
		{
			byte[] hashValue;
			var valueToHash = string.IsNullOrEmpty(password) ? string.Empty : password;
			using (var pbkdf2 = new Rfc2898DeriveBytes(valueToHash, salt, iterationCount))
			{
				hashValue = pbkdf2.GetBytes(DerivedKeyLength);
			}
			return hashValue;
		}

		private static bool VerifyPassword(string passwordGuess, string actualSavedHashResults)
		{
			//ingredient #1: password salt byte array
			var salt = new byte[SaltByteLength];

			//ingredient #2: byte array of password
			var actualPasswordByteArr = new byte[DerivedKeyLength];

			//convert actualSavedHashResults to byte array
			var actualSavedHashResultsBtyeArr = Convert.FromBase64String(actualSavedHashResults);

			//ingredient #3: iteration count
			var iterationCountLength = actualSavedHashResultsBtyeArr.Length - (salt.Length + actualPasswordByteArr.Length);
			var iterationCountByteArr = new byte[iterationCountLength];
			Buffer.BlockCopy(actualSavedHashResultsBtyeArr, 0, salt, 0, SaltByteLength);
			Buffer.BlockCopy(actualSavedHashResultsBtyeArr, SaltByteLength, actualPasswordByteArr, 0, actualPasswordByteArr.Length);
			Buffer.BlockCopy(actualSavedHashResultsBtyeArr, (salt.Length + actualPasswordByteArr.Length), iterationCountByteArr, 0, iterationCountLength);
			var passwordGuessByteArr = GenerateHashValue(passwordGuess, salt, BitConverter.ToInt32(iterationCountByteArr, 0));
			return ConstantTimeComparison(passwordGuessByteArr, actualPasswordByteArr);
		}

		private static bool ConstantTimeComparison(byte[] passwordGuess, byte[] actualPassword)
		{
			uint difference = (uint)passwordGuess.Length ^ (uint)actualPassword.Length;
			for (var i = 0; i < passwordGuess.Length && i < actualPassword.Length; i++)
			{
				difference |= (uint)(passwordGuess[i] ^ actualPassword[i]);
			}

			return difference == 0;
		}
	}
}