using System;
using System.Collections.Generic;

namespace Coredata
{

    //public class DummyNode : ISpmNode
    //{
    //    public IEnumerable<ISpmNode> GetChildNodes()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int GetId()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public string GetName()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public SpmNodeType GetNodeType()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public class SpmBase
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public SpmBase(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public SpmBase()
        {
        }
    }


}
