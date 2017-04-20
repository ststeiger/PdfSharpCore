using System;
using System.Collections.Generic;
using System.Text;

namespace Stammbaum.DataStructures
{

    public class TreeInfo
    {
        public System.Collections.Generic.List<PersonData> AllData;
        public System.Collections.Generic.List<System.Collections.Generic.List<PersonData>> ls;


        public int MaxGeneration;
        public int MaxPresence;


        public TreeInfo()
        {
            this.ls = new System.Collections.Generic.List<System.Collections.Generic.List<PersonData>>();
        }


    }


}
