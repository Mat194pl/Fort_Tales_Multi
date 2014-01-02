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

namespace Fort_Tales_Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 


    public struct SVisibleBlocks
    {
        public int ObjectID;
        public int PlayerID;
        public int X;
        public int Y;
    }

    public struct SVisibleUnit
    {
        public int UnitID;
        public int PlayerID;
        public int X;
        public int Y;

        public SVisibleUnit(int uid, int playerid, int x, int y)
        {
            UnitID = uid;
            PlayerID = playerid;
            X = x;
            Y = y;
        }
    }

    public struct SPlayerAction
    {
        public enum ActionType { MOUSE_POSITION, MOUSE_CLICKED_LEFT, MOUSE_CLICKED_RIGHT };
        public ActionType Type;
        public int X;
        public int Y;
    }


    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CPlayer Player;
        SpriteFont Font1;
        bool IsConnected;
        SPlayerAction PlayerAction;


        int VisibleStartX;
        int VisibleStartY;
        SVisibleBlocks[,] VisibleBlocks = new SVisibleBlocks[20, 14];
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
        Vector2 Camera;


        Texture2D SpriteSheet;
        Texture2D GUITexture;
        Texture2D TPointer;


        CInputWindow InputWindow;

        NetClient Client;
        NetPeerConfiguration Config;

        enum GameState { MAINMENU_PLAYERNAME, MAINMENU_IP, LOBBY, PLAYING }
        enum PacketType { LOGIN, PLAYERDATA, PLAYERSINLOBBY, OBJECT, PLAYERACTION, PLAYERID, PING, GAMEOBJECTS };
        enum PlayerActions { CAMERA_CHANGE, BUILD_HOUSE, BUILD_BARRACKS, BUILD_WALL, BUILD_ROAD };
        enum ClickedElements { NONE, BUILD_HOUSE, BUILD_BARRACKS, BUILD_WALL, BUILD_ROAD };
        enum Menus { NONE, BUILD_MENU };

        ClickedElements ClickedElement = ClickedElements.NONE;
        Menus Menu = Menus.NONE;
        GameState gameState;

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
            for (int i = 0; i < VisibleBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < VisibleBlocks.GetLength(1); j++)
                {
                    VisibleBlocks[i, j].ObjectID = 0;
                    VisibleBlocks[i, j].PlayerID = 0;
                }
            }
            
            IsConnected = false;
            Player = new CPlayer(" ");
            InputWindow = new CInputWindow();
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.IsFullScreen = false;
            Window.Title = "Fort Tales";
            graphics.ApplyChanges();
            base.Initialize();
        }

        private void InitializeClient(string name, string hostip)
        {
            Config = new NetPeerConfiguration("game")
            {
                //SimulatedMinimumLatency = 0.2f, 
                //SimulatedLoss = 0.1f
            };
            Client = new NetClient(Config);
            Client.Start();
           
            NetOutgoingMessage sendMsg = Client.CreateMessage();
            sendMsg.Write((byte)PacketType.LOGIN);
            sendMsg.Write(name);
            Client.Connect(hostip, 14242, sendMsg);
            IsConnected = true;
            Console.WriteLine("Connection done");
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
            SpriteSheet = Content.Load<Texture2D>("SpriteSheet");
            MenuItems.Add(new CMenuItem(300, 545, 50, 50, ref SpriteSheet, 1));
            MenuItems.Add(new CMenuItem(300, 545, 50, 50, ref SpriteSheet, 2));
            MenuItems.Add(new CMenuItem(350, 545, 50, 50, ref SpriteSheet, 3));
            MenuItems.Add(new CMenuItem(400, 545, 50, 50, ref SpriteSheet, 4));
            MenuItems.Add(new CMenuItem(450, 545, 50, 50, ref SpriteSheet, 5));

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
                case GameState.MAINMENU_PLAYERNAME:
                    if (keyboard != lastkeyboard)
                        InputWindow.AddKey(keyboard);

                    if (InputWindow.IsEnter)
                    {
                        gameState = GameState.MAINMENU_IP;
                        Player.Name = InputWindow.ReturnText();
                        InputWindow.Clear();
                    }
                    break;

                case GameState.MAINMENU_IP:
                    if (keyboard != lastkeyboard)
                        InputWindow.AddKey(keyboard);

                    if (InputWindow.IsEnter)
                    {
                        InitializeClient(Player.Name, InputWindow.ReturnText());
                        Player.Name = InputWindow.ReturnText();
                        InputWindow.Clear();
                        gameState = GameState.LOBBY;
                    }
                    break;

                case GameState.PLAYING:
                    {
                        HandleGameplayInput();
                        ManageMenus();
                        UpdateGUI();
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
            Camera = new Vector2(0, 0);
            CameraMove();
            if (mouse.LeftButton == ButtonState.Pressed && lastmouse.LeftButton == ButtonState.Released)
            {
                PlayerAction.Type = SPlayerAction.ActionType.MOUSE_CLICKED_LEFT;
                Console.WriteLine("Left Click");
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
                        switch (PlayerAction.Type)
                        {
                            case SPlayerAction.ActionType.MOUSE_POSITION:
                                if (Camera.X != 0 || Camera.Y != 0)
                                {
                                    outmsg.Write(PlayerID);
                                    outmsg.Write((byte)PlayerActions.CAMERA_CHANGE);
                                    outmsg.Write((int)Camera.X);
                                    outmsg.Write((int)Camera.Y);
                                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                                }
                                break;

                            case SPlayerAction.ActionType.MOUSE_CLICKED_LEFT:
                                switch (ClickedElement)
                                {
                                    case ClickedElements.BUILD_HOUSE:
                                        if (VisibleBlocks[(PlayerAction.X - VisibleStartX) / 50, (PlayerAction.Y - VisibleStartY) / 50].ObjectID != -1 && (new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                        {
                                            outmsg.Write(PlayerID);
                                            outmsg.Write((byte)PlayerActions.BUILD_HOUSE);
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X));
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y));
                                            Console.WriteLine(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X.ToString() + " " + VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y.ToString());
                                            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                                        }
                                        break;

                                    case ClickedElements.BUILD_BARRACKS:
                                        if (VisibleBlocks[(PlayerAction.X - VisibleStartX) / 50, (PlayerAction.Y - VisibleStartY) / 50].ObjectID != -1 && (new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                        {
                                            outmsg.Write(PlayerID);
                                            outmsg.Write((byte)PlayerActions.BUILD_BARRACKS);
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X));
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y));
                                            Console.WriteLine(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X.ToString() + " " + VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y.ToString());
                                            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                                        }
                                        break;

                                    case ClickedElements.BUILD_WALL:
                                        if (VisibleBlocks[(PlayerAction.X - VisibleStartX) / 50, (PlayerAction.Y - VisibleStartY) / 50].ObjectID != -1 && (new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                        {
                                            outmsg.Write(PlayerID);
                                            outmsg.Write((byte)PlayerActions.BUILD_WALL);
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X));
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y));
                                            Console.WriteLine(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X.ToString() + " " + VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y.ToString());
                                            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                                        }
                                        break;

                                    case ClickedElements.BUILD_ROAD:
                                        if (VisibleBlocks[(PlayerAction.X - VisibleStartX) / 50, (PlayerAction.Y - VisibleStartY) / 50].ObjectID != -1 && (new Rectangle(PlayerAction.X, PlayerAction.Y, 1, 1).Intersects(new Rectangle(0, 0, 800, 500))))
                                        {
                                            outmsg.Write(PlayerID);
                                            outmsg.Write((byte)PlayerActions.BUILD_ROAD);
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X));
                                            outmsg.Write((int)(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y));
                                            Console.WriteLine(VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].X.ToString() + " " + VisibleBlocks[(mouse.X - VisibleStartX) / 50, (mouse.Y - VisibleStartY) / 50].Y.ToString());
                                            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                                        }
                                        break;
                                }
                                break;
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

                            case NetIncomingMessageType.Data:

                                //Console.WriteLine(inc.ReadByte().ToString());
                                byte i = inc.ReadByte();
                                switch (i)
                                {
                                    case (byte)PacketType.OBJECT:
                                        Console.WriteLine("Object");
                                        int nr = inc.ReadInt16();
                                        Console.WriteLine(nr.ToString());
                                        break;

                                    case (byte)PacketType.PLAYERID:
                                        Console.WriteLine("PlayerID");
                                        if (PlayerID == -1)
                                        {
                                            PlayerID = inc.ReadInt32();
                                        }
                                        Console.WriteLine(PlayerID.ToString());
                                        gameState = GameState.PLAYING;
                                        break;

                                    case (byte)PacketType.PLAYERSINLOBBY:
                                        int count = inc.ReadInt32();
                                        Players.Clear();
                                        for (int j = 0; j < count; j++)
                                        {
                                            Players.Add(new CPlayer(inc.ReadString()));
                                        }
                                        break;

                                    default:
                                        Console.WriteLine(inc.ReadByte().ToString());
                                        break;
                                }
                                break;

                        }
                        break;

                    case GameState.PLAYING:
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.Data:
                                switch (inc.ReadByte())
                                {
                                    case (byte)PacketType.GAMEOBJECTS:
                                        int countx = inc.ReadInt32();
                                        int county = inc.ReadInt32();
                                        VisibleUnits.Clear();
                                        VisibleStartX = inc.ReadInt32();
                                        VisibleStartY = inc.ReadInt32();
                                        //Console.WriteLine("\nMap:\n\n");
                                        for (int j = 0; j < county; j++)
                                        {
                                            for (int i = 0; i < countx; i++)
                                            {
                                                VisibleBlocks[i, j].ObjectID = inc.ReadInt32();
                                                VisibleBlocks[i, j].PlayerID = inc.ReadInt32();
                                                VisibleBlocks[i, j].X = inc.ReadInt32();
                                                VisibleBlocks[i, j].Y = inc.ReadInt32();
                                                //Console.Write(VisibleBlocks[i, j].ObjectID.ToString());
                                            }
                                            //Console.WriteLine();
                                        }
                                        int countunits = inc.ReadInt32();
                                        for (int i = 0; i < countunits; i++)
                                        {
                                            VisibleUnits.Add(new SVisibleUnit(inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32(), inc.ReadInt32()));
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

        private void UpdateGUI()
        {
            switch (PlayerAction.Type)
            {
                case SPlayerAction.ActionType.MOUSE_POSITION:
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
                    break;
                case SPlayerAction.ActionType.MOUSE_CLICKED_LEFT:
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
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void ManageMenus()
        {
            foreach (CMenuItem m in MenuItems)
            {
                m.Visible = false;
                m.Selected = false;
                m.Hover = false;
                switch (Menu)
                {
                    case Menus.NONE:
                        if (m.GetID() == 1)
                        {
                            m.Visible = true;
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
                        break;

                }
            }
        }

        private void CameraMove()
        {
            if (mouse.X < 100)
            {
                Camera.X = 1;
                if (mouse.X < 66)
                {
                    Camera.X = 2;
                    if (mouse.X < 33)
                    {
                        Camera.X = 4;
                    }
                }
            }
            if (mouse.X > WindowWidth - 100)
            {
                Camera.X = -1;
                if (mouse.X > WindowWidth - 66)
                {
                    Camera.X = -2;
                    if (mouse.X > WindowWidth - 33)
                    {
                        Camera.X = -4;
                    }
                }
            }
            if (mouse.Y < 100)
            {
                Camera.Y = 1;
                if (mouse.Y < 66)
                {
                    Camera.Y = 2;
                    if (mouse.Y < 33)
                    {
                        Camera.Y = 4;
                    }
                }
            }
            if (mouse.Y > WindowHeight - 100)
            {
                Camera.Y = -1;
                if (mouse.Y > WindowHeight - 66)
                {
                    Camera.Y = -2;
                    if (mouse.Y > WindowHeight - 33)
                    {
                        Camera.Y = -4;
                    }
                }
            }
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
                case GameState.MAINMENU_PLAYERNAME:
                    DrawLobby_PlayerName();
                    break;

                case GameState.MAINMENU_IP:
                    DrawLobby_IP();
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

        private void DrawLobby_PlayerName()
        {
            spriteBatch.DrawString(Font1, "Enter Nick:", new Vector2(200, 100), Color.White);
            if (InputWindow.ReturnText() != null)
                spriteBatch.DrawString(Font1, InputWindow.ReturnText(), new Vector2(200, 120), Color.White);
        }

        private void DrawLobby_IP()
        {
            spriteBatch.DrawString(Font1, "Enter IP:", new Vector2(200, 100), Color.White);
            if (InputWindow.ReturnText() != null)
                spriteBatch.DrawString(Font1, InputWindow.ReturnText(), new Vector2(200, 120), Color.White);
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
            spriteBatch.Draw(GUITexture, new Vector2(0, 0), Color.White);
            foreach (CMenuItem m in MenuItems)
            {
                if (m.Visible)
                {
                    m.Draw(spriteBatch, Font1);
                }
                //Console.WriteLine(m.Hover);
            }
        }

        private void DrawGame()
        {
            
            #region Blocks
            for (int i = 0; i < VisibleBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < VisibleBlocks.GetLength(1); j++)
                    spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(300, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < VisibleBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < VisibleBlocks.GetLength(1); j++)
                {
                    if (VisibleBlocks[i, j].ObjectID != -1)
                    {

                        if (VisibleBlocks[i, j].ObjectID == 20) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(0, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 21) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 22) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 23) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 24) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 25) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 26) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 27) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 28) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 29) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 30) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 31) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 32) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 33) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 34) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 35) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 36) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 37) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 38) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 39) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 40) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 200 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 70) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(0, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 71) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 72) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 73) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 74) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(250, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 75) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 76) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 77) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 78) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 79) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 80) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 81) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 82) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 83) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 84) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 85) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 86) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 87) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)3 / (float)2), new Vector2(50, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 88) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 89) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * ((float)1 / (float)2), new Vector2(0, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 90) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 150 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, MathHelper.Pi * (1), new Vector2(50, 50), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 50) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(0, 0 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 51) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(50, 0 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 52) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(100, 0 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 53) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(150, 0 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 60) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(0, 50 + (VisibleBlocks[i, j].PlayerID) * 250, 100, 100), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                        if (VisibleBlocks[i, j].ObjectID == 100) spriteBatch.Draw(SpriteSheet, new Vector2(VisibleStartX + (i * 50), VisibleStartY + (j * 50)), new Rectangle(200, 0 + (VisibleBlocks[i, j].PlayerID) * 250, 50, 50), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);

                    }
                }
            }
            #endregion
            DrawGUI();
            DrawMouse();
        }

        private void DrawMouse()
        {
            spriteBatch.Draw(TPointer, new Vector2(mouse.X, mouse.Y), null, Color.White, 0, new Vector2(20, 20), 1, SpriteEffects.None, 0);
        }
    }
}
