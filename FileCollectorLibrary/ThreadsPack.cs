
namespace FileCollectorLibrary
{
    public class ThreadsPack
    {
        public MoveFilesList MoveFiles { get; set; }
        public DateToXml DateXml { get; set; }

        public ThreadsPack(MoveFilesList moveFilesList, DateToXml dateToXml)
        {
            MoveFiles = moveFilesList;
            DateXml = dateToXml;
        }
    }
}
