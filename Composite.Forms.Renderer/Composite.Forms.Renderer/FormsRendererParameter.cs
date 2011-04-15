using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Data;
using Composite.Core.Types;

namespace Composite.Forms.Renderer
{
	internal class FormsRendererProperty: IEnumerable<char>
	{
		private string propertyName;

		public FormsRendererProperty(string propertyName)
		{
			this.propertyName = propertyName;
		}

		#region IEnumerable<char> Members

		public IEnumerator<char> GetEnumerator()
		{
			return propertyName.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return propertyName.GetEnumerator();
		}

		#endregion

		public override string ToString()
		{
			var data = FormsRendererDataScope.CurrentData;

			if (data == null)
			{
				return base.ToString();
			}

			var propertyInfo = data.DataSourceId.InterfaceType.GetProperty(propertyName);

			if (propertyInfo == null)
			{
				throw new InvalidOperationException(string.Format("Datatype '{0}.{1}' does not have property '{2}'", data.DataSourceId.InterfaceType.Namespace, data.DataSourceId.InterfaceType.Name, propertyName));
			}

			object value = propertyInfo.GetValue(data, null);

			List<ForeignKeyAttribute> foreignKeyAttributes = propertyInfo.GetCustomAttributesRecursively<ForeignKeyAttribute>().ToList();
			if (foreignKeyAttributes.Count > 0)
			{
				IData foreignData = data.GetReferenced(propertyInfo.Name);

				value = DataAttributeFacade.GetLabel(foreignData);
			}
			if (value == null)
			{
				value = string.Empty;
			}

			return value.ToString();
		}
	}
}
