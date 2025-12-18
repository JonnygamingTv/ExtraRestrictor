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

        [XmlArrayItem(ElementName = "Item")]
        public List<RestrictedItem> RestrictedCrafting;

        [XmlArrayItem(ElementName = "Vehicle")]
        public List<RestrictedItem> RestrictedVehicleEnter;

        public bool IgnoreAdmins;

        public void LoadDefaults()
        {
            Restricted = new List<RestrictedItem>
            {
                new RestrictedItem { Bypass = "bypass.explosives", Id = 519 },
                new RestrictedItem { Id = 1441 }
            };
            RestrictedCrafting = new List<RestrictedItem>
            {
                new RestrictedItem { Bypass = "bypass.crafta", Id = 519 },
                new RestrictedItem { Id = 1441 }
            };
            RestrictedVehicleEnter = new List<RestrictedItem> {
                new RestrictedItem { Bypass = "bypass.police", Id = 33 }
            };
            IgnoreAdmins = true;
        }
    }
}
