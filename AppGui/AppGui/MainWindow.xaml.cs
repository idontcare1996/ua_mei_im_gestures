using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using multimodal;
using CSGSI;
using CSGSI.Nodes;
using WindowsInput.Native;
using WindowsInput;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AppGui
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public struct Gamestate
    {
        public int money;
        public bool IsPlanted;
        public int round_number;
        public int bullets;
        public int max_bullets;
        public int health;
        public int armour;
        public int roundkills;
        public int total_kills;
        public int total_deaths;
        public int spectators;
        public int atual_round;
        public int round_counter;
    }
    static class Prices
    {
        public const int ak47 = 2700;
        public const int awp = 4750;
        public const int deagle = 700;
        public const int defuse = 400;
        public const int kelvar_cap = 1000;
        public const int p250 = 300;
        public const int mp7 = 1500;
        public const int ump45 = 1200;


    }
    

    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
        private Tts t = new Tts();
        private static GameStateListener gsl;
        public Gamestate gamestate = new Gamestate();
        

      
        public MainWindow()
        {
            InitializeComponent();
            gsl = new GameStateListener(3000);
            gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            if (!gsl.Start())
            {
                Environment.Exit(0);
            }
            
            gamestate.money = 800;
            gamestate.round_counter = 0;
            
            Console.WriteLine("Listening... n\n");
            
            //t.Speak("Por favor, espere um pouco enquanto tentamos conectar...");
            mmiC = new MmiCommunication("localhost",8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();

        }
       
        void OnNewGameState(GameState gs)
        {
            
            if (!gamestate.IsPlanted &&
               gs.Round.Phase == RoundPhase.Live &&
               gs.Round.Bomb == BombState.Planted &&
               gs.Previously.Round.Bomb == BombState.Undefined)
            {
                Console.WriteLine("Bomb has been planted.");
                t.Speak("A bomba foi plantada, explodirá em 45 segundos");
                gamestate.IsPlanted = true;
            }
            else if (gamestate.IsPlanted && gs.Round.Phase == RoundPhase.FreezeTime)
            {
                gamestate.IsPlanted = false;
            }
            
            
            gamestate.money = gs.Player.State.Money;
            gamestate.round_number = gs.Map.Round;
            gamestate.bullets = gs.Player.Weapons.ActiveWeapon.AmmoClip;
            gamestate.max_bullets = gs.Player.Weapons.ActiveWeapon.AmmoClipMax;
            gamestate.health = gs.Player.State.Health;
            gamestate.armour = gs.Player.State.Armor;
            gamestate.roundkills = gs.Player.State.RoundKills;
            gamestate.total_kills = gs.Player.MatchStats.Kills;
            gamestate.total_deaths = gs.Player.MatchStats.Deaths;
            gamestate.spectators = gs.Map.CurrentSpectators;
            
            if (gamestate.round_counter < gamestate.round_number)
            {
                Thread.Sleep(17000);
                gamestate.round_counter = gamestate.round_number;
                if (gamestate.armour <= 70)
                {                   
                   // t.Speak("Aconselho-te a comprar um capacete");
                }

                if (gamestate.money >= 4500)
                {
                  //  t.Speak("Podes comprar um kit...");
                }
            }

        }
       

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);

            InputSimulator inputsim = new InputSimulator();
            
            double confidence = Double.Parse((string)json.recognized[0].ToString());
            
            String command = (string)json.recognized[1].ToString();
           
            /*
            String command2 = (string)json.recognized[2].ToString();
            
            String details = (string)json.recognized[3].ToString();

            String weapon = (string)json.recognized[4].ToString();
            */

            if (t.getSpeech() == true)
            {                
                return;
            }

            if (confidence < 0.3)
            {
                t.Speak("Desculpe, pode repetir?");
            }
                       
            else
            {
                switch (command)
                {
                    case "CROUCH":
                        {
                            // CÓDIGO PARA DAR INPUT DA TECLA: VK_K ( Tecla K do teclado )
                            inputsim.Keyboard.KeyDown(VirtualKeyCode.LCONTROL);
                            Thread.Sleep(1000);                               
                            
                            break;
                        }
                    case "DAB":
                        {
                            // CÓDIGO PARA DAR INPUT DA TECLA: VK_K ( Tecla K do teclado )
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_Y);
                            Thread.Sleep(100);
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_D);
                            Thread.Sleep(50);
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_A);
                            Thread.Sleep(50);
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_B);
                            Thread.Sleep(50);
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                            Thread.Sleep(50);
                            break;
                        }                    
                    case "HEY":
                        {
                            // CÓDIGO PARA DAR INPUT DA TECLA: VK_K ( Tecla K do teclado )
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_X);
                            Thread.Sleep(25);
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_3);
                            break;
                        }
                    case "HOLD":
                        {
                            // CÓDIGO PARA DAR INPUT DA TECLA: VK_K ( Tecla K do teclado )
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_Z);
                            Thread.Sleep(25);
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
                            break;
                        }
                    case "RELOAD":
                        {
                            // CÓDIGO PARA DAR INPUT DA TECLA: VK_K ( Tecla K do teclado )
                            inputsim.Keyboard.KeyPress(VirtualKeyCode.VK_R);
                            break;
                        }
                    default:
                        {
                            
                            break;
                        }
                }
                   
                                         

                                                                   
                    
                
            }                        
        }
    }
}

