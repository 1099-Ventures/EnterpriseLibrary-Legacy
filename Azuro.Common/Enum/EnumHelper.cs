using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azuro.Common
{
	public class EnumHelper<T> where T : struct, System.IComparable
	{
		private enum SupportedType
		{
			Byte,
			SByte,
			Int16,
			UInt16,
			Int32,
			UInt32,
			Int64,
			UInt64
		};

		public delegate T DelegateT(T Op1, T Op2);
		public delegate bool DelegateBool(T Op1, T Op2);
		public static readonly System.Type UnderlyingType;

		public static readonly DelegateT Add;
		public static readonly DelegateT Subtract;
		public static readonly DelegateT Multiply;
		public static readonly DelegateT Divide;
		public static readonly DelegateT Power;
		public static readonly DelegateT And;
		public static readonly DelegateT Or;
		public static readonly DelegateT Xor;

		static EnumHelper()
		{
			System.Type basetype = typeof(T);

			if (!basetype.IsEnum)
			{
				throw (new System.ArgumentException("T must be an Enum"));
			}

			UnderlyingType = System.Enum.GetUnderlyingType(basetype);

			switch ((SupportedType)System.Enum.Parse(typeof(SupportedType), UnderlyingType.Name))
			{
				case SupportedType.Byte:
				case SupportedType.SByte:
					{
						Add = new DelegateT(Int8Add);
						Subtract = new DelegateT(Int8Subtract);
						Multiply = new DelegateT(Int8Multiply);
						Divide = new DelegateT(Int8Divide);
						Power = new DelegateT(Int8Power);
						And = new DelegateT(Int8And);
						Or = new DelegateT(Int8Or);
						Xor = new DelegateT(Int8Xor);
						break;
					}
				case SupportedType.Int16:
				case SupportedType.UInt16:
					{
						Add = new DelegateT(Int16Add);
						Subtract = new DelegateT(Int16Subtract);
						Multiply = new DelegateT(Int16Multiply);
						Divide = new DelegateT(Int16Divide);
						Power = new DelegateT(Int16Power);
						And = new DelegateT(Int16And);
						Or = new DelegateT(Int16Or);
						Xor = new DelegateT(Int16Xor);
						break;
					}
				case SupportedType.Int32:
				case SupportedType.UInt32:
					{
						Add = new DelegateT(Int32Add);
						Subtract = new DelegateT(Int32Subtract);
						Multiply = new DelegateT(Int32Multiply);
						Divide = new DelegateT(Int32Divide);
						Power = new DelegateT(Int32Power);
						And = new DelegateT(Int32And);
						Or = new DelegateT(Int32Or);
						Xor = new DelegateT(Int32Xor);
						break;
					}
				case SupportedType.Int64:
				case SupportedType.UInt64:
					{
						Add = new DelegateT(Int64Add);
						Subtract = new DelegateT(Int64Subtract);
						Multiply = new DelegateT(Int64Multiply);
						Divide = new DelegateT(Int64Divide);
						Power = new DelegateT(Int64Power);
						And = new DelegateT(Int64And);
						Or = new DelegateT(Int64Or);
						Xor = new DelegateT(Int64Xor);
						break;
					}
				default:
					throw new ArgumentException("Type not supported");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumText"></param>
		/// <returns></returns>
		public static T ParseEnum(string enumText)
		{
			object enumValue = null;
			foreach (string s in enumText.Split('|'))
			{
				if (enumValue == null)
					enumValue = Enum.Parse(typeof(T), s);
				else
					enumValue = Or((T)System.Convert.ChangeType(enumValue, UnderlyingType), (T)System.Convert.ChangeType(Enum.Parse(typeof(T), s, true), UnderlyingType));

			}

			return (T)enumValue;
		}

		public static bool Equal(T Op1, T Op2)
		{
			return (Op1.CompareTo(Op2) == 0);
		}

		#region Int8 Methods
		private static T Int8Add(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				+ (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int8Subtract(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				- (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int8Multiply(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				* (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int8Divide(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				/ (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int8Power(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				Math.Pow((System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				, (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType))
				, UnderlyingType));
		}

		private static T Int8And(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				& (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int8Or(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				| (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int8Xor(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.SByte)System.Convert.ChangeType(Op1, UnderlyingType)
				^ (System.SByte)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}
		#endregion //	Int8 Methods

		#region Int16 Methods
		private static T Int16Add(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				+ (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int16Subtract(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				- (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int16Multiply(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				* (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int16Divide(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				/ (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int16Power(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				Math.Pow((System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				, (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType))
				, UnderlyingType));
		}

		private static T Int16And(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				& (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int16Or(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				| (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int16Xor(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int16)System.Convert.ChangeType(Op1, UnderlyingType)
				^ (System.Int16)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}
		#endregion //	Int16 Methods

		#region Int32 Methods
		private static T Int32Add(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				+ (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int32Subtract(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				- (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int32Multiply(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				* (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int32Divide(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				/ (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int32Power(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				Math.Pow((System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				, (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType))
				, UnderlyingType));
		}

		private static T Int32And(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				& (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int32Or(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				| (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int32Xor(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int32)System.Convert.ChangeType(Op1, UnderlyingType)
				^ (System.Int32)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}
		#endregion //	Int32 Methods

		#region Int64 Methods
		private static T Int64Add(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				+ (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int64Subtract(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				- (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int64Multiply(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				* (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int64Divide(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				/ (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int64Power(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				Math.Pow((System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				, (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType))
				, UnderlyingType));
		}

		private static T Int64And(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				& (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int64Or(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				| (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}

		private static T Int64Xor(T Op1, T Op2)
		{
			return ((T)System.Convert.ChangeType(
				(System.Int64)System.Convert.ChangeType(Op1, UnderlyingType)
				^ (System.Int64)System.Convert.ChangeType(Op2, UnderlyingType)
				, UnderlyingType));
		}
		#endregion //	Int64 Methods
	}
}
