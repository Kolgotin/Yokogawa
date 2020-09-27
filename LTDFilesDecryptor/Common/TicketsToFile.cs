using LTDFilesDecryptor.Model;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace LTDFilesDecryptor.Common
{
    public class TicketsToFile
    {
        public List<TicketsModel> LoadFromFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<TicketsModel>));
            List<TicketsModel> tickets;

            FileInfo lastDateTimeFile = new FileInfo(filePath);
            if (File.Exists(filePath))
            {
                FileStream fs = lastDateTimeFile.OpenRead();
                tickets = (List<TicketsModel>)serializer.Deserialize(fs);
                fs.Close();
                return tickets;
            }
            return null;
        }

        public void SaveToFile(string folderPath, List<TicketsModel> tickets)
        {
            FileInfo ticketsFile = new FileInfo(Path.Combine(folderPath,"values","tickets.xml"));
            FileStream fs;
            var serializer = new XmlSerializer(typeof(List<TicketsModel>));
            if (ticketsFile.Exists)
            {
                fs = ticketsFile.Open(FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = ticketsFile.Create();
            }
            serializer.Serialize(fs, tickets);
            fs.Close();
        }
    }
}
