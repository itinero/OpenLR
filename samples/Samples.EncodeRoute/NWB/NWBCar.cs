using Itinero.Attributes;
using Itinero.Profiles;
using System.Collections.Generic;

namespace Samples.EncodeRoute.NWB
{
    /// <summary>
    /// A vehicle profile for NWB-based shapefiles.
    /// </summary>
    public class NWBCar : Vehicle
    {
        /// <summary>
        /// Gets the unique name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "NWB.Car";
            }
        }

        /// <summary>
        /// Gets the profile whitelist.
        /// </summary>
        public override HashSet<string> ProfileWhiteList
        {
            get
            {
                return new HashSet<string>(new string[] { "BST_CODE", "BAANSUBSRT", "RIJRICHTNG", "WEGBEHSRT", "HECTO_LTTR" });
            }
        }

        /// <summary>
        /// Adds a number of keys to the given whitelist when they are relevant for this vehicle.
        /// </summary>
        /// <returns>True if the edge with the given attributes is usefull for this vehicle.</returns>
        public override bool AddToWhiteList(IAttributeCollection attributes, Whitelist whitelist)
        {
            return true;
        }

        /// <summary>
        /// Gets series of attributes and returns the factor and speed that applies. Adds relevant tags to a whitelist.
        /// </summary>
        public override FactorAndSpeed FactorAndSpeed(IAttributeCollection attributes, Whitelist whiteList)
        {
            string highwayType;
            if (attributes == null || !attributes.TryGetValue("BST_CODE", out highwayType))
            {
                return Itinero.Profiles.FactorAndSpeed.NoFactor;
            }
            float speed = 70;
            switch (highwayType)
            {
                case "BVD":
                    speed = 50;
                    break;
                case "AF":
                case "OP":
                    speed = 70;
                    break;
                case "HR":
                    speed = 120;
                    break;
            }
            string oneway;
            short direction = 0;
            if (attributes.TryGetValue("RIJRICHTNG", out oneway))
            {
                if (oneway == "H")
                {
                    direction = 1;
                }
                else if (oneway == "T")
                {
                    direction = 2;
                }
            }

            return new Itinero.Profiles.FactorAndSpeed()
            {
                Constraints = null,
                Direction = direction,
                SpeedFactor = 1.0f / speed,
                Value = 1.0f / speed
            };
        }
    }
}
