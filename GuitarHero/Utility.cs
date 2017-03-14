using System.Text;

namespace GuitarHero
{
    internal static class Utility
    {
        public static Encoding Latin1Encoding;

        static Utility()
        {
            Latin1Encoding = Encoding.GetEncoding("iso-8859-1");
        }
    }
}
