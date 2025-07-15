using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Azuro.Common;

namespace Azuro.Data
{
    /// <summary>
    /// This class sets an underlying IDataReader and behaves exactly like an
    /// IDataReader, expect that it will prefill an entity on Read as well.
    /// </summary>
    /// <typeparam name="T">The data entity that this reader will operate on.</typeparam>
    public class EntityDataReader<T> : IDataReader where T : DataEntity
    {
        private IDataReader m_reader;
        private T m_entity;
        private int m_fillDepth;
        private DataObject m_dataObject;
        private readonly DataTable m_schemaTable;
        private readonly string m_sqlCommand;

        /// <summary>
        /// The underlying entity filled in this reader.
        /// </summary>
        public T Value
        {
            get { return m_entity; }
            private set { m_entity = value; }
        }

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="reader">The IDataReader to contain in this EntityDataReader.</param>
        public EntityDataReader( IDataReader reader )
            : this( reader, null, 1 )
        {
        }

        public EntityDataReader( IDataReader reader, DataObject dataObject, int fillDepth )
            : this( reader, dataObject, fillDepth, null )
        {
        }

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="reader">The IDataReader to contain in this EntityDataReader.</param>
        /// <param name="dataObject">The data object.</param>
        /// <param name="fillDepth">The fill depth.</param>
        public EntityDataReader( IDataReader reader, DataObject dataObject, int fillDepth, string sqlCommand )
        {
            m_reader = reader;
            m_fillDepth = fillDepth;
            m_dataObject = dataObject;
            m_schemaTable = reader.GetSchemaTable();
            m_sqlCommand = sqlCommand;
        }

        #region IDataReader Members

        /// <summary>
        /// IDataReader.Close()
        /// </summary>
        public void Close()
        {
            m_reader.Close();
        }

        /// <summary>
        /// IDataReader.Depth
        /// </summary>
        public int Depth
        {
            get { return m_reader.Depth; }
        }

        /// <summary>
        /// IDataReader.GetSchemaTable()
        /// </summary>
        /// <returns></returns>
        public DataTable GetSchemaTable()
        {
            return m_reader.GetSchemaTable();
        }

        /// <summary>
        /// IDataReader.IsClosed
        /// </summary>
        public bool IsClosed
        {
            get { return m_reader.IsClosed; }
        }

        /// <summary>
        /// IDataReader.NextResult()
        /// </summary>
        /// <returns>true if there is another result.</returns>
        public bool NextResult()
        {
            return m_reader.NextResult();
        }

        /// <summary>
        /// IDataReader.Read()
        /// </summary>
        /// <returns>true if there is another row.</returns>
        public bool Read()
        {
            Value = null;

            if( m_reader.Read() )
            {
                Value = Util.CreateObject<T>( typeof( T ) );

                if( (m_dataObject == null) || (m_fillDepth < 2) )
                {
                    Value.Fill( m_reader );
                }
                else
                {
                    m_dataObject.FillEntity( Value, this, m_fillDepth, false, m_schemaTable, m_sqlCommand );
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// IDataReader.RecordsAffected
        /// </summary>
        public int RecordsAffected
        {
            get { return m_reader.RecordsAffected; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose()
        /// </summary>
        public void Dispose()
        {
            m_reader.Dispose();
        }

        #endregion

        #region IDataRecord Members

        /// <summary>
        /// IDataReader.FieldCount
        /// </summary>
        public int FieldCount
        {
            get { return m_reader.FieldCount; }
        }

        /// <summary>
        /// IDataReader.GetBoolean()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A boolean value.</returns>
        public bool GetBoolean( int i )
        {
            return m_reader.GetBoolean( i );
        }

        /// <summary>
        /// IDataReader.GetByte()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A byte value.</returns>
        public byte GetByte( int i )
        {
            return m_reader.GetByte( i );
        }

        /// <summary>
        /// IDataReader.GetBytes()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <param name="fieldOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns>The number of bytes returned.</returns>
        public long GetBytes( int i, long fieldOffset, byte[] buffer, int bufferoffset, int length )
        {
            return m_reader.GetBytes( i, fieldOffset, buffer, bufferoffset, length );
        }

        /// <summary>
        /// IDataReader.GetChar()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A char value.</returns>
        public char GetChar( int i )
        {
            return m_reader.GetChar( i );
        }

        /// <summary>
        /// IDataReader.GetChars()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <param name="fieldoffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns>The number of bytes returned.</returns>
        public long GetChars( int i, long fieldoffset, char[] buffer, int bufferoffset, int length )
        {
            return m_reader.GetChars( i, fieldoffset, buffer, bufferoffset, length );
        }

        /// <summary>
        /// IDataReader.GetData()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>An IDataReader.</returns>
        public IDataReader GetData( int i )
        {
            return m_reader.GetData( i );
        }

        /// <summary>
        /// IDataReader.GetDataTypeName()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns></returns>
        public string GetDataTypeName( int i )
        {
            return m_reader.GetDataTypeName( i );
        }

        /// <summary>
        /// IDataReader.GetDateTime()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A DateTime value.</returns>
        public DateTime GetDateTime( int i )
        {
            return m_reader.GetDateTime( i );
        }

        /// <summary>
        /// IDataReader.GetDecimal()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A decimal value.</returns>
        public decimal GetDecimal( int i )
        {
            return m_reader.GetDecimal( i );
        }

        /// <summary>
        /// IDataReader.GetDouble()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A double value.</returns>
        public double GetDouble( int i )
        {
            return m_reader.GetDouble( i );
        }

        /// <summary>
        /// IDataReader.GetFieldType()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>The Type of the column at ordinal i.</returns>
        public Type GetFieldType( int i )
        {
            return m_reader.GetFieldType( i );
        }

        /// <summary>
        /// IDataReader.GetFloat()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A float value.</returns>
        public float GetFloat( int i )
        {
            return m_reader.GetFloat( i );
        }

        /// <summary>
        /// IDataReader.
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A Guid value.</returns>
        public Guid GetGuid( int i )
        {
            return m_reader.GetGuid( i );
        }

        /// <summary>
        /// IDataReader.GetInt16()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A short value.</returns>
        public short GetInt16( int i )
        {
            return m_reader.GetInt16( i );
        }

        /// <summary>
        /// IDataReader.GetInt32()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A int value.</returns>
        public int GetInt32( int i )
        {
            return m_reader.GetInt32( i );
        }

        /// <summary>
        /// IDataReader.GetInt64()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A long value.</returns>
        public long GetInt64( int i )
        {
            return m_reader.GetInt64( i );
        }

        /// <summary>
        /// IDataReader.GetName()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A string value.</returns>
        public string GetName( int i )
        {
            return m_reader.GetName( i );
        }

        /// <summary>
        /// IDataReader.GetOrdinal()
        /// </summary>
        /// <param name="name">The ordinal number of the column to get.</param>
        /// <returns>The ordinal of a column with name "name".</returns>
        public int GetOrdinal( string name )
        {
            return m_reader.GetOrdinal( name );
        }

        /// <summary>
        /// IDataReader.GetString()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>A string value.</returns>
        public string GetString( int i )
        {
            return m_reader.GetString( i );
        }

        /// <summary>
        /// IDataReader.GetValue()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>An object value.</returns>
        public object GetValue( int i )
        {
            return m_reader.GetValue( i );
        }

        /// <summary>
        /// IDataReader.GetObjects()
        /// </summary>
        /// <param name="values">The values to fill.</param>
        /// <returns>The number of values returned.</returns>
        public int GetValues( object[] values )
        {
            return m_reader.GetValues( values );
        }

        /// <summary>
        /// IDataReader.IsDBNull()
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>true if its a DBNull.</returns>
        public bool IsDBNull( int i )
        {
            return m_reader.IsDBNull( i );
        }

        /// <summary>
        /// IDataReader.this[]
        /// </summary>
        /// <param name="name">The name of the column to return.</param>
        /// <returns>The column value.</returns>
        public object this[string name]
        {
            get { return m_reader[name]; }
        }

        /// <summary>
        /// IDataReader.this[]
        /// </summary>
        /// <param name="i">The ordinal number of the column to get.</param>
        /// <returns>The column value.</returns>
        public object this[int i]
        {
            get { return m_reader[i]; }
        }

        #endregion
    }
}
