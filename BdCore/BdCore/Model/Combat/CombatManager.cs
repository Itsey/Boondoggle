namespace Plisky.Boondoggle2 {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using System.Diagnostics;

    public class CombatManager {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        
        /// <summary>
        /// Inject a new instance of bilge, or change the trace level of the current instance. To set the trace level ensure that
        /// the first parameter is null.  To set bilge simply pass a new instance of bilge.
        /// </summary>
        /// <param name="blg">An instance of Bilge to use inside this Hub</param>
        /// <param name="tl">If specified and blg==null then will alter the tracelevel of the current Bilge</param>
        public void InjectBilge(Bilge blg, TraceLevel tl = TraceLevel.Off) {
            if (blg != null) {
                b = blg;
            } else {
                b.CurrentTraceLevel = tl;
            }
        }



        private const int PERCENTAGEDELTA_PERSPEEDUNIT = 8;

        private bd2World map;
        public Bd2CombatCalculator Calcs { get; set; }

        public CombatManager(bd2World mp, Bd2CombatCalculator rnd) {
            map = mp;
            Calcs = rnd;
        }

        public CombatResult ResolveAttack(CombatAttack ca) {
            if (ca==null) {
                throw new BdBaseException("Can not resolve an attack without a combat attack");
            }
            ca.Validate();
            CombatResult result = new CombatResult();            

            bool canHit = true;

            if (!ca.Victim.IsAlive()) {
                canHit = false;
            }

            if ((canHit) && (!map.IsLOSBetween(ca.Attacker.Position, ca.Victim.Position))) {
               b.Verbose.Log( string.Format("WeaponFire ({0} --> {1}) - Miss, No Line of sight.", ca.Attacker.Bot.Name, ca.Victim.Bot.Name));
                canHit = false;
            }

            if ((canHit) && (!Calcs.CanMountPointHitTarget(ca.Attacker.Heading, ca.WeaponInstance.MountPoint, ca.Attacker.Position, ca.Victim.Position))) {
               b.Verbose.Log( string.Format("WeaponFire ({0} --> {1}) - Miss, Mountpoint Cant Target.", ca.Attacker.Bot.Name, ca.Victim.Bot.Name));
                canHit = false;
            }

            if (!canHit) {
                result.DidHit = false;
                result.TotalDamage = 0;
            } else {
                double hC = ca.Weapon.BaseHitChance;
                hC += GetSpeedModifier(ca.Attacker.Speed, ca.Victim.Speed);
                hC += GetDistanceModifier(map.GetDistanceBetween(ca.Attacker.Position, ca.Victim.Position));

                result.DidHit = Calcs.DidAchievePercentage(hC);
                b.Info.Log( string.Format("WeaponFire ({0} --> {1}) - Hit Chance {2} - Hit : {3}", ca.Attacker.Bot.Name, ca.Victim.Bot.Name, hC, result.DidHit));
                if (result.DidHit) {
                    result.TotalDamage = Calcs.D10Rolls(ca.Weapon.D10DamageRolls);
                }
            }
            return result;
        }

        

        private double GetDistanceModifier(int p) {
            if (p < 4) {
                return 4 * PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            if (p > 10) {
                return (p - 10) / 5 * PERCENTAGEDELTA_PERSPEEDUNIT * -1;
            }
            return 0;
        }

        private double GetSpeedModifier(int p1, int p2) {
            double result = 0;
            result += GetSingleSpeedModifier(p1);
            result += GetSingleSpeedModifier(p2);
            return result;
        }

        public double GetSingleSpeedModifier(int p1) {
            if (p1 == 0) {
                return PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            double result = 0;
            if (p1 > 3) {
                result -= PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            if (p1 > 4) {
                result -= PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            if (p1 > 5) {
                result -= PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            if (p1 > 6) {
                result -= PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            if (p1 > 7) {
                result -= PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            if (p1 > 8) {
                result -= PERCENTAGEDELTA_PERSPEEDUNIT;
            }
            return result;
        }
    }
}