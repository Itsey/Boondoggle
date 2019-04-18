namespace Plisky.Boondoggle2 {

    public interface IProvideBotInteractivity {

        void ChangeSpeed(BoonBotBase targetBot, int byThisMuch);

        void ChangeHeading(BoonBotBase targetBot, double byThisMuch);

        EquipmentInstallationResult MountEquipment(BoonBotBase targetBot, int equipmentIdentifier, MountPoint mountPoint);

        EquipmentUseResult UseEquipmentItem(BoonBotBase targetBot, EquipmentUseDetails eud);
    }
}