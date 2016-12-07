using System;
using System.Collections.Generic;

namespace Coredata
{
    /// <summary>
    /// Типы узлов
    /// </summary>
    public enum SpmNodeType
    {
        SntSystem = 0,
        SntClass,
        SntObject
    }

    /// <summary>
    /// интерфейс для дерева объектов
    /// </summary>
    public interface ISpmNode: ISpmBase
    {
        IEnumerable<ISpmNode> GetChildNodes();
        SpmNodeType GetNodeType();
    }

    public interface ISpmBase
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    public interface ISpmLoader
    {
        // TODO свойства
        //bool SaveOneObject(int sysId, int classId, string name, Dictionary<double, double> values, string comment = "");
        bool SaveObjToDb(SpmObject spmObj);
    }
}