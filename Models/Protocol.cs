using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GaffarovaAlbina.Models
{
    public class Protocol
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public List<Assignment> Assignments { get; set; }

        public Protocol() { }
        public Protocol(ProtocolDTO protocolDTO, List<Assignment> assignmentsID)
        {
            Name = protocolDTO.Name;
            Date = protocolDTO.Date;
            Assignments = assignmentsID;
        }
    }

    public class ProtocolDTO
    {
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public List<long> assignmentsID { get; set; }
    }

    public class ProtocolVM
    {
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int AmountAssig { get; }

        public ProtocolVM() { }
        public ProtocolVM(Protocol protocol)
        {
            Name = protocol.Name;
            Date = protocol.Date;
            if (protocol.Assignments != null && protocol.Assignments.Count != 0)
                AmountAssig = protocol.Assignments.Count;
            else
                AmountAssig = 0;
        }
    }
}
