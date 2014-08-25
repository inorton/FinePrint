using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinePrint
{
    public class TechTree
    {
        public static bool AreWingsUnlocked()
        {
            return Util.haveAnyTechnology("AdvancedCanard",
                "StandardCtrlSrf",
                "airplaneTail",
                "CanardController",
                "deltaWing",
                "noseConeAdapter",
                "rocketNoseCone", 
                "smallCtrlSrf", 
                "standardNoseCone",
                "sweptWing", 
                "tailfin", 
                "wingConnector", 
                "winglet3");
        }

        public static bool ArePortsUnlocked()
        {
            return Util.haveAnyTechnology("dockingPort1",
                "dockingPort2",
                "dockingPort3",
                "dockingPortLarge",
                "dockingPortLateral");
        }

        protected static bool ArePowerPartsUnlocked()
        {
            return Util.haveAnyTechnology("rtg",
                "largeSolarPanel",
                "solarPanels1",
                "solarPanels2",
                "solarPanels3",
                "solarPanels4",
                "solarPanels5");
        }

        public static bool AreAntennaeUnlocked()
        {
            return Util.haveAnyTechnology("commDish",
                "longAntenna",
                "mediumDishAntenna");
        }

        public static bool AreWheelsUnlocked()
        {
            return Util.haveAnyTechnology("roverWheel1",
                "roverWheel2",
                "roverWheel3",
                "wheelMed");
        }

        public static bool AreProbesUnlocked()
        {
            return Util.haveAnyTechnology("probeCoreCube",
                "probeCoreHex",
                "probeCoreOcto",
                "probeCoreOcto2",
                "probeCoreSphere",
                "probeStackLarge",
                "probeStackSmall");
        }

        public static bool AreSatellitesUnlocked()
        {
            return (AreAntennaeUnlocked() && AreProbesUnlocked() && ArePowerPartsUnlocked());
        }

        public static bool AreFacilitiesUnlocked()
        {
            return (AreAntennaeUnlocked() && ArePortsUnlocked() && ArePowerPartsUnlocked());
        }
    }
}
