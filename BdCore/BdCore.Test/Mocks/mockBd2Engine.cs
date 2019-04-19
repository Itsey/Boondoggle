namespace Plisky.Boondoggle2.Test {

    using Plisky.Boondoggle2;
    using System;
    using System.Drawing;

    public class mockBd2Engine : bd2Engine, IEngineEquipmentProvider{

        public Point Mock_DirectGetBotLocation(Guid guid) {
            var bt = GetMappedBotByPublicId(guid);
            return bt.Position;
        }

        public void Mock_DirectSetBotLocation(Guid guid, Point desiredLoc) {
            var bt = GetMappedBotByPublicId(guid);
            bt.Position = desiredLoc;
            if (!activeWorld.IsFreeWorldSpace(desiredLoc)) {
                throw new InvalidOperationException("You cant direct set outside the world or in a wall.");
            }
        }

        public void Mock_DirectSetBotSpeed(Guid guid, int p) {
            var bt = GetMappedBotByPublicId(guid);
            bt.Speed = p;
        }

        public void Mock_DirectSetBotHeading(Guid guid, double heading) {
            var bt = GetMappedBotByPublicId(guid);
            bt.Heading = heading;
        }

        public void Mock_DirectSetBotProperties(Guid g, Point loc, double heading, int speed) {
            Mock_DirectSetBotHeading(g, heading);
            Mock_DirectSetBotLocation(g, loc);
            Mock_DirectSetBotSpeed(g, speed);
        }

        public Guid Mock_GetFirstBotPublicId() {
            foreach (var v in mappedObjects) {
                var f = v as MappedBot;
                if (f != null) {
                    return f.Bot.PublicId;
                }
            }
            throw new InvalidOperationException("No Bots Found");
        }

        public Guid Mock_GetSecondBotPublicId() {
            bool skippedFirst = false;

            foreach (var v in mappedObjects) {
                var f = v as MappedBot;
                if (f != null) {
                    if (skippedFirst) {
                        return f.Bot.PublicId;
                    } else {
                        skippedFirst = true;
                    }
                }
            }
            throw new InvalidOperationException("Not enough Bots Found");
        }

        public void Mock_SetBotLife(Guid g, int newLife = 0) {
            var bt = GetMappedBotByPublicId(g);
            bt.LifeRemaining = newLife;
        }

        public MappedBot Mock_GetBotMapOBjectByPublicId(Guid botID) {
            var bt = GetMappedBotByPublicId(botID);
            return bt;
        }

        public void Mock_DirectSetBotCharge(Guid botID, int chargeLevel) {
            var bt = GetMappedBotByPublicId(botID);
            bt.ChargeRemaining = chargeLevel;
        }

        public void Mock_DirectSetBotEnergyLevels(Guid botID, int chargeLevel, int powerLevel) {
            var bt = GetMappedBotByPublicId(botID);
            bt.PowerRemaining = powerLevel;
            bt.ChargeRemaining = chargeLevel;
        }

        public double CalculateToHitPercent(Guid owningBotId, int tempScanKey, ActiveEquipment activeEquip) {
            var owner = GetMappedBotByPublicId(owningBotId);
            Point sourceLoc = owner.Position;
            Point destLoc = GetMappedBotById(activeData.GetEngineIdFromScanId(owner.EngineId, tempScanKey)).Position;
            

            if (!combatCore.Calcs.CanMountPointHitTarget(owner.Heading, MountPoint.Forward, sourceLoc, destLoc)) {
                return 0;
            }
            if (!this.activeWorld.IsLOSBetween(sourceLoc, destLoc)) {
                return 0;
            }

            return 1;
        }

        public void Mock_CreateArtificalScanId(int p, int ownerId, int targetId) {
            activeData.RegisterTeporaryKey(p, ownerId, targetId);
        }

        public void Mock_CreateArtificalScanId(int p, Guid ownerGuid, Guid targetGuid) {
            Mock_CreateArtificalScanId(p, GetMappedBotByPublicId(ownerGuid).EngineId, GetMappedBotByPublicId(targetGuid).EngineId);
        }

        public ActiveEquipment Mock_GetEquipmentInstanceById(Guid instanceId) {
            foreach (var v in this.installedEquipment) {
                if (v.InstanceId == instanceId) {
                    return v;
                }
            }
            return null;
        }

        public EquipmentSupport InjectedEquipment { get; set; }
        public ActiveEquipment CreateActiveEquipmentInstance(int equipmentIdentifier) {
            if(InjectedEquipment == null) {
                throw new InvalidOperationException("Must provide injected equipment");
            }
            return null;
        }

        public bool IsValidEquipmentLocation(int equipmentIdentifier, MountPoint mp) {
            if (InjectedEquipment == null) {
                throw new InvalidOperationException("Must provide injected equipment");
            }

            return true;
        }
    }
}