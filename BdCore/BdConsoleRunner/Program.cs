using Plisky.Boondoggle2;
using Plisky.Boondoggle2.Reference;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Plisky.Plumbing;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Plisky.Boondoggle2.Runner {
    class Program {

        private const int SLEEPAMOUNT = 0;
        private const int MAXTURNCOUNT = 200;  
        private const string MAPNAME = "newDefaultMap";  
        private static int ActiveTurn = -1;
        private static bool stopAtEnd = false;

        static void Main(string[] args) {
            Bilge b = new Bilge(tl: TraceLevel.Verbose);
            b.AddHandler(new TCPHandler("127.0.0.1", 9060));
            ConfigHub.Current.InjectBilge(b);

            Console.WriteLine("Online....");
            
            b.Info.Log( "Boondoggle, Online....");

            MachineConfigurations.PerformMachineConfig();
            b.Info.Log( "Machine configuration established, beginning contest");

            BattleRunnerControl brc;

            if ((args.Length > 0) && (File.Exists(args[0]))) {
                b.Info.Log( "Control File Detected - Using: ", args[0]);
                brc = new BattleRunnerControl(args[0]);
            } else {
                brc = new BattleRunnerControl(string.Format("bdgl_2_0_{0}", DateTime.Now.ToString("ddmmyy_hh_mm_ss")), "Dummy Battle");
                brc.AddContestant(new KevBot());
                brc.AddContestant(new Reference_RightTurnBot());
                brc.AddContestant(new BorisBot());                
            }

            b.Info.Log( "Context established, starting main contest");
            try {
#if false
                bd2XmlOutputter xmloutput = new bd2XmlOutputter();
                xmloutput.Initialise(brc.BattleUniqueName);
                xmloutput.StoreControlData(brc);
#else

                bd2ConsoleOutputter consoleOut = new bd2ConsoleOutputter();
                consoleOut.Initialise(brc.BattleUniqueName);
#endif

                bd2Engine mainEngine = new bd2Engine();
                mainEngine.InjectBotSupport();
                mainEngine.InjectEquipmentSupport(new EquipmentSupport(new HardcodedEquipmentRepository()));
                mainEngine.RegisterForMessages();
                

                bd2MapRepository bdmr = new bd2MapRepository();

                var mp = bdmr.GetMapByName(MAPNAME);

                mp.MapType = MapConditionType.LastBotStanding;
                mainEngine.AddWorld(new bd2World(mp));
                mainEngine.DumpWorld();
                foreach (var v in brc.GetContestants()) {
                    mainEngine.AddBot(v);
                }

                mainEngine.StartBattle();

                b.Info.Log( "About to enter loop");
                do {
                    ActiveTurn = mainEngine.Turn;

                    mainEngine.PerformNextTick();

                    if (SLEEPAMOUNT > 0) {
                        SlowDownExecution();
                    }

                    if (ActiveTurn > MAXTURNCOUNT) {
                        b.Info.Log("The contest has gone on too long- exiting....");
                        Console.WriteLine("Protection Abort....");
                        mainEngine.ShutdownBattle();
                    }
                } while (mainEngine.BattleActive);
                b.Info.Log( "Battle No Longer Active.");
            } catch (BdBaseException bx) {
                Console.WriteLine("ERROR");
                Console.WriteLine(bx.Message);
            }

            if (stopAtEnd) {
                Console.WriteLine("Done");
                Console.ReadLine();
            }
        }

        private static void SlowDownExecution() {
            Thread.Sleep(SLEEPAMOUNT);
        }
    }
}
