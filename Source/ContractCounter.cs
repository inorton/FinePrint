using Contracts;
using FinePrint.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinePrint
{
    public class ContractCounter
    {
        static ContractCounter singleton = new ContractCounter();
        static object sync = new object();

        Dictionary<Type, ContractLimit> limits = new Dictionary<Type, ContractLimit>();

        public static ContractCounter GetInstance()
        {
            return singleton;
        }

        public void RegisterContractLimits(ContractBase c, int maxactive, int maxoffered)
        {
            lock (sync)
            {                
                limits[c.GetType()] = new ContractLimit() { Active = maxactive, Offered = maxoffered };
            }
        }

        /// <summary>
        /// Check that there is room allowed for the given contract type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool CheckLimits<T>(T c)
            where T : Contract
        {
            var acs = Util.GetContracts<T>();
            lock (sync)
            {
                ContractLimit l;
                if (limits.TryGetValue(typeof(T), out l))
                {
                    int offeredContracts = Util.CountContractState(acs, Contract.State.Offered);
                    if (l.Offered >= offeredContracts) return false;
                    int activeContracts = Util.CountContractState(acs, Contract.State.Active);
                    if (l.Active >= activeContracts) return false;
                }
            }
            return true;
        }
    }
}
