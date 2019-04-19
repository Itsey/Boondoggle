namespace Plisky.Boondoggle2.Runner {

    using Plisky.Boondoggle2;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    public class bd2XmlOutputter {
        private const int TURNS_BEFORE_WRITE = 500;

        private XElement eventsParentElement;
        private XElement staticInfoElement;
        private int ActiveTurn = 0;
        private int ActiveTick = 0;
        private string battleName;
        private string saveDir;
        private XDocument output;
        private static Dictionary<int, string> contestantNames = new Dictionary<int, string>();
        private Hub hub;

        public bd2XmlOutputter() {
            hub = Hub.Current;
            output = new XDocument();
        }

        public void Initialise(string bname) {
            saveDir = ConfigHub.Current.GetSetting("OutputDirectory", true);
            if (!Directory.Exists(saveDir)) {
                Directory.CreateDirectory(saveDir);
            }
            battleName = bname;
            output = CreateXDocument();
        }

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

        internal void StoreControlData(BattleRunnerControl brc) {
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

        public void InjectHub(Hub newhub) {
            hub = newhub;
        }

        private static string GetContestantName(int id) {
            return "Contestant : " + contestantNames[id];
        }

        private Action<Message_Ui> uim;
        private Action<Message_Game> msgg;
        private Action<Message_GameCombat> msggc;

        internal void RegisterForMessages() {
            uim = hub.LookFor<Message_Ui>(msg => {
                PerformUIMessage(msg);
            });

            msgg = hub.LookFor<Message_Game>(msg => {
                PerformGameMessage(msg);
            });

            msggc = hub.LookFor<Message_GameCombat>(msg => {
                PerformCombatMessage(msg);
            });
        }

        private void PerformCombatMessage(Message_GameCombat msg) {
            Console.WriteLine("Combat Message - " + msg.MessageKind.ToString() + " : " + msg.SubKind.ToString());
        }

        private void PerformUIMessage(Message_Ui msg) {
            string msgtxt = "Unknown UI Message ";
            bool writeToUI = true;

            switch (msg.SubKind) {
                case KnownSubkinds.BotEnterWorld:

                    //msg.ObjectIdentity
                    BotEnterWorldContext ctxtBew = (BotEnterWorldContext)msg.RequestContext;
                    contestantNames.Add(ctxtBew.ObjectId, ctxtBew.BotName);
                    msgtxt = GetContestantName(ctxtBew.ObjectId) + " ...... arrives";
                    //LogXmlContestantInfo(msg.ObjectIdentity, "displayName", msg.SParameter);
                    LogBotEnterWorld(msg.MessageKind, msg.SubKind, ctxtBew);
                    break;

                case KnownSubkinds.BotFanfareOccurred:
                    SystemMessageContext smc = (SystemMessageContext)msg.RequestContext;

                    msgtxt = GetContestantName(smc.BotId) + " says .... " + smc.Message;
                    //LogXmlBotTransmission("UI", msg.SubKind.ToString(), msg.ObjectIdentity, msg.SParameter);
                    LogBotMessage(msg.MessageKind, msg.SubKind, smc);
                    break;

                case KnownSubkinds.BotEndOccured:

                    BotEndContext bdc = (BotEndContext)msg.RequestContext;
                    if(bdc.Reason == BotEndReason.Depleted) {
                        msgtxt = GetContestantName(bdc.BotId) + " Is Depleted......";
                        LogXmlBotCombatEvent("UI", msg.SubKind.ToString(), bdc.BotId);
                    } else {
                        msgtxt = GetContestantName(bdc.BotId) + " DIES! ";
                        LogBotDeathEvent(msg.MessageKind, msg.SubKind, bdc);
                    }
                    break;


                case KnownSubkinds.WeaponFire:
                    var ctxt = (UICombatContext)msg.RequestContext;
                    string nm = GetContestantName(ctxt.AggressorId);
                    string vnm = GetContestantName(ctxt.VictimId);
                    int weaponId = ctxt.WeaponTypeId;
                    string endText = " but MISSES!  ";
                    if (ctxt.DidHit) {
                        endText = " and hits for " + ctxt.Damage.ToString() + "  ";
                    }
                    msgtxt = nm + " shoots at " + vnm + " W[" + weaponId.ToString() + "]" + endText;
                    LogXmlCombatEvent(msg.MessageKind, msg.SubKind, ctxt);
                    break;

                case KnownSubkinds.DirectionChange:
                    var nic = (NavigationInfoContext)msg.RequestContext;
                    msgtxt = string.Format(GetContestantName(nic.BotId) + " changes to direction " + nic.NewHeading.ToString());
                    writeToUI = false;
                    LogXmlNavigationEvent(msg.MessageKind, msg.SubKind, nic);
                    break;

                case KnownSubkinds.ChangeSpeed:
                    var nicspd = (NavigationInfoContext)msg.RequestContext;
                    msgtxt = string.Format(GetContestantName(nicspd.BotId) + " changes speed " + nicspd.SpeedDelta.ToString());
                    writeToUI = false;
                    LogXmlNavigationEvent(msg.MessageKind, msg.SubKind, nicspd);
                    break;

                case KnownSubkinds.EndGameStatus:
                    EndGameRequestContext egrc = (EndGameRequestContext)msg.RequestContext;
                    msgtxt = "End Game : " + egrc.EndGameDataDump;
                    break;

                case KnownSubkinds.BotStatus:
                    OutputBotStatusMessage(msg);
                    writeToUI = false;
                    break;
            }

            if (writeToUI) {
                msgtxt += "(" + msg.SubKind.ToString() + ")";
                SendOutputMessage(msgtxt);
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

        private void PerformGameMessage(Message_Game msg) {
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

        //private void LogXmlContestantInfo(int id, string propType, string propVal) {
        //    staticInfoElement.Add(new XElement("contestantInfo",
        //        new XElement("id-bot", id),
        //        new XElement(propType, propVal)
        //        ));
        //}

        private void SendOutputMessage(string msgtxt) {
            Console.WriteLine("Turn: " + ActiveTurn.ToString() + " >> " + msgtxt);
        }

        private void LogXmlSystemMessage(string eventType, string eventSubType, string p3) {
            var eventEl = GetEventElement(eventType, eventSubType);
            eventEl.Add(new XElement("msg", p3));
            eventsParentElement.Add(eventEl);
        }

        internal void AddSummary(string p1, int p2) {
            throw new NotImplementedException();
        }
    }
}