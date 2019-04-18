namespace Plisky.Boondoggle2 {

    public interface IKnowWhatBotsDo {

        int GetCurrentSpeed(BoonBotBase publicId);

        double GetCurrentHeading(BoonBotBase boonBotBase);
    }
}