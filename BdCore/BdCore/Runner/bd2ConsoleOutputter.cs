namespace Plisky.Boondoggle2.Runner {

    using Plisky.Boondoggle2;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    public class bd2ConsoleOutputter : BaseBdOutputter{
        public bd2ConsoleOutputter() {
                    
        }

        protected override void ActualInitialise() {
            Console.WriteLine("Initialise");
        }

      

        protected override void ActualPerformCombatMessage(Message_GameCombat msg) {
            Console.WriteLine("Combat Message - " + msg.MessageKind.ToString() + " : " + msg.SubKind.ToString());
        }


        protected override void ActualPerformBotEnterWorld_UI(Message_Ui msg, BotEnterWorldContext ctxtBew) {
            string msgtxt = GetContestantName(ctxtBew.ObjectId) + " ...... arrives";
            Console.WriteLine(msgtxt);            
        }

        protected override void ActualOnBotMessage_UI(MainMessageKind messageKind, KnownSubkinds subKind, SystemMessageContext smc) {
            string msgtxt = GetContestantName(smc.BotId) + " says .... " + smc.Message;
            Console.WriteLine(msgtxt);
        }

        protected override void ActualBotEndEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, BotEndContext bdc) {
            string msgtxt;
            if (bdc.Reason == BotEndReason.Depleted) {
                msgtxt = GetContestantName(bdc.BotId) + " Is Depleted......";                
            } else {
                msgtxt = GetContestantName(bdc.BotId) + " DIES! ";                
            }
            Console.WriteLine(msgtxt);
        }

        protected override void ActualWeaponFire_UI(MainMessageKind messageKind, KnownSubkinds subKind, UICombatContext ctxt) {
            string nm = GetContestantName(ctxt.AggressorId);
            string vnm = GetContestantName(ctxt.VictimId);
            int weaponId = ctxt.WeaponTypeId;
            string endText = " but MISSES!  ";
            if (ctxt.DidHit) {
                endText = " and hits for " + ctxt.Damage.ToString() + "  ";
            }
            string msgtxt = nm + " shoots at " + vnm + " W[" + weaponId.ToString() + "]" + endText;

            Console.WriteLine(msgtxt);
        }

        protected override void ActualNavitationEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, NavigationInfoContext nic) {
            string msgtxt;
            if (subKind== KnownSubkinds.ChangeDirection) {
                msgtxt = string.Format(GetContestantName(nic.BotId) + " changes to direction " + nic.NewHeading.ToString());
            } else {
                msgtxt = string.Format(GetContestantName(nic.BotId) + " changes speed " + nic.SpeedDelta.ToString());
            }
            Console.WriteLine(msgtxt) ;
        }

        protected override void ActualEndGame_UI(MainMessageKind messageKind, KnownSubkinds subKind, EndGameRequestContext egrc) {
            
            string msgtxt = "End Game : " + egrc.EndGameDataDump;
            Console.WriteLine(msgtxt);

        }

        protected override void ActualOutputBotStatusMessage_UI(Message_Ui msg) {
            //OutputBotStatusMessage(msg);
        }


        protected override void ActualPerformGameMessage(Message_Game msg) {
            switch (msg.SubKind) {
                case KnownSubkinds.BotPositionChange:
                    MapObjectPositionChangeContext ctxt = (MapObjectPositionChangeContext)msg.RequestContext;
                    //LogXmlGEMovement2(msg.MessageKind, msg.SubKind, ctxt);
                    break;

                case KnownSubkinds.TurnStart:
                case KnownSubkinds.TickStart:
                    GameStructureNotificationContext ctxt2 = (GameStructureNotificationContext)msg.RequestContext;
                    if (ctxt2.Turn % 250 == 0) {
                        Console.WriteLine("Turn " + ctxt2.Turn + "  battle still running");
                    }
                   // LogStructureMessage(msg.MessageKind, msg.SubKind, ctxt2);
                    break;

                case KnownSubkinds.BattleStarts:
                    SystemMessageContext ctxt4 = (SystemMessageContext)msg.RequestContext;
                   // LogSystemMessage(msg.MessageKind, msg.SubKind, ctxt4);
                    break;

                case KnownSubkinds.BattleEnds:
                   // CloseDownAndRecreateXDocument();
                    break;

                case KnownSubkinds.GameCombatEvent:
                default:
                    Console.WriteLine("UNLOGGED - Game Message - " + msg.MessageKind.ToString() + " : " + msg.SubKind.ToString());
                    break;
            }
        }



    }
}