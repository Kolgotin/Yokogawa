using System;

namespace LTDFilesDecryptionLibrary
{
    public class TicketAtRange
    {
        public double SumValue { get; private set; }
        public int CorrectCount { get; private set; }
        public int StrangeCount { get; private set; }
        public DateTime DateTimeValue { get; set; }
        TicketInfo Ticket;

        public double? CorrectMinValue => Ticket.CorrectMinValue;
        public double? CorrectMaxValue => Ticket.CorrectMaxValue;
        public string TicketName => Ticket.TicketName;

        public TicketAtRange(TicketInfo ticket, DateTime dateTime)
        {
            Ticket = ticket;
            DateTimeValue = dateTime;
        }

        public bool Merge(double val)
        {
            if ((CorrectMinValue.HasValue && CorrectMinValue.Value > val) || (CorrectMaxValue.HasValue && CorrectMaxValue.Value < val))
            {
                StrangeCount++;
                return false;
            }
            else
            {
                SumValue += val;
                CorrectCount++;
                return true;
            }
        }
    }
}
