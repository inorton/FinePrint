﻿using System;
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
	public class ARMContract : Contract
	{
		CelestialBody targetBody = null;
        string asteroidClass = "A";
        bool isLanding = false;

		protected override bool Generate()
		{
            if (Util.haveTechnology("GrapplingDevice") == false)
                return false;

            System.Random generator = new System.Random(base.MissionSeed);

            //ARM fails generation on duplicates, so we can't have many out at once.
            int totalContracts = ContractSystem.Instance.GetCurrentContracts<ARMContract>().Count();
            if (totalContracts >= 2)
                return false;

			if (this.prestige == Contract.ContractPrestige.Trivial)
			{
                asteroidClass = "A";
				targetBody = Planetarium.fetch.Home;
			}
			else if (this.prestige == Contract.ContractPrestige.Significant)
			{
                if (generator.Next(0, 101) > 50)
                    asteroidClass = "B";
                else
                    asteroidClass = "C";

				List<CelestialBody> bodies = GetBodies_Reached(true, false);

				if (bodies.Count == 0)
					return false;

				targetBody = bodies[UnityEngine.Random.Range(0, bodies.Count)];

                if (generator.Next(0, 100) > 90)
                    targetBody = Planetarium.fetch.Sun;
			}
			else if (this.prestige == Contract.ContractPrestige.Exceptional)
			{
                if (generator.Next(0, 101) > 50)
                    asteroidClass = "D";
                else
                    asteroidClass = "E";

                targetBody = GetNextUnreachedTarget(1, true, true);

                if (targetBody == null)
                {
                    List<CelestialBody> bodies = GetBodies_Reached(true, false);

                    if (bodies.Count == 0)
                        return false;

                    targetBody = bodies[UnityEngine.Random.Range(0, bodies.Count)];
                }

                if (generator.Next(0, 100) > 70)
    				targetBody = Planetarium.fetch.Sun;
			}

            if (targetBody == null)
                targetBody = Planetarium.fetch.Home;

            this.AddParameter(new AsteroidParameter(asteroidClass, false), null);

			if (targetBody == Planetarium.fetch.Sun)
			{
				this.AddParameter(new LocationAndSituationParameter(targetBody, Vessel.Situations.ESCAPING, "vessel"), null);
			}
			else
			{
                if (targetBody == Planetarium.fetch.Home && generator.Next(0, 101) > 80)
                {
                    isLanding = true;
                    this.AddParameter(new LocationAndSituationParameter(targetBody, Vessel.Situations.LANDED, "vessel"), null);
                }
                else
    				this.AddParameter(new LocationAndSituationParameter(targetBody, Vessel.Situations.ORBITING, "vessel"), null);
			}

			base.AddKeywords(new string[] { "asteroidretrieval" });
			base.SetExpiry();
			base.SetDeadlineYears(7.0f, targetBody);

            base.SetFunds(15000f, 90000f, this.targetBody);
            base.SetReputation(450f, 225f, this.targetBody);
            base.SetScience(225f, this.targetBody);

            //Prevent duplicate contracts shortly before finishing up.
            foreach (ARMContract active in ContractSystem.Instance.GetCurrentContracts<ARMContract>())
            {
                if (active.targetBody == this.targetBody && active.asteroidClass == this.asteroidClass)
                    return false;
            }

            this.AddParameter(new KillControlsParameter(10), null);

			return true;
		}

		public override bool CanBeCancelled()
		{
			return true;
		}

		public override bool CanBeDeclined()
		{
			return true;
		}

		protected override string GetHashString()
		{
			return (this.MissionSeed.ToString() + this.DateAccepted.ToString());
		}

		protected override string GetTitle()
		{
            if (targetBody != Planetarium.fetch.Sun)
            {
                if ( isLanding )
                    return "Bring a newly discovered Class " + asteroidClass + " asteroid to " + targetBody.theName + " and land it.";
                else
                    return "Bring a newly discovered Class " + asteroidClass + " asteroid into an orbit around " + targetBody.theName + ".";
            }
            else
                return "Eject a Class " + asteroidClass + " asteroid out of the solar system.";
		}

		protected override string GetDescription()
		{
			//those 3 strings appear to do nothing
			return TextGen.GenerateBackStories(Agent.Name, Agent.GetMindsetString(), "retrieving an asteroid", "snooker", "large rocks", new System.Random().Next());
		}

		protected override string GetSynopsys()
		{
			if (targetBody != Planetarium.fetch.Sun)
            {
                if (isLanding)
                {
                    return "Capture a new Class " + asteroidClass + " asteroid, then bring it to " + targetBody.theName + " and land it...gently.";
                }
                else
                {
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                            return "Capture a new Class " + asteroidClass + " asteroid, then bring it into a stable orbit around " + targetBody.theName + " to test our capabilities.";
                        case 1:
                            return "Capture a new Class " + asteroidClass + " asteroid, then bring it into a stable orbit around " + targetBody.theName + ". Why? FOR SCIENCE!";
                        default:
                            return "Mission control says low Kerbin orbit is getting a bit crowded. Capture a new Class " + asteroidClass + " asteroid and take it into orbit around " + targetBody.theName + " instead.";
                    }
                }
			}
			else
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
					case 0:
                        return "Capture a new Class " + asteroidClass + " asteroid, then put it on an extrasolar trajectory. The less of these things orbiting the sun, the better.";
					case 1:
                        return "The last Class " + asteroidClass + " asteroid that passed Kerbin nearly wiped out our species, capture one and get rid of it.";
					default:
                        return "How do you feel about throwing a Class " + asteroidClass + " rock out of the solar system?";
				}
			}
		}

		protected override string MessageCompleted()
		{
            if (targetBody != Planetarium.fetch.Sun)
                return "You successfully captured an asteroid and brought it to " + targetBody.theName + ".";
			else
			{
				switch (UnityEngine.Random.Range(0, 3))
				{
					case 0:
						return "You successfuly flung a rock out of the solar system.";
					case 1:
						return "That asteroid is OUTTA HERE!";
					default:
						return "Looks like the asteroid is on a solar escape trajectory. Mission complete.";
				}
			}
		}

		protected override void OnLoad(ConfigNode node)
		{
			Util.LoadNode(node, "ARMContract", "targetBody", ref targetBody, Planetarium.fetch.Home);
            Util.LoadNode(node, "ARMContract", "asteroidClass", ref asteroidClass, "A");
            Util.LoadNode(node, "ARMContract", "isLanding", ref isLanding, false);
		}

		protected override void OnSave(ConfigNode node)
		{
			int bodyID = targetBody.flightGlobalsIndex;
			node.AddValue("targetBody", bodyID);
            node.AddValue("asteroidClass", asteroidClass);
            node.AddValue("isLanding", isLanding);
		}

		public override bool MeetRequirements()
		{
            return true;
		}

		protected static CelestialBody GetNextUnreachedTarget(int depth, bool removeSun, bool removeKerbin)
		{
			var bodies = Contract.GetBodies_NextUnreached(depth, null);
			if (bodies != null)
			{
				if (removeSun)
					bodies.Remove(Planetarium.fetch.Sun);

				if (removeKerbin)
					bodies.Remove(Planetarium.fetch.Home);

				if (bodies.Count > 0)
					return bodies[UnityEngine.Random.Range(0, bodies.Count - 1)];
			}
			return null;
		}
	}
}