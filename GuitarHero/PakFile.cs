using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarHero
{
    public static class PakFile
    {
        public static PakArchive Open(string pakPath, string pabPath = null)
        {
            if (pabPath == null)
            {
                pabPath = pakPath.Replace(".pak.xen", ".pab.xen");
            }

            FileStream pakStream = new FileStream(pakPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            if (File.Exists(pabPath) && pabPath != pakPath)
            {
                FileStream pabStream = new FileStream(pabPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return new PakArchive(pakStream, pabStream);
            }
            else
            {
                return new PakArchive(pakStream);
            }
        }
    }
}
