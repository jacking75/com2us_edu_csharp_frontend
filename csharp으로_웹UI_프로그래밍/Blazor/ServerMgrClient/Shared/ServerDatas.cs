using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMgrClient.Shared
{
    public class SProgram
    {
        public string name { get; set; }
        public string arguments { get; set; }
        public string path { get; set; }
    }

    // This will be serialized into a JSON Contact object
    public class Host
    {
        public string name { get; set; }
        public List<SProgram> programs { get; set; }
    }

    // This will be serialized into a JSON array of Contact objects
    public class HostsCollection
    {
        public ICollection<Host> hosts { get; set; }

        public HostsCollection()
        {
        }
    }
}
