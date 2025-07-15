using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Azuro.Common.Configuration.Design
{
    //  TODO: Change this to use a generically provided Entity Framework context
	//public class ConfigurationLookupConnection : IConfigurationLookupConnection
	//{
	//	public List<T> GetData<T>(ITypeDescriptorContext context, IServiceProvider provider, object value)
	//		where T : DataEntity, new()
	//	{
	//		ConfigurationLookupBindingAttribute clba =
	//			(ConfigurationLookupBindingAttribute)
	//			context.PropertyDescriptor.Attributes[typeof(ConfigurationLookupBindingAttribute)];
	//		if (clba == null)
	//			throw new InvalidOperationException("The type must have a [ConfigurationLookupBindingAttribute] defined.");
	//		DataObject _do = DataObject.Create(clba.ConnectionString);
	//		T de = Activator.CreateInstance<T>();

	//		return _do.List<T>(de);
	//	}
	//}
}
