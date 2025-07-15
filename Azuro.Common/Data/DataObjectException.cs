using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Azuro.Data
{
   /// <summary>
   /// Exception thrown by a DataObject component
   /// </summary>
   [Serializable]
   public class DataObjectException : Exception
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="T:DataObjectException"/> class.
      /// </summary>
      public DataObjectException() : base() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="T:DataObjectException"/> class.
      /// </summary>
      /// <param name="message">The message.</param>
      public DataObjectException( string message ) : base( message ) { }

      /// <summary>
      /// Initializes a new instance of the <see cref="T:DataObjectException"/> class.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="innerException">The inner exception.</param>
      public DataObjectException( string message, Exception innerException ) : base( message, innerException ) { }

      /// <summary>
      /// Initializes a new instance of the <see cref="T:DataObjectException"/> class.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
      /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
      /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
      protected DataObjectException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }


      /// <summary>
      /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
      /// <exception cref="T:System.ArgumentNullException">The info parameter is a null reference (Nothing in Visual Basic). </exception>
      /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
      //[SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
      public override void GetObjectData( SerializationInfo info, StreamingContext context )
      {
         if( info == null )
            throw new ArgumentNullException( "info" );

         base.GetObjectData( info, context );
         //info.AddValue("PropertyName", this.propertyValueField, typeof(propertyType));
      }

   }
}