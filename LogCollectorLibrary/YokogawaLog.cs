using System;
using System.Collections.Generic;

namespace LogCollectorLibrary
{
    public class YokogawaLog : IEquatable<YokogawaLog>
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public DateTime DateTimeUMTEvent { get; set; }
        public int UnknownColumn1Id { get; set; }
        public int TypeEvent { get; set; }
        public int IindexNumberEvent { get; set; }
        public int WriterId { get; set; }
        public DateTime ExactDateTime { get; set; }
        public string UnknownColumn2 { get; set; }
        public string UnknownColumn3 { get; set; }
        public int ControllersAndStationsId { get; set; }
        public string PositionOrAlarmOrBlockName { get; set; }
        public string MessageText { get; set; }

        public bool Equals(YokogawaLog other)
            => other != null &&
                   UnknownColumn1Id == other.UnknownColumn1Id &&
                   TypeEvent == other.TypeEvent &&
                   IindexNumberEvent == other.IindexNumberEvent &&
                   WriterId == other.WriterId &&
                   ExactDateTime == other.ExactDateTime &&
                   UnknownColumn2 == other.UnknownColumn2 &&
                   ControllersAndStationsId == other.ControllersAndStationsId &&
                   PositionOrAlarmOrBlockName == other.PositionOrAlarmOrBlockName;

        public override bool Equals(object obj) 
            => Equals(obj as YokogawaLog);

        public override int GetHashCode()
        {
            var hashCode = -1148860628;
            hashCode = hashCode * -1521134295 + UnknownColumn1Id.GetHashCode();
            hashCode = hashCode * -1521134295 + TypeEvent.GetHashCode();
            hashCode = hashCode * -1521134295 + IindexNumberEvent.GetHashCode();
            hashCode = hashCode * -1521134295 + WriterId.GetHashCode();
            hashCode = hashCode * -1521134295 + ExactDateTime.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UnknownColumn2);
            hashCode = hashCode * -1521134295 + ControllersAndStationsId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PositionOrAlarmOrBlockName);
            return hashCode;
        }
    }
}