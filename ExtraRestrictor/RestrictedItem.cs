using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ExtraConcentratedJuice.ExtraRestrictor
{
    public class RestrictedItem
    {
        [XmlText]
        public ushort Id { get; set; }
        [XmlAttribute("BypassPermission")]
        public string Bypass { get; set; }

        public RestrictedItem() { }
    }
}
