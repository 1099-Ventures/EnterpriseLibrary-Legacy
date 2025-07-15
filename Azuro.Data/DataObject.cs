using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using Azuro.Caching;
using Azuro.Common;
using Azuro.Common.Collections;
using Azuro.Common.Configuration;
using Azuro.Common.Validation;
using System.Collections;

namespace Azuro.Data
{
	/// <summary>
	/// Summary description for DataObject.
	/// </summary>
	public partial class DataObject
	{
		protected string m_connectionString = string.Empty;
		protected string m_dataAccessAssembly = string.Empty;
		protected string m_dataAccessType = string.Empty;
		protected SqlTextCommand m_sqlTextCommand = null;
		protected DataAccessConfigObjectSection m_configSection = null;
		protected IDataAccess m_data = null;
		protected bool m_treatAsTextCmd = false;
		protected bool m_derivedParameters = false;
		protected int m_connectionTimeout = 0;
		[NonSerialized]
		protected readonly DataObjectCache m_cache;

		/// <summary>
		/// The connection string for the data access.
		/// </summary>
		public string ConnectionString
		{
			get { return m_connectionString; }
			set { m_connectionString = value; }
		}

		/// <summary>
		/// Set this when you wish to control the connection timeout.
		/// </summary>
		public int ConnectionTimeout
		{
			get { return m_connectionTimeout; }
			set { m_connectionTimeout = value; }
		}

		/// <summary>
		/// The name of the assembly containing the data access code for this DAO
		/// </summary>
		public string DataAccessAssembly
		{
			get { return m_dataAccessAssembly; }
			set { m_dataAccessAssembly = value; }
		}

		/// <summary>
		/// The typename of the Type that will be loaded
		/// </summary>
		public string DataAccessType
		{
			get { return m_dataAccessType; }
			set { m_dataAccessType = value; }
		}

		/// <summary>
		/// A get property that returns the Data Access Interface for this object.
		/// </summary>
		protected IDataAccess DataAccess
		{
			get
			{
				if (m_data == null)
				{
					if (DataAccessAssembly.Length == 0 || ConnectionString.Length == 0)
						throw new ArgumentException("The data access type or connection string was not set.");

					m_data = Util.CreateObject<IDataAccess>(DataAccessAssembly, DataAccessType, ConnectionString);
				}
				return m_data;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [treat as text command].
		/// </summary>
		/// <value><c>true</c> if [treat as text command]; otherwise, <c>false</c>.</value>
		public bool TreatAsTextCommand
		{
			get { return m_treatAsTextCmd; }
			set { m_treatAsTextCmd = value; }
		}

		/// <summary>
		/// Creates the specified data access name.
		/// </summary>
		/// <param name="dataAccessName">Name of the data access.</param>
		/// <returns></returns>
		public static DataObject Create(string dataAccessName)
		{
			return Create<DataObject>(dataAccessName);
		}

		/// <summary>
		/// A static method that will create a DataObject of the specified Type T.
		/// </summary>
		/// <typeparam name="T">The type to create.</typeparam>
		/// <param name="dataAccessName">The configuration section name to use for data access.</param>
		/// <returns>A created type T.</returns>
		public static T Create<T>(string dataAccessName) where T : DataObject
		{
			DataAccessConfigObjectSection objCfg = GetConfiguration(dataAccessName);
			if (objCfg == null)
				throw new ArgumentNullException("dataAccessName", string.Format("Data Access Configuration Section with Name [{0}] was not found.", dataAccessName));
			return Create<T>(objCfg);
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <param name="dataAccessName">Name of the data access.</param>
		/// <returns></returns>
		protected static DataAccessConfigObjectSection GetConfiguration(string dataAccessName)
		{
			//DataAccessConfigSection cfg = (DataAccessConfigSection)ConfigurationManager.GetSection( "Azuro.Data" );
			DataAccessConfigSection cfg = ConfigurationHelper.GetSection<DataAccessConfigSection>("Azuro.Data");
			return cfg[dataAccessName];
		}

		/// <summary>
		/// A static method that will create a DataObject of the specified Type T.
		/// </summary>
		/// <typeparam name="T">The type to create.</typeparam>
		/// <param name="dataAccessConfig">The configuration object to use for data access.</param>
		/// <returns>A created type T.</returns>
		public static T Create<T>(DataAccessConfigObjectSection dataAccessConfig) where T : DataObject
		{
			object o = Activator.CreateInstance(typeof(T), dataAccessConfig.Assembly, dataAccessConfig.Type, dataAccessConfig.ConnectionString);
			Configure(dataAccessConfig, (DataObject)o);
			return (T)o;
		}

		/// <summary>
		/// Configures the specified data access config.
		/// </summary>
		/// <param name="dataAccessConfig">The data access config.</param>
		/// <param name="_do">The _do.</param>
		protected static void Configure(DataAccessConfigObjectSection dataAccessConfig, DataObject _do)
		{
			_do.m_configSection = dataAccessConfig;
			if (dataAccessConfig.SqlTextCommandLocation != null)
				_do.m_sqlTextCommand = SqlTextCommandHandler.Load(dataAccessConfig.SqlTextCommandWrapper, dataAccessConfig.SqlTextCommandLocation);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataObject"/> class.
		/// </summary>
		public DataObject()
		{
			m_cache = new DataObjectCache(this);

			//	TODO: Attribute on class with connection string for cfg?

			DataConnectionAttribute dca = AttributeHelper.GetCustomAttribute<DataConnectionAttribute>(this.GetType());
			if (dca != null)
			{
				Configure(GetConfiguration(dca.Configuration), this);
				m_dataAccessAssembly = m_configSection.Assembly;
				m_dataAccessType = m_configSection.Type;
				m_connectionString = m_configSection.ConnectionString;
			}
		}

		/// <summary>
		/// ctor.
		/// </summary>
		/// <param name="assembly">The assembly to load the data access interface from.</param>
		/// <param name="type">The type that should be loaded from the assembly.</param>
		/// <param name="connectionString">The database connection string to use.</param>
		public DataObject(string assembly, string type, string connectionString)
		{
			m_cache = new DataObjectCache(this);

			m_dataAccessAssembly = assembly;
			m_dataAccessType = type;
			m_connectionString = connectionString;
		}

		/// <summary>
		/// Return a dataset representing a list of data.
		/// </summary>
		/// <param name="entity">The data entity for which this list is performed. This entity must have a StoredProcedureAttribute defined on it.</param>
		/// <returns>A dataset containing the resultant list data.</returns>
		public DataSet List(DataEntity entity)
		{
			return ExecuteDataSet(entity.ListProcedure, entity);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<T> List<T>() where T : DataEntity, new()
		{
			return List<T>(AttributeHelper.GetCustomAttribute<StoredProcedureAttribute>(typeof(T)).ListProcedure, null, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public List<T> List<T>(DataEntity entity) where T : DataEntity, new()
		{
			return List<T>(entity.ListProcedure, entity, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listProcedure"></param>
		/// <returns></returns>
		public List<T> List<T>(string listProcedure) where T : DataEntity, new()
		{
			return List<T>(listProcedure, null, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listProcedure"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public List<T> List<T>(string listProcedure, DataEntity entity) where T : DataEntity, new()
		{
			return List<T>(listProcedure, entity, 0);
		}

		public List<T> List<T>(int fillDepth) where T : DataEntity, new()
		{
			return List<T>(AttributeHelper.GetCustomAttribute<StoredProcedureAttribute>(typeof(T)).ListProcedure, null, fillDepth);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="fillDepth"></param>
		/// <returns></returns>
		public List<T> List<T>(DataEntity entity, int fillDepth) where T : DataEntity, new()
		{
			return List<T>(entity.ListProcedure, entity, fillDepth);
		}

		/// <summary>
		/// This method will return a System.Collection.Generic.List&lt;&gt; of Entities
		/// </summary>
		/// <typeparam name="T">The type of entity which enherits from ADataEntityBase</typeparam>
		/// <param name="listProcedure">The special list procedure to be called</param>
		/// <param name="fillDepth">The depth the entity and its collections have to be populated</param>
		/// <returns></returns>
		public List<T> List<T>(string listProcedure, int fillDepth) where T : DataEntity, new()
		{
			return List<T>(listProcedure, null, fillDepth);
		}

		/// <summary>
		/// This method will return a System.Collections.Generic.List&lt;&gt; of Entities
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listProcedure"></param>
		/// <param name="entity"></param>
		/// <param name="fillDepth"></param>
		/// <returns></returns>
		public List<T> List<T>(string listProcedure, DataEntity entity, int fillDepth) where T : DataEntity, new()
		{
			using (IDataReader reader = ExecuteReader(listProcedure, entity))
			{
				List<T> list = new List<T>();

				while (reader.Read())
				{
					T t = new T();
					FillEntity(t, reader, fillDepth, false, null, listProcedure);
					list.Add(t);
				}

				return list;
			}
		}

		/// <summary>
		/// Insert the data in the entity into the database using the preset StoredProcedureAttribute.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>The number of rows affected. Should be 1 or 0 for success or failure.</returns>
		public int Insert(DataEntity entity)
		{
			if (!entity.IsDirty)
				return 0;

			int rowsAffected = Execute(entity.InsertProcedure, entity);
			entity.IsDirty = (rowsAffected == 0);

			return rowsAffected;
		}

		/// <summary>
		/// Updates the data in the database based on the values in the entity.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>The number of rows affected. Should be >0 for success or 0 failure.</returns>
		public int Update(DataEntity entity)
		{
			if (!entity.IsDirty)
				return 0;
			int rowsAffected = Execute(entity.UpdateProcedure, entity);
			entity.IsDirty = rowsAffected == 0;
			return rowsAffected;
		}

		/// <summary>
		/// bulk save of a list
		/// <para>When saving bulk over remoting, one can optimize the efficiency</para>
		/// <para>by getting a smaller subset of data, eg:</para><para>
		/// <example>DataObject.Save(entities.FindAll(ImplementedEntity.CheckDirty));</example>
		/// </para></summary>
		/// <typeparam name="T">Type of entity to be saved, derived from ADataEntityBase</typeparam>
		/// <param name="entities">Generic collection that needs to be saved</param>
		public void Save<T>(List<T> entities) where T : DataEntity
		{
			entities.ForEach(delegate(T entity)
							{
								Save(entity);
							});
		}

		/// <summary>
		/// single entity save
		/// </summary>
		/// <param name="entity">Entity that have to be saved, derived from ADataEntityBase</param>
		public int Save(DataEntity entity)
		{
			int result;

			if (!HasBeenSaved(entity))
			{
				foreach (KeyColumn key in entity.KeyColumns)
				{
					key.GenerateValue(entity);
				}

				result = Insert(entity);
			}
			else
			{
				result = Update(entity);
			}

			return result;
		}

		/// <summary>
		/// Determines whether the entity has been saved. i.e. If all key properties have a non-default value
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>
		/// 	<c>true</c> if the entity ha been saved otherwise, <c>false</c>.
		/// </returns>
		public bool HasBeenSaved(DataEntity entity)
		{
			foreach (KeyColumn key in entity.KeyColumns)
			{
				if (key.HasDefaultValue(entity))
				{
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		public void Fetch(DataEntity entity)
		{
			Fetch(entity, true, 1);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="fetchProcedure">The fetch procedure to use instead of the default fetch.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		public void Fetch(string fetchProcedure, DataEntity entity)
		{
			Fetch(fetchProcedure, entity, true, 1);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="fillDepth">This indicates to what depth the Autofill feature should operate, its a safety measure 
		/// to prevent recursive entities from causing an infinite loop.</param>
		public void Fetch(DataEntity entity, int fillDepth)
		{
			Fetch(entity, true, fillDepth);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="fetchProcedure">The fetch procedure to use instead of the default fetch.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="fillDepth">This indicates to what depth the Autofill feature should operate, its a safety measure 
		/// to prevent recursive entities from causing an infinite loop.</param>
		public void Fetch(string fetchProcedure, DataEntity entity, int fillDepth)
		{
			Fetch(fetchProcedure, entity, true, fillDepth);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="throwOnEmpty">A boolean value indicating whether to raise an exception if the value is not found.</param>
		/// <returns>True on success, else false.</returns>
		public bool Fetch(DataEntity entity, bool throwOnEmpty)
		{
			return Fetch(entity, throwOnEmpty, 1);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="fetchProcedure">The fetch procedure to use instead of the default fetch.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="throwOnEmpty">A boolean value indicating whether to raise an exception if the value is not found.</param>
		/// <returns>True on success, else false.</returns>
		public bool Fetch(string fetchProcedure, DataEntity entity, bool throwOnEmpty)
		{
			return Fetch(fetchProcedure, entity, throwOnEmpty, 1);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="throwOnEmpty">A boolean value indicating whether to raise an exception if the value is not found.</param>
		/// <param name="fillDepth">This indicates to what depth the Autofill feature should operate, its a safety measure 
		/// to prevent recursive entities from causing an infinite loop.</param>
		/// <returns>True on success, else false.</returns>
		public bool Fetch(DataEntity entity, bool throwOnEmpty, int fillDepth)
		{
			return Fetch(entity.FetchProcedure, entity, throwOnEmpty, fillDepth);
		}

		/// <summary>
		/// Fetches a single row instance of data from the database and populates the values into the entity.
		/// </summary>
		/// <param name="fetchProcedure">The fetch procedure to use instead of the default fetch.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="throwOnEmpty">A boolean value indicating whether to raise an exception if the value is not found.</param>
		/// <param name="fillDepth">This indicates to what depth the Autofill feature should operate, its a safety measure 
		/// to prevent recursive entities from causing an infinite loop.</param>
		/// <returns>True on success, else false.</returns>
		public bool Fetch(string fetchProcedure, DataEntity entity, bool throwOnEmpty, int fillDepth)
		{
			FieldInfo fieldInfo = m_cache.GetFieldInfo(entity.GetType());

			bool success;
			using (IDataReader reader = ExecuteReader(fetchProcedure, entity))
			{
				if ((success = reader.Read()))
				{
					DataTable schema = fieldInfo.GetFetchSchemaTable(fetchProcedure);
					if (schema == null)
					{
						schema = reader.GetSchemaTable();
						fieldInfo.UpdateFetchSchemaTable(fetchProcedure, schema);
					}

					FillEntity(entity, reader, fillDepth, throwOnEmpty, schema, fetchProcedure);
				}
				else
				{
					if (throwOnEmpty)
						throw new DataObjectException("The specified key was not found in the database for a Fetch operation.");
				}

				CleanupObjects();
			}

			return success;
		}

		protected internal void FillEntity(DataEntity entity, IDataReader reader, int fillDepth, bool throwOnEmpty)
		{
			FillEntity(entity, reader, fillDepth, throwOnEmpty, null);
		}

		protected internal void FillEntity(DataEntity entity, IDataReader reader, int fillDepth, bool throwOnEmpty, DataTable dt)
		{
			FillEntity(entity, reader, fillDepth, throwOnEmpty, dt, null);
		}

		protected internal void FillEntity(DataEntity entity, IDataReader reader, int fillDepth, bool throwOnEmpty, DataTable dt, string sqlCommand)
		{
			Type entityType = entity.GetType();
			FieldInfo fieldInfo = m_cache.GetFieldInfo(entityType);

			//	Fill the entity first.
			//	This is somewhat inefficient, as we must reflect the type a second time, 
			//	but if we don't do this, values may not be set for the below
			//	feature.
			entity.Fill(reader, fieldInfo.Properties, dt, sqlCommand);

			if (fillDepth > 0)
			{
				foreach (var kv in fieldInfo.DatabaseRelatedEntityAttributes)
				{
					DatabaseRelatedEntityAttribute dbrea = kv.Key;
					PropertyInfo pi = kv.Value;

					if (pi.CanRead)
					{
						if (!dbrea.FillByDefault)
							continue;

						switch (dbrea.EntityRelationType)
						{
							case EntityRelationType.Fetch:
								{
									object parmEntity = CreateRelatedEntity(pi, entity, dbrea.EntityType);

									if (Fetch((DataEntity)parmEntity, throwOnEmpty, (fillDepth - 1)))
										pi.SetValue(entity, parmEntity, null);

									break;
								}

							case EntityRelationType.List:
								{
									FieldInfo relatedFieldInfo = m_cache.GetFieldInfo(dbrea.EntityType);
									relatedFieldInfo.CacheListMethod();

									if (relatedFieldInfo.ListMethod != null)
									{
										object parmEntity = CreateRelatedEntity(pi, entity, dbrea.EntityType);
										object collection = relatedFieldInfo.ListMethod(this, parmEntity, (fillDepth - 1));

										Type listType = typeof(List<>).MakeGenericType(dbrea.EntityType);

										//Check if the related entity property is a List<> of the entity type
										// If it is then the whole list can simply be set.
										// If it is not then the items must be added one at a time
										if (pi.PropertyType.IsAssignableFrom(listType))
										{
											pi.SetValue(entity, collection, null);
										}
										else
										{
											Type icollType = typeof(ICollection<>).MakeGenericType(dbrea.EntityType);

											//Does the related entity property implemented ICollection<>
											if (icollType.IsAssignableFrom(pi.PropertyType))
											{
												IEnumerable enumerable = (IEnumerable)collection;
												object dest = (ICollection)pi.GetValue(entity, null);

												//Get the ICollection<>.Add(...) method
												MethodInfo addMi = icollType.GetMethod("Add");

												//Create a delegate to the add method
												CallMethodNoResult addMethod = m_cache.LateBound.CreateMethodCallNoResult(addMi);

												//Add all the itesm to the collection
												foreach (object item in enumerable)
												{
													addMethod(dest, item);
												}
											}
											else
											{
												throw new DataObjectException("Unknown related entity collection type: " + pi.PropertyType + " on " + entity.GetType().FullName);
											}
										}
									}
									break;
								}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the properties for the given type and caches them for later use.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A collection of PropertyInfo for the given type.</returns>
		private List<PropertyInfo> GetProperties(Type type)
		{
			string cacheName = "DataObject:PropertyInfo[]:" + type.FullName;
			List<PropertyInfo> properties = (List<PropertyInfo>)CacheManager.Instance[cacheName];

			if (properties == null)
			{
				properties = new List<PropertyInfo>();
				properties.AddRange(type.GetProperties());
				CacheManager.Instance.Add(cacheName, properties);
			}

			return properties;
		}

		private MethodInfo GetMethod(string name, Type genericType, Type[] parameterTypes, bool isGeneric)
		{
			string parmList = string.Empty;
			foreach (Type t in parameterTypes)
				parmList += t.Name + ",";
			parmList.Remove(parmList.Length - 1);
			string cacheName = string.Format("DataObject:MethodInfo:{0}'{1}{2}({3})",
						   name, isGeneric,
						   isGeneric ? "<" + genericType.Name + ">" : null,
						   parmList);

			MethodInfo mi = CacheManager.Instance[cacheName] as MethodInfo;
			if (mi == null)
			{
				mi = this.GetType().GetMethod(name, parameterTypes);
				if (mi.Name == name)
				{
					if (isGeneric && mi.IsGenericMethod)
					{
						MethodInfo genMI = mi.MakeGenericMethod(genericType);
						CacheManager.Instance.Add(cacheName, genMI);
						mi = genMI;
					}
				}
			}
			return mi;
		}

		/// <summary>
		/// Creates the related entity.
		/// </summary>
		/// <param name="pi">The pi.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <returns></returns>
		private object CreateRelatedEntity(PropertyInfo pi, DataEntity entity, Type targetType)
		{
			object parmEntity = Activator.CreateInstance(targetType);
			DatabaseRelationMappingAttribute[] dbrmas = AttributeHelper.GetCustomAttributes<DatabaseRelationMappingAttribute>(pi);
			if (dbrmas != null)
			{
				foreach (DatabaseRelationMappingAttribute dbrma in dbrmas)
				{
					if (dbrma.IsValid())
					{
						PropertyInfo piTarget = GetProperties(targetType).Find(item => item.Name == dbrma.TargetProperty);
						object targetValue = null;
						if (dbrma.TargetValue != null)
						{
							targetValue = Util.ChangeType(dbrma.TargetValue, piTarget.PropertyType);
						}
						else
						{
							PropertyInfo piSource = GetProperties(entity.GetType()).Find(item => item.Name == dbrma.SourceProperty);
							targetValue = piSource.GetValue(entity, null);
						}
						piTarget.SetValue(parmEntity, targetValue, null);
					}
					else
						throw new ArgumentException(string.Format("DatabaseRelationMappingAttribute [{0}]:[{1}]:[{2}] is Invalid",
						   dbrma.SourceProperty, dbrma.TargetProperty, dbrma.TargetValue));
				}
			}
			return parmEntity;
		}

		/// <summary>
		/// Delete the data that matches criteria set in the supplied entity.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>The number of rows affected by the delete operation.</returns>
		public int Delete(DataEntity entity)
		{
			using (IDbConnection conn = DataAccess.CreateConnection())
			{
				Dictionary<string, IDataParameter> parameters = GetParameters(conn, entity.DeleteProcedure, entity, null);
				int retI = DataAccess.Execute(conn, CreateCommand(conn, entity.DeleteProcedure, parameters));
				CleanupObjects();
				return retI;
			}
		}

		/// <summary>
		/// Return a dataset for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A Dataset of the data.</returns>
		public DataSet ExecuteDataSet(string sqlCommand)
		{
			return ExecuteDataSet(sqlCommand, (DataEntity)null);
		}

		/// <summary>
		/// Return a dataset for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="parameters">A dictionary of parameters</param>
		/// <returns>A Dataset of the data.</returns>
		public DataSet ExecuteDataSet(string sqlCommand, Dictionary<string, object> parameters)
		{
			return ExecuteDataSet(sqlCommand, null, parameters);
		}

		/// <summary>
		/// Return a dataset for the supplied sql command.
		/// </summary>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A Dataset of the data.</returns>
		public DataSet ExecuteDataSet(DataEntity entity)
		{
			return ExecuteDataSet(entity.ListProcedure, entity, null);
		}

		/// <summary>
		/// Return a dataset for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A Dataset of the data.</returns>
		public DataSet ExecuteDataSet(string sqlCommand, DataEntity entity)
		{
			return ExecuteDataSet(sqlCommand, entity, null);
		}

		/// <summary>
		/// Return a dataset for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="parmValues">A dictionary of parameters and values</param>
		/// <returns>A Dataset of the data.</returns>
		public DataSet ExecuteDataSet(string sqlCommand, DataEntity entity, Dictionary<string, object> parmValues)
		{
			using (IDbConnection conn = DataAccess.CreateConnection())
			{
				CommandType cmdtype = CommandType.StoredProcedure;

				Dictionary<string, IDataParameter> parameters;

				if (entity != null || parmValues != null)
				{
					parameters = GetParameters(conn, sqlCommand, entity, parmValues);
				}
				else
				{
					//No parameters are to be set.
					parameters = new Dictionary<string, IDataParameter>();

					if (TreatAsTextCommand)
					{
						cmdtype = CommandType.Text;
						TreatAsTextCommand = false;
					}
				}

				DataSet ds = DataAccess.ExecuteDataSet(conn, CreateCommand(conn, sqlCommand, cmdtype, parameters));
				CleanupObjects();
				return ds;
			}
		}

		/// <summary>
		/// Creates a data reader for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>An IDataReader.</returns>
		public IDataReader ExecuteReader(string sqlCommand)
		{
			return ExecuteReader(sqlCommand, null, null);
		}

		/// <summary>
		/// Creates a data reader for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>An IDataReader.</returns>
		public IDataReader ExecuteReader(string sqlCommand, DataEntity entity)
		{
			return ExecuteReader(sqlCommand, entity, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IDataReader ExecuteReader(string sqlCommand, Dictionary<string, object> parameters)
		{
			return ExecuteReader(sqlCommand, null, parameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <param name="entity"></param>
		/// <param name="parmValues"></param>
		/// <returns></returns>
		public IDataReader ExecuteReader(string sqlCommand, DataEntity entity, Dictionary<string, object> parmValues)
		{
			//The connection is closed by the reader CommandBehavior.CloseConnection
			IDbConnection conn = DataAccess.CreateConnection();

			CommandType cmdtype = CommandType.StoredProcedure;
			Dictionary<string, IDataParameter> parameters;

			if (!TreatAsTextCommand)
			{
				parameters = GetParameters(conn, sqlCommand, entity, parmValues);
			}
			else
			{
				//no parameters are to be set.
				parameters = new Dictionary<string, IDataParameter>();

				cmdtype = CommandType.Text;
				TreatAsTextCommand = false;
			}

			IDataReader rdr = DataAccess.ExecuteReader(conn, CreateCommand(conn, sqlCommand, cmdtype, parameters));
			CleanupObjects();
			return rdr;
		}

		/// <summary>
		/// Creates a data reader for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>An IDataReader.</returns>
		public EntityDataReader<T> ExecuteReader<T>(string sqlCommand) where T : DataEntity
		{
			return ExecuteReader<T>(sqlCommand, null);
		}

		/// <summary>
		/// Creates a data reader for the supplied sql command.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>An IDataReader.</returns>
		public EntityDataReader<T> ExecuteReader<T>(string sqlCommand, DataEntity entity) where T : DataEntity
		{
			return ExecuteReader<T>(sqlCommand, entity, 1);
		}

		/// <summary>
		/// Creates a data reader for the supplied sql command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="fillDepth">The fill depth.</param>
		/// <returns>An IDataReader.</returns>
		public EntityDataReader<T> ExecuteReader<T>(string sqlCommand, DataEntity entity, int fillDepth) where T : DataEntity
		{
			return new EntityDataReader<T>(ExecuteReader(sqlCommand, entity), this, fillDepth, sqlCommand);
		}

		/// <summary>
		/// Execute a piece of sql.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>The number of rows affected by the sql command.</returns>
		public int Execute(string sqlCommand)
		{
			return Execute(sqlCommand, null);
		}

		/// <summary>
		/// Execute a piece of sql.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>The number of rows affected by the sql command.</returns>
		public int Execute(string sqlCommand, DataEntity entity)
		{
			using (IDbConnection conn = DataAccess.CreateConnection())
			{
				Dictionary<string, IDataParameter> parameters = null;

				CommandType cmdtype = CommandType.StoredProcedure;
				List<PropertyInfo> outProperties = new List<PropertyInfo>();
				if (entity != null)
				{
					parameters = DeriveParameters(conn, sqlCommand);

					foreach (PropertyInfo pi in entity.GetType().GetProperties())
					{
						DatabaseColumnAttribute dbca = (DatabaseColumnAttribute)Attribute.GetCustomAttribute(pi, typeof(DatabaseColumnAttribute));

						if (dbca == null)
							continue;

						string parmName = BuildParameterName(dbca.ParameterName);

						if (pi.PropertyType.BaseType != typeof(Array))
						{
							if (!parameters.ContainsKey(parmName))
								continue;

							if (parameters[parmName].Direction == ParameterDirection.InputOutput
							   || parameters[parmName].Direction == ParameterDirection.Output
							   || parameters[parmName].Direction == ParameterDirection.ReturnValue)
							{
								outProperties.Add(pi);
							}
						}
						if (pi.CanWrite)
						{
							//	TODO: Check here whether the base type, or array entity type is another data entity
							if (pi.PropertyType.BaseType == typeof(Array))
							{
								// HACK: Adriaan, need this for binary inputs into a blob field
								if (parameters.ContainsKey(parmName) && parameters[parmName].DbType == DbType.Binary)
								{
									parameters[parmName].Value = ValidateInputValue(pi.GetValue(entity, null), dbca, pi);
								}
								else
								{
									Array oArray = (Array)pi.GetValue(entity, null);
									for (int i = 0; i < oArray.Length; ++i)
									{
										parmName = BuildParameterName(dbca.ParameterName) + i;
										if (parameters.ContainsKey(parmName))
											parameters[parmName].Value =
											   ValidateInputValue(oArray.GetValue(i), dbca, pi);
									}
								}
							}
							else
							{
								parameters[parmName].Value = ValidateInputValue(pi.GetValue(entity, null), dbca, pi);
							}
						}
					}
				}
				else if (TreatAsTextCommand)
				{
					//	Clear the parameter cache if no parameters are to be set.
					cmdtype = CommandType.Text;
					TreatAsTextCommand = false;
				}

				int retval = DataAccess.Execute(conn, CreateCommand(conn, sqlCommand, cmdtype, parameters));

				//	Handle any output parameters
				foreach (PropertyInfo pi in outProperties)
				{
					DatabaseColumnAttribute dbca = (DatabaseColumnAttribute)Attribute.GetCustomAttribute(pi, typeof(DatabaseColumnAttribute));
					string parmName = DataAccess.ParameterPlaceholder + dbca.ParameterName;
					pi.SetValue(entity, parameters[parmName].Value, null);
				}

				CleanupObjects();

				return retval;
			}
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as Type T.
		/// </summary>
		/// <typeparam name="T">The type to cast the return value to.</typeparam>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>The number of rows affected by the sql command.</returns>
		public T ExecuteScalar<T>(string sqlCommand)
		{
			return ExecuteScalar<T>(sqlCommand, null, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as Type T.
		/// </summary>
		/// <typeparam name="T">The type to cast the return value to.</typeparam>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>The number of rows affected by the sql command.</returns>
		public T ExecuteScalar<T>(string sqlCommand, DataEntity entity)
		{
			return ExecuteScalar<T>(sqlCommand, entity, null);
		}

		public T ExecuteScalar<T>(string sqlCommand, Dictionary<string, object> parameters)
		{
			return ExecuteScalar<T>(sqlCommand, null, parameters);
		}

		public T ExecuteScalar<T>(string sqlCommand, DataEntity entity, Dictionary<string, object> parameters)
		{
			object retVal = ExecuteScalar(sqlCommand, entity, parameters);

			return (retVal != null) ? (T)Util.ChangeType(retVal, typeof(T)) : default(T);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as an Int32.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>An integer value.</returns>
		public int ExecuteScalarInt32(string sqlCommand)
		{
			return ExecuteScalarInt32(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as an Int32.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>An integer value.</returns>
		public int ExecuteScalarInt32(string sqlCommand, DataEntity entity)
		{
			return Util.FieldValueToInt32(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as an Int64.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A 64-bit integer value.</returns>
		public Int64 ExecuteScalarInt64(string sqlCommand)
		{
			return ExecuteScalarInt64(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as an Int64.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A 64-bit integer value.</returns>
		public Int64 ExecuteScalarInt64(string sqlCommand, DataEntity entity)
		{
			return Util.FieldValueToInt64(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Boolean.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A Boolean value.</returns>
		public bool ExecuteScalarBoolean(string sqlCommand)
		{
			return ExecuteScalarBoolean(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Boolean.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A Boolean value.</returns>
		public bool ExecuteScalarBoolean(string sqlCommand, DataEntity entity)
		{
			return Util.FieldValueToBoolean(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a String.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A String value.</returns>
		public string ExecuteScalarString(string sqlCommand)
		{
			return ExecuteScalarString(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a String.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A String value.</returns>
		public string ExecuteScalarString(string sqlCommand, DataEntity entity)
		{
			object o = ExecuteScalar(sqlCommand, entity, null);
			return (o != null) ? o.ToString() : null;
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Decimal.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A Decimal value.</returns>
		public decimal ExecuteScalarDecimal(string sqlCommand)
		{
			return ExecuteScalarDecimal(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Decimal.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A Decimal value.</returns>
		public decimal ExecuteScalarDecimal(string sqlCommand, DataEntity entity)
		{
			return Util.FieldValueToDecimal(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Double.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A Double value.</returns>
		public double ExecuteScalarDouble(string sqlCommand)
		{
			return ExecuteScalarDouble(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Double.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A Double value.</returns>
		public double ExecuteScalarDouble(string sqlCommand, DataEntity entity)
		{
			return Util.FieldValueToDouble(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Float.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A Float value.</returns>
		public float ExecuteScalarFloat(string sqlCommand)
		{
			return ExecuteScalarFloat(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a Float.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A Float value.</returns>
		public float ExecuteScalarFloat(string sqlCommand, DataEntity entity)
		{
			return (float)Util.FieldValueToDouble(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a DateTime.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <returns>A DateTime value.</returns>
		public DateTime ExecuteScalarDateTime(string sqlCommand)
		{
			return ExecuteScalarDateTime(sqlCommand, null);
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row as a DateTime.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <returns>A DateTime value.</returns>
		public DateTime ExecuteScalarDateTime(string sqlCommand, DataEntity entity)
		{
			return Util.FieldValueToDateTime(ExecuteScalar(sqlCommand, entity, null));
		}

		/// <summary>
		/// Executes the supplied sql and returns the first column of the the first row.
		/// </summary>
		/// <param name="sqlCommand">The sql to execute.</param>
		/// <param name="entity">A data entity from which to derive the parameters and stored procedure values.</param>
		/// <param name="parameters">A name value collection of parameters</param>
		/// <returns>An object representing the first column's value.</returns>
		protected object ExecuteScalar(string sqlCommand, DataEntity entity, Dictionary<string, object> parmValues)
		{
			using (IDbConnection conn = DataAccess.CreateConnection())
			{
				CommandType cmdType = CommandType.StoredProcedure;
				Dictionary<string, IDataParameter> parameters;

				if (!TreatAsTextCommand)
				{
					parameters = GetParameters(conn, sqlCommand, entity, parmValues);
				}
				else
				{
					parameters = new Dictionary<string, IDataParameter>();
					cmdType = CommandType.Text;
					TreatAsTextCommand = false;
				}

				object retVal = DataAccess.ExecuteScalar(conn, CreateCommand(conn, sqlCommand, cmdType, parameters));
				CleanupObjects();
				return retVal;
			}
		}

		/// <summary>
		/// Check the caching manager for parameters, else fetch them from the database.
		/// </summary>
		/// <param name="spName">The stored procedure name to fetch parameters for.</param>
		protected Dictionary<string, IDataParameter> DeriveParameters(IDbConnection conn, string spName)
		{
			Dictionary<string, IDataParameter> parameters;

			string cacheName = string.Format("{0}:{1}", m_configSection.Name, spName);

			lock (CacheManager.Instance)
			{
				if (CacheManager.Instance[cacheName] != null)
				{
					parameters = CopyParameters((Dictionary<string, IDataParameter>)CacheManager.Instance[cacheName]);
				}
				else
				{
					if (m_sqlTextCommand != null)
					{
						if (m_sqlTextCommand.Procedures.ContainsKey(spName))
						{
							SqlTextProcedure proc = m_sqlTextCommand.Procedures[spName];

							parameters = new Dictionary<string, IDataParameter>(StringComparer.InvariantCultureIgnoreCase);

							foreach (KeyValuePair<string, SqlTextParameter> kvp in proc.Parameters)
							{
								IDataParameter dp = CreateParameter(BuildParameterName(kvp.Key), TypeToDbType(kvp.Value.DataType), kvp.Value.Direction);
								parameters.Add(dp.ParameterName, dp);
							}
						}
						else
							throw new IndexOutOfRangeException(string.Format("The command requested was not found in the text wrapper: {0}.", spName));
					}
					else
					{
						parameters = CopyParameters(DataAccess.DeriveParameters(conn, spName));
					}

					CacheManager.Instance.Add(cacheName, CopyParameters(parameters));
				}
			}

			return parameters;
		}

		/// <summary>
		/// Copy the parameter list to get a clean set that is unassociated with a Command.
		/// </summary>
		/// <param name="copy">The dictionary to copy.</param>
		/// <returns>A copy of the parameter list.</returns>
		protected Dictionary<string, IDataParameter> CopyParameters(Dictionary<string, IDataParameter> copy)
		{
			Dictionary<string, IDataParameter> parms = new Dictionary<string, IDataParameter>(StringComparer.InvariantCultureIgnoreCase);
			foreach (KeyValuePair<string, IDataParameter> kvp in copy)
			{
				parms.Add(kvp.Key, CreateParameter(kvp.Key, kvp.Value.DbType, kvp.Value.Direction));
			}
			return parms;
		}

		/// <summary>
		/// Gets the parameter values of the supplied sql procedure from the entity passed in.
		/// </summary>
		/// <param name="spName">The sql command for which to set parameters.</param>
		/// <param name="entity">The entity used for source data.</param>
		protected Dictionary<string, IDataParameter> GetParameters(IDbConnection conn, string spName, DataEntity entity, Dictionary<string, object> parmValues)
		{
			int h1 = spName.GetHashCode();
			int h2 = (entity != null) ? entity.GetType().GetHashCode() : 0;
			int hash = (((h1 << 5) + h1) ^ h2);

			Dictionary<string, IDataParameter> parameters = DeriveParameters(conn, spName);

			if (parameters.Count > 0)
			{
				if (entity != null)
				{
					List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>> props;

					props = m_cache.GetPropertyCache(spName, entity);

					if (props == null)
					{
						props = new List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>>();
						PropertyInfo[] properties = entity.GetType().GetProperties();

						foreach (PropertyInfo pi in properties)
						{
							if (pi.CanWrite)
							{
								DatabaseColumnAttribute dbca = (DatabaseColumnAttribute)Attribute.GetCustomAttribute(pi, typeof(DatabaseColumnAttribute));

								if (dbca == null)
									continue;

								props.Add(new Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>(
								   pi,
								   dbca,
								   m_cache.LateBound.CreatePropertyGetter(pi.Name, entity.GetType())));
							}
						}

						m_cache.CacheProperties(spName, entity, props);
					}

					foreach (Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet> trip in props)
					{
						PropertyInfo pi = trip.A;
						DatabaseColumnAttribute dbca = trip.B;
						PropertyGet getter = trip.C;

						string parmName = BuildParameterName(dbca.ParameterName);

						if (parameters.ContainsKey(parmName))
							parameters[parmName].Value = ValidateInputValue(getter(entity), dbca, pi);
					}
				}
				else if (parmValues != null)
				{
					foreach (KeyValuePair<string, object> kvp in parmValues)
					{
						string parmName = BuildParameterName(kvp.Key);

						if (parameters.ContainsKey(parmName))
							parameters[parmName].Value = kvp.Value;
					}
				}
			}

			return parameters;
		}

		protected void CleanupObjects()
		{
			m_treatAsTextCmd = m_derivedParameters = false;
		}

		/// <summary>
		/// Validate a value for compliance with any validation attributes set on the property.
		/// </summary>
		/// <param name="p">The value to test.</param>
		/// <param name="dbca">The database column attribute.</param>
		/// <param name="pi">The PropertyInfo for the property that's being checked.</param>
		/// <returns>The validated or default value.</returns>
		protected object ValidateInputValue(object p, DatabaseColumnAttribute dbca, PropertyInfo pi)
		{
			if (p == null || Convert.IsDBNull(p))
			{
				if (!dbca.AllowNulls)
					throw new ArgumentNullException(string.Format("Database column [{0}] on [{1}.{2}] does not allow nulls.",
								dbca.ColumnName, pi.DeclaringType.FullName, pi.Name));

				return dbca.DefaultValue;
			}

			foreach (AValidatorAttribute va in pi.GetCustomAttributes(typeof(AValidatorAttribute), true))
			{
				if (!va.IsValid(p))
					throw new ValidationException(va.Message, va);
			}

			//	HACK: JU - TimeSpan does not like going in, change to Ticks and assign Int64 out
			if (p is TimeSpan)
				return DateTime.Today.AddTicks(((TimeSpan)p).Ticks);
			else
				return p;
		}

		/// <summary>
		/// Create a Database command command object filled the correct parameters for the sql text.
		/// </summary>
		/// <param name="sqlCommand">The sql command to create a command for.</param>
		/// <returns>A valid Database command for the underlying database connection type.</returns>
		protected IDbCommand CreateCommand(IDbConnection conn, string sqlCommand, Dictionary<string, IDataParameter> parameters)
		{
			return CreateCommand(conn, sqlCommand, CommandType.StoredProcedure, parameters);
		}

		/// <summary>
		/// Create a Database command command object filled the correct parameters for the sql text.
		/// </summary>
		/// <param name="sqlCommand">The sql command to create a command for.</param>
		/// <param name="cmdType">The command type of this command.</param>
		/// <returns>A valid Database command for the underlying database connection type.</returns>
		protected IDbCommand CreateCommand(IDbConnection conn, string sqlCommand, CommandType cmdType, Dictionary<string, IDataParameter> parameters)
		{
			IDbCommand cmd = null;

			if (m_sqlTextCommand != null && cmdType != CommandType.Text)
			{
				if (m_sqlTextCommand.Procedures.ContainsKey(sqlCommand))
				{
					cmd = DataAccess.CreateCommand(conn, m_sqlTextCommand.Procedures[sqlCommand].CommandText);
					cmd.CommandType = CommandType.Text;
				}
				else
					throw new IndexOutOfRangeException(string.Format("The command requested was not found in the text wrapper: {0}.", sqlCommand));
			}
			else
			{
				cmd = DataAccess.CreateCommand(conn, sqlCommand);
				cmd.CommandType = cmdType;
			}

			if (parameters != null)
			{
				foreach (KeyValuePair<string, IDataParameter> kvp in parameters)
				{
					cmd.Parameters.Add(kvp.Value);
				}
			}

			//	Set connection and command timeouts
			if (ConnectionTimeout > 0)
				cmd.CommandTimeout = ConnectionTimeout;

			return cmd;
		}

		/// <summary>
		/// Create IDataParameters of the correct type for the underlying database connection.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter value.</param>
		/// <param name="direction">The parameter direction.</param>
		/// <returns>A valid IDataParameter.</returns>
		protected IDataParameter CreateParameter(string name, DbType type, ParameterDirection direction)
		{
			IDataParameter dp = DataAccess.CreateParameter();
			dp.ParameterName = name;
			dp.DbType = type;
			dp.Direction = direction;
			dp.Value = DBNull.Value;
			return dp;
		}

		/// <summary>
		/// Create a parameter name taking the parameter marker for the underlying connection type into account.
		/// </summary>
		/// <param name="name">The name to use for creating the parameter name.</param>
		/// <returns>A valid parameter name.</returns>
		protected string BuildParameterName(string name)
		{
			return DataAccess.ParameterPlaceholder + name;
		}

		/// <summary>
		/// Convert native .NET types to DbType.
		/// </summary>
		/// <param name="dataType">The Type to convert.</param>
		/// <returns>A DbType.</returns>
		protected DbType TypeToDbType(Type dataType)
		{
			TypeCode tc = Type.GetTypeCode(dataType);
			switch (tc)
			{
				case TypeCode.Char:
					return DbType.StringFixedLength;
				case TypeCode.DBNull:
					return DbType.Object;
				default:
					{
						string t = tc.ToString();
						t = t.Substring(t.LastIndexOf('.') + 1);
						return (DbType)Enum.Parse(typeof(DbType), t);
					}
			}
		}
	}
}
