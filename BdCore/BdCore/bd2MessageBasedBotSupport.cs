using Plisky.Plumbing;
using System;

namespace Plisky.Boondoggle2 {

    public abstract class bd2BaseBotEngineSupport : bd2BaseModel, IProvideBotInteractivity, IKnowWhatBotsDo, IEngineEquipmentProvider {

        protected abstract void ActualChangeHeading(BoonBotBase targetBot, double byThisMuch);

        protected abstract void ActualChangeSpeed(BoonBotBase targetBot, int byThisMuch);

        protected abstract EquipmentInstallationResult ActualMountEquipment(BoonBotBase targetBot, int equipmentIdentifier, MountPoint mountPoint);

        protected abstract EquipmentUseResult ActualUseEquipmentItem(BoonBotBase targetBot, EquipmentUseDetails eud);

        protected abstract int ActualGetCurrentSpeed(BoonBotBase publicId);

        protected abstract double ActualGetCurrentHeading(BoonBotBase boonBotBase);

        protected abstract ActiveEquipment ActualCreateActiveEquipmentInstance(int equipmentIdentifier);

        protected abstract bool ActualIsValidEquipmentLocation(int equipmentIdentifier, MountPoint mp);

        //iprovideinteractivity start
        public void ChangeHeading(BoonBotBase targetBot, double byThisMuch) {
            if (targetBot == null) {
                throw new BdBaseException("The bot can not be null for a change of direction");
            }
            ActualChangeHeading(targetBot, byThisMuch);
        }

        public void ChangeSpeed(BoonBotBase targetBot, int byThisMuch) {
            if (targetBot == null) {
                throw new BdBaseException("The bot can not be null for a change of direction");
            }
            ActualChangeSpeed(targetBot, byThisMuch);
        }

        public EquipmentInstallationResult MountEquipment(BoonBotBase targetBot, int equipmentIdentifier, MountPoint mountPoint) {
            if (targetBot == null) {
                throw new BdBaseException("The bot can not be null for a change of direction");
            }
            return ActualMountEquipment(targetBot, equipmentIdentifier, mountPoint);
        }

        public EquipmentUseResult UseEquipmentItem(BoonBotBase targetBot, EquipmentUseDetails eud) {
            if (targetBot == null) {
                throw new BdBaseException("The bot can not be null for a change of direction");
            }
            if (eud == null) {
                throw new BdBaseException("The EUD mustn be specified");
            }
            return ActualUseEquipmentItem(targetBot, eud);
        }

        //iprovideinteractivityend

        //iknowwhatbotsdo
        public int GetCurrentSpeed(BoonBotBase targetBot) {
            if (targetBot == null) {
                throw new BdBaseException("The bot can not be null for get speed");
            }
            return ActualGetCurrentSpeed(targetBot);
        }

        public double GetCurrentHeading(BoonBotBase targetBot) {
            if (targetBot == null) {
                throw new BdBaseException("The bot can not be null for gethEading");
            }
            return ActualGetCurrentHeading(targetBot);
        }

        public ActiveEquipment CreateActiveEquipmentInstance(int equipmentIdentifier) {
            return ActualCreateActiveEquipmentInstance(equipmentIdentifier);
        }

        public bool IsValidEquipmentLocation(int equipmentIdentifier, MountPoint mp) {
            return ActualIsValidEquipmentLocation(equipmentIdentifier, mp);
        }

        //iknowwhatbotsdoend
    }

    public class bd2MessageBasedBotSupport : bd2BaseBotEngineSupport {

        protected override void ActualChangeHeading(BoonBotBase targetBot, double byThisMuch) {
            hub.Launch<Message_BotPerformAction>(new Message_BotPerformAction(MainMessageKind.MapObjectMovementChange, KnownSubkinds.ChangeDirection) {
                PublicBotId = targetBot.PublicId,
                DParameter = byThisMuch
            });
        }

        protected override void ActualChangeSpeed(BoonBotBase targetBot, int byThisMuch) {
            NavigationInfoContext nic = new NavigationInfoContext();
            nic.PublicBotId = targetBot.PublicId;
            nic.SpeedDelta = byThisMuch;

            hub.Launch<Message_BotPerformAction>(new Message_BotPerformAction(MainMessageKind.MapObjectMovementChange, KnownSubkinds.ChangeSpeed) {
                RequestContext = nic
            });
        }

        protected override int ActualGetCurrentSpeed(BoonBotBase targetBot) {
            var qry = new Message_Query(MainMessageKind.QueryBotStatus, KnownSubkinds.ReadSpeed);
            qry.PublicBotId = targetBot.PublicId;
            hub.Launch<Message_Query>(qry);
            NavigationInfoContext nic = (NavigationInfoContext)qry.ResponseContext;
            b.Assert.True(nic != null, "The response cant have a null value");
            return nic.SpeedDelta;
        }

        protected override double ActualGetCurrentHeading(BoonBotBase targetBot) {
            var qry = new Message_Query(MainMessageKind.QueryBotStatus, KnownSubkinds.ReadHeading);
            qry.PublicBotId = targetBot.PublicId;
            hub.Launch<Message_Query>(qry);
            return qry.DParameter;
        }

        protected override EquipmentInstallationResult ActualMountEquipment(BoonBotBase targetBot, int equipmentIdentifier, MountPoint mountPoint) {
            EquipmentInstallationContext eic = new EquipmentInstallationContext();
            eic.OwningBotIdentity = targetBot.PublicId;
            eic.EquipmentIdentifier = equipmentIdentifier;
            eic.MountPoint = mountPoint;

            var msg = new Message_BotPerformAction(MainMessageKind.Workshop, KnownSubkinds.InstallEquipment) {
                PublicBotId = eic.OwningBotIdentity,
                RequestContext = eic
            };

            hub.Launch<Message_BotPerformAction>(msg);

            EquipmentInstallationResult eur = (EquipmentInstallationResult)msg.ResponseContext;
            //            b.Assert.True(eur != null, "The return result is null, the wrong type of return is being done or no message was handled.");
            return eur;
        }

        protected override EquipmentUseResult ActualUseEquipmentItem(BoonBotBase targetBot, EquipmentUseDetails eud) {
            EquipmentUseRequestContext euc = new EquipmentUseRequestContext();
            euc.OwningBotIdentity = targetBot.PublicId;
            euc.RequestedEquipmentInstance = eud.InstanceIdentity;
            euc.IParam = eud.IParam;

            var msg = new Message_BotPerformAction(MainMessageKind.BotActivity, KnownSubkinds.UseEquipment) {
                PublicBotId = targetBot.PublicId,
                RequestContext = euc
            };

            hub.Launch<Message_BotPerformAction>(msg);

            EquipmentUseResult eur = (EquipmentUseResult)msg.ResponseContext;
            return eur;
        }

        protected override ActiveEquipment ActualCreateActiveEquipmentInstance(int equipmentIdentifier) {
            throw new NotImplementedException();
        }

        protected override bool ActualIsValidEquipmentLocation(int equipmentIdentifier, MountPoint mp) {
            throw new NotImplementedException();
        }
    }
}