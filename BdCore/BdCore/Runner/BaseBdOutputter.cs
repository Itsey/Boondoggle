using System;
using System.Collections.Generic;

namespace Plisky.Boondoggle2.Runner {
    public abstract class BaseBdOutputter : bd2BaseModel {
        protected string battleName;
        protected int ActiveTurn = 0;
        protected int ActiveTick = 0;
        protected Dictionary<int, string> contestantNames = new Dictionary<int, string>();

        private Action<Message_Ui> uim;
        private Action<Message_Game> msgg;
        private Action<Message_GameCombat> msggc;


        protected string GetContestantName(int id) {
            return "Contestant : " + contestantNames[id];
        }




        protected abstract void ActualInitialise();
     
        protected abstract void ActualPerformGameMessage(Message_Game msg);
        protected abstract void ActualPerformCombatMessage(Message_GameCombat msg);

        public override void RegisterMessages() {
            if (needToRegister) {
                base.RegisterMessages();

                uim = hub.LookFor<Message_Ui>(msg => {
                    PerformUIMessage(msg);
                });

                msgg = hub.LookFor<Message_Game>(msg => {
                    ActualPerformGameMessage(msg);
                });

                msggc = hub.LookFor<Message_GameCombat>(msg => {
                    ActualPerformCombatMessage(msg);
                });
            }
        }


        protected abstract void ActualPerformBotEnterWorld_UI(Message_Ui msg, BotEnterWorldContext ctxtBew);
        protected abstract void ActualEndGame_UI(MainMessageKind messageKind, KnownSubkinds subKind, EndGameRequestContext egrc);

        protected abstract void ActualOutputBotStatusMessage_UI(Message_Ui msg);
        protected abstract void ActualNavitationEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, NavigationInfoContext nicspd);

        protected abstract void ActualWeaponFire_UI(MainMessageKind messageKind, KnownSubkinds subKind, UICombatContext ctxt);

        protected abstract void ActualBotEndEvent_UI(MainMessageKind messageKind, KnownSubkinds subKind, BotEndContext bdc);

        protected abstract void ActualOnBotMessage_UI(MainMessageKind messageKind, KnownSubkinds subKind, SystemMessageContext smc);

        protected void PerformUIMessage(Message_Ui msg) {

            switch (msg.SubKind) {
                case KnownSubkinds.BotEnterWorld:
                    BotEnterWorldContext ctxtBew = (BotEnterWorldContext)msg.RequestContext;
                    contestantNames.Add(ctxtBew.ObjectId, ctxtBew.BotName);
                    ActualPerformBotEnterWorld_UI(msg, ctxtBew);
                    break;

                case KnownSubkinds.BotFanfareOccurred:
                    SystemMessageContext smc = (SystemMessageContext)msg.RequestContext;
                    ActualOnBotMessage_UI(msg.MessageKind, msg.SubKind, smc);
                    break;

                case KnownSubkinds.BotEndOccured:
                    BotEndContext bdc = (BotEndContext)msg.RequestContext;
                    ActualBotEndEvent_UI(msg.MessageKind, msg.SubKind, bdc);
                    break;

                case KnownSubkinds.WeaponFire:
                    var ctxt = (UICombatContext)msg.RequestContext;
                    ActualWeaponFire_UI(msg.MessageKind, msg.SubKind, ctxt);
                    break;

                case KnownSubkinds.DirectionChange:
                case KnownSubkinds.ChangeSpeed:
                    var nicspd = (NavigationInfoContext)msg.RequestContext;
                    ActualNavitationEvent_UI(msg.MessageKind, msg.SubKind, nicspd);
                    break;

                case KnownSubkinds.EndGameStatus:
                    EndGameRequestContext egrc = (EndGameRequestContext)msg.RequestContext;
                    ActualEndGame_UI(msg.MessageKind, msg.SubKind, egrc);
                    break;

                case KnownSubkinds.BotStatus:
                    ActualOutputBotStatusMessage_UI(msg);                    
                    break;
            }
            
        }

        

        protected override void OnHubChanged() {
            base.OnHubChanged();
            RegisterMessages();
        }

        public void Initialise(string bname) {
            battleName = bname;
            ActualInitialise();
            RegisterMessages();
        }

        
    }
}