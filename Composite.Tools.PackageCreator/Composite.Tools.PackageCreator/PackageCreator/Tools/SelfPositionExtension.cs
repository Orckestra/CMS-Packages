using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Data;

namespace Composite.Tools.PackageCreator
{
	public static class SelfPositionExtension
	{
		public static object _lock = new object();

		[ThreadStatic]
		private static Dictionary<Type, bool> _selfList = null;
		private static Dictionary<Type, bool> SelfList
		{
			get
			{
				lock (_lock)
				{
					if (_selfList == null)
					{
						_selfList = new Dictionary<Type, bool>();
					}
					return _selfList;
				}

			}
		}

		[ThreadStatic]
		private static Dictionary<DataSourceId, int> _selfPosition = null;
		private static Dictionary<DataSourceId, int> SelfPosition
		{
			get
			{
				lock (_lock)
				{
					if (_selfPosition == null)
					{
						_selfPosition = new Dictionary<DataSourceId, int>();
					}
					return _selfPosition;
				}
			}
		}

		public static int GetSelfPosition(this IData data)
		{
			if (data.IsSelf())
			{
				if (!SelfPosition.ContainsKey(data.DataSourceId))
				{
					var type = data.DataSourceId.InterfaceType;
					var list = new Dictionary<DataSourceId, List<DataSourceId>>();
					var dataPosition = new Dictionary<DataSourceId, int>();
					var datas = DataFacade.GetData(type);
					foreach (IData item in datas)
					{
						list[item.DataSourceId] = item.GetReferees(type).Select(d => d.DataSourceId).ToList();
					}

					var prevCount = list.Count;
					for (int i = 1; ; i++)
					{
						var toRemoveFromList = new List<DataSourceId>();
						foreach (var item in list)
						{
							if (item.Value.Count() == 0)
							{
								dataPosition.Add(item.Key, -i);
								toRemoveFromList.Add(item.Key);
							}

						}
						foreach (var key in toRemoveFromList)
						{
							list.Remove(key);
						}
						var prevDataPosition = dataPosition.Keys.ToList();
						list = list.ToDictionary(d => d.Key, d => d.Value.Except(prevDataPosition).ToList());

						if (list.Count == 0)
							break;
						else if (prevCount == list.Count)
							throw new InvalidOperationException(string.Format("ForeignKey error for type '{0}'. Circle in data detected.", type.ToString()));
						else
							prevCount = list.Count;

					}

					foreach (var item in dataPosition)
					{
						SelfPosition[item.Key] = item.Value;
					}
				}
				return SelfPosition[data.DataSourceId];

			}
			return 0;
		}

		private static bool IsSelf(this IData data)
		{
			var type = data.DataSourceId.InterfaceType;
			if (!SelfList.ContainsKey(type))
			{
				var foreignTypes = DataAttributeFacade.GetDataReferencePropertyInfoes(type).Select(d => d.TargetType);


				if (foreignTypes.Contains(type))
					SelfList[type] = true;
				else
					SelfList[type] = false;
			}
			return SelfList[type];
		}
	}
}
