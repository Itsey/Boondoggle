namespace Plisky.Boondoggle2 {
    public interface IEngineEquipmentProvider {

        //make sure bot is assignede guid and only the bot can consume that guid
        // make sure guids are unique
        // make sure all calls are for valid guid - penalise if broken
        ActiveEquipment CreateActiveEquipmentInstance(int equipmentIdentifier);
        bool IsValidEquipmentLocation(int equipmentIdentifier, MountPoint mp);
    }
}