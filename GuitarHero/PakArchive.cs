using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarHero
{
    public class PakArchive
    {
        public IReadOnlyList<PakEntry> Entries => entries;

        private List<PakEntry> entries;
    }
}
