using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Data;

namespace Composite.Tools.PackageCreator
{
    public static class SelfPositionExtension
    {
        public static object _lock = new object();

        [ThreadStatic] private static Dictionary<Type, bool> _selfList;

        [ThreadStatic] private static Dictionary<DataSourceId, int> _selfPosition;

        private static Dictionary<Type, bool> SelfList
        {
            get
            {
                lock (_lock)
                {
                    return _selfList ?? (_selfList = new Dictionary<Type, bool>());
                }
            }
        }

        private static Dictionary<DataSourceId, int> SelfPosition
        {
            get
            {
                lock (_lock)
                {
                    return _selfPosition ?? (_selfPosition = new Dictionary<DataSourceId, int>());
                }
            }
        }

        public static int GetSelfPosition(this IData data)
        {
            if (!data.IsSelf())
                return 0;

            if (SelfPosition.ContainsKey(data.DataSourceId))
                return SelfPosition[data.DataSourceId];

            var type = data.DataSourceId.InterfaceType;
            var list = new Dictionary<DataSourceId, List<DataSourceId>>();
            var dataPosition = new Dictionary<DataSourceId, int>();
            var datas = DataFacade.GetData(type);

            foreach (IData item in datas)
                list[item.DataSourceId] = item.GetReferees(type)
                    .Where(d => d.DataSourceId.PublicationScope == item.DataSourceId.PublicationScope)
                    .Select(d => d.DataSourceId).ToList();

            var prevCount = list.Count;
            for (var i = 1;; i++)
            {
                var toRemoveFromList = new List<DataSourceId>();
                foreach (var item in list.Where(item => !item.Value.Any()))
                {
                    dataPosition.Add(item.Key, -i);
                    toRemoveFromList.Add(item.Key);
                }

                foreach (var key in toRemoveFromList) list.Remove(key);

                var prevDataPosition = dataPosition.Keys.ToList();
                list = list.ToDictionary(d => d.Key, d => d.Value.Except(prevDataPosition).ToList());

                if (list.Count == 0)
                    break;
                if (prevCount == list.Count)
                    throw new InvalidOperationException(
                        $"ForeignKey error for type '{type}'. Circle in data detected.");
                prevCount = list.Count;
            }

            foreach (var item in dataPosition) SelfPosition[item.Key] = item.Value;

            return SelfPosition[data.DataSourceId];
        }

        private static bool IsSelf(this IData data)
        {
            var type = data.DataSourceId.InterfaceType;
            if (SelfList.ContainsKey(type))
                return SelfList[type];

            var foreignTypes = DataAttributeFacade.GetDataReferencePropertyInfoes(type).Select(d => d.TargetType);


            if (foreignTypes.Contains(type))
                SelfList[type] = true;
            else
                SelfList[type] = false;
            return SelfList[type];
        }
    }
}