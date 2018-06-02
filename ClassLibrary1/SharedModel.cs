using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.Runtime.Serialization;

namespace Shared.DataModels
{
    [ProtoContract(SkipConstructor = true)]
    public class Request
    {
        [ProtoMember(1)]
        public Guid CorrelationID { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }
    }

    [DataContract]
    public class Response
    {
        [DataMember]
        public Guid CorrelationID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Value { get; set; }
    }
}
