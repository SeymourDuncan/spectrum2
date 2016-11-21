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
    public interface ISpmNode
    {
        IEnumerable<ISpmNode> GetChildNodes();
        SpmNodeType GetNodeType();
        string GetName();
    }


    public interface ISpmLoader
    {
        // TODO свойства
        //bool SaveOneObject(int sysId, int classId, string name, Dictionary<double, double> values, string comment = "");
        bool SaveObjToDb(SpmObject spmObj);
    }
}