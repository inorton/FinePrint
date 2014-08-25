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
    public abstract class ContractBase : Contract
    {
        protected override bool Generate()
        {
            RegisterLimits();
            return ContractCounter.GetInstance().CheckLimits(this) 
                && HasRequiredTech() 
                && GenerateContract();
        }

        public abstract int GetMaxActiveContracts();

        public abstract int GetMaxOfferedContracts();

        public virtual void RegisterLimits()
        {
            ContractCounter.GetInstance().RegisterContractLimits(this, GetMaxActiveContracts(), GetMaxOfferedContracts());
        }

        public virtual bool HasRequiredTech()
        {
            return true;
        }

        public abstract bool GenerateContract();

        public override bool CanBeCancelled()
        {
            return true;
        }

        public override bool CanBeDeclined()
        {
            return true;
        }

        /// <summary>
        /// Class name shortcut for LoadNode
        /// </summary>
        public string ClassName
        {
            get
            {
                return GetType().Name;
            }
        }

        protected override string GetHashString()
        {
            return (this.MissionSeed.ToString() + this.DateAccepted.ToString());
        }

        protected CelestialBody GetNextUnreachedTarget(int depth, bool removeSun, bool removeKerbin)
        {
            System.Random generator = new System.Random(MissionSeed);
            var bodies = Contract.GetBodies_NextUnreached(depth, null);

            if (bodies != null)
            {
                if (removeSun)
                    bodies.Remove(Planetarium.fetch.Sun);

                if (removeKerbin)
                    bodies.Remove(Planetarium.fetch.Home);

                if (bodies.Count > 0)
                    return bodies[generator.Next(0, bodies.Count - 1)];
            }
            return null;
        }

    }
}
