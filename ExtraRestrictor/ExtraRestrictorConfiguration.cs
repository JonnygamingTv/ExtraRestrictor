using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ExtraConcentratedJuice.ExtraRestrictor
{
    public class ExtraRestrictorConfiguration : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "Item")]
        public List<RestrictedItem> Restricted;

        public bool IgnoreAdmins;

        public void LoadDefaults()
        {
            Restricted = new List<RestrictedItem>
            {
                new RestrictedItem { Bypass = "bypass.explosives", Id = 519 },
                new RestrictedItem { Id = 1441 }
            };
            IgnoreAdmins = true;
        }
    }
}
