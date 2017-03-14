using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarHero
{
    public class PakArchive : IDisposable
    {
        private List<PakEntry> entries;
        private FileStream pakStream;
        private FileStream pabStream;
        private long pabOffset = -1;

        public IReadOnlyList<PakEntry> Entries => entries;

        private void ReadHeader()
        {
        }

        public PakArchive(FileStream pakStream) {
            this.pakStream = pakStream;
            this.ReadHeader();
        }

        public PakArchive(FileStream pakStream, FileStream pabStream) : this(pakStream) {
            this.pabStream = pabStream;
            this.pabOffset = pakStream.Length;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing)
                {
                    this.pakStream.Dispose();
                    this.pabStream.Dispose();
                }
                
                entries = null;
                disposedValue = true;
            }
        }
        
        public void Dispose() {
            Dispose(true);
        }
        #endregion
    }
}
