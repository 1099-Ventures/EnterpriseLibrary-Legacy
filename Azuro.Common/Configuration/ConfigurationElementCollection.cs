using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace Azuro.Configuration
{
	public class ConfigurationElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return Activator.CreateInstance<T>();
		}

		protected override ConfigurationElement CreateNewElement(string elementName)
		{
			return (T)Activator.CreateInstance(typeof(T), elementName);
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			foreach (PropertyInfo pi in element.GetType().GetProperties())
			{
				foreach (ConfigurationPropertyAttribute cpa in pi.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), false))
				{
					if (cpa.IsKey)
						return pi.GetValue(element, null);
				}
			}
			return null;
		}

		public T this[int index]
		{
			get { return (T)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		public new T this[string Name]
		{
			get { return (T)BaseGet(Name); }
		}

		public int IndexOf(T element)
		{
			return BaseIndexOf(element);
		}

		public void Add(T element)
		{
			BaseAdd(element);
		}

		public void Remove(T element)
		{
			if (BaseIndexOf(element) >= 0)
				BaseRemove(element);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Remove(string name)
		{
			BaseRemove(name);
		}

		public void Clear()
		{
			BaseClear();
		}
	}
}
