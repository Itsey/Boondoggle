using Plisky.Diagnostics;
using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plisky.Boondoggle2 {


    /// <summary>
    /// Server side construct that manages an active setup of installed equipment within a signle bot.  This is designed
    /// to group all of the bot related equipment together in one place.  However the master kit reference is not storeed
    /// here but in the engine itself - this simply describes which instances are where.  
    /// </summary>
    public class ActiveLoadout {


        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        // Paste in constructor? >> b = useThisBilge ?? new Bilge(tl: TraceLevel.Off);
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



        IEngineEquipmentProvider provide;
        private Dictionary<Guid, ActiveEquipment> kitByIdentity = new Dictionary<Guid, ActiveEquipment>();
        private Dictionary<MountPoint, List<EquipmentInstallationResult>> installedKit = new Dictionary<MountPoint, List<EquipmentInstallationResult>>();
        private bool hasPowerpackBeenInstalled;
        BotFrame activeFrame;

        public ActiveLoadout(IEngineEquipmentProvider prov, BotFrame bf) {
            if(prov==null) {
                throw new InvalidOperationException("Can not operate without a provider");
            }
            if (bf == null) {
                throw new InvalidOperationException("CAn not operate without a frame");
            }
            activeFrame = bf;
            provide = prov;
            foreach(var v in bf.GetAccesibleMountpoints()) {
                installedKit.Add(v, new List<EquipmentInstallationResult>());
            }
        }


        public int TotalLoadedWeight { get; set; }

        public int AvailableWeight { get; set; }

        public int AvailableBodySpaces { get; set; }

        public bool HasTurret { get; set; }

        public int AvailableTurrentSpaces { get; set; }



        public EquipmentInstallationResult AddEquipment(int equipmentIdentifier, MountPoint mp) {
            EquipmentInstallationResult result = new EquipmentInstallationResult();
            ActiveEquipment ae= provide.CreateActiveEquipmentInstance(equipmentIdentifier);
            if (ae == null) {
                throw new BdBaseException("The engine provider refused to create the equipment, unexpected.");
            }
            result.EquipmentId = equipmentIdentifier;
            result.InstanceId = ae.InstanceId;
            result.Result = InstallationResult.Installed;

            if (!installedKit.ContainsKey(mp)) {
                // installedKit.Add(mp, new List<EquipmentInstallationResult>());
                b.Warning.Log("Warning, failing to install kit because the frame has no mountpoint at the position requested", string.Format("kitid {0} mpId {1} ", equipmentIdentifier, mp));
                result.Result = InstallationResult.Fail_InvalidMountpoint;
                return result;
            }
            bool canInstall = provide.IsValidEquipmentLocation(equipmentIdentifier, mp);
            if (!canInstall) {
                result.Result = InstallationResult.Fail_InvalidMountpoint;
                return result; 
            }
            //Must check for space
            if (!WillEquipmentFitInMountPoint(ae, mp)) {
                result.Result = InstallationResult.Fail_NoSpace;
                return result;
            }

          
            installedKit[mp].Add(result);
            kitByIdentity.Add(result.InstanceId, ae);

            if (ae.Classification == ItemClassification.PowerPack) {
                if (hasPowerpackBeenInstalled) {
                    result.Result = InstallationResult.Fail_InvalidCombination;
                } else {
                    hasPowerpackBeenInstalled = true;
                }
            }


            return result;
        }
#if false
        public EquipmentInstallationResult GetEquipmentByInstanceId(Guid instanceIdentity) {
            return kitByIdentity[instanceIdentity];
        }
#endif

        // TODO : Unit test that forces this to fail
        private bool WillEquipmentFitInMountPoint(ActiveEquipment ae, MountPoint mp) {
            b.Warning.Log("This is hardcoded");
            int spaceTaken = 0;
            foreach(var ep in GetEquipmentInMountPoint(mp)) {
                spaceTaken += kitByIdentity[ep.InstanceId].UnderlyingItem.SpaceRequired;
            }
            int space = activeFrame.GetTotalSpaceForMountPoint(mp);
            return space >= ae.UnderlyingItem.SpaceRequired;
             
        }

        public IEnumerable<EquipmentInstallationResult> GetEquipmentInMountPoint(MountPoint mp) {
            foreach(var v in installedKit[mp]) {
                yield return v;
            }
        }
    }
}
