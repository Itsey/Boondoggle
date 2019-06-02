namespace Plisky.Boondoggle2.Runner {

    using Plisky.Boondoggle2;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    public class bd2XmlOutputter : BaseBdOutputter{
        private const int TURNS_BEFORE_WRITE = 500;

        private XElement eventsParentElement;
        private XElement staticInfoElement;
                
        private string saveDir;
        private XDocument output;
       

        private XDocument CreateXDocument() {
            eventsParentElement = new XElement("events");
            var result = new XDocument(
                new XElement("bdBattle",
                new XElement("meta"),
                eventsParentElement));

            if (ActiveTurn == 0) {
                staticInfoElement = new XElement("staticInfo");
                result.Element("bdBattle").Add(staticInfoElement);
            } else {
                staticInfoElement = null;
            }

            return result;
        }


        public bd2XmlOutputter() {
        
            output = new XDocument();
        }

        protected override void ActualInitialise() {            
            saveDir = ConfigHub.Current.GetSetting("OutputDirectory", true);
            if (!Directory.Exists(saveDir)) {
                Directory.CreateDirectory(saveDir);
            }            
            output = CreateXDocument();
        }

    

        public void StoreControlData(BattleRunnerControl brc) {
            string s = brc.GetControlData();
            string controlFilename = Path.Combine(saveDir, battleName + ".control");
            File.WriteAllText(controlFilename, s);
        }

        private void CloseDownAndRecreateXDocument() {
            XDocument current = output;
            current.Save(Path.Combine(saveDir, GetFilename() + ".xml"));
            output = CreateXDocument();
        }

        private string GetFilename() {
            return battleName + string.Format("T{0}_{1}", ActiveTurn, ActiveTick);
        }



       
      

        protected override void ActualPerformCombatMessage(Message_GameCombat msg) {
            Console.WriteLine("Combat Message - " + msg.MessageKind.ToString() + " : " + msg.SubKind.ToString());
        }


        protected override void ActualPerformBotEnterWorld_UI(Message_Ui msg, BotEnterWorldContext ctxtBew) {
            LogBotEnterWorld(msg.MessageKind, msg.SubKind, ctxtBew);
        }

        protected override void ActualOnBotMessage_UI(MainMessageKind messageKind, KnownSubkinds subKind, SystemMessageContext smc) {
            LogBotMessage(messageKind, subKind, smc);
        }

        protected override void ActualBotEndEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, BotEndContext bdc) {
            string msgtxt;
            if (bdc.Reason == BotEndReason.Depleted) {
                msgtxt = GetContestantName(bdc.BotId) + " Is Depleted......";
                LogXmlBotCombatEvent("UI", subKind.ToString(), bdc.BotId);
            } else {
                msgtxt = GetContestantName(bdc.BotId) + " DIES! ";
                LogBotDeathEvent(messageKind, subKind, bdc);
            }
        }

        protected override void ActualWeaponFire_UI(MainMessageKind messageKind, KnownSubkinds subKind, UICombatContext ctxt) {
            LogXmlCombatEvent(messageKind, subKind, ctxt);
        }

        protected override void ActualNavitationEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, NavigationInfoContext nicspd) {
            LogXmlNavigationEvent(messageKind, subKind, nicspd);
        }

        protected override void ActualEndGame_UI(MainMessageKind messageKind, KnownSubkinds subKind, EndGameRequestContext egrc) {
         // do nothing
        }

        protected override void ActualOutputBotStatusMessage_UI(Message_Ui msg) {
            OutputBotStatusMessage(msg);
        }


        protected override void ActualPerformGameMessage(Message_Game msg) {
            switch (msg.SubKind) {
                case KnownSubkinds.BotPositionChange:
                    MapObjectPositionChangeContext ctxt = (MapObjectPositionChangeContext)msg.RequestContext;
                    LogXmlGEMovement2(msg.MessageKind, msg.SubKind, ctxt);
                    break;

                case KnownSubkinds.TurnStart:
                case KnownSubkinds.TickStart:
                    GameStructureNotificationContext ctxt2 = (GameStructureNotificationContext)msg.RequestContext;
                    if (ctxt2.Turn % 250 == 0) {
                        Console.WriteLine("Turn " + ctxt2.Turn + "  battle still running");
                    }
                    LogStructureMessage(msg.MessageKind, msg.SubKind, ctxt2);
                    break;

                case KnownSubkinds.BattleStarts:
                    SystemMessageContext ctxt4 = (SystemMessageContext)msg.RequestContext;
                    LogSystemMessage(msg.MessageKind, msg.SubKind, ctxt4);
                    break;

                case KnownSubkinds.BattleEnds:
                    CloseDownAndRecreateXDocument();
                    break;

                case KnownSubkinds.GameCombatEvent:
                default:
                    Console.WriteLine("UNLOGGED - Game Message - " + msg.MessageKind.ToString() + " : " + msg.SubKind.ToString());
                    break;
            }
        }


        private void LogXmlNavigationEvent(MainMessageKind mainMessageKind, KnownSubkinds uIMessageSubKind, NavigationInfoContext nic) {
            var el = GetEventElement(mainMessageKind.ToString(), uIMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(nic.GetType());
                sr.WriteObject(w, nic);
            }
            eventsParentElement.Add(el);
        }

        private void LogBotDeathEvent(MainMessageKind mainMessageKind, KnownSubkinds uIMessageSubKind, BotEndContext bdc) {
            var el = GetEventElement(mainMessageKind.ToString(), uIMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(bdc.GetType());
                sr.WriteObject(w, bdc);
            }
            eventsParentElement.Add(el);
        }

        private void LogBotEnterWorld(MainMessageKind mainMessageKind, KnownSubkinds uIMessageSubKind, BotEnterWorldContext ctxtBew) {
            var el = GetEventElement(mainMessageKind.ToString(), uIMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(ctxtBew.GetType());
                sr.WriteObject(w, ctxtBew);
            }
            eventsParentElement.Add(el);
        }

    
        private void LogBotMessage(MainMessageKind mainMessageKind, KnownSubkinds uIMessageSubKind, SystemMessageContext smc) {
            var el = GetEventElement(mainMessageKind.ToString(), uIMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(smc.GetType());
                sr.WriteObject(w, smc);
            }
            eventsParentElement.Add(el);
        }

        private void LogSystemMessage(MainMessageKind mainMessageKind, KnownSubkinds gameMessageSubKind, SystemMessageContext ctxt4) {
            var el = GetEventElement(mainMessageKind.ToString(), gameMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(ctxt4.GetType());
                sr.WriteObject(w, ctxt4);
            }
            eventsParentElement.Add(el);
        }

        private void LogXmlGEMovement2(MainMessageKind mainMessageKind, KnownSubkinds gameMessageSubKind, MapObjectPositionChangeContext context) {
            var el = GetEventElement(mainMessageKind.ToString(), gameMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(context.GetType());
                sr.WriteObject(w, context);
            }
            eventsParentElement.Add(el);
        }

        private void LogXmlBotStatusEvent2(MainMessageKind mainMessageKind, KnownSubkinds uIMessageSubKind, BotStatusRequestContext ctxt) {
            var el = GetEventElement(mainMessageKind.ToString(), uIMessageSubKind.ToString());

            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(ctxt.GetType());
                sr.WriteObject(w, ctxt);
            }

            eventsParentElement.Add(el);
        }

        private void LogStructureMessage(MainMessageKind mainMessageKind, KnownSubkinds gameMessageSubKind, GameStructureNotificationContext ctxt2) {
            var el = GetEventElement(mainMessageKind.ToString(), gameMessageSubKind.ToString());

            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(ctxt2.GetType());
                sr.WriteObject(w, ctxt2);
            }

            eventsParentElement.Add(el);
            ActiveTurn = ctxt2.Turn;
            ActiveTick = ctxt2.Tick;
            if (gameMessageSubKind == KnownSubkinds.TurnStart) {
                if ((ctxt2.Turn > 0) && (ctxt2.Turn % TURNS_BEFORE_WRITE == 0)) {
                    CloseDownAndRecreateXDocument();
                }
            }
        }

        private void LogXmlGEMovement(MainMessageKind mainMessageKind, KnownSubkinds gameMessageSubKind, int objId, Point point) {
            var el = GetEventElement(mainMessageKind.ToString(), gameMessageSubKind.ToString());
            el.Add(new XElement("pt-x", point.X),
                new XElement("pt-y", point.Y),
                new XElement("id-bot1", objId));
            eventsParentElement.Add(el);
        }

        private void OutputBotStatusMessage(Message_Ui msg) {
            BotStatusRequestContext ctxt = (BotStatusRequestContext)msg.RequestContext;
            LogXmlBotStatusEvent2(msg.MessageKind, msg.SubKind, ctxt);
        }

        private void LogXmlCombatEvent(MainMessageKind mainMessageKind, KnownSubkinds gameMessageSubKind, UICombatContext context) {
            var el = GetEventElement(mainMessageKind.ToString(), gameMessageSubKind.ToString());
            using (var w = el.CreateWriter()) {
                var sr = new DataContractSerializer(context.GetType());
                sr.WriteObject(w, context);
            }
            eventsParentElement.Add(el);
        }

        private XElement GetEventElement(string eventType, string eventSubType) {
            return new XElement("event",
                new XAttribute("type", eventType),
                new XAttribute("subtype", eventSubType));
        }

        private void LogXmlBotCombatEvent(string eventType, string eventSubType, int p3) {
            var eventEl = GetEventElement(eventType, eventSubType);
            eventsParentElement.Add(eventEl);
        }

        private void LogXmlBotTransmission(string eventType, string eventSubType, int botId, string messageToSend) {
            var eventEl = GetEventElement(eventType, eventSubType);
            eventEl.Add(new XElement("id-bot1", botId),
                new XElement("message", messageToSend));
            eventsParentElement.Add(eventEl);
        }


        private void SendOutputMessage(string msgtxt) {
            Console.WriteLine("Turn: " + ActiveTurn.ToString() + " >> " + msgtxt);
        }

        private void LogXmlSystemMessage(string eventType, string eventSubType, string p3) {
            var eventEl = GetEventElement(eventType, eventSubType);
            eventEl.Add(new XElement("msg", p3));
            eventsParentElement.Add(eventEl);
        }

    }
}