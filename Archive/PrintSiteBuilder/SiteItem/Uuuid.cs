using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using PrintSiteBuilder.Interfaces;


namespace PrintSiteBuilder.SiteItem
{
    public class Uuid
    {
        public IPrint2 iPrint;
        public Uuid(IPrint2 iPrint) 
        {
            this.iPrint  = iPrint;
        }
        public void CreateUuidFiles()
        {
            var uuid = new List<string> { EncryptUuid(iPrint.Uuid), EncryptUuid("partner") };
            File.WriteAllLines($@"{iPrint.path.PrintUuidDir}\uuid.txt", uuid);
            //File.WriteAllLines($@"{iPrint.path.PrintUuidDir}\1pass.txt", pass);
        }
        public string EncryptUuid(string uuid)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(uuid);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
