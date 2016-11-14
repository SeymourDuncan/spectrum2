using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace Coredata
{

    public class DummyNode : ISpmNode
    {
        public IEnumerable<ISpmNode> GetChildNodes()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public SpmNodeType GetNodeType()
        {
            throw new NotImplementedException();
        }
    }


    public class SpmBase: ObservableObject
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
