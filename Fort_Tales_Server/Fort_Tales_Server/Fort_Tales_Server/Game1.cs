using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;
using Fort_Tales;
using System.Net;

namespace Fort_Tales_Server
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public struct SVisibleBlocks
    {
        public int ID;
        public int Hp;
        public int MaxHp;
        public int Loyality;
        public int ObjectID;
        public int PlayerID;
        public bool Builded;
        public int BuilidngTime;
        public int MaxBuildingTime;
        public int X;
        public int Y;

        public SVisibleBlocks(int id, int oid, int pid, bool b, int hp, int maxhp, int l, int bt, int mbt, int x, int y)
        {
            ID = id;
            ObjectID = oid;
            PlayerID = pid;
            Builded = b;
            X = x;
            Y = y;
            Hp = hp;
            MaxHp = maxhp;
            Loyality = l;
            BuilidngTime = bt;
            MaxBuildingTime = mbt;
        }
    }

    public class SMapUpdate
    {
        public int PlayerID;
        public bool Buildable;

        public SMapUpdate(int pid, bool b)
        {
            PlayerID = pid;
            Buildable = b;
        }
    }

    public struct SVisibleUnit
    {
        public int Type;
        public int UnitID;
        public int PlayerID;
        public int Hp;
        public int MaxHp;
        public int X;
        public int Y;

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

        int PlayersCount = 0;

        enum PlayerActions { CAMERA_CHANGE, BUILD_HOUSE, BUILD_BARRACKS, BUILD_WALL, BUILD_ROAD, BUILD_GATE, BUILD_FARM, BUILD_MINE, MOVE_UNIT, BUILD_WATCHTOWER };

        int Timer1 = 0;
        int Timer2 = 0;
        int PlayerSend = 0;

        Texture2D MapTexture;

        CBlock[,] Blocks = new CBlock[60, 60];
        SMapUpdate[,] MapUpdate;
        CWallBuilder WallBuilder;
        CRoadBuilder RoadBuilder;
        CBuilder Builder;
        CRoadFinder RoadFinder;
        CSelection Selection = new CSelection();
        SpriteFont Font1;
        CMapBuilder MapBuilder;

        CPlayer[] Players = new CPlayer[4];

        enum PacketType { LOGIN, PLAYERDATA, PLAYERSINLOBBY, OBJECT, PLAYERACTION, PLAYERID, GAMEOBJECTS, MAP };
        enum MapPacketType { MAP, CREATEMAP, NEEDMAP, UPDATEMAP };
        enum GameType { LOBBY, PLAYING }
        GameType gameType;
        // Server object
        static NetServer Server;
        // Configuration object
        static NetPeerConfiguration Config;

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
            gameType = GameType.LOBBY;
            /*for (int i = 20; i <= 40; i++)
            {
                Blocks[i, 5].SetObjectID(i);
                Blocks[i, 5].SetPlayerID(1);
            }*/
            for (int i = 0; i < Players.GetLength(0); i++)
            {
                Players[i] = new CPlayer();
            }
            MapTexture = Content.Load<Texture2D>("MapTexture");
            
            Blocks = new CBlock[MapTexture.Width, MapTexture.Height];
            for (int i = 0; i < Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < Blocks.GetLength(1); j++)
                {
                    Blocks[i, j] = new CBlock();
                    Blocks[i, j].Init(i, j);
                }
            }
            MapBuilder = new CMapBuilder(ref Blocks, MapTexture);
            MapBuilder.GenerateMapFromTexture();
            WallBuilder = new CWallBuilder(ref Blocks);
            RoadBuilder = new CRoadBuilder(ref Blocks);
            Builder = new CBuilder(ref Blocks);
            RoadFinder = new CRoadFinder(ref Blocks);
            InitializeServer();

            base.Initialize();
            Players[0] = new CPlayer("Bot", 0);
            Players[0].AddUnit(3, 5, ref Blocks, ref RoadFinder);
            Builder.BuildWatchtower(5, 5, ref Players[0]);
            Builder.BuildHouse(4, 5, ref Players[0]);
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
            Config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            Config.MaximumTransmissionUnit = 8191;

            // Create new server based on the configs just defined
            Server = new NetServer(Config);
            // Start it
            Server.Start();
            Server.RegisterReceivedCallback(new SendOrPostCallback(ProcessNetworkMessages));
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
            
            
            
            PlayersCount++;
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
            //ProcessNetworkMessages();
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        private void ProcessSendingNetworkMessages()
        {
            NetOutgoingMessage outmsg = Server.CreateMessage();
            switch (gameType)
            {
                case GameType.LOBBY:
                    /*if (PlayersInLobby)
                    {
                        outmsg.Write((int)PacketType.PLAYERSINLOBBY);
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
                    }*/
                    break;

                case GameType.PLAYING:
                    GameplayUpdate();
                    SendGameplay();
                    break;
            }

        }

        private void GameplayUpdate()
        {
            foreach (CPlayer p in Players)
            {
                if (p.IsPlaying())
                {
                    p.ResourcesIncome = 0;
                    p.FoodIncome = 0;
                    p.FoodIncome -= p.Population * 2;
                    p.ResourcesIncome += p.Population * 4;
                    foreach (CPlayerObject o in p.Objects)
                    {
                        if (o.GetObjectID() == 120)
                        {
                            p.FoodIncome += 4;
                        }
                    }

                    if (Timer2 == 15 && Timer1 == 15)
                    {
                        p.Resources += p.ResourcesIncome;
                        p.Food += p.FoodIncome;
                        if (p.Population < p.MaxPopulation && p.Food > 0) p.Population += p.MaxPopulation / 5;
                        if (p.Resources < 0) p.Resources = 0;
                        if (p.Food < 0) p.Food = 0;
                        if (p.Population > p.MaxPopulation) p.Population = p.MaxPopulation;
                    }
                    foreach (CPlayerObject o in p.Objects)
                    {
                        o.Update();
                    }

                    foreach (CUnit u in p.Units)
                    {
                        if (u.Hp <= 0)
                        {
                            p.Units.Remove(u);
                            break;
                        }
                        u.Move(ref Players[p.ID], ref RoadBuilder);
                        if (u.Way.Count != 0)
                        {
                            if (u.Obstale != 0)
                            {
                                RoadFinder.FindRoad(new Vector2(u.x, u.y), new Vector2(u.dx, u.dy), u, ref Players[p.ID]);
                            }
                            else
                            {
                                u.Move(ref Players[p.ID], ref RoadBuilder);
                            }
                        }
                        foreach (CPlayerObject o in p.Objects)
                        {
                            if (o.GetObjectID() == 100)
                            {
                                if (o.IsNear(u.x, u.y, 5))
                                {
                                    if (o.Loyality <= 10000)
                                    {
                                        o.Loyality++;
                                    }
                                }
                            }
                        }
                        bool attack = false;
                        CUnit au = null;
                        CPlayerObject ao = null;
                        foreach (CPlayer p2 in Players)
                        {
                            if (p != p2)
                            {
                                foreach (CUnit ue in p2.Units)
                                {
                                    if (ue.Hp <= 0)
                                    {
                                        p2.Units.Remove(ue);
                                        break;
                                    }
                                    if (u.IsNear(ue.x, ue.y, 1))
                                    {
                                        if (!attack)
                                        {
                                            attack = true;
                                            au = ue;
                                            break;
                                        }
                                        Console.WriteLine("Enemy is near");
                                    }
                                }
                                try
                                {
                                    foreach (CPlayerObject c in p2.Objects)
                                    {
                                        if (c.GetHp() <= 0)
                                        {
                                            CPlayerObject po = c;
                                            Builder.Remove(ref po, ref Players[p2.ID]);
                                        }
                                        if (c.objectId == 100)
                                        {
                                            if (c.IsNear(u.x, u.y, 5))
                                            {
                                                if (c.Loyality <= 10000)
                                                {
                                                    c.Loyality--;
                                                    //Console.WriteLine("Loyality decrase");
                                                }
                                                if (c.Loyality < 3000)
                                                {
                                                    c.ChangePlayer(ref Players[p.ID], ref Players[p2.ID], ref Blocks);
                                                }
                                            }
                                        }
                                        if (c.IsNear(u.x, u.y, 1))
                                        {
                                            if (!attack)
                                            {
                                                attack = true;
                                                ao = c;
                                            }
                                        }
                                        if (p2.ID != Blocks[c.GetX(), c.GetY()].GetPlayerID())
                                        {
                                            c.ChangePlayer(ref Players[p.ID], ref Players[p2.ID], ref Blocks);
                                        }
                                    }
                                }
                                catch (InvalidOperationException)
                                {

                                }
                            }
                        }
                        if (attack)
                        {
                            if (au != null)
                            {
                                au.Hp--;
                            }
                            if (ao != null)
                            {
                                ao.AttackThis(1);
                            }

                        }
                        attack = false;
                        
                        //List<CPlayerObject> ObjectToRemove = new List<CPlayerObject>();                     
                    }
                }
            }
            
            
            
            if (Timer1 >= 20)
            {
                Timer1 = 0;
                Timer2++;
            }
            if (Timer2 >= 20)
            {
                Timer2 = 0;
            }
            Timer1++;
        }

        private void SendGameplay()
        {
            #region Listing visible untis and objects
            List<SVisibleBlocks> VisibleBlocks = new List<SVisibleBlocks>();
            List<SVisibleUnit> VisibleUnits = new List<SVisibleUnit>();
            foreach (CPlayer p in Players)
            {
                if (p.IsPlaying())
                {
                    foreach (CPlayerObject o in p.Objects)
                    {
                        if (o.objectId == Blocks[o.x, o.y].GetObjectID())
                        {
                            VisibleBlocks.Add(new SVisibleBlocks(o.Id, o.GetObjectID(), p.ID, o.Builded, o.GetHp(), o.GetMaxHp(), o.Loyality, o.BuildingTime, o.MaxBuildingTime, o.x * 50, o.y * 50));
                        }
                        else
                        {
                            VisibleBlocks.Add(new SVisibleBlocks(o.Id, Blocks[o.x, o.y].GetObjectID(), p.ID, o.Builded, o.GetHp(), o.GetMaxHp(), o.Loyality, o.BuildingTime, o.MaxBuildingTime, o.x * 50, o.y * 50));
                        }
                    }
                    foreach (CUnit u in p.Units)
                    {
                        VisibleUnits.Add(new SVisibleUnit(u.ID, u.Type, p.ID, u.Hp, u.MaxHp, (int)u.realX, (int)u.realY));
                    }
                }
            }
            #endregion
            foreach (CPlayer p in Players)
            {
                if (p.IsPlaying() && !p.IsMapLoaded && p.NeedMap && p.Name != "Bot")
                {
                    //Console.WriteLine("Sending Map " + p.MapCount);
                    #region NEEDMAP
                    NetOutgoingMessage outmsg = Server.CreateMessage();
                    outmsg.Write((int)PacketType.MAP);
                    outmsg.Write((int)PacketType.MAP);
                    int count = 0;
                    outmsg.Write((int)50);
                    if (p.MapLastX == -1 && p.MapLastY == -1)
                    {
                        p.MapLastX = 0;
                        p.MapLastY = 0;
                        p.MapCount = 0;
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        int x = p.MapLastX;
                        int y = p.MapLastY;
                        outmsg.Write(x);
                        outmsg.Write(y);
                        outmsg.Write(Blocks[x, y].Terrain_type);
                        outmsg.Write(Blocks[x, y].GetPlayerID());
                        count++;
                        p.MapCount++;
                        p.MapLastX++;
                        if (p.MapLastX >= Blocks.GetLength(0))
                        {
                            p.MapLastX = 0;
                            p.MapLastY++;
                        }
                        if (p.MapLastY >= Blocks.GetLength(1))
                        {
                            p.IsMapLoaded = true;
                            break;
                        }                        
                    }
                    outmsg.Write(p.MapCount);
                    Server.SendUnconnectedMessage(outmsg, p.IP_external);
                    p.NeedMap = false;
                    #endregion
                }
                else
                {
                    if (p.IsPlaying() && p.Name != "Bot")
                    {
                        NetOutgoingMessage outmsg = Server.CreateMessage();
                        outmsg.Write((int)PacketType.GAMEOBJECTS);
                        outmsg.Write(p.Resources);
                        outmsg.Write(p.ResourcesIncome);
                        outmsg.Write(p.Food);
                        outmsg.Write(p.FoodIncome);
                        outmsg.Write(p.Population);
                        outmsg.Write(p.MaxPopulation);
                        outmsg.Write(VisibleBlocks.Count);
                        for (int i = 0; i < VisibleBlocks.Count; i++)
                        {
                                outmsg.Write(VisibleBlocks[i].ID);
                                outmsg.Write(VisibleBlocks[i].ObjectID);
                                outmsg.Write(VisibleBlocks[i].PlayerID);                               
                                outmsg.Write(VisibleBlocks[i].Builded);
                                outmsg.Write(VisibleBlocks[i].Hp);
                                outmsg.Write(VisibleBlocks[i].MaxHp);
                                outmsg.Write(VisibleBlocks[i].Loyality);
                                outmsg.Write(VisibleBlocks[i].BuilidngTime);
                                outmsg.Write(VisibleBlocks[i].MaxBuildingTime);
                                outmsg.Write(VisibleBlocks[i].X);
                                outmsg.Write(VisibleBlocks[i].Y);
                        }
                        outmsg.Write(VisibleUnits.Count);
                        for (int i = 0; i < VisibleUnits.Count; i++)
                        {
                            outmsg.Write(VisibleUnits[i].UnitID);
                            outmsg.Write(VisibleUnits[i].Type);
                            outmsg.Write(VisibleUnits[i].PlayerID);
                            outmsg.Write(VisibleUnits[i].Hp);
                            outmsg.Write(VisibleUnits[i].MaxHp);
                            outmsg.Write(VisibleUnits[i].X);
                            outmsg.Write(VisibleUnits[i].Y);
                        }
                        Server.SendUnconnectedMessage(outmsg, p.IP_external);
                    }
                }
            }
        }

       

        private void ProcessNetworkMessages(object peer)
        {
            NetIncomingMessage inc;
            if ((inc = Server.ReadMessage()) != null)
            {
                //Console.WriteLine("Incoming Message: ");
                switch (gameType)
                {
                    case GameType.LOBBY:
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.UnconnectedData:
                                if (inc.ReadInt32() == (int)PacketType.LOGIN)
                                {
                                    #region LOGIN_MESSAGE
                                    Console.WriteLine("Incoming LOGIN");
                                    string text = inc.ReadString();
                                    bool Isplayed = false;
                                    foreach (CPlayer pl in Players)
                                    {
                                        if (pl.IsPlaying())
                                        {
                                            if (pl.Name == text)
                                            {
                                                Isplayed = true;
                                            }
                                        }
                                    }
                                    if (!Isplayed)
                                    {
                                        Console.WriteLine(text);
                                        //Console.WriteLine(inc.Position.ToString());
                                        int id = PlayersCount;
                                        PlayersCount++;
                                        Players[id] = new CPlayer(text, id);
                                        Players[id].IP_external = inc.SenderEndPoint;
                                        Players[id].WindowWidth = inc.ReadInt32();
                                        Players[id].WindowHeight = inc.ReadInt32();
                                        Console.WriteLine(Players[id].IP_external.ToString());
                                        //Players[id].IP_internal = inc.ReadIPEndPoint();
                                        Console.WriteLine("Zapisano adresy IP");
                                        //Players[0].Camera.X = 20;
                                        NetOutgoingMessage outmsg = Server.CreateMessage();
                                        PlayersInLobby = true;
                                        outmsg.Write((int)PacketType.PLAYERID);
                                        outmsg.Write(id);
                                        Console.WriteLine(id.ToString() + PlayersCount.ToString());
                                        Server.SendUnconnectedMessage(outmsg, Players[id].IP_external);
                                        //Server.SendMessage(outmsg, Server.Connections[id - 1], NetDeliveryMethod.ReliableOrdered);      
                                        Blocks[10 * id + 2, 20 * id + 2].SetPlayerID(id);
                                        Builder.BuildWatchtower(10 * id + 10, 20 * id + 10, ref Players[id]);
                                        Players[id].AddUnit(10 * id + 11, 20 * id + 10, ref Blocks, ref RoadFinder);
                                        Players[id].AddUnit(10 * id + 10, 20 * id + 11, ref Blocks, ref RoadFinder);
                                        
                                        //Console.WriteLine("Adding Unit");
                                        outmsg = null;
                                        outmsg = Server.CreateMessage();
                                        outmsg.Write((int)PacketType.MAP);
                                        outmsg.Write((int)MapPacketType.CREATEMAP);
                                        outmsg.Write(Blocks.GetLength(0));
                                        outmsg.Write(Blocks.GetLength(1));
                                        Server.SendUnconnectedMessage(outmsg, Players[id].IP_external);
                                        Console.WriteLine("Sended CREATEMAP_MESSAGE" + id.ToString() + " " + Players[id].IP_external.ToString());
                                        Players[id].IsMapLoaded = false;
                                        Players[id].MapLastX = -1;
                                        Players[id].MapLastY = -1;
                                        Players[id].NeedMap = true;
                                        gameType = GameType.PLAYING;
                                        
                                    }
                                    #endregion
                                }
                                break;
                        }

                        break;

                    case GameType.PLAYING:   
                        
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.UnconnectedData:
                                int type = inc.ReadInt32();
                                Console.WriteLine("Type of packet: " + type.ToString());
                                switch (type)
                                {
                                    case (int)PacketType.LOGIN:
                                       #region LOGIN_MESSAGE
                                    Console.WriteLine("Incoming LOGIN");
                                    string text = inc.ReadString();
                                    bool Isplayed = false;
                                        foreach(CPlayer pl in Players)
                                        {
                                            if (pl.Name == text)
                                            {
                                                Isplayed = true;
                                                break;
                                            }

                                        }
                                        if (Isplayed)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine(text);
                                            //Console.WriteLine(inc.Position.ToString());
                                            int id = PlayersCount;
                                            Players[id] = new CPlayer(text, id);
                                            Players[id].IP_external = inc.SenderEndPoint;
                                            Players[id].WindowWidth = inc.ReadInt32();
                                            Players[id].WindowHeight = inc.ReadInt32();
                                            Console.WriteLine(Players[id].IP_external.ToString());
                                            //Players[id].IP_internal = inc.ReadIPEndPoint();
                                            Console.WriteLine("Zapisano adresy IP");
                                            //Players[0].Camera.X = 20;
                                            NetOutgoingMessage outmsg = Server.CreateMessage();
                                            PlayersInLobby = true;
                                            outmsg.Write((int)PacketType.PLAYERID);
                                            outmsg.Write(id);
                                            Console.WriteLine(id.ToString());
                                            Console.WriteLine("Connectiobs count: " + Server.ConnectionsCount.ToString());
                                            Server.SendUnconnectedMessage(outmsg, Players[id].IP_external);
                                            //Server.SendMessage(outmsg, Server.Connections[id - 1], NetDeliveryMethod.ReliableOrdered);      
                                            Blocks[10 * id + 2, 20 * id + 2].SetPlayerID(id);
                                            Builder.BuildWatchtower(10 * id + 10, 20 * id + 10, ref Players[id]);
                                            Players[id].AddUnit(2, 2, ref Blocks, ref RoadFinder);
                                            Players[id].AddUnit(3, 2, ref Blocks, ref RoadFinder);
                                            Players[id].AddUnit(0, 0, ref Blocks, ref RoadFinder);
                                            RoadFinder.FindRoad((new Vector2(2, 2)), new Vector2(2, 10), Players[id].Units[0], ref Players[id]);
                                            RoadFinder.FindRoad((new Vector2(Players[id].Units[1].x, Players[id].Units[1].y)), new Vector2(6, 6), Players[id].Units[1], ref Players[id]);
                                            //Console.WriteLine("Adding Unit");
                                            outmsg = null;
                                            outmsg = Server.CreateMessage();
                                            outmsg.Write((int)PacketType.MAP);
                                            outmsg.Write((int)MapPacketType.CREATEMAP);
                                            outmsg.Write(Blocks.GetLength(0));
                                            outmsg.Write(Blocks.GetLength(1));
                                            Server.SendUnconnectedMessage(outmsg, Players[id].IP_external);
                                            Console.WriteLine("Sended CREATEMAP_MESSAGE" + id.ToString() + " " + Players[id].IP_external.ToString());
                                            Players[id].IsMapLoaded = false;
                                            Players[id].MapLastX = -1;
                                            Players[id].MapLastY = -1;
                                            Players[id].NeedMap = true;
                                            gameType = GameType.PLAYING;
                                            PlayersCount++;
                                        }
                                    #endregion
                                    break;
                                    
                                    case (int)PacketType.MAP:
                                    Console.WriteLine("MapPacketType Incoming");
                                    int ptype = inc.ReadInt32();
                                    Console.WriteLine(ptype.ToString());
                                    switch (ptype)
                                    {
                                        case (int)MapPacketType.NEEDMAP:
                                            Console.WriteLine("Need Map Request");
                                            int playerid2 = inc.ReadInt32();
                                            Players[playerid2].NeedMap = true;
                                            Console.WriteLine("ok");
                                            break;

                                        case (int)MapPacketType.UPDATEMAP:
                                            int playerid3 = inc.ReadInt32();
                                            int x1 = inc.ReadInt32();
                                            int y1 = inc.ReadInt32();
                                            //Console.WriteLine("Player: " + playerid3 + " want to update map!");
                                            MapUpdate = new SMapUpdate[Players[playerid3].WindowWidth / 50, Players[playerid3].WindowHeight / 50];
                                            for (int i = 0; i < MapUpdate.GetLength(0); i++)
                                            {
                                                for (int j = 0; j < MapUpdate.GetLength(1); j++)
                                                {
                                                    if ((i + x1) >= 0 && (i + x1) < Blocks.GetLength(0) && (j + y1) >= 0 && (j + y1) < Blocks.GetLength(1))
                                                    {
                                                        //Console.Write(Blocks[i + x1, j + y1].GetPlayerID().ToString() + " ");
                                                        MapUpdate[i, j] = new SMapUpdate(Blocks[i + x1, j + y1].GetPlayerID(), Blocks[i + x1, j + y1].buildable);
                                                    }
                                                    else
                                                    {
                                                        MapUpdate[i, j] = new SMapUpdate(-2, false);
                                                    }
                                                }
                                                //Console.WriteLine("");
                                            }
                                            NetOutgoingMessage outmsg2 = Server.CreateMessage();
                                            outmsg2.Write((int)PacketType.MAP);
                                            outmsg2.Write((int)MapPacketType.UPDATEMAP);
                                            outmsg2.Write(x1);
                                            outmsg2.Write(y1);
                                            outmsg2.Write(MapUpdate.GetLength(0));
                                            outmsg2.Write(MapUpdate.GetLength(1));
                                            for (int i = 0; i < MapUpdate.GetLength(0); i++)
                                            {
                                                for (int j = 0; j < MapUpdate.GetLength(1); j++)
                                                {
                                                    outmsg2.Write(MapUpdate[i, j].PlayerID);
                                                    outmsg2.Write(MapUpdate[i, j].Buildable);
                                                }
                                            }
                                            Server.SendUnconnectedMessage(outmsg2, Players[playerid3].IP_external);

                                            break;
                                    }

                                        break;

                                    case (int)PacketType.PLAYERACTION:
                                        Console.Write("unconnecteddata: ");

                                        int playerid = inc.ReadInt32();
                                        //playerid = inc.SequenceChannel;
                                        int action = inc.ReadInt32();
                                        int x;
                                        int y;
                                        switch (action)
                                        {
                                            case (int)PlayerActions.CAMERA_CHANGE:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Players[playerid].Camera.X += x;
                                                Players[playerid].Camera.Y += y;
                                                //Console.Write(" changecamera");
                                                //Console.WriteLine("Camera change: " + x.ToString() + " " + y.ToString() + "   " + Players[playerid].Camera.X.ToString() + " " + Players[playerid].Camera.Y.ToString());
                                                break;

                                            case (int)PlayerActions.BUILD_HOUSE:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build house: " + x.ToString() + " " + y.ToString());
                                                Builder.BuildHouse(x, y, ref Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_BARRACKS:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build barracks: " + x.ToString() + " " + y.ToString());
                                                Builder.BuildBarracks(x, y, ref Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_WALL:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build wall: " + x.ToString() + " " + y.ToString());
                                                WallBuilder.BuildWall(x, y, Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_ROAD:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build road: " + x.ToString() + " " + y.ToString());
                                                RoadBuilder.BuildRoad(x, y, Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_GATE:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build gate: " + x.ToString() + " " + y.ToString());
                                                Builder.BuildGate(x, y, ref Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_FARM:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build farm: " + x.ToString() + " " + y.ToString());
                                                Builder.BuildFarm(x, y, ref Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_MINE:
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                Console.WriteLine("Build mine: " + x.ToString() + " " + y.ToString());
                                                Builder.BuildMine(x, y, ref Players[playerid]);
                                                break;

                                            case (int)PlayerActions.BUILD_WATCHTOWER:
                                                int uid = inc.ReadInt32();
                                                int index2 = -1;
                                                index2 = Players[playerid].Units.FindIndex(i => i.ID == uid);
                                                if (index2 != -1)
                                                {
                                                    x = Players[playerid].Units[index2].x;
                                                    y = Players[playerid].Units[index2].y;
                                                    Console.WriteLine("Build watchtower: " + x.ToString() + " " + y.ToString());
                                                    Players[playerid].Units.RemoveAt(index2);
                                                    Builder.BuildWatchtower(x, y, ref Players[playerid]);
                                                }
                                                break;

                                            case (int)PlayerActions.MOVE_UNIT:
                                                int s = inc.ReadInt32();
                                                x = inc.ReadInt32();
                                                y = inc.ReadInt32();
                                                int index = -1;
                                                index = Players[playerid].Units.FindIndex(t => t.ID == s);
                                                if (index != -1)
                                                {
                                                    RoadFinder.FindRoad(new Vector2(Players[playerid].Units[index].x, Players[playerid].Units[index].y), new Vector2((int)(x - Players[playerid].Camera.X) / 50, (int)(y - Players[playerid].Camera.Y) / 50), Players[playerid].Units[index], ref Players[playerid]);
                                                    Console.WriteLine("MoveUnit");
                                                }
                                                break;
                                        }
                                        break;
                                }
                                break;
                        
                        }
                        break;
                }
                Server.Recycle(inc);
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
