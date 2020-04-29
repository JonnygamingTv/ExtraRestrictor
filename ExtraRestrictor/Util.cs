using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraConcentratedJuice.ExtraRestrictor
{
    public static class Util
    {
        public static string Translate(string TranslationKey, params object[] Placeholders) =>
            ExtraRestrictor.Instance.Translations.Instance.Translate(TranslationKey, Placeholders);
    }
}