using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Contracts;
using Contracts.Parameters;
using KSP;
using KSPAchievements;
using FinePrint.Contracts.Parameters;

namespace FinePrint.Contracts
{
    public class LowLevelAerialContract : AerialContract
    {
        public override int GetMaxActiveContracts()
        {
            return 5;
        }

        public override int GetMaxOfferedContracts()
        {
            return 3;
        }

        protected override int SetAltitudeRange(int additionalWaypoints)
        {
            switch (targetBody.GetName())
            {
                case "Duna":
                    additionalWaypoints = 1;
                    minAltitude = 500.0;
                    maxAltitude = 16000.0;
                    break;
                case "Laythe":
                    additionalWaypoints = 1;
                    minAltitude = 1000.0;
                    maxAltitude = 15000.0;
                    break;
                case "Eve":
                    additionalWaypoints = 1;
                    minAltitude = 500.0;
                    maxAltitude = 20000.0;
                    break;
                case "Kerbin":
                    additionalWaypoints = 3;
                    minAltitude = 500.0;
                    maxAltitude = 12500.0;
                    break;
                default:
                    return base.SetAltitudeRange(additionalWaypoints);
            }
            return additionalWaypoints;
        }
        
        protected override string GetSynopsys()
        {
            return "There are bits of " + targetBody.theName + " that we don't know much about, fly low over them, see stuff and don't crash.";
        }
    }
}
