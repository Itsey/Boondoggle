namespace Plisky.Boondoggle2 {

    public enum KnownSubkinds {
        Undefined = 0,
        MockMessage = 1,
        // Game message subkinds
        BotPositionChange=10,

        GameCombatEvent=15,
        BattleStarts=20,
        TurnStart=25,
        TickStart=30,
        BattleEnds=35,

        //UI Message Subkinds
        BotEnterWorld=500,

        BotFanfareOccurred=510,

        //TODO : REMOVE BattleStarts,
        BotDeathOccured=520,
        BotEndOccured = 525,
        BotDepletedOccured =530,
        WeaponFire,
        TargetHit,
        CollisionOccured,
        BotStatus,
        EndGameStatus,

        // Query
        ReadSpeed=1010,

        ReadHeading=1020,
        DirectionChange=1030,


            //Action Message Kinds
        ChangeSpeed=2010,
        ChangeDirection=2020,
        InstallEquipment,
        UseEquipment,
        MockSubkind
    }
}