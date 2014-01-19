using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;
using Lidgren.Network.Xna;
using System.Net;

namespace Fort_Tales_Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 


    public class SVisibleBlocks
    {
        public int ID;
        public int ObjectID;
        public int PlayerID;
        public bool Builded;
        public int X;
        public int Y;
        public int Hp;
        public int MaxHp;
        public int BuildingTime;
        public int MaxBuildingTime;
        public int Loyality;

        public SVisibleBlocks(int id, int oid, int pid, bool b, int hp, int maxhp, int l, int bt, int mbt, int x, int y)
        {
            ID = id;
            ObjectID = oid;
            PlayerID = pid;
            Builded = b;
            Hp = hp;
            MaxHp = maxhp;
            Loyality = l;
            X = x;
            Y = y;
            BuildingTime = bt;
            MaxBuildingTime = mbt;
        }

        public bool IsOver(int x, int y)
        {
            if (new Rectangle(x, y, 1, 1).Intersects(new Rectangle(X, Y, 50, 50)))
            {
                return true;
            }
            return false;
        }
    }

    public class SVisibleUnit
    {
        public int UnitID;
        public int PlayerID;
        public int Hp;
        public int MaxHp;
        public int X;
        public int Y;
        public int Type;

        public SVisibleUnit(int uid, int t, int playerid, int hp, int mhp, int x, int y)
        {
            Type = t;
            UnitID = uid;
            PlayerID = playerid;
            Hp = hp;
            MaxHp = mhp;
            X = x;
            Y = y;
        }

        public bool IsOver(int x, int y)
        {
            if (new Rectangle(x, y, 1, 1).Intersects(new Rectangle(X, Y, 50, 50)))
            {
                return true;
            }
            return false;
        }
    }

    public struct SPlayerAction
    {
        public enum ActionType { MOUSE_POSITION, MOUSE_CLICKED_LEFT, MOUSE_CLICKED_RIGHT };
        public ActionType Type;
        public int X;
        public int Y;
    }

    public struct SCamera
    {
        public int X;
        public int Y;
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CPlayer Player;
        SpriteFont Font1;
        SpriteFont Font2;
        SpriteFont Font3;
        bool IsConnected;
        SPlayerAction PlayerAction;

        int SelectedObject = -1;
        int SelectedUnit = -1;

        bool IsMapLoaded = false;
        SCamera Camera;
        int MapCount = 0;

        Song Song_MainMenu;
        SoundEffect Sound_Button_Hover;

        int VisibleStartX;
        int VisibleStartY;
        List<SVisibleBlocks> VisibleBlocks = new List<SVisibleBlocks>();
        List<SVisibleUnit> VisibleUnits = new List<SVisibleUnit>();
        List<CMenuItem> MenuItems = new List<CMenuItem>();

        int PlayerID = -1;

        List<CGameObject> Objects = new List<CGameObject>();

        MouseState mouse;
        new Vector2 mouse_res;
        MouseState lastmouse;
        KeyboardState keyboard;
        KeyboardState lastkeyboard;
        int WindowHeight = 600;
        int WindowWidth = 800;
        List<CPlayer> Players = new List<CPlayer>();
        Block[,] Blocks = new Block[100, 300];

        IPEndPoint Master_Server_IP;

        List<CButton> MainMenuButtons = new List<CButton>();

        Texture2D SpriteSheet;
        Texture2D GUITexture;
        Texture2D TPointer;
        Texture2D MainMenu;

        int Timer = 0;

        CInputWindow InputWindow;
        CInputWindow NameInputWindow;
        CInputWindow IPInputWindow;

        NetClient Client;
        NetPeerConfiguration Config;

        enum GameState { MAINMENU, LOBBY, PLAYING }
        enum PacketType { LOGIN, PLAYERDATA, PLAYERSINLOBBY, OBJECT, PLAYERACTION, PLAYERID, GAMEOBJECTS, MAP };
        enum MapPacketType { MAP, CREATEMAP, NEEDMAP, UPDATEMAP };
        enum PlayerActions { CAMERA_CHANGE, BUILD_HOUSE, BUILD_BARRACKS, BUILD_WALL, BUILD_ROAD, BUILD_GATE, BUILD_FARM, BUILD_MINE, MOVE_UNIT, BUILD_WATCHTOWER };
        enum ClickedElements { NONE, BUILD_HOUSE, BUILD_BARRACKS, BUILD_WALL, BUILD_ROAD, BUILD_GATE, BUILD_FARM, BUILD_MINE };
        enum Menus { NONE, BUILD_MENU, UNIT_MENU, OBJECT_MENU };
        enum MainMenuCategory { MAIN, IP_ENTER, LOBBY, SEARCHSERVERS, SECOND };

        ClickedElements ClickedElement = ClickedElements.NONE;
        Menus Menu = Menus.NONE;
        GameState gameState;
        MainMenuCategory mainMenuCategory;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            VisibleStartX = 0;
            VisibleStartY = 0;
            Camera.X = 0;
            Camera.Y = 0;
            MediaPlayer.IsRepeating = true;
            gameState = GameState.MAINMENU;
            mainMenuCategory = MainMenuCategory.MAIN;
            MainMenuButtons.Add(new CButton("Quit", 275, 450, 250, 50, 0, 0));
            MainMenuButtons.Add(new CButton("Start", 275, 375, 250, 50, 0, 1));
            MainMenuButtons.Add(new CButton("Join to IP", 275, 225, 250, 50, 1, 0));
            MainMenuButtons.Add(new CButton("Find Servers", 275, 300, 250, 50, 1, 1));
            MainMenuButtons.Add(new CButton("Quit", 275, 450, 250, 50, 1, 2));
            MainMenuButtons.Add(new CButton("Quit", 275, 450, 250, 50, 2, 1));
            MainMenuButtons.Add(new CButton("Start", 275, 375, 250, 50, 2, 0));
            IsConnected = false;
            Player = new CPlayer(" ");     
            
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.IsFullScreen = false;
            Window.Title = "Fort Tales";
            graphics.ApplyChanges();
            base.Initialize();
        }

        private void InitializeClient(string name, string hostip)
        {
            if (!IsConnected)
            {
                Config = new NetPeerConfiguration("game");
                MediaPlayer.Stop();
                Config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
                //Config.Port = 14242;
                Client = new NetClient(Config);
                Console.WriteLine("Sending");
                Client.Start();
                Console.WriteLine("Sending");
                Master_Server_IP = new IPEndPoint(NetUtility.Resolve(hostip), 14242);
                NetOutgoingMessage sendMsg = Client.CreateMessage();
                sendMsg.Write((int)PacketType.LOGIN);
                sendMsg.Write(name);
                sendMsg.Write(WindowWidth);
                sendMsg.Write(WindowHeight);
                NetworkException e = new NetworkException();
                IsConnected = true;
                Console.WriteLine(Master_Server_IP.ToString());
                Client.SendUnconnectedMessage(sendMsg, Master_Server_IP);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw SpriteSheet.
            GUITexture = Content.Load<Texture2D>("GameplayGUI");
            TPointer = Content.Load<Texture2D>("mouse_1");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Font");
            Font2 = Content.Load<SpriteFont>("Font2");
            Font3 = Content.Load<SpriteFont>("Font3");
            MainMenu = Content.Load<Texture2D>("Menu");
            Song_MainMenu = Content.Load<Song>("MainMenu_Song");
            Sound_Button_Hover = Content.Load<SoundEffect>("button_hover");
            SpriteSheet = Content.Load<Texture2D>("SpriteSheet");
            NameInputWindow = new CInputWindow(275, 300, 250, 50);
            IPInputWindow = new CInputWindow(275, 300, 250, 50);
            //MediaPlayer.Play(Song_MainMenu);
            MenuItems.Add(new CMenuItem(110, 530, 60, 60, ref SpriteSheet, 1));      
            MenuItems.Add(new CMenuItem(110, 530, 60, 60, ref SpriteSheet, 2));
            MenuItems.Add(new CMenuItem(180, 530, 60, 60, ref SpriteSheet, 3));
            MenuItems.Add(new CMenuItem(250, 530, 60, 60, ref SpriteSheet, 4));
            MenuItems.Add(new CMenuItem(320, 530, 60, 60, ref SpriteSheet, 5));
            MenuItems.Add(new CMenuItem(390, 530, 60, 60, ref SpriteSheet, 6));
            MenuItems.Add(new CMenuItem(460, 530, 60, 60, ref SpriteSheet, 7));
            MenuItems.Add(new CMenuItem(530, 530, 60, 60, ref SpriteSheet, 8));
            MenuItems.Add(new CMenuItem(110, 530, 60, 60, ref SpriteSheet, 9));
            MenuItems[0].Visible = true;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (IsConnected) ProcessNetworkMessages();
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
            switch (gameState)
            {
                case GameState.MAINMENU:
                    HandleGameplayInput();
                    UpdateGUI();
                    if (keyboard != lastkeyboard)
                    {
                        if (mainMenuCategory == MainMenuCategory.MAIN)
                            NameInputWindow.AddKey(keyboard);
                        if (mainMenuCategory == MainMenuCategory.IP_ENTER)
                            IPInputWindow.AddKey(keyboard);
                    }
                    if (NameInputWindow.IsEnter)
                    {
                        if (mainMenuCategory == MainMenuCategory.MAIN)
                        {
                            mainMenuCategory = MainMenuCategory.SECOND;
                            Player.Name = NameInputWindow.ReturnText();
                        }                       

                        //InputWindow.Clear();
                    }
                    if (IPInputWindow.IsEnter)
                    {
                        if (mainMenuCategory == MainMenuCategory.IP_ENTER)
                        {
                            if (IPInputWindow.ReturnText() != null)
                            {
                                gameState = GameState.LOBBY;
                                InitializeClient(Player.Name, IPInputWindow.ReturnText());
                            }
                        }
                    }
                    break;

                /*case GameState.MAINMENU_IP:
                    if (keyboard != lastkeyboard)
                        InputWindow.AddKey(keyboard);

                    if (InputWindow.IsEnter)
                    {
                        InitializeClient(Player.Name, InputWindow.ReturnText());
                        Player.Name = InputWindow.ReturnText();
                        Console.WriteLine(Player.Name);
                        InputWindow.Clear();
                        if (IsConnected)
                        {
                            gameState = GameState.LOBBY;
                        }
                    }
                    break;*/

                case GameState.PLAYING:
                    {
                        Timer++;
                        HandleGameplayInput();
                        ManageMenus();
                        UpdateGUI();
                        if (IsMapLoaded)
                        {
                            if (Timer == 50)
                            {
                                UpdateMap();
                            }
                        }
                        if (Timer >= 120) Timer = 0;
                    }
                    break;
            }
            ProcessSendingNetworkMessages();
            lastkeyboard = keyboard;
            lastmouse = mouse;
            base.Update(gameTime);
        }

        private void HandleGameplayInput()
        {
            PlayerAction.X = mouse.X;
            PlayerAction.Y = mouse.Y;
            if (PlayerAction.X < 0) PlayerAction.X = 0;
            if (PlayerAction.X > 800) PlayerAction.X = 800;
            if (PlayerAction.Y < 0) PlayerAction.X = 0;
            if (PlayerAction.Y > 600) PlayerAction.X = 600;
            CameraMove();
            /*if (mouse.MiddleButton == ButtonState.Pressed && lastmouse.MiddleButton == ButtonState.Released)
            {
                
                int index = -1;
                if (SelectedObject != -1)
                {
                    index = VisibleBlocks.FindIndex(i => i.ID == SelectedObject);
                    if (index != -1)
                    {
                        Console.WriteLine(VisibleBlocks[index].ObjectID.ToString() + " " + VisibleBlocks[index].X.ToString() + " " + VisibleBlocks[index].Y.ToString());
                    }
                }
                index = -1;
                if (SelectedUnit != -1)
                {
                    index = VisibleUnits.FindIndex(i => i.UnitID == SelectedUnit);
                    if (index != -1)
                    {
                        Console.WriteLine("Selected Unit: " + VisibleUnits[index].X.ToString() + " " + VisibleUnits[index].Y.ToString() + " " + (PlayerAction.X + Camera.X).ToString() + " " + (PlayerAction.Y + Camera.Y).ToString());
                    }
                }
            }*/

            if (mouse.LeftButton == ButtonState.Pressed && lastmouse.LeftButton == ButtonState.Released)
            {
                PlayerAction.Type = SPlayerAction.ActionType.MOUSE_CLICKED_LEFT;
               // Console.WriteLine("Left Click");
            }
            else
            {
                if (mouse.RightButton == ButtonState.Pressed && lastmouse.RightButton == ButtonState.Released)
                {
                    PlayerAction.Type = SPlayerAction.ActionType.MOUSE_CLICKED_RIGHT;
                }
                else
                {
                    PlayerAction.Type = SPlayerAction.ActionType.MOUSE_POSITION;
                }
            }
        }

        private void ProcessSendingNetworkMessages()
        {
            switch (gameState)
            {
                case GameState.PLAYING:
                    NetOutgoingMessage outmsg = Client.CreateMessage();
                    if (gameState == GameState.PLAYING)
                    {
                        if (!IsMapLoaded)
                        {
                            NetOutgoingMessage outmsg2 = Client.CreateMessage();
                            outmsg2.Write((int)PacketType.MAP);
                            outmsg2.Write((int)MapPacketType.NEEDMAP);
                            outmsg2.Write(PlayerID);
                            Client.SendUnconnectedMessage(outmsg2, Master_Server_IP);
                            //Console.WriteLine("Sended map request");
                        }
                        else
                        {
                            switch (PlayerAction.Type)
                            {
                                case SPlayerAction.ActionType.MOUSE_CLICKED_LEFT:
                                    switch (ClickedElement)
                                    {
                                        case ClickedElements.BUILD_HOUSE:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_HOUSE);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;

                                        case ClickedElements.BUILD_BARRACKS:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_BARRACKS);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;

                                        case ClickedElements.BUILD_WALL:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_WALL);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;

                                        case ClickedElements.BUILD_ROAD:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_ROAD);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;

                                        case ClickedElements.BUILD_GATE:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_GATE);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;

                                        case ClickedElements.BUILD_FARM:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_FARM);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;

                                        case ClickedElements.BUILD_MINE:
                                            if ((new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                            {
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_MINE);
                                                outmsg.Write((int)((PlayerAction.X - Camera.X) / 50));
                                                outmsg.Write((int)((PlayerAction.Y - Camera.Y) / 50));
                                                Console.WriteLine(((int)((PlayerAction.X - Camera.X) / 50)).ToString() + " " + ((int)((PlayerAction.Y - Camera.Y) / 50)).ToString());
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage inc;
            if ((inc = Client.ReadMessage()) != null)
            {
                switch (gameState)
                {
                    case GameState.LOBBY:
                        switch (inc.MessageType)
                        {

                            case NetIncomingMessageType.UnconnectedData:

                                //Console.WriteLine(inc.ReadByte().ToString());
                                int i = inc.ReadInt32();
                                switch (i)
                                {
                                    case (int)PacketType.OBJECT:
                                        Console.WriteLine("Object");
                                        int nr = inc.ReadInt16();
                                        Console.WriteLine(nr.ToString());
                                        break;

                                    case (int)PacketType.PLAYERID:
                                        Console.WriteLine("PlayerID");
                                        if (PlayerID == -1)
                                        {
                                            PlayerID = inc.ReadInt32();
                                        }
                                        Console.WriteLine("PlayerID: " + PlayerID.ToString());

                                        break;

                                    case (int)PacketType.MAP:
                                        int ptype = inc.ReadInt32();
                                        switch (ptype)
                                        {
                                            case (int)MapPacketType.CREATEMAP:
                                                int x = inc.ReadInt32();
                                                int y = inc.ReadInt32();
                                                Blocks = null;
                                                Blocks = new Block[x, y];
                                                for (int m = 0; m < x; m++)
                                                {
                                                    for (int n = 0; n < x; n++)
                                                    {
                                                        Blocks[m, n] = new Block(m, n);
                                                    }
                                                }
                                                Console.WriteLine("Map created!");
                                                gameState = GameState.PLAYING;
                                                break;
                                        }
                                        break;
                                    case (int)PacketType.PLAYERSINLOBBY:
                                        int count = inc.ReadInt32();
                                        Players.Clear();
                                        for (int j = 0; j < count; j++)
                                        {
                                            Players.Add(new CPlayer(inc.ReadString()));
                                        }
                                        break;

                                    default:
                                        //Console.WriteLine("Byte: " + inc.ReadByte().ToString());
                                        break;
                                }
                                break;

                        }
                        break;

                    case GameState.PLAYING:
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.UnconnectedData:
                                switch (inc.ReadInt32())
                                {                                    
                                    case (int)PacketType.MAP:
                                        int ptype = inc.ReadInt32();
                                        switch (ptype)
                                        {

                                            case (int)PacketType.MAP:
                                                Console.WriteLine("Incoming Map!");
                                                int bcount = inc.ReadInt32();
                                                //Console.Write(" bcount " + bcount.ToString());
                                                for (int j = 0; j < bcount; j++)
                                                {
                                                    int bx = inc.ReadInt32();
                                                    int by = inc.ReadInt32();
                                                    int t = inc.ReadInt32();
                                                    int p = inc.ReadInt32();
                                                    //Console.WriteLine(bx.ToString() + " " + by.ToString() + " " + t.ToString() + " " + p.ToString());
                                                    Blocks[bx, by].TerrainType = t;
                                                    Blocks[bx, by].PlayerID = p;
                                                    if (bx - 1 == Blocks.GetLength(0) && by - 1 == Blocks.GetLength(1))
                                                    {
                                                        IsMapLoaded = true;
                                                        Console.WriteLine("Map Ready!");
                                                        break;
                                                    }
                                                }
                                                MapCount = inc.ReadInt32();
                                                if (MapCount == (Blocks.GetLength(0) * Blocks.GetLength(1)))
                                                {
                                                    IsMapLoaded = true;
                                                }
                                                Console.Write("Map Count" + MapCount.ToString());

                                                break;

                                            case (int)MapPacketType.UPDATEMAP:
                                                int x1 = inc.ReadInt32();
                                                int y1 = inc.ReadInt32();
                                                int width = inc.ReadInt32();
                                                int height = inc.ReadInt32();
                                                Console.WriteLine("Map updating");
                                                for (int m = 0; m < width; m++)
                                                {
                                                    for (int n = 0; n < height; n++)
                                                    {
                                                        int pid = inc.ReadInt32();
                                                        bool b = inc.ReadBoolean();

                                                        Blocks[x1 + m, y1 + n].Buildable = b;
                                                        Blocks[x1 + m, y1 + n].PlayerID = pid;

                                                    }
                                                }
                                                break;
                                        }
                                        break;

                                    case (int)PacketType.GAMEOBJECTS:
                                        VisibleBlocks.Clear();
                                        VisibleUnits.Clear();
                                        Player.Resources = inc.ReadInt32();
                                        Player.ResourcesIncome = inc.ReadInt32();
                                        Player.Food = inc.ReadInt32();
                                        Player.FoodIncome = inc.ReadInt32();
                                        Player.Population = inc.ReadInt32();
                                        Player.MaxPopulation = inc.ReadInt32();
                                        int ObjectsCount = inc.ReadInt32();
                                        //Console.WriteLine("\nMap:\n\n");
                                        for (int i = 0; i < ObjectsCount; i++)
                                        {

                                            VisibleBlocks.Add(new SVisibleBlocks(inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadBoolean(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32()));                                                
                                                //Console.Write(block.ObjectID.ToString());
                                            
                                            //Console.WriteLine();
                                        }
                                        int UnitsCount = inc.ReadInt32();
                                        for (int i = 0; i < UnitsCount; i++)
                                        {
                                            VisibleUnits.Add(new SVisibleUnit(inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32()));
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                }
                Client.Recycle(inc);
            }
        }

        private void UpdateMap()
        {
            if (IsMapLoaded)
            {
                NetOutgoingMessage outmsg = Client.CreateMessage();
                outmsg.Write((int)PacketType.MAP);
                outmsg.Write((int)MapPacketType.UPDATEMAP);
                outmsg.Write(PlayerID);
                outmsg.Write((-Camera.X) / 50);
                outmsg.Write((-Camera.Y) / 50);
                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                //Console.WriteLine((-Camera.X / 50).ToString() + " " + (-Camera.Y / 50).ToString());
            }
        }

        private void UpdateGUI()
        {
            switch (PlayerAction.Type)
            {
                case SPlayerAction.ActionType.MOUSE_POSITION:
                    switch (gameState)
                    {
                        case GameState.PLAYING:
                            #region GameState_Playing
                            foreach (CMenuItem m in MenuItems)
                            {
                                if (m.Visible)
                                {
                                    if (m.IsIn(mouse.X, mouse.Y))
                                    {
                                        m.Hover = true;
                                        //Console.WriteLine("Chech IsIn");
                                        //Console.WriteLine(m.Hover.ToString());
                                    }
                                    else
                                    {
                                        m.Hover = false;
                                    }
                                }
                            }
                            #endregion
                            break;

                        case GameState.MAINMENU:
                            #region GameState_MainMenu
                            //Console.WriteLine("MainMenu");
                            foreach (CButton b in MainMenuButtons)
                            {
                                b.Visible = false;                              
                                if (mainMenuCategory == MainMenuCategory.MAIN)
                                {                                   
                                    if (b.Category == 0)
                                    {
                                        b.Visible = true;
                                    }
                                }
                                if (mainMenuCategory == MainMenuCategory.SECOND)
                                {
                                    if (b.Category == 1)
                                    {
                                        b.Visible = true;
                                    }
                                }
                                if (mainMenuCategory == MainMenuCategory.IP_ENTER)
                                {
                                    if (b.Category == 2)
                                    {
                                        b.Visible = true;
                                    }
                                }
                                if (b.IsOver(PlayerAction.X, PlayerAction.Y))
                                {
                                    b.Hover = true;
                                    if (!b.IsPlayedSong)
                                    {
                                        Sound_Button_Hover.Play();
                                        b.IsPlayedSong = true;
                                    }
                                }
                                else
                                {
                                    b.Hover = false;
                                    b.IsPlayedSong = false;
                                }

                            }
                            #endregion
                            break;
                    }
                    break;
                case SPlayerAction.ActionType.MOUSE_CLICKED_LEFT:
                    switch (gameState)
                    {
                        case GameState.PLAYING:
                            #region GameState_Playing
                            if (PlayerAction.Y < 450)
                            {
                                SelectedUnit = -1;
                                SelectedObject = -1;
                                if (Menu == Menus.OBJECT_MENU) Menu = Menus.NONE;
                                if (Menu == Menus.UNIT_MENU) Menu = Menus.NONE;
                                if (Menu == Menus.BUILD_MENU && ClickedElement == ClickedElements.NONE) Menu = Menus.NONE;
                            }
                            foreach (CMenuItem m in MenuItems)
                            {
                                if (m.Visible)
                                {
                                    if (m.IsIn(mouse.X, mouse.Y))
                                    {
                                        m.Selected = true;
                                        switch (m.GetID())
                                        {
                                            case 1:
                                                Menu = Menus.BUILD_MENU;
                                                break;

                                            case 2:
                                                ClickedElement = ClickedElements.BUILD_HOUSE;
                                                break;

                                            case 3:
                                                ClickedElement = ClickedElements.BUILD_BARRACKS;
                                                break;

                                            case 4:
                                                ClickedElement = ClickedElements.BUILD_WALL;
                                                break;

                                            case 5:
                                                ClickedElement = ClickedElements.BUILD_ROAD;
                                                break;

                                            case 6:
                                                ClickedElement = ClickedElements.BUILD_GATE;
                                                break;

                                            case 7:
                                                ClickedElement = ClickedElements.BUILD_FARM;
                                                break;

                                            case 8:
                                                ClickedElement = ClickedElements.BUILD_MINE;
                                                break;

                                            case 9:
                                                NetOutgoingMessage outmsg = Client.CreateMessage();
                                                outmsg.Write((int)PacketType.PLAYERACTION);
                                                outmsg.Write(PlayerID);
                                                outmsg.Write((int)PlayerActions.BUILD_WATCHTOWER);
                                                outmsg.Write(SelectedUnit);
                                                Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                                                break;
                                        }
                                    }
                                }
                            }
                            foreach (SVisibleUnit u in VisibleUnits)
                            {
                                if (u.IsOver(PlayerAction.X - Camera.X, PlayerAction.Y - Camera.Y) && u.PlayerID == PlayerID)
                                {
                                    SelectedUnit = u.UnitID;
                                    Menu = Menus.UNIT_MENU;
                                    //Console.WriteLine(u.UnitID.ToString());
                                }
                            }
                            foreach (SVisibleBlocks u in VisibleBlocks)
                            {
                                if (u.IsOver(PlayerAction.X - Camera.X, PlayerAction.Y - Camera.Y) && u.PlayerID == PlayerID)
                                {
                                    SelectedObject = u.ID;
                                    Menu = Menus.OBJECT_MENU;
                                    //Console.WriteLine(u.UnitID.ToString());
                                }
                            }
                            #endregion
                            break;

                        case GameState.MAINMENU:
                            #region GameState_MainMenu
                            foreach (CButton b in MainMenuButtons)
                            {
                                if (b.Hover && b.Visible)
                                {
                                    switch (b.Category)
                                    {
                                        case 0:
                                            switch (b.ID)
                                            {
                                                case 0:
                                                    this.Exit();
                                                    break;

                                                case 1:
                                                    if (NameInputWindow.ReturnText() != null)
                                                    {
                                                        Player.Name = NameInputWindow.ReturnText();
                                                        mainMenuCategory = MainMenuCategory.SECOND;
                                                    }
                                                    break;
                                            }
                                            break;

                                        case 1:
                                            switch (b.ID)
                                            {
                                                case 0:
                                                    mainMenuCategory = MainMenuCategory.IP_ENTER;
                                                    break;

                                                case 1:
                                                    mainMenuCategory = MainMenuCategory.SEARCHSERVERS;
                                                    break;

                                                case 2:
                                                    this.Exit();
                                                    break;
                                            }
                                            break;

                                        case 2:
                                            switch (b.ID)
                                            {
                                                case 0:
                                                    InitializeClient(Player.Name, IPInputWindow.ReturnText());
                                                    gameState = GameState.LOBBY;
                                                    break;

                                                case 1:
                                                    this.Exit();
                                                    break;
                                            }
                                            break;
                                    }
                                }
                            }
                            #endregion
                            break;
                    }
                    break;

                case SPlayerAction.ActionType.MOUSE_CLICKED_RIGHT:
                    switch (Menu)
                    {
                        default:
                            if (PlayerAction.Y > 450) Menu = Menus.NONE;
                            ClickedElement = ClickedElements.NONE;
                            break;
                    }
                    if (SelectedUnit != -1)
                    {
                        NetOutgoingMessage outmsg = Client.CreateMessage();
                        outmsg.Write((int)PacketType.PLAYERACTION);
                        outmsg.Write(PlayerID);
                        outmsg.Write((int)PlayerActions.MOVE_UNIT);
                        outmsg.Write(SelectedUnit);
                        outmsg.Write(PlayerAction.X - Camera.X);
                        outmsg.Write(PlayerAction.Y - Camera.Y);
                        Client.SendUnconnectedMessage(outmsg, Master_Server_IP);
                    }
                    break;
            }
        }
        
            


        private void ManageMenus()
        {
            if (Menu == Menus.NONE || Menu == Menus.BUILD_MENU || Menu == Menus.UNIT_MENU)
            {
                foreach (CMenuItem m in MenuItems)
                {
                    m.Visible = false;
                    m.Hover = false;
                    m.Selected = false;
                    switch (ClickedElement)
                    {
                        case ClickedElements.BUILD_HOUSE:
                            if (m.GetID() == 2)
                            {
                                m.Selected = true;
                            }
                            break;

                        case ClickedElements.BUILD_BARRACKS:
                            if (m.GetID() == 3)
                            {
                                m.Selected = true;
                            }
                            break;

                        case ClickedElements.BUILD_WALL:
                            if (m.GetID() == 4)
                            {
                                m.Selected = true;
                            }
                            break;

                        case ClickedElements.BUILD_ROAD:
                            if (m.GetID() == 5)
                            {
                                m.Selected = true;
                            }
                            break;

                        case ClickedElements.BUILD_GATE:
                            if (m.GetID() == 6)
                            {
                                m.Selected = true;
                            }
                            break;

                        case ClickedElements.BUILD_FARM:
                            if (m.GetID() == 7)
                            {
                                m.Selected = true;
                            }
                            break;

                        case ClickedElements.BUILD_MINE:
                            if (m.GetID() == 8)
                            {
                                m.Selected = true;
                            }
                            break;
                    }
                    switch (Menu)
                    {
                        case Menus.NONE:
                            if (m.GetID() == 1)
                            {
                                m.Visible = true;
                            }
                            break;

                        case Menus.UNIT_MENU:
                            int index = -1;
                            index = VisibleUnits.FindIndex(i => i.UnitID == SelectedUnit && i.PlayerID == PlayerID);
                            if (index != -1)
                            {
                                if (m.GetID() == 9)
                                {
                                    m.Visible = true;
                                }
                            }
                            break;

                        case Menus.BUILD_MENU:
                            if (m.GetID() == 2)
                            {
                                m.Visible = true;
                            }
                            if (m.GetID() == 3)
                            {
                                m.Visible = true;
                            }
                            if (m.GetID() == 4)
                            {
                                m.Visible = true;
                            }
                            if (m.GetID() == 5)
                            {
                                m.Visible = true;
                            }
                            if (m.GetID() == 6)
                            {
                                m.Visible = true;
                            }
                            if (m.GetID() == 7)
                            {
                                m.Visible = true;
                            }
                            if (m.GetID() == 8)
                            {
                                m.Visible = true;
                            }
                            break;

                    }
                }
            }
        }

        private void CameraMove()
        {
            if (mouse.X < 100)
            {
                Camera.X += 1;
                if (mouse.X < 66)
                {
                    Camera.X += 2;
                    if (mouse.X < 33)
                    {
                        Camera.X += 4;
                    }
                }
            }
            if (mouse.X > WindowWidth - 100)
            {
                Camera.X += -1;
                if (mouse.X > WindowWidth - 66)
                {
                    Camera.X += -2;
                    if (mouse.X > WindowWidth - 33)
                    {
                        Camera.X += -4;
                    }
                }
            }
            if (mouse.Y < 100)
            {
                Camera.Y += 1;
                if (mouse.Y < 66)
                {
                    Camera.Y += 2;
                    if (mouse.Y < 33)
                    {
                        Camera.Y += 4;
                    }
                }
            }
            if (mouse.Y > WindowHeight - 50)
            {
                Camera.Y += -1;
                if (mouse.Y > WindowHeight - 33)
                {
                    Camera.Y += -2;
                    if (mouse.Y > WindowHeight - 11)
                    {
                        Camera.Y += -4;
                    }
                }
            }
            if (-Camera.X < 0) Camera.X = 0;
            if (-Camera.X > Blocks.GetLength(0) * 50 - WindowWidth) Camera.X = - (Blocks.GetLength(0) * 50 - WindowWidth);
            if (-Camera.Y < 0) Camera.Y = 0;
            if (-Camera.Y > Blocks.GetLength(1) * 50 - WindowHeight) Camera.Y = - (Blocks.GetLength(1) * 50 - WindowHeight);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.MAINMENU:
                    DrawMainMenu();
                    DrawMouse();
                    break;

                case GameState.LOBBY:
                    DrawLobby();
                    break;

                case GameState.PLAYING:
                    DrawGame();
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMainMenu()
        {
            spriteBatch.Draw(MainMenu, new Vector2(0, 0), Color.White);
            foreach (CButton b in MainMenuButtons)
            {
                if (b.Visible)
                {
                    if (b.Hover) spriteBatch.Draw(SpriteSheet, new Vector2(b.X, b.Y), new Rectangle(600, 50, 250, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                    else spriteBatch.Draw(SpriteSheet, new Vector2(b.X, b.Y), new Rectangle(600, 0, 250, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(Font3, b.Text, new Vector2(b.X + (b.Width - b.Text.Length*16) / 2 + 2, b.Y + 12), Color.Black);
                    spriteBatch.DrawString(Font3, b.Text, new Vector2(b.X + (b.Width - b.Text.Length*16) / 2, b.Y + 10), Color.WhiteSmoke);
                }
            }
            //spriteBatch.DrawString(Font1, "Enter Nick:", new Vector2(200, 100), Color.White);
            if (mainMenuCategory == MainMenuCategory.MAIN)
            {
                
                spriteBatch.Draw(SpriteSheet, new Vector2(NameInputWindow.X, NameInputWindow.Y), new Rectangle(600, 100, 250, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                string text;
                if (NameInputWindow.ReturnText() != null)
                {
                    text = NameInputWindow.ReturnText() + "_";
                }
                else
                {
                    text = "_";
                }
                spriteBatch.DrawString(Font3, text, new Vector2(NameInputWindow.X + (NameInputWindow.Width - text.Length * 16) / 2 + 2, NameInputWindow.Y + 12), Color.Black);
                spriteBatch.DrawString(Font3, text, new Vector2(NameInputWindow.X + (NameInputWindow.Width - text.Length * 16) / 2, NameInputWindow.Y + 10), Color.WhiteSmoke);
            }
            if (mainMenuCategory == MainMenuCategory.IP_ENTER)
            {
                spriteBatch.Draw(SpriteSheet, new Vector2(IPInputWindow.X, IPInputWindow.Y), new Rectangle(600, 100, 250, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                string text;
                if (IPInputWindow.ReturnText() != null)
                {
                    text = IPInputWindow.ReturnText() + "_";
                }
                else
                {
                    text = "_";
                }
                spriteBatch.DrawString(Font3, text, new Vector2(IPInputWindow.X + (IPInputWindow.Width - text.Length * 16) / 2 + 2, IPInputWindow.Y + 12), Color.Black);
                spriteBatch.DrawString(Font3, text, new Vector2(IPInputWindow.X + (IPInputWindow.Width - text.Length * 16) / 2, IPInputWindow.Y + 10), Color.WhiteSmoke);           
            }
        }

        private void DrawLobby()
        {
            spriteBatch.DrawString(Font1, "Players: ", new Vector2(200, 50), Color.White);
            int y = 100;
            if (Players != null)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    if (Players[i].Name != " ")
                    {
                        spriteBatch.DrawString(Font1, Players[i].Name, new Vector2(200, y), Color.White);
                        y += 30;
                    }
                }
            }
        }

        private void DrawGUI()
        {
            foreach (SVisibleUnit u in VisibleUnits)
            {
                if (u.IsOver(PlayerAction.X - Camera.X, PlayerAction.Y - Camera.Y))
                {
                    DrawRectangle(4, new Rectangle(u.X + Camera.X, u.Y + Camera.Y, 50, 50), Color.Yellow);
                }
                if (u.Hp < u.MaxHp)
                {
                    DrawFillRectangle(Color.Black, u.X + Camera.X + 2, u.Y + Camera.Y + 38, 10, 46);
                    DrawFillRectangle(Color.Green, u.X + Camera.X + 4, u.Y + Camera.Y + 40, 8, (int)((float)((float)u.Hp / u.MaxHp) * 42));
                }
            }
            foreach (SVisibleBlocks u in VisibleBlocks)
            {
                
                if (u.Hp < u.MaxHp)
                {
                    DrawFillRectangle(Color.Black, u.X + Camera.X + 2, u.Y + Camera.Y + 38, 10, 46);
                    DrawFillRectangle(Color.Green, u.X + Camera.X + 4, u.Y + Camera.Y + 40, 8, (int)((float)((float)u.Hp / u.MaxHp) * 42));
                }
                if (u.ObjectID == 100)
                {
                    DrawFillRectangle(Color.Black, u.X + Camera.X + 2, u.Y + Camera.Y + 38, 10, 46);
                    Color col = Color.Green;
                    if (u.PlayerID == 0) col = Color.Blue;
                    if (u.PlayerID == 1) col = Color.Red;
                    DrawFillRectangle(col, u.X + Camera.X + 4, u.Y + Camera.Y + 40, 8, (int)((float)((float)u.Loyality / 10000) * 42));
                }
                if (u.IsOver(PlayerAction.X - Camera.X, PlayerAction.Y - Camera.Y))
                {
                    DrawRectangle(4, new Rectangle(u.X + Camera.X, u.Y + Camera.Y, 50, 50), Color.Yellow);
                }
                if (SelectedUnit != -1)
                {
                    int index = -1;
                    index = VisibleUnits.FindIndex(i => i.UnitID == SelectedUnit && i.PlayerID == PlayerID);
                    if (index != -1)
                    {
                        DrawFillRectangle(Color.Black, 25, 575, 10, 50);
                        DrawFillRectangle(Color.Lime, 27, 577, 6, (int)((float)VisibleUnits[index].Hp / VisibleUnits[index].MaxHp) * 46);
                        spriteBatch.DrawString(Font1, VisibleUnits[index].Hp.ToString() + " / " + VisibleUnits[index].MaxHp.ToString(), new Vector2(21, 585), Color.Black);
                        spriteBatch.DrawString(Font1, VisibleUnits[index].Hp.ToString() + " / " + VisibleUnits[index].MaxHp.ToString(), new Vector2(20, 584), Color.White);
                        DrawRectangle(6, new Rectangle(VisibleUnits[index].X + Camera.X, VisibleUnits[index].Y + Camera.Y, 50, 50), Color.Red);
                    }
                }
            spriteBatch.Draw(GUITexture, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(Font2, "Resources: ", new Vector2(12, 2), Color.Black);
            spriteBatch.DrawString(Font2, "Resources: ", new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(Font2, Player.Resources.ToString() + " (" + Player.ResourcesIncome.ToString() + ") ", new Vector2(12, 17), Color.Black);
            spriteBatch.DrawString(Font2, Player.Resources.ToString() + " (" + Player.ResourcesIncome.ToString() + ") ", new Vector2(10, 15), Color.White);
            spriteBatch.DrawString(Font2, "Food: ", new Vector2(212, 2), Color.Black);
            spriteBatch.DrawString(Font2, "Food: ", new Vector2(210, 0), Color.White);
            spriteBatch.DrawString(Font2, Player.Food.ToString() + " (" + Player.FoodIncome.ToString() + ") ", new Vector2(212, 17), Color.Black);
            spriteBatch.DrawString(Font2, Player.Food.ToString() + " (" + Player.FoodIncome.ToString() + ") ", new Vector2(210, 15), Color.White);
            spriteBatch.DrawString(Font2, "Population: ", new Vector2(412, 2), Color.Black);
            spriteBatch.DrawString(Font2, "Population: ", new Vector2(410, 0), Color.White);
            spriteBatch.DrawString(Font2, Player.Population.ToString() + " / " + Player.MaxPopulation.ToString(), new Vector2(412, 17), Color.Black);
            spriteBatch.DrawString(Font2, Player.Population.ToString() + " / " + Player.MaxPopulation.ToString(), new Vector2(410, 15), Color.White);
            if (Menu == Menus.BUILD_MENU || Menu == Menus.NONE || Menu == Menus.UNIT_MENU)
            {
                foreach (CMenuItem m in MenuItems)
                {
                    if (m.Visible)
                    {
                        m.Draw(spriteBatch, Font1);
                    }
                    //Console.WriteLine(m.Hover);
                }
            }
            
                /*if (u.ObjectID == 100)
                {
                    DrawFillRectangle(Color.Black, u.X + Camera.X + 2, u.Y + Camera.Y + 38, 10, 46);
                    DrawFillRectangle(Color., u.X + Camera.X + 4, u.Y + Camera.Y + 40, 8, (int)((float)(u.Hp / u.MaxHp) * 42));
                }*/
            }
            if (SelectedObject != -1)
            {
                int index = -1;
                index = VisibleBlocks.FindIndex(i => i.ID == SelectedObject && i.PlayerID == PlayerID);
                if (index != -1)
                {
                    DrawRectangle(6, new Rectangle(VisibleBlocks[index].X + Camera.X, VisibleBlocks[index].Y + Camera.Y, 50, 50), Color.Red);
                    if (Menu == Menus.OBJECT_MENU)
                    {
                        switch (VisibleBlocks[index].ObjectID)
                        {
                            case 20:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(0, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 21:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 22:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 23:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 24:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 25:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 26:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 27:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);                      
                                break;
                            case 28:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);                       
                                break;
                            case 29:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 30:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 31:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 32:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 33:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 34:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 35:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 36:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 37:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 38:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 39:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 40:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 200 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 70:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(0, 150 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 71:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 72:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 73:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 74:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 75:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 76:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 77:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 78:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 79:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 80:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 81:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 82:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 83:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 84:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 85:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 86:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 87:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 88:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 89:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 90:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 150 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 50:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(0, 0 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 51:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(50, 0 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 52:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 0 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 53:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 0 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 60:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(0, 50 + (PlayerID) * 250, 100, 100), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 100:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 0 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 110:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 100 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 111:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 100 + (PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 120:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(100, 100 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 121:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(200, 50 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 122:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(250, 100 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 123:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(300, 150 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                            case 130:
                                spriteBatch.Draw(SpriteSheet, new Vector2(25, 520), new Rectangle(150, 100 + (PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                break;
                        }
                        DrawFillRectangle(Color.Black, 25, 575, 10, 50);
                        DrawFillRectangle(Color.Lime, 27, 577, 6, (int)((float)VisibleBlocks[index].Hp/VisibleBlocks[index].MaxHp)*46);
                        spriteBatch.DrawString(Font1, VisibleBlocks[index].Hp.ToString() + " / " + VisibleBlocks[index].MaxHp.ToString(), new Vector2(21, 585), Color.Black);
                        spriteBatch.DrawString(Font1, VisibleBlocks[index].Hp.ToString() + " / " + VisibleBlocks[index].MaxHp.ToString(), new Vector2(20, 584),Color.White);
                        
                    }
                }             
            }
            
        }

        private void DrawGame()
        {

            if (IsMapLoaded)
            {
                #region Blocks

                for (int i = 0; i < Blocks.GetLength(0); i++)
                {
                    for (int j = 0; j < Blocks.GetLength(1); j++)
                    {
                        if (Blocks[i, j].TerrainType == 0) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(300, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].TerrainType == 1) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(600, 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].TerrainType == 2) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(700, 150, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].TerrainType == 3) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(600, 150, 100, 100), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].TerrainType == 4) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(750, 200, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].TerrainType == 5) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(750, 150, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].TerrainType == 6) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(700, 200, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        //if (Blocks[i, j].TerrainType == -1) spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(300, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (Blocks[i, j].PlayerID != -1)
                        {
                            spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(350, 0 + Blocks[i, j].PlayerID * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        }
                        if (Menu == Menus.BUILD_MENU)
                        {
                            if (!Blocks[i, j].Buildable)
                                spriteBatch.Draw(SpriteSheet, new Vector2((Blocks[i, j].X + Camera.X), (Blocks[i, j].Y + Camera.Y)), new Rectangle(150, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        }
                    }
                }

                foreach (SVisibleBlocks block in VisibleBlocks)
                {
                    if (block.ObjectID != -1)
                    {
                  
                        //Console.Write(block.ObjectID.ToString() + " ");
                        if (block.ObjectID == 20) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(0, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 21) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 22) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 23) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 24) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 25) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 26) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 27) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 28) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 29) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 30) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 31) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 32) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 33) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 34) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 35) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 36) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 37) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 38) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 39) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 40) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 200 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 70) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(0, 150 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 71) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 72) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 73) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 74) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 75) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 76) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 77) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 78) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 79) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 80) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 81) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 82) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 83) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 84) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 85) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 86) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 87) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 88) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 89) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 90) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 150 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 50) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(0, 0 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 51) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(50, 0 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 52) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 0 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 53) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 0 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 60) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(0, 50 + (block.PlayerID) * 250, 100, 100), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 100) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 0 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 110) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 100 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 111) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 100 + (block.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 120) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(100, 100 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 121) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(200, 50 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 122) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(250, 100 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 123) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(300, 150 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (block.ObjectID == 130) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 100 + (block.PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);

                        if (!block.Builded)
                        {
                            spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(150, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                            int pr = (int)(((float)(block.MaxBuildingTime - block.BuildingTime) / block.MaxBuildingTime) * 100);
                            if (pr < 14) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(300, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                            else
                                if (pr < 29) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(350, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                else
                                    if (pr < 43) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(400, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                    else
                                        if (pr < 57) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(450, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                        else
                                            if (pr < 71) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(300, 100, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                            else
                                                if (pr < 85) spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(350, 100, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                                                else spriteBatch.Draw(SpriteSheet, new Vector2(block.X + Camera.X, block.Y + Camera.Y), new Rectangle(400, 100, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        }

                        if (block.ObjectID == 100)
                        {

                        }
                    }
                }
            
                #endregion
                DrawUnits();
                DrawGUI();
                DrawMouse();
            }
            else
            {
                spriteBatch.DrawString(Font1, "Map is loading: " + MapCount.ToString() + " / " + (Blocks.GetLength(0)*Blocks.GetLength(1)).ToString() + " (" + ((int)(((float)MapCount/(float)(Blocks.GetLength(0)*Blocks.GetLength(1)) * 100))).ToString() + "%)" , new Vector2(300, 290), Color.Red);
            }          
        }

        private void DrawUnits()
        {
            foreach (SVisibleUnit u in VisibleUnits)
            {
                spriteBatch.Draw(SpriteSheet, new Vector2(u.X + Camera.X, u.Y + Camera.Y), new Rectangle(250, 0 + (u.PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
            }
        }

        public void DrawRectangle(int bw, Rectangle r, Color c)
        {
            var t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData(new[] { Color.Yellow });
            spriteBatch.Draw(t, new Rectangle(r.Left - bw/2, r.Top, bw, r.Height), c); // Left
            spriteBatch.Draw(t, new Rectangle(r.Right - bw / 2, r.Top, bw, r.Height + bw), c); // Right
            spriteBatch.Draw(t, new Rectangle(r.Left - bw / 2 , r.Top, r.Width, bw), c); // Top
            spriteBatch.Draw(t, new Rectangle(r.Left - bw / 2, r.Bottom, r.Width, bw), c); // Bottom
        }
        
        private void DrawFillRectangle(Color c, int x, int y, int h, int w)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { c });
            spriteBatch.Draw(texture, new Rectangle(x, y, w, h), c);
        }

        private void DrawMouse()
        {
            spriteBatch.Draw(TPointer, new Vector2(mouse.X, mouse.Y), null, Color.White, 0, new Vector2(20, 20), 1, SpriteEffects.None, 0);
            
        }
    }
}
