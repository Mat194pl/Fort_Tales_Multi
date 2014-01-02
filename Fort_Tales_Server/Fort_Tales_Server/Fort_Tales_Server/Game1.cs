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
using Fort_Tales;

namespace Fort_Tales_Server
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
        bool PlayersInLobby = false;

        SPlayerAction PlayerAction;

        int PlayersCount;

        enum PlayerActions { CAMERA_CHANGE, BUILD_HOUSE, BUILD_BARRACKS, BUILD_WALL, BUILD_ROAD };

        CBlock[,] Blocks = new CBlock[100, 300];
        CWallBuilder WallBuilder;
        CRoadBuilder RoadBuilder;
        CBuilder Builder;
        CRoadFinder RoadFinder;
        CSelection Selection = new CSelection();
        SpriteFont Font1;

        CPlayer[] Players = new CPlayer[4];

        enum PacketType { LOGIN, PLAYERDATA, PLAYERSINLOBBY, OBJECT, PLAYERACTION, PLAYERID, PING, GAMEOBJECTS };
        enum GameType { LOBBY, PLAYING }
        GameType gameType;
        // Server object
        static NetServer Server;
        // Configuration object
        static NetPeerConfiguration Config;

        public Game1()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 300; j++)
                {
                    Blocks[i, j] = new CBlock();
                    Blocks[i, j].Init(i, j);
                }
            }
            WallBuilder = new CWallBuilder(ref Blocks);
            RoadBuilder = new CRoadBuilder(ref Blocks);
            Builder = new CBuilder(ref Blocks);
            RoadFinder = new CRoadFinder(ref Blocks);
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
            gameType = GameType.LOBBY;
            Blocks[2, 8].SetObjectID(50);
            Blocks[3, 10].SetObjectID(52);
            Blocks[3, 10].SetPlayerID(1);
            for (int i = 0; i < Players.GetLength(0); i++)
            {
                Players[i] = new CPlayer();
            }
            InitializeServer();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Font");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            ProcessSendingNetworkMessages();
            ProcessNetworkMessages();
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }



        private void ProcessSendingNetworkMessages()
        {
            NetOutgoingMessage outmsg = Server.CreateMessage();
            switch (gameType)
            {
                case GameType.LOBBY:
                    if (PlayersInLobby)
                    {
                        outmsg.Write((Byte)PacketType.PLAYERSINLOBBY);
                        outmsg.Write(Players.GetLength(0));
                        if (Players.GetLength(0) != 0)
                        {
                            foreach (CPlayer p in Players)
                            {
                                if (p.IsPlaying())
                                {
                                    outmsg.Write(p.Name);
                                }
                            }
                        }
                        Server.SendMessage(outmsg, Server.Connections[0], NetDeliveryMethod.ReliableOrdered, 0);
                    }
                    break;

                case GameType.PLAYING:
                    GameplayUpdate();
                    SendGameplay();
                    break;
            }

        }

        private void GameplayUpdate()
        {

        }

        private void SendGameplay()
        {
            foreach (CPlayer p in Players)
            {
                if (p.IsPlaying())
                {
                    SVisibleBlocks[,] VisibleBlocks = new SVisibleBlocks[18, 14];
                    List<SVisibleUnit> VisibleUnits = new List<SVisibleUnit>();
                    int startx = (p.Camera.X - (p.Camera.X / 50) * 50);
                    int starty = (p.Camera.Y - (p.Camera.Y / 50) * 50);
                    int a = 0;
                    int b = 0;
                    //Console.WriteLine("\nMap:\n\n");
                    for (int j = (-p.Camera.Y / 50) - 1; j < ((-p.Camera.Y + 600) / 50 + 1); j++)
                    {
                        for (int i = (-p.Camera.X / 50) - 1; i < ((-p.Camera.X + 800) / 50 + 1); i++)
                        {
                            //Console.WriteLine(i.ToString() + " " + j.ToString());
                            if (i >= 0 && i < Blocks.GetLength(0) && j >= 0 && j < Blocks.GetLength(1))
                            {
                                VisibleBlocks[a, b].ObjectID = Blocks[i, j].GetObjectID();
                                VisibleBlocks[a, b].PlayerID = Blocks[i, j].GetPlayerID();
                                VisibleBlocks[a, b].X = Blocks[i, j].GetX();
                                VisibleBlocks[a, b].Y = Blocks[i, j].GetY();
                            }
                            else
                            {
                                VisibleBlocks[a, b].ObjectID = -1;
                                VisibleBlocks[a, b].PlayerID = -1;
                            }
                            a++;
                        }
                        a = 0;
                        b++;
                    }
                    int countunits = 0;
                    foreach (CUnit u in p.Units)
                    {
                        if (u.realX > startx && u.realX < startx + 20 * 50 && u.realY > starty && u.realY < starty + 14 * 50)
                        {
                            VisibleUnits.Add(new SVisibleUnit((int)u.Type, p.ID, (int)u.realX + (int)p.Camera.X, (int)u.realY + (int)p.Camera.Y));
                            countunits++;
                        }
                    }
                    NetOutgoingMessage outmsg = Server.CreateMessage();
                    outmsg.Write((byte)PacketType.GAMEOBJECTS);
                    outmsg.Write(VisibleBlocks.GetLength(0));
                    outmsg.Write(VisibleBlocks.GetLength(1));
                    outmsg.Write(startx);
                    outmsg.Write(starty);
                    for (int i = 0; i < 14; i++)
                    {
                        for (int j = 0; j < 18; j++)
                        {
                            outmsg.Write(VisibleBlocks[j, i].ObjectID);
                            outmsg.Write(VisibleBlocks[j, i].PlayerID);
                            outmsg.Write(VisibleBlocks[j, i].X);
                            outmsg.Write(VisibleBlocks[j, i].Y);
                        }
                    }
                    outmsg.Write(countunits);
                    for (int i = 0; i < countunits; i++)
                    {
                        outmsg.Write(VisibleUnits[i].UnitID);
                        outmsg.Write(VisibleUnits[i].PlayerID);
                        outmsg.Write(VisibleUnits[i].X);
                        outmsg.Write(VisibleUnits[i].Y);
                    }
                    Server.SendMessage(outmsg, Server.Connections[p.ID], NetDeliveryMethod.ReliableOrdered);
                }
            }
        }


        private void ProcessNetworkMessages()
        {
            NetIncomingMessage inc;
            if ((inc = Server.ReadMessage()) != null)
            {
                switch (gameType)
                {
                    case GameType.LOBBY:
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.ConnectionApproval:
                                if (inc.ReadByte() == (byte)PacketType.LOGIN)
                                {
                                    Console.WriteLine("Incoming LOGIN");
                                    string text = inc.ReadString();
                                    Console.WriteLine(text);
                                    // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                                    inc.SenderConnection.Approve();
                                    //Console.WriteLine(inc.Position.ToString());
                                    int id = PlayersCount;
                                    Players[id] = new CPlayer(text, id);
                                    Builder.BuildWatchtower(10, 10, ref Players[id]);
                                    //Players[0].Camera.X = 20;
                                    NetOutgoingMessage outmsg = Server.CreateMessage();
                                    PlayersInLobby = true;
                                    outmsg.Write((Byte)PacketType.PLAYERID);
                                    outmsg.Write(id);
                                    Server.SendMessage(outmsg, Server.Connections[0], NetDeliveryMethod.ReliableOrdered);
                                    gameType = GameType.PLAYING;
                                    PlayersCount++;
                                }
                                break;

                        }

                        break;

                    case GameType.PLAYING:                        
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.Data:
                                int playerid = inc.ReadInt32();
                                byte action = inc.ReadByte();
                                switch (action)
                                {
                                    case (byte)PlayerActions.CAMERA_CHANGE:
                                        int x = inc.ReadInt32();
                                        int y = inc.ReadInt32();
                                        Players[playerid].Camera.X += x;
                                        Players[playerid].Camera.Y += y;
                                        Console.WriteLine("Camera change: " + x.ToString() + " " + y.ToString() + "   " + Players[playerid].Camera.X.ToString() + " " + Players[playerid].Camera.Y.ToString());
                                        break;

                                    case (byte)PlayerActions.BUILD_HOUSE:
                                        int xa = inc.ReadInt32();
                                        int ya = inc.ReadInt32();
                                        Console.WriteLine("Build house: " + xa.ToString() + " " + ya.ToString());
                                        Builder.BuildHouse(xa, ya, ref Players[playerid]);
                                        break;
                                    
                                    case (byte)PlayerActions.BUILD_BARRACKS:
                                        int xb = inc.ReadInt32();
                                        int yb = inc.ReadInt32();
                                        Console.WriteLine("Build barracks: " + xb.ToString() + " " + yb.ToString());
                                        Builder.BuildBarracks(xb, yb, ref Players[playerid]);
                                        break;

                                    case (byte)PlayerActions.BUILD_WALL:
                                        int xc = inc.ReadInt32();
                                        int yc = inc.ReadInt32();
                                        Console.WriteLine("Build house: " + xc.ToString() + " " + yc.ToString());
                                        WallBuilder.BuildWall(xc, yc, Players[playerid]);
                                        break;

                                    case (byte)PlayerActions.BUILD_ROAD:
                                        int xd = inc.ReadInt32();
                                        int yd = inc.ReadInt32();
                                        Console.WriteLine("Build house: " + xd.ToString() + " " + yd.ToString());
                                        RoadBuilder.BuildRoad(xd, yd, Players[playerid]);
                                        break;
                                }
                                break;
                        }
                        break;
                }
                Server.Recycle(inc);
            }
        }

        private void InitializeServer()
        {
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            Config = new NetPeerConfiguration("game");

            // Set server port
            Config.Port = 14242;

            // Max client amount
            Config.MaximumConnections = 200;

            // Enable New messagetype. Explained later
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);



            // Create new server based on the configs just defined
            Server = new NetServer(Config);

            // Start it
            Server.Start();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(Font1, "Players: ", new Vector2(200, 50), Color.White);
            int y = 100;
            foreach (CPlayer p in Players)
            {
                if (p.IsPlaying())
                {
                    spriteBatch.DrawString(Font1, p.Name, new Vector2(200, y), Color.White);
                    y += 30;
                }
            }
            if (gameType == GameType.PLAYING)
            {
                spriteBatch.DrawString(Font1, "Game Started", new Vector2(200, 10), Color.White);
            }
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
