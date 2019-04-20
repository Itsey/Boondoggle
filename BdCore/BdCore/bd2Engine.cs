namespace Plisky.Boondoggle2 {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;

    public class bd2Engine : bd2BaseModel {
        
        public bool CanBattleStart { get; private set; }

        public bool BattleActive { get; private set; }

        public bool HavePrepared { get; private set; }

        public int AliveBotCount() {
            int remainingBots = 0;

            EachBot(bt => {
                if (bt.IsAlive()) {
                    remainingBots++;
                }
            });
            return remainingBots;
        }

        public int Turn { get; set; }

        public int Tick { get; set; }

        public bool HasActiveWorld {
            get {
                return activeWorld != null;
            }
        }

        // Protected to allow Mock version to access and manipulate the data.
        protected bd2World activeWorld;

        protected List<MappedObject> mappedObjects = new List<MappedObject>();
        protected List<BotManagementReference> botReferences = new List<BotManagementReference>();
        protected IKnowWhatBotsDo botSupportQuery;
        protected IProvideBotInteractivity botSupportActivity;
        protected EquipmentSupport equipment;
        protected List<ActiveEquipment> installedEquipment = new List<ActiveEquipment>();
        protected ActiveTurnData activeData = null;
        protected CombatManager combatCore = null;

        private bd2MessageBasedBotSupport defaultSupport;
        private bool HasSupportBeenInjected = false;

        public void InjectBotSupport(IKnowWhatBotsDo query = null, IProvideBotInteractivity activity = null) {
            HasSupportBeenInjected = true;
            if (query == null) {
                b.Verbose.Log("Creating internal message support");
                defaultSupport = new bd2MessageBasedBotSupport();
                botSupportQuery = defaultSupport;
                botSupportActivity = defaultSupport;
                defaultSupport.InjectHub(hub);
            } else {
                botSupportQuery = query;
                botSupportActivity = activity;
            }
        }

        private MappedBot CreateMappedBot(BoonBotBase bot) {
            var result = new MappedBot(bot);
            result.Position = Point.Empty;
            result.IsActive = false;
            mappedObjects.Add(result);
            CreateNewBotMamagementReference(bot, result.EngineId);
            return result;
        }

        private void CreateNewBotMamagementReference(BoonBotBase bot, int p) {
            BotManagementReference bmr = new BotManagementReference();
            bmr.EngineIdentity = p;
            bmr.PublicIdentity = Guid.NewGuid();
            bmr.DirectReference = bot;
            botReferences.Add(bmr);
        }

        public bd2Engine() {
            b.Info.Log("bd2Engine online.");
        }

        public void AddWorld(bd2World desiredWorld, Bd2CombatCalculator rndManager = null) {
            activeWorld = desiredWorld;
            if (rndManager == null) {
                rndManager = new DefaultCalcsRules();
            }
            combatCore = new CombatManager(activeWorld, rndManager);
        }



        private Guid GetPublicIdFromPrivateId(int p) {
            foreach (var v in botReferences) {
                if (v.EngineIdentity == p) {
                    return v.PublicIdentity;
                }
            }
            b.Verbose.Log("Failed to locate bot by engine ID, returning empty ID. Engine ID: " + p.ToString());
            return Guid.Empty;
        }

        public MappedBot GetMappedBotById(int engineIdentity) {
            return (MappedBot)GetMappedObjectById(engineIdentity);
        }

        public MappedBot GetMappedBotByPublicId(Guid publicId) {
            b.Verbose.Log("Trying to locate bot by public id:" + publicId.ToString());
            foreach (var v in botReferences) {
                if (v.PublicIdentity == publicId) {
                    var mappedBot = GetMappedObjectById(v.EngineIdentity);
                    b.Assert.True(mappedBot != null, "The mapped bot cant be null when we have already found it.");
                    return (MappedBot)mappedBot;
                }
            }

            b.Verbose.Log("Failed to locate bot by public ID, returning null. Public ID " + publicId.ToString());

#if DEBUG
            b.Verbose.Log("Extended  Diagnostics follow:");
            foreach (var v in botReferences) {
                b.Verbose.Log(string.Format("EngineId [{0}] - PublicId [{1}] - Name [{2}]", v.EngineIdentity, v.PublicIdentity, v.DirectReference.Name));
            }
#endif

            return null;
        }

        public MappedObject GetMappedObjectById(int objectId) {
            foreach (var v in mappedObjects) {
                if (v.EngineId == objectId) {
                    return v;
                }
            }
            return null;
        }

        public int AddBot(BoonBotBase bot) {
            if (activeWorld == null) {
                throw new BdBaseException("No world loaded");
            }

            if (botReferences.Count >= activeWorld.Map.MaxSupportedBots) {
                throw new BdBaseException("Too many bots");
            }

            MappedBot mo = CreateMappedBot(bot);
            this.CanBattleStart = true;

            return mo.EngineId;
        }

        public void StartBattle() {
            CheckForSupportInjection();
            b.Assert.True(botSupportActivity != null, "Engine/Start Battle - You can not start a battle without an form of BotSupport");
            b.Assert.True(botSupportQuery != null, "Engine/Start Battle - You can not start a battle without an form of BotSupport");

            EachBot((mappedBot) => {
                Guid g = GetPublicIdFromPrivateId(mappedBot.EngineId);
                b.Info.Log("Preparing Bot " + mappedBot.Bot.Name + " Granting Guid: " + g.ToString());
                PerformEngineInitOnBot(mappedBot);
                mappedBot.Bot.PrepareForBattle(botSupportActivity, botSupportQuery, g);

                PerformBotInitialisation(mappedBot);
                SystemMessageContext smc = new SystemMessageContext();
                smc.BotId = mappedBot.EngineId;
                smc.Message = mappedBot.Bot.FanfareMessage;

                hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.BotFanfareOccurred) {
                    RequestContext = smc,
                });
            });
            // Defaults allow the main loop to tick over to turn 1 tick 0 on first increment.
            Turn = 0;
            Tick = 10;
            HavePrepared = true;
        }

        private void PerformBotInitialisation(MappedBot mappedBot) {
            b.Info.Log("Preparing Bot " + mappedBot.Bot.Name);
            b.Assert.True(equipment != null, "An Equipment support must be available before bots are initialised");

            var pp = mappedBot.Bot.GetPowerPack();
            if (pp == null) {
                throw new BdBaseException("All bots must have a powerpack.");
            }
            var powerPackTemplate = (PowerPackEquipmentItem)equipment.GetEquipmentTypeById(pp.EquipmentId);
            mappedBot.PowerRemaining = powerPackTemplate.TotalPower;
            mappedBot.ChargeRemaining = powerPackTemplate.ChargePerTurn;
            mappedBot.TurnsAccelerationActionsRemaining = powerPackTemplate.Acceleration;
        }

        private void CheckForSupportInjection() {
            if (!HasSupportBeenInjected) {
                InjectBotSupport();
            }
        }

        private void PerformEngineInitOnBot(MappedBot mo) {
            SetBotInitialValues(mo);

            b.Verbose.Log("Lanching engine init UI messages, engine informing UI of position etc");

            BotEnterWorldContext bewC = new BotEnterWorldContext();
            bewC.BotName = mo.Bot.Name;
            bewC.ObjectId = mo.EngineId;
            bewC.BotVersion = mo.Bot.Version;

            hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.BotEnterWorld) {
                RequestContext = bewC,
            });

            MapObjectPositionChangeContext mopcc = new MapObjectPositionChangeContext();
            mopcc.Destination = mo.Position;
            mopcc.ObjectIdentity = mo.EngineId;
            hub.Launch<Message_Game>(new Message_Game(MainMessageKind.MapObjectMovementChange, KnownSubkinds.BotPositionChange) {
                RequestContext = mopcc
            });

#if DEBUG
            b.Info.Log("Extended diagnostics, checking to see whether one bot sits on another one");
            if (!activeWorld.IsFreeWorldSpace(mo.Position)) {
                b.Info.Log("Object " + mo.EngineId.ToString() + " on top of boundary at " + mo.Position.ToString());
                throw new BdBaseException("Mapped object on top of world boundary (" + mo.Position.ToString() + "), fault.");
            }

            foreach (var v in mappedObjects) {
                if (v.EngineId == mo.EngineId) { continue; }

                if (v.Position == mo.Position) {
                    throw new BdBaseException("Cant add a bot on top of an existing mapped object");
                }
            }
#endif
        }

        private void SetBotInitialValues(MappedBot mo) {
            mo.LifeRemaining = 100;
            mo.Position = activeWorld.ReturnNextStartLocation();
            mo.Speed = 0;
            mo.Heading = 0;
            mo.IsActive = true;
            mo.ChargeRemaining = 0;
            mo.PowerRemaining = 0;
        }

        public void ShutdownBattle() {
            BattleActive = false;
            PerformBattleShutdownEvents();
        }

        public void PerformNextTick() {
            if (!HavePrepared) {
                throw new BdBaseException("Must prepare before performing next tick");
            }


            WriteDiagnosticTurnSummary();  // Conditional, debug only.


            ManageDynamicTurnData_Pre();

            PerformBotPerTickManipulation();

            Tick = (Tick + 1) % 11;
            LaunchTurnNotification();

            if (Tick == 0) {
                if (Turn == 0) {
                    BattleStarts();
                }
                b.Info.Log("New Turn starts, creating activeData");
                Turn++; Tick++;

                // TODO : UNIT TEST MAKE SURE IT GOES 1,1  1,2,  1,3  --> TakeActionOnEachBot(activeData);
                activeData = new ActiveTurnData();

                PerformBotPerTurnManipulation();
            }

            PerformMovementForTick();
            DEBUG_CheckForDeadBots();
            CheckEndCondition();
            if (!BattleActive) {
                PerformBattleShutdownEvents();
                return;
            }
            TakeActionOnEachBot(activeData);

            SendBotStatusNotifications();
            ManageDynamicTurnData_Post();
        }


        private void PerformBotPerTurnManipulation() {
            EachActiveBot(bt => {
                var pp = (PowerPackEquipmentItem)equipment.GetEquipmentTypeById(bt.Bot.GetPowerPack().EquipmentId);
                bt.TurnsAccelerationActionsRemaining = pp.Acceleration;
            });
        }

        private void PerformBattleShutdownEvents() {
            string data = "Bots:" + Environment.NewLine;
            EachBot(bt => {
                data += "Bot - " + bt.EngineId.ToString() + " : " + bt.Bot.PublicId.ToString() + " : " + bt.Bot.Name + Environment.NewLine;
            });
            data += "Equipment: " + Environment.NewLine;
            foreach (var v in installedEquipment) {
                data += "Bot:  " + v.OwningBotId.ToString() + " -- " + v.EquipmentId.ToString() + " U: " + v.UseCount.ToString() + "   " + Environment.NewLine;
            }

            EndGameRequestContext egrq = new EndGameRequestContext();
            egrq.EndGameDataDump = data;

            hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.EndGameStatus) {
                RequestContext = egrq
            });

            hub.Launch<Message_Game>(new Message_Game(MainMessageKind.GameStructure, KnownSubkinds.BattleEnds));
        }

        private void SendBotStatusNotifications() {
            EachBot(nxt => {
                Message_Ui statusMessage = new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.BotStatus);
                BotStatusRequestContext ctxt = new BotStatusRequestContext();
                ctxt.CurrentCharge = nxt.ChargeRemaining;
                ctxt.Id = nxt.EngineId;
                ctxt.Power = nxt.PowerRemaining;

                statusMessage.RequestContext = ctxt;
                hub.Launch<Message_Ui>(statusMessage);
            });
        }

        private void LaunchTurnNotification() {
            GameStructureNotificationContext gsnc = new GameStructureNotificationContext();
            gsnc.Tick = Tick;
            gsnc.Turn = Turn;

            if (Tick == 0) {
                hub.Launch<Message_Game>(new Message_Game(MainMessageKind.GameStructure, KnownSubkinds.TurnStart) {
                    RequestContext = gsnc,
                });
            }
            hub.Launch<Message_Game>(new Message_Game(MainMessageKind.GameStructure, KnownSubkinds.TickStart) {
                RequestContext = gsnc,
            });
        }

        private void ManageDynamicTurnData_Pre() {
            if (Tick == 11) {
                ActiveTurnData atd = new ActiveTurnData();
                atd.LastTickRecords = new Dictionary<int, LastTickRecord>();
                foreach (int v in activeData.LastTickRecords.Keys) {
                    atd.LastTickRecords.Add(v, activeData.LastTickRecords[v]);
                }
            }
        }

        private void ManageDynamicTurnData_Post() {
        }

        private void BattleStarts() {
            BattleActive = true;
            SystemMessageContext smc = new SystemMessageContext();
            smc.BotId = -1;
            smc.Message = "Let Battle Commence....";
            hub.Launch<Message_Game>(new Message_Game(MainMessageKind.GameStructure, KnownSubkinds.BattleStarts) {
                RequestContext = smc
            });

            activeData = new ActiveTurnData();
        }

        private void PerformBotPerTickManipulation() {
            EachActiveBot(bt => {
                var pp = (PowerPackEquipmentItem)equipment.GetEquipmentTypeById(bt.Bot.GetPowerPack().EquipmentId);
                SpeedRating sr = GetSpeedRating(bt.Speed);
                int speedBasedPowerDrain = 1;
                if (sr == SpeedRating.Stopped) {
                    speedBasedPowerDrain = 0;
                } else {
                    speedBasedPowerDrain = pp.GetPowerDrain(sr);
                }
                int drainForSpeed = BdConstants.EnergyDrainBaseValue * speedBasedPowerDrain;
                //bt.PowerRemaining -= 10;  // Hardcoded drain so at least some drain occurs....

                int chargeDifference = pp.ChargePerTurn - bt.ChargeRemaining;
                bt.PowerRemaining -= chargeDifference;
                bt.PowerRemaining -= drainForSpeed;
                if (!CheckForDepletedBot(bt)) {
                    bt.ChargeRemaining = pp.ChargePerTurn;
                }
            });
        }

        private SpeedRating GetSpeedRating(int p) {
            if (p == 0) {
                return SpeedRating.Stopped;
            }
            if (p <= 5) {
                return SpeedRating.Slow;
            }
            if (p <= 8) {
                return SpeedRating.Medium;
            }
            return SpeedRating.Fast;
        }

        private bool CheckForDepletedBot(MappedBot bt) {
            if (bt.PowerRemaining <= 0) {
                DeactivateBot(bt);
                BotEndContext bec = new BotEndContext();
                bec.BotId = bt.EngineId;
                bec.Reason = BotEndReason.Depleted;

                hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.BotEndOccured) {
                    //ObjectIdentity = bt.EngineId
                    RequestContext = bec
                });
                return true;
            }
            return false;
        }

        private void DeactivateBot(MappedBot bt) {
            bt.IsActive = false;
            bt.Speed = 0;
            bt.ChargeRemaining = 0;
        }

        [Conditional("Debug")]
        private void WriteDiagnosticTurnSummary() {
            string tt = "Turn " + Turn.ToString() + "  Tick  " + Tick.ToString();
            string diags = string.Empty;

            EachBot(nxt => {
                diags += nxt.Bot.Name + " @" + nxt.Position.ToString() + " S:" + nxt.Speed.ToString() + " H:" + nxt.Heading.ToString() + Environment.NewLine +
                    " Li:" + nxt.LifeRemaining.ToString() + "Pw: " + nxt.PowerRemaining + " Cg: " + nxt.ChargeRemaining + Environment.NewLine;
            });
            b.Info.Log(tt, diags);

            DumpWorld();
        }

        public void DumpWorld() {
            List<string> matches = new List<string>();
            int noMatches = 0;
            b.Info.Log("WORLD DUMP");
            for (int y = activeWorld.Map.Height; y >= 1; y--) {
                string nextLine = string.Empty;
                for (int x = 1; x <= activeWorld.Map.Width; x++) {
                    var cp = new Point(x, y);

                    foreach (var n in mappedObjects) {
                        if (n.Position == cp) {
                            noMatches++;
                            matches.Add(string.Format("{0} [{1}] : H: {1} S: {2}", noMatches, n.EngineId, n.Heading, n.Speed));
                            nextLine += noMatches.ToString();
                            continue;
                        }
                    }
                    var v = activeWorld.Map.GetTileAtPosition(cp);

                    if (v == MapTile.BoundaryWall1) {
                        nextLine += "_";
                    } else {
                        nextLine += "+";
                    }
                }

                //b.Info.FurtherInfo(nextLine);
            }
            foreach (string z in matches) {
                b.Info.Log(z);
            }
            b.Info.Log("DONE");
        }

        private void CheckEndCondition() {
            int activeBots = 0;

            EachBot((mb) => {
                if (mb.IsActive) {
                    activeBots++;
                }
            });

            if (activeBots <= 1) {
                b.Info.Log("There are only one or more bots remaining - map end game type: " + activeWorld.Map.MapType.ToString());
                if ((activeWorld.Map.MapType == MapConditionType.LastBotStanding)) {
                    BattleActive = false;
                }
                if (activeBots == 0) {
                    b.Info.Log("No Bots Remaining - map end game type: " + activeWorld.Map.MapType.ToString());
                    BattleActive = false;
                }
            }
        }

        [Conditional("DEBUG")]
        private void DEBUG_CheckForDeadBots() {
            EachBot((mappedBot) => {
                if (!mappedBot.IsAlive()) {
                    if ((mappedBot.IsActive) && (!mappedBot.DeathNotificationOccured)) {
                        b.Warning.Log("Bot is dead, removing. - THIS SHOULD NOT OCCUR - " + mappedBot.EngineId);
                        PerformBotDeath(mappedBot, BotEndReason.Unknown_Debug);
                    }
                }
            });
        }

        private void PerformMovementForTick() {
            List<MappedObjectPotentialMove> potentialMoves = new List<MappedObjectPotentialMove>();
            EachActiveBot(bt => {
                MappedObjectPotentialMove res;
                if (SpeedTriggersMoveForTick(Tick, bt.Speed)) {
                    res = MoveMappableObject(bt);
                } else {
                    res = new MappedObjectPotentialMove(bt);
                    res.DesiredPosition = bt.Position;
                }
                potentialMoves.Add(res);
            });

            List<string> collisionsAlreadyRecorded = new List<string>();

            b.Verbose.Log("All potential moves are recorded, resolving collisions");
            foreach (var v in potentialMoves) {
                if (v.HasBoundaryCollision) {
                    ApplyCollision(v);
                } else {
                    foreach (var p in potentialMoves) {
                        if (v.Underlying.EngineId == p.Underlying.EngineId) { continue; }

                        b.Assert.True(v.Underlying.Position != p.Underlying.Position, "Two objects sat on top of each other problem.  [" + v.Underlying.Position.ToString() + "][" + v.Underlying.Position.ToString() + "]");

                        if ((v.DesiredPosition == p.DesiredPosition) || (v.Underlying.Position == p.DesiredPosition) || (v.DesiredPosition == p.Underlying.Position)) {
                            string key = CreateKeyForCollision(v, p);

                            if (!collisionsAlreadyRecorded.Contains(key)) {
                                collisionsAlreadyRecorded.Add(key);
                                ApplyCollision(v, p);
                            }
                        }
                    }
                }
            }

            b.Verbose.Log("Move potentials done, finalising");
            foreach (var move in potentialMoves) {
                if (move.Underlying.Position != move.DesiredPosition) {
                    move.Underlying.Position = move.DesiredPosition;
                    MapObjectPositionChangeContext ctxt = new MapObjectPositionChangeContext();
                    ctxt.ObjectIdentity = move.Underlying.EngineId;
                    ctxt.Destination = move.DesiredPosition;

                    bd2TickAction act = new bd2TickAction();
                    act.ActionType = LastTickEventType.Moved;
                    CreateNextTurnNotificationMessage(move.Underlying.EngineId, act);

                    hub.Launch<Message_Game>(new Message_Game(MainMessageKind.MapObjectMovementChange, KnownSubkinds.BotPositionChange) {
                        RequestContext = ctxt
                    });
                }
            }
        }

        private string CreateKeyForCollision(MappedObjectPotentialMove moveOne, MappedObjectPotentialMove moveTwo) {
            if (moveTwo.Underlying.EngineId < moveOne.Underlying.EngineId) {
                return moveTwo.Underlying.EngineId.ToString() + moveOne.Underlying.EngineId.ToString();
            } else {
                return moveOne.Underlying.EngineId.ToString() + moveTwo.Underlying.EngineId.ToString();
            }
        }

        private void ApplyCollision(MappedObjectPotentialMove moveOne, MappedObjectPotentialMove moveTwo) {
            if ((moveOne.DesiredPosition == moveTwo.DesiredPosition) && (moveOne.Underlying.Speed > 0) && (moveTwo.Underlying.Speed > 0)) {
                b.Verbose.Log("Two bots moving into same space collision - checking speeds @ " + moveOne.DesiredPosition.ToString());
                if (moveOne.Underlying.Speed > moveTwo.Underlying.Speed) {
                    moveTwo.DesiredPosition = moveTwo.Underlying.Position;
                } else {
                    moveOne.DesiredPosition = moveOne.Underlying.Position;
                }
            } else {
                b.Verbose.Log("Passthrough collision (one passes through others current loc) - no one moves @ " + moveOne.DesiredPosition.ToString());
                moveOne.DesiredPosition = moveOne.Underlying.Position;
                moveTwo.DesiredPosition = moveTwo.Underlying.Position;
            }

            ApplyCollision(moveOne, false);
            ApplyCollision(moveTwo, false);
        }

        private void ApplyCollision(MappedObjectPotentialMove v, bool isBoundary = true) {
            v.HasBoundaryCollision = isBoundary;
            v.HasCollided = true;

            NotifyOfCollision(v);

            v.Underlying.Speed = 0;
            v.DesiredPosition = v.Underlying.Position;
            // TODO : Unit test, confirm that on a collision the object does not move on (into the boundary wall);

            ApplyDamageToObject(v.Underlying.EngineId, BdConstants.CollisionDamageBaseValue, DamageType.Collision);
        }

        private void NotifyOfCollision(MappedObjectPotentialMove v) {
            hub.Launch<Message_GameCombat>(new Message_GameCombat(MainMessageKind.CombatMessage, KnownSubkinds.CollisionOccured));
            hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.CollisionOccured));

            bd2TickAction act = new bd2TickAction();
            act.ActionType = LastTickEventType.Collision;
            CreateNextTurnNotificationMessage(v.Underlying.EngineId, act);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "damageType")]
        private void ApplyDamageToObject(int botEngineId, int damageAmount, DamageType damageType) {
            var bt = GetMappedBotById(botEngineId);
            bt.LifeRemaining -= damageAmount;
            if (!bt.IsAlive()) {
                BotEndReason rsn;

                switch (damageType) {
                    case DamageType.Collision:
                        rsn = BotEndReason.Collision;
                        break;

                    case DamageType.Projectile:
                        rsn = BotEndReason.Shot;
                        break;

                    default:
                        throw new InvalidOperationException("Unvalid reason");
                }

                PerformBotDeath(bt, rsn);
            }
        }

        private void PerformBotDeath(MappedBot bt, BotEndReason bdr) {
            b.Assert.True(!bt.DeathNotificationOccured, "You shouldn't notify of a bots death twice.  Notification for " + bt.Bot.Name + " already sent");
            bt.DeathNotificationOccured = true;

            BotEndContext bdc = new BotEndContext();
            bdc.BotId = bt.EngineId;
            bdc.Reason = bdr;

            hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.BotEndOccured) {
                RequestContext = bdc
            });
            bt.Speed = 0;
            bt.IsActive = false;
        }

        private static bool SpeedTriggersMoveForTick(int tt, int speed) {
            if (speed == 0) { return false; }
            if (speed == 10) { return true; }

            return speed >= tt;
        }

        private MappedObjectPotentialMove MoveMappableObject(MappedObject v) {
            b.Assert.True(activeWorld.IsFreeWorldSpace(v.Position), "The bot should not be in a non free space");

            MappedObjectPotentialMove result = new MappedObjectPotentialMove(v);
            result.DesiredPosition = activeWorld.CalculateNextPositionForObject(v.Position, v.Heading);
            if (!activeWorld.IsFreeWorldSpace(result.DesiredPosition)) {
                result.HasBoundaryCollision = true;
            }
            return result;
        }

        private void TakeActionOnEachBot(ActiveTurnData turnData) {
            if (BattleActive) {
                // TODO: CHECK INACTIVE and DEAD BOTS DONT GET TURNS (unit test)
                // TODO : UNIT TEST, bot that thorws exception is killed
                EachActiveBot((mappedBot) => {
                    LastTickRecord ltr;
                    if (turnData.LastTickRecords.ContainsKey(mappedBot.EngineId)) {
                        ltr = turnData.LastTickRecords[mappedBot.EngineId];
                        turnData.LastTickRecords.Remove(mappedBot.EngineId);
                    } else {
                        ltr = new LastTickRecord();
                    }
                    try {
                        mappedBot.Bot.TakeAction(Turn, Tick, ltr);
                    } catch (Exception ex) {
                        b.Info.Dump(ex, "Exception in BOT" + mappedBot.Bot.Name);
                        PerformBotDeath(mappedBot, BotEndReason.ExceptionOccured);
                    }
                });
            }
        }

        private void EachActiveBot(Action<MappedBot> act) {
            foreach (var i in botReferences) {
                var gmo = (MappedBot)GetMappedObjectById(i.EngineIdentity);
                if (gmo.IsActive == true) {
                    act(gmo);
                }
            }
        }

        private void EachBot(Action<MappedBot> act) {
            foreach (var i in botReferences) {
                var gmo = (MappedBot)GetMappedObjectById(i.EngineIdentity);
                act(gmo);
            }
        }

        private Action<Message_BotPerformAction> actionHandler;
        private Action<Message_Game> gameHandler;
        private Action<Message_Query> queryHandler;

        private bool hasRegistered = false;

        public void RegisterForMessages() {
            b.Info.Log("Engine Message Registration Completes.");
            if (hasRegistered) {
                throw new InvalidOperationException("Engine is trying to register for messages a second time. This is invalid.");
            }
            hasRegistered = true;
            gameHandler = hub.LookFor<Message_Game>(gameMessage => {
                b.Verbose.Log("Recieving Message_Game");
                //if (gameMessage.SubKind == GameMessageSubKind.)
            });

            actionHandler = hub.LookFor<Message_BotPerformAction>(actionMessage => {
                b.Verbose.Log("Recieving Message_Action");
                if (actionMessage.SubKind == KnownSubkinds.InstallEquipment) {
                    var res = PerformEquipmentInstallation(actionMessage);
                    actionMessage.ResponseContext = res;
                }
                if (actionMessage.SubKind == KnownSubkinds.UseEquipment) {
                    var res = PerformEquipmentUsage((EquipmentUseRequestContext)actionMessage.RequestContext);
                    actionMessage.ResponseContext = res;
                }

                if (actionMessage.SubKind == KnownSubkinds.ChangeSpeed) {
                    b.Verbose.Log("Recieving Message_Action - SpeedChange");

                    NavigationInfoContext nic = (NavigationInfoContext)actionMessage.RequestContext;
                    var bt = GetMappedBotByPublicId(nic.PublicBotId);
                    b.Assert.True(bt != null, "Error, unable to map bot by public id, code cant continue.");
                    ChangeBotSpeed(bt, nic.SpeedDelta);
                }

                if (actionMessage.SubKind == KnownSubkinds.ChangeDirection) {
                    b.Verbose.Log("Recieving Message_Action - DirectionChange");

                    var bt = GetMappedBotByPublicId(actionMessage.PublicBotId);
                    ChangeObjectHeading(actionMessage, bt);
                }
            });

            queryHandler = hub.LookFor<Message_Query>(queryMessage => {
                b.Assert.True(queryMessage.PublicBotId != Guid.Empty, "Cant query data for a non existant bot.");

                b.Verbose.Log("Recieving Message_Query - " + queryMessage.SubKind.ToString());
                if (queryMessage.SubKind == KnownSubkinds.ReadSpeed) {
                    var bt = GetMappedBotByPublicId(queryMessage.PublicBotId);
                    NavigationInfoContext nic = new NavigationInfoContext();
                    nic.SpeedDelta = bt.Speed;
                    //queryMessage.IParam = bt.Speed;
                    queryMessage.ResponseContext = nic;
                }

                if (queryMessage.SubKind == KnownSubkinds.ReadHeading) {
                    var bt = GetMappedBotByPublicId(queryMessage.PublicBotId);
                    queryMessage.DParameter = bt.Heading;
                }
            });
        }

        private void ChangeObjectHeading(Message_BotPerformAction actionMessage, MappedBot bt) {
            bt.Heading = actionMessage.DParameter;
            NavigationInfoContext nic = new NavigationInfoContext();
            nic.SetBot(bt);
            nic.BotId = bt.EngineId;
            nic.NewHeading = bt.Heading;


            //nic.Kind = MainMessageKind.MapObjectMovementChange;
            //nic.SubKind = KnownSubkinds.DirectionChange;

            hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.MapObjectMovementChange, KnownSubkinds.DirectionChange) {
                RequestContext = nic
            });
        }

        private void ChangeBotSpeed(MappedBot bt, int p) {
            if (bt.TurnsAccelerationActionsRemaining > 0) {
                var ppi = (PowerPackEquipmentItem)equipment.GetEquipmentTypeById(bt.Bot.GetPowerPack().EquipmentId);

                bt.TurnsAccelerationActionsRemaining--;
                int tSpeed = bt.Speed + p;
                if ((tSpeed >= 0) && (tSpeed <= ppi.MaxSpeed)) {
                    bt.Speed = tSpeed;

                    NavigationInfoContext nic = new NavigationInfoContext();
                    nic.SetBot(bt);
                    nic.SpeedDelta = tSpeed;

                    hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.MapObjectMovementChange, KnownSubkinds.ChangeSpeed) {
                        RequestContext = nic
                    });

                }
            }
        }

        private EquipmentUseResult PerformEquipmentUsage(EquipmentUseRequestContext ctxt) {
            var bot = GetMappedBotByPublicId(ctxt.OwningBotIdentity);
            var activeEquipmentInstance = GetActiveEquipmentByInstanceId(ctxt);
            var equipTemplate = equipment.GetEquipmentTypeById(activeEquipmentInstance.EquipmentId);

            activeEquipmentInstance.UseCount++;

            if (equipTemplate.Classification == ItemClassification.Scanner) {
                ScannerEquipmentItem sci = (ScannerEquipmentItem)equipTemplate;
                return PerformScannerUsage(sci, activeEquipmentInstance, bot);
            }
            if (equipTemplate.Classification == ItemClassification.OffsensiveWeapon) {
                OffensiveWeaponEquipmentItem owi = (OffensiveWeaponEquipmentItem)equipTemplate;
                return PerformFireWeapon(owi, activeEquipmentInstance, bot, ctxt);
            }
            b.Warning.Log("Unknown equipment usage type, result being returned is a dummy");
            EquipmentUseResult result = new EquipmentUseResult();
            result.State = UsageEndState.Success;
            return result;
        }

        private EquipmentUseResult PerformFireWeapon(OffensiveWeaponEquipmentItem owi, ActiveEquipment activeEquipmentInstance, MappedBot attacker, EquipmentUseRequestContext ctxt) {
            EquipmentUseResult result = new EquipmentUseResult();

            if (activeEquipmentInstance.CooldownTicksRemaining > 0) {
                result.State = UsageEndState.Fail_CooldownActive;
                return result;
            }

            MappedBot victim = ConvertTemporaryScanKeyToBot(attacker.EngineId, ctxt.IParam);

            if (victim == null) {
                result.State = UsageEndState.Fail_InvalidTarget;
                b.Warning.Log("The bot tried to target an invalid key.  This shouldnt happen");
                return result;
            }

            b.Assert.True(attacker.EngineId != victim.EngineId, "You cant shoot at yourself.");

            activeEquipmentInstance.RoundsRemaining -= 1;
            if (activeEquipmentInstance.RoundsRemaining <= 0) {
                result.State = UsageEndState.Fail_NoAmmo;
                return result;
            }

            if (!attacker.ConsumeCharge(owi.BaseChargeCost)) {
                result.State = UsageEndState.Fail_NoCharge;
                return result;
            }

            CombatAttack ca = new CombatAttack();
            ca.Attacker = attacker;
            ca.Victim = victim;
            ca.Weapon = owi;
            ca.WeaponInstance = activeEquipmentInstance;
            CombatResult cr = combatCore.ResolveAttack(ca);

            UICombatContext context = new UICombatContext();
            context.AggressorId = attacker.EngineId;
            context.VictimId = victim.EngineId;
            context.WeaponTypeId = owi.UniqueId;
            context.DidHit = cr.DidHit;
            context.Damage = cr.TotalDamage;

            hub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.UIMessage, KnownSubkinds.WeaponFire) {
                RequestContext = context
            });

            if (cr.DidHit) {
                ApplyWeaponfireHit(victim, cr);
            }

            result.State = UsageEndState.Success;
            return result;
        }

        private void ApplyWeaponfireHit(MappedBot victim, CombatResult cr) {
            ApplyDamageToObject(victim.EngineId, cr.TotalDamage, DamageType.Projectile);

            bd2TickAction ta = new bd2TickAction();
            ta.ActionType = LastTickEventType.Shot;

            CreateNextTurnNotificationMessage(victim.EngineId, ta);
        }

        private void CreateNextTurnNotificationMessage(int p, bd2TickAction action) {
            LastTickRecord ltr;
            if (activeData.LastTickRecords.ContainsKey(p)) {
                ltr = activeData.LastTickRecords[p];
            } else {
                ltr = new LastTickRecord();
                activeData.LastTickRecords.Add(p, ltr);
            }

            ltr.Events.Push(action);
        }

        private EquipmentUseResult PerformScannerUsage(ScannerEquipmentItem equipTemplate, ActiveEquipment activeEquipmentInstance, MappedBot owningBot) {
            ScanEquipmentUseResult result = new ScanEquipmentUseResult();
            if (!owningBot.ConsumeCharge(equipTemplate.ChargeConsumed)) {
                result.State = UsageEndState.Fail_NoCharge;
                return result;
            }

            Point mapOffset = owningBot.Position;

            foreach (var vSrc in equipTemplate.GetAllScanPoints()) {
                Point v = new Point(vSrc.X + mapOffset.X, vSrc.Y + mapOffset.Y);

                ScanTileResult str = ScanTileResult.Unscanned;
                MapTile mt = MapTile.DefaultGround;

                if (activeWorld.IsValidSpace(v)) {
                    var tileOccupant = GetTileOccupantByLocation(v);
                    if (tileOccupant != null) {
                        if (tileOccupant.EngineId == owningBot.EngineId) {
                            str = ScanTileResult.You;
                        } else {
                            int ctk = CreateTemporaryScanKey(owningBot.EngineId, tileOccupant.EngineId);
                            result.AddPointOfInterest(v, ctk);
                            str = tileOccupant.IsAlive() ? ScanTileResult.Bot : ScanTileResult.Wreckage;
                        }
                    } else {
                        mt = activeWorld.Map.GetTileAtPosition(v);
                    }
                } else {
                    mt = MapTile.BoundaryWall1;
                }

                if (str == ScanTileResult.Unscanned) {
                    // If there was not an active occupant, then look to the map.
                    switch (mt) {
                        case MapTile.BoundaryWall1: str = ScanTileResult.SolidWall; break;
                        case MapTile.DefaultGround: str = ScanTileResult.Unoccupied; break;
                        default: throw new BdBaseException("DEFAULT - invalid tile mapping - " + mt.ToString());
                    }
                }

                result.SetDimensions(equipTemplate.MinimumXScanned, equipTemplate.MinimumYScanned, equipTemplate.TotalWidthScanned, equipTemplate.TotalHeightScanned);
                result.SetScanResultAtPosition(vSrc, str);
            }

            return result;
        }

        private int CreateTemporaryScanKey(int owningBotEngineId, int targetBotEngineId) {
            int lud = ++activeData.LastUsedTemporaryKey;
            activeData.RegisterTeporaryKey(lud, owningBotEngineId, targetBotEngineId);
            return lud;
        }

        private MappedBot ConvertTemporaryScanKeyToBot(int sourceBotId, int scanId) {
            int tid = activeData.GetEngineIdFromScanId(sourceBotId, scanId);
            return GetMappedBotById(tid);
        }

        private MappedBot GetTileOccupantByLocation(Point v) {
            foreach (var mop in mappedObjects) {
                if (mop.Position == v) {
                    // Theres an object in this position
                    var mb = GetMappedBotById(mop.EngineId);
                    b.Assert.True(mb != null, "Not implemented anything mapped thats not a bot yet, if this changes update scanner logic.");
                    return mb;
                }
            }
            return null;
        }

        private ActiveEquipment GetActiveEquipmentByInstanceId(EquipmentUseRequestContext ctxt) {
            foreach (var v in installedEquipment) {
                if (v.InstanceId == ctxt.RequestedEquipmentInstance) {
                    return v;
                }
            }
            throw new BdBaseException("That Equipment Item has not been registered");
        }

        private EquipmentInstallationResult PerformEquipmentInstallation(Message_BotPerformAction actionMessage) {
            EquipmentInstallationContext eic = (EquipmentInstallationContext)actionMessage.RequestContext;
            EquipmentInstallationResult result = new EquipmentInstallationResult();

            var baseEquipment = equipment.GetEquipmentTypeById(eic.EquipmentIdentifier);
            ActiveEquipment ei = new ActiveEquipment(baseEquipment);
            result.InstanceId = ei.InstanceId = Guid.NewGuid();
            result.EquipmentId = ei.EquipmentId = eic.EquipmentIdentifier;
            result.Result = InstallationResult.Installed;

            ei.OwningBotId = eic.OwningBotIdentity;
            ei.MountPoint = eic.MountPoint;

            ei.Classification = KnownEquipmentIds.GetClassificationFromId(eic.EquipmentIdentifier);
            if (ei.Classification == ItemClassification.OffsensiveWeapon) {
                var eq = equipment.GetWeaponEquipmentById(eic.EquipmentIdentifier);
                ei.RoundsRemaining = eq.InitialAmmunition;
            }
            installedEquipment.Add(ei);
            return result;
        }

        public void InjectEquipmentSupport(EquipmentSupport equipmentSupport) {
            b.Verbose.Log("Equipment support being injected into engine.");
            equipment = equipmentSupport;
        }
    }
}