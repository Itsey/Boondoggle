using System;
using System.Collections.Generic;
using Plisky.Boondoggle2.Runner;

namespace Plisky.Boondoggle2.Test {
    internal class MockBdOutputter : BaseBdOutputter {
        public int UIMessageCount { get; set; }
        public int GameMessageCount { get; set; }
        public int CombatMessageCount { get; set; }

        protected override void ActualInitialise() {
            
        }

        internal string GetBattleName() {
            return this.battleName;
        }

        internal int GetTurn() {
            return this.ActiveTurn;
        }

        internal int GetTick() {
            return this.ActiveTick;
        }

        protected override void ActualPerformGameMessage(Message_Game msg) {
            GameMessageCount++;
        }

        protected override void ActualPerformCombatMessage(Message_GameCombat msg) {
            CombatMessageCount++;
        }

        protected override void ActualPerformBotEnterWorld_UI(Message_Ui msg, BotEnterWorldContext ctxtBew) {
            UIMessageCount++;
        }

        protected override void ActualEndGame_UI(MainMessageKind messageKind, KnownSubkinds subKind, EndGameRequestContext egrc) {
            UIMessageCount++;
        }

        protected override void ActualOutputBotStatusMessage_UI(Message_Ui msg) {
            UIMessageCount++;
        }

        protected override void ActualNavitationEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, NavigationInfoContext nicspd) {
            UIMessageCount++;
        }

        protected override void ActualWeaponFire_UI(MainMessageKind messageKind, KnownSubkinds subKind, UICombatContext ctxt) {
            UIMessageCount++;
        }

        protected override void ActualBotEndEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, BotEndContext bdc) {
            UIMessageCount++;
        }

        protected override void ActualOnBotMessage_UI(MainMessageKind messageKind, KnownSubkinds subKind, SystemMessageContext smc) {
            UIMessageCount++;
        }
    }
}