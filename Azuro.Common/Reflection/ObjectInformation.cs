using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace Azuro.Common
{
   /// <summary>
   /// Display object type information.
   /// </summary>
   public static class ObjectInformation
   {
      /// <summary>
      /// Unpacks the object.
      /// </summary>
      /// <param name="o">The object to unpack.</param>
      /// <returns></returns>
      public static string Unpack( object o )
      {
         if( o == null )
         {
            return string.Empty;
         }

         StringBuilder sb = new StringBuilder();
         sb.AppendFormat( "== Begin Unpack Object [{0}] ==", o.GetType().FullName );
         sb.AppendLine();

         sb.AppendLine( "Properties" );
         UnpackProperties( sb, o, 1 );

         sb.AppendFormat( "== End Unpack Object [{0}] ==", o.GetType().FullName );
         sb.AppendLine();

         return sb.ToString();
      }

      /// <summary>
      /// Formats the type.
      /// </summary>
      /// <param name="type">The type.</param>
      /// <returns></returns>
      public static string FormatType( Type type )
      {
         if( IsNullableType( type ) )
         {
            var nc = new NullableConverter( type );
            return FormatType( nc.UnderlyingType ) + "?";
         }

         if( type.IsGenericType )
         {
            var txt = new StringBuilder();

            txt.Append( type.FullName.Substring( 0, type.FullName.IndexOf( "`" ) ) );
            txt.Append( "<" );

            Type[] args = type.GetGenericArguments();
            for( int i = 0; i < args.Length; ++i )
            {
               if( i > 0 )
               {
                  txt.Append( ", " );
               }

               txt.Append( FormatType( args[ i ] ) );
            }

            txt.Append( ">" );
            return txt.ToString();
         }

         return type.FullName;
      }

      /// <summary>
      /// Determines whether [is nullable type] [the specified type].
      /// </summary>
      /// <param name="type">The type.</param>
      /// <returns>
      /// 	<c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.
      /// </returns>
      private static bool IsNullableType( Type type )
      {
         return (type.IsGenericType && type.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ));
      }

      /// <summary>
      /// Unpacks the properties.
      /// </summary>
      /// <param name="sb">The sb.</param>
      /// <param name="o">The o.</param>
      /// <param name="depth">The maximum depth.</param>
      private static void UnpackProperties( StringBuilder sb, object o, int depth )
      {
         if( o == null )
            return;

         foreach( PropertyInfo pi in o.GetType().GetProperties() )
         {
            try
            {
               object pVal = null;
               ParameterInfo[] parmInfo = pi.GetIndexParameters();
               if( parmInfo.Length > 0 )
               {
                  UnpackIndexedProperty( sb, pi, parmInfo, depth );
                  continue;
               }
               else
               {
                  switch( pi.Name )
                  {
                     case "DeclaringMethod":
                     case "GenericParameterAttributes":
                     case "GenericParameterPosition":
                        break;

                     default:
                        pVal = pi.GetValue( o, null );
                        UnpackPropertyValue( sb, pi, pVal, depth );
                        break;
                  }
               }

               //	Arbitrary value to avoid Stack Overflow
               //	also skip nulls, strings and datetimes
               if( pVal == null || pVal is string || pVal is DateTime || depth > 3 )
                  continue;

               if( pVal is IEnumerable )
               {
                  foreach( object element in (IEnumerable)pVal )
                  {
                     UnpackProperties( sb, element, depth + 1 );
                  }
               }
               else if( !pVal.GetType().IsPrimitive )
               {
                  UnpackProperties( sb, pVal, depth + 1 );
               }
            }
            catch( Exception ex )
            {
               sb.AppendFormat( "{2}[{0}] => {1}", pi.Name, ex.ToString(), new string( ' ', 3 * depth ) ).AppendLine();
            }
         }
      }

      /// <summary>
      /// Unpacks the property value.
      /// </summary>
      /// <param name="sb">The sb.</param>
      /// <param name="pi">The pi.</param>
      /// <param name="pVal">The p val.</param>
      private static void UnpackPropertyValue( StringBuilder sb, PropertyInfo pi, object pVal, int depth )
      {
         sb.AppendFormat( "{3}[{0}] => [{1}]: [{2}]", pi.Name, pi.PropertyType, pVal, new string( ' ', 3 * depth ) );
         sb.AppendLine();
      }

      /// <summary>
      /// Unpacks the indexed property.
      /// </summary>
      /// <param name="sb">The sb.</param>
      /// <param name="pi">The pi.</param>
      /// <param name="parmInfo">The parm info.</param>
      private static void UnpackIndexedProperty( StringBuilder sb, PropertyInfo pi, ParameterInfo[] parmInfo, int depth )
      {
         sb.AppendFormat( "{2}[{0}] => [{1}] [", pi.Name, pi.PropertyType, new string( ' ', 3 * depth ) );
         foreach( ParameterInfo p in parmInfo )
         {
            sb.AppendFormat( "{0} [{1}], ", p.Name, p.ParameterType );
         }
         sb.Remove( sb.Length - 2, 2 );
         sb.AppendLine( "]" );
      }
   }
}
