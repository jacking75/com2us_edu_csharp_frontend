using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMgrClient2.Shared
{
    public class SProgram
    {
        public int Version { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string LBStatus { get; set; }
    }

    // This will be serialized into a JSON Contact object
    public class Host
    {
        public string Name { get; set; }
        public List<SProgram> Programs { get; set; } = new();
    }

    // This will be serialized into a JSON array of Contact objects
    public class HostsCollection
    {
        public List<Host> Hosts { get; set; }

        public HostsCollection()
        {
        }
    }
}
