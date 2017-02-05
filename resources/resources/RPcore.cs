﻿using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

//raptube69
//Ver 1.0.0

public class RPcore : Script
{
    //Calculate distance between two vectors
    public float vecdist(Vector3 v1, Vector3 v2)
    {
        Vector3 vf = new Vector3();
        vf.X = v1.X - v2.X;
        vf.Y = v1.Y - v2.Y;
        vf.Z = v1.Z - v2.Z;
        return (float)Math.Sqrt(vf.X * vf.X + vf.Y * vf.Y + vf.Z * vf.Z);
    }

    //Color data used for identification of color values
    public struct ColorData
    {
        public List<int> colors; //Color values
        public string color_name; //Name belonging to those color values

        public ColorData(string name, List<int> x)
        {
            colors = new List<int>();
            colors = x;
            color_name = name;
        }
    }

    //Color names by color values
    public ColorData[] color_names = new ColorData[12]
    {
        new ColorData("Black", new List<int>() {0, 1, 11, 12, 15, 17, 21, 22, 147}),
        new ColorData("Silver", new List<int>() {2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 14, 17, 18, 19, 20, 23, 24, 25, 26, 117, 118, 119, 156, 144}),
        new ColorData("Chrome", new List<int>() {120 }),
        new ColorData("Red", new List<int>() {27, 28, 29, 30, 31, 32, 33, 34, 35, 39, 40, 43, 44, 45, 46, 47, 48, 150, 143}),
        new ColorData("Orange", new List<int>() {36, 38, 41, 123, 124, 130, 138}),
        new ColorData("Yellow", new List<int>() {42, 37, 88, 89, 91, 126, 160, 158, 159}),
        new ColorData("Green", new List<int>() {49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 92, 125, 128, 133, 139, 151, 155}),
        new ColorData("Blue", new List<int>() {61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 73, 74, 75, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 127, 140, 146, 157}),
        new ColorData("Purple", new List<int>() {71, 72, 76, 141, 142, 145, 148, 149}),
        new ColorData("Brown", new List<int>() {90, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 114, 115, 116, 129, 152, 153, 154}),
        new ColorData("White", new List<int>() {111, 112, 113, 121, 122, 131, 132, 134}),
        new ColorData("Pink", new List<int>() {135, 136, 137})
    };

    //Locations for the login screen
    Vector3[] loginscreen_locations = new Vector3[5]
    {
        new Vector3(-438.796, 1075.821, 353.000),
        new Vector3(2495.127, 6196.140, 202.541),
        new Vector3(-1670.700, -1125.000, 50.000),
        new Vector3(1655.813, 0.889, 200.0),
        new Vector3(1070.206, -711.958, 70.483)
    };

    //Data that is collected for each vehicle
    public struct VehicleData
    {
        public int vehicle_id;
        public Vehicle vehicle_hash;
        public bool engine_on;
        public bool vehicle_locked;
        public int engine_health;
        public int primary_color;
        public int secondary_color;
        public string color_name;
        public Vector3 position;
        public Vector3 rotation;
        public int dirt_level;
        public string license_plate;
        public string owner_name;
        public string faction;

        public VehicleData(bool value)
        {
            vehicle_id = -1;
            vehicle_hash = null;
            engine_on = false;
            vehicle_locked = true;
            engine_health = 100;
            primary_color = 0;
            secondary_color = 0;
            color_name = "Black";
            position = new Vector3(0.0, 0.0, 0.0);
            rotation = new Vector3(0.0, 0.0, 0.0);
            dirt_level = 0;
            license_plate = "TJM000";
            owner_name = "null";
            faction = "Civillian";
        }
    }

    //Data for fines
    public struct FineData
    {
        public int id;
        public int amount;
        public bool paid;
        public string dateissued;
        public int timedue;
        public string information;

        public FineData(int d, int mnt, string info, string dateissue, int time)
        {
            id = d;
            amount = mnt;
            information = info;
            paid = false;
            dateissued = dateissue;
            timedue = time;
        }
    }

    //Data for item stored in inventories
    public struct ItemData
    {
        public int id;
        public string type;
        public string description;
    }

    public struct PlayerData
    {
        public int player_id;
        public string player_real_name; //Name of connected user
        public string player_fake_name; //Name displayed in-game
        public string password;
        public bool is_offline;
        public bool is_logged;
        public bool is_registered;
        public bool data_reset; //Used for validation
        //Data
        public Vector3 position;
        public Vector3 rotation;
        public Ped player_ped_hash;
        public int armor;
        public int health;
        public int money_in_hand;
        public int money_in_bank;
        public int pay_check;
        public int vehicles_owned;
        public int paid_fines;
        public int unpaid_fines;
        public string phone_number;
        public string faction;
        public List<VehicleData> vehicles;
        public List<FineData> fines;
        public List<ItemData> inventory;

        public NetHandle entityAttached; //Attach objects to player (Used for animations) (Needed so objects can be properly deleted whenever animation is over)

        public PlayerData(bool value)
        {
            player_id = -1;
            player_real_name = "";
            player_fake_name = "Test McTest";
            password = "";
            is_offline = true;
            is_logged = false;
            is_registered = false;
            data_reset = true;
            //----
            position = new Vector3(0.0, 0.0, 0.0);
            rotation = new Vector3(0.0, 0.0, 0.0);
            player_ped_hash = null;
            armor = 0;
            health = 100;
            money_in_hand = 0;
            money_in_bank = 0;
            pay_check = 0;
            vehicles_owned = 0;
            paid_fines = 0;
            unpaid_fines = 0;
            phone_number = "000-0000";
            faction = "Civillian";
            vehicles = new List<VehicleData>();
            fines = new List<FineData>();
            inventory = new List<ItemData>();
            entityAttached = new NetHandle();
        }
    }

    public List<PlayerData> PlayerDatabase = new List<PlayerData>();

    //ID pools
    public List<int> RandomIDPlayerPool = new List<int>();
    public List<int> RandomIDVehiclePool = new List<int>();
    public List<int> RandomIDFinePool = new List<int>();
    int players_online = 0;

    public RPcore()
    {
        //Event handlers
        API.onResourceStart += OnResourceStartHandler;
        API.onPlayerBeginConnect += OnPlayerBeginConnectHandler;
        API.onPlayerConnected += OnPlayerConnectedHandler;
        API.onPlayerDisconnected += OnPlayerDisconnectedHandler;
        API.onPlayerFinishedDownload += OnPlayerFinishedDownloadHandler;
        API.onClientEventTrigger += OnClientEventTriggerHandler;
        API.onChatMessage += OnChatMessageHandler;
        API.onChatCommand += OnChatCommandHandler;
    }

    public void OnChatMessageHandler(Client player, string message, CancelEventArgs e)
    {
        e.Cancel = true;
    }

    public void OnChatCommandHandler(Client player, string command, CancelEventArgs e)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            API.consoleOutput("cmd: " + command);
            if (command.StartsWith("/login"))
            {
                if (PlayerDatabase[indx].is_registered == false)
                {
                    API.sendChatMessageToPlayer(player, "(0x00) You are not registered! Register using /register ~b~(password)~w~.");
                    e.Cancel = true;
                }
                else if (PlayerDatabase[indx].is_logged == true)
                {
                    API.sendChatMessageToPlayer(player, "(0x00) You are already ~b~logged ~w~in!");
                    e.Cancel = true;
                }
            }
            else if (command.StartsWith("/register"))
            {
                if (PlayerDatabase[indx].is_registered == true)
                {
                    API.sendChatMessageToPlayer(player, "(0x01) are already ~b~registered~w~!");
                    e.Cancel = true;
                }
                else if (PlayerDatabase[indx].is_logged == true)
                {
                    API.sendChatMessageToPlayer(player, "(0x01) You are already ~b~registered~w~!");
                    e.Cancel = true;
                }
            }
            else
            {
                if (PlayerDatabase[indx].is_registered == false)
                {
                    API.sendChatMessageToPlayer(player, "(0x02) You are not registered! Register using /register ~b~(password)~w~.");
                    e.Cancel = true;
                }
                else if (PlayerDatabase[indx].is_logged == false)
                {
                    API.sendChatMessageToPlayer(player, "(0x02) You are not logged in! Login using /login ~b~(password)~w~.");
                    e.Cancel = true;
                }
            }
        }
    }

    public void OnClientEventTriggerHandler(Client player, string eventName, params object[] arguments)
    {
        //Indicator for vehicles
        if (eventName == "indicator_left")
        {
            if (API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_left") != null)
                API.sendNativeToAllPlayers(0xB5D45264751B7DF0, API.getPlayerVehicle(player), 1, !API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_left"));
            if (API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_left") != null)
                API.setEntitySyncedData(API.getPlayerVehicle(player), "indicator_left", !API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_left"));
        }
        else if (eventName == "indicator_right")
        {
            if (API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_right") != null)
                API.sendNativeToAllPlayers(0xB5D45264751B7DF0, API.getPlayerVehicle(player), 0, !API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_right"));
            if (API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_right") != null)
                API.setEntitySyncedData(API.getPlayerVehicle(player), "indicator_right", !API.getEntitySyncedData(API.getPlayerVehicle(player), "indicator_right"));
        }
    }

    string getColorNameByInt(int col)
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < color_names[i].colors.Count; j++)
            {
                if (color_names[i].colors[j] == col)
                    return color_names[i].color_name;
            }
        }
        return "null";
    }

    //Check pools for already existing IDs
    public bool doesIDPlayerExist(int id)
    {
        for (int i = 0; i < RandomIDPlayerPool.Count; i++)
            if (RandomIDPlayerPool[i] == id)
                return true;
        return false;
    }

    public bool doesIDVehicleExist(int id)
    {
        for (int i = 0; i < RandomIDVehiclePool.Count; i++)
            if (RandomIDVehiclePool[i] == id)
                return true;
        return false;
    }

    public bool doesIDFineExist(int id)
    {
        for (int i = 0; i < RandomIDFinePool.Count; i++)
            if (RandomIDFinePool[i] == id)
                return true;
        return false;
    }

    public void OnResourceStartHandler()
    {
        API.consoleOutput("RPcore is initializing...");
        //Initialize ID pools only at beginning
        for (int i = 0; i < 1000; i++)
        {
            Random rnd = new Random();
            int temp = rnd.Next(1, 100000);
            while (doesIDPlayerExist(temp))
                temp = rnd.Next(1, 100000);
            RandomIDPlayerPool.Add(temp);
        }
        for (int i = 0; i < 1000; i++)
        {
            Random rnd = new Random();
            int temp = rnd.Next(1, 100000);
            while (doesIDVehicleExist(temp))
                temp = rnd.Next(1, 100000);
            RandomIDVehiclePool.Add(temp);
        }
        for (int i = 0; i < 1000; i++)
        {
            Random rnd = new Random();
            int temp = rnd.Next(1, 100000);
            while (doesIDFineExist(temp))
                temp = rnd.Next(1, 100000);
            RandomIDFinePool.Add(temp);
        }
        API.consoleOutput("RPcore has initialized!");
    }

    public void OnPlayerBeginConnectHandler(Client player, CancelEventArgs e)
    {
        API.consoleOutput("Player (" + player.name + ") begun connecting...");
        //???
    }

    public void OnPlayerConnectedHandler(Client player)
    {
        players_online++;
        API.sendChatMessageToPlayer(player, "~w~--~b~raptube69's RP Server~w~--           v0.0.0");
        API.sendChatMessageToPlayer(player, "~w~Player(s) Online: ~b~" + players_online);
        API.consoleOutput("Player (" + player.name + ") is connected!");
        //Initialize player's data

        bool player_exists = false;

        //Check if player already exists
        for (int i = 0; i < PlayerDatabase.Count; i++)
        {
            if (PlayerDatabase[i].player_real_name == player.name)
            {
                player_exists = true;
                break;
            }
        }

        if (player_exists) //Already exists so just fetch him a new ID
        {
            API.consoleOutput("Player (" + player.name + ") already exists!");
            int indx = getPlayerIndexByRealName(player.name);
            if (indx != -1)
            {
                PlayerData plr_temp = PlayerDatabase[indx];
                plr_temp.player_id = RandomIDPlayerPool[0];
                RandomIDPlayerPool.RemoveAt(0);
                plr_temp.is_offline = false;
                PlayerDatabase[indx] = plr_temp;
            }
            API.sendChatMessageToPlayer(player, "Please login using /login ~b~(password)");
        }
        else //Fetch him new data
        {
            API.consoleOutput("Player (" + player.name + ") is new!");
            PlayerData plr_temp = new PlayerData(true);
            plr_temp.player_real_name = player.name;
            plr_temp.player_id = RandomIDPlayerPool[0];
            RandomIDPlayerPool.RemoveAt(0);
            plr_temp.is_offline = false;
            PlayerDatabase.Add(plr_temp);
            API.sendChatMessageToPlayer(player, "Please register using /register ~b~(password)");
        }

        //Login screen random location
        Random rnd = new Random();
        int temp = rnd.Next(0, 5);
        API.setPlayerSkin(player, API.pedNameToModel("Mani"));
        API.setEntityPosition(player, loginscreen_locations[temp]);
        API.setEntityPositionFrozen(player, true);
        API.setEntityTransparency(player, 0);
        API.setEntityInvincible(player, true);
        API.setEntityCollisionless(player, true);

    }

    //Used to get information
    public int getPlayerIDByName(string name)
    {
        for (int i = 0; i < PlayerDatabase.Count; i++)
        {
            if (PlayerDatabase[i].player_real_name == name)
                return PlayerDatabase[i].player_id;
        }
        return -1;
    }

    public int getPlayerIndexByRealName(string name)
    {
        for (int i = 0; i < PlayerDatabase.Count; i++)
        {
            if (PlayerDatabase[i].player_real_name == name)
                return i;
        }
        return -1;
    }

    public int getPlayerIndexByFakeName(string name)
    {
        for (int i = 0; i < PlayerDatabase.Count; i++)
        {
            if (PlayerDatabase[i].player_fake_name == name)
                return i;
        }
        return -1;
    }

    public string getVehicleOwnerByID(int id)
    {
        for (int i = 0; i < PlayerDatabase.Count; i++)
        {
            for (int j = 0; j < PlayerDatabase[i].vehicles.Count; j++)
            {
                if (PlayerDatabase[i].vehicles[j].vehicle_id == id)
                    return PlayerDatabase[i].vehicles[j].owner_name;
            }
        }
        return "null";
    }

    public int getVehicleIndexByOwnerName(string name, int id)
    {
        int indx = getPlayerIndexByFakeName(name);
        for (int i = 0; i < PlayerDatabase[indx].vehicles.Count; i++)
        {
            if (PlayerDatabase[indx].vehicles[i].vehicle_id == id)
                return i;
        }
        return -1;
    }

    public string getOwnerNameByVehicleID(int id)
    {
        for (int i = 0; i < PlayerDatabase.Count; i++)
        {
            for (int j = 0; j < PlayerDatabase[i].vehicles.Count; j++)
            {
                if (PlayerDatabase[i].vehicles[j].vehicle_id == id)
                    return PlayerDatabase[i].vehicles[j].owner_name;
            }
        }
        return "null";
    }

    public void OnPlayerDisconnectedHandler(Client player, string reason)
    {
        API.consoleOutput("Player (" + player.name + ") has disconnected! Reason: " + reason);
        //Deinitialize stuff
        int id = getPlayerIDByName(player.name);
        if (id == -1)
            API.consoleOutput("UNINITIALIZED ID DETECTED FROM PLAYER: " + player.name);
        else
        {
            //Player disconnected, save his stuff and then recycle his ID
            RandomIDPlayerPool.Add(id);
            int plr_indx = getPlayerIndexByRealName(player.name);
            if (plr_indx != -1)
            {
                PlayerData temp = PlayerDatabase[plr_indx];
                temp.player_id = -1;
                temp.is_offline = true;
                if (temp.is_logged == true)
                {
                    temp.position = API.getEntityPosition(player);
                    temp.rotation = API.getEntityRotation(player);
                    temp.is_logged = false;
                }
                PlayerDatabase[plr_indx] = temp;
            }
        }
        players_online--;
    }

    public void OnPlayerFinishedDownloadHandler(Client player)
    {
        //doesnt actually trigger, ever. wtf
    }

    [Command("myid")]
    public void getMyID(Client player)
    {
        int id = getPlayerIDByName(player.name);
        API.sendChatMessageToPlayer(player, "Your ID is: ~b~" + id + ".");
    }

    [Command("pos")] //debug
    public void getPos(Client player)
    {
        Vector3 vec = API.getEntityPosition(player);
        API.sendChatMessageToPlayer(player, "X:" + vec.X + "Y:" + vec.Y + "Z:" + vec.Z);
    }

    [Command("lock")]
    public void lockFunc(Client player)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            List<NetHandle> vehs = new List<NetHandle>();
            vehs = API.getAllVehicles();
            float smallestDist = 100.0f;
            NetHandle closestveh = new NetHandle();
            bool found = false;
            for (int i = 0; i < vehs.Count; i++)
            {
                float vr = vecdist(API.getEntityPosition(vehs[i]), API.getEntityPosition(player)); //Get distance between car and player
                if (vr < smallestDist)
                {
                    smallestDist = vr;
                    closestveh = vehs[i];
                    found = true;
                }
            }

            if (found) //Found SOME car
            {
                if (smallestDist < 2.5f) //Close enough?
                {
                    if (getOwnerNameByVehicleID(API.getEntitySyncedData(closestveh, "id")) == PlayerDatabase[indx].player_fake_name)
                    {
                        API.setVehicleLocked(closestveh, true);
                        API.sendChatMessageToPlayer(player, "You have ~b~locked ~w~the car.");
                        API.setEntitySyncedData(closestveh, "locked", true);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "You don't own this vehicle!");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "No car found nearby.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "No car found nearby.");
            }
        }
    }

    //Same as above but unlocks car
    [Command("unlock")]
    public void unlockFunc(Client player)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            List<NetHandle> vehs = new List<NetHandle>();
            vehs = API.getAllVehicles();
            float smallestDist = 100.0f;
            NetHandle closestveh = new NetHandle();
            bool found = false;
            for (int i = 0; i < vehs.Count; i++)
            {
                float vr = vecdist(API.getEntityPosition(vehs[i]), API.getEntityPosition(player));
                if (vr < smallestDist)
                {
                    smallestDist = vr;
                    closestveh = vehs[i];
                    found = true;
                }
            }

            if (found)
            {
                if (smallestDist < 2.5f)
                {
                    if (getOwnerNameByVehicleID(API.getEntitySyncedData(closestveh, "id")) == PlayerDatabase[indx].player_fake_name)
                    {
                        API.setVehicleLocked(closestveh, false);
                        API.sendChatMessageToPlayer(player, "You have ~b~unlocked ~w~the car.");
                        API.setEntitySyncedData(closestveh, "locked", false);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "You don't own this vehicle!");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "No car found nearby.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "No car found nearby.");
            }

        }
    }

    [Command("engine")]
    public void engineFunc(Client player)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            if (!API.isPlayerInAnyVehicle(player))
            {
                API.sendChatMessageToPlayer(player, "You are not in a vehicle!");
            }
            else
            {
                if (API.getPlayerVehicleSeat(player) != -1)
                {
                    API.sendChatMessageToPlayer(player, "You are not the driver!");
                }
                else
                {
                    if (getOwnerNameByVehicleID(API.getEntitySyncedData(API.getPlayerVehicle(player), "id")) == PlayerDatabase[indx].player_fake_name)
                    {
                        API.setVehicleEngineStatus(API.getPlayerVehicle(player), !API.getVehicleEngineStatus(API.getPlayerVehicle(player))); //Just inverse the engine state
                        if (API.getVehicleEngineStatus(API.getPlayerVehicle(player)) == true)
                            API.sendChatMessageToPlayer(player, "You have turned the engine ~g~ON.");
                        else
                            API.sendChatMessageToPlayer(player, "You have turned the engine ~r~OFF.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "You don't own this vehicle!");
                    }
                }
            }
        }
    }

    //Used to open car doors
    [Command("open", GreedyArg = true)]
    public void doorOpenFunc(Client player, string action)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            List<NetHandle> vehs = new List<NetHandle>();
            vehs = API.getAllVehicles();
            float smallestDist = 100.0f;
            NetHandle closestveh = new NetHandle();
            bool found = false;
            for (int i = 0; i < vehs.Count; i++)
            {
                float vr = vecdist(API.getEntityPosition(vehs[i]), API.getEntityPosition(player));
                if (vr < smallestDist)
                {
                    smallestDist = vr;
                    closestveh = vehs[i];
                    found = true;
                }
            }

            if (found)
            {
                if (smallestDist < 3.5f)
                {
                    //if (getOwnerNameByVehicleID(API.getEntitySyncedData(closestveh, "id")) == PlayerDatabase[indx].player_fake_name)
                    //{
                    if (API.getEntitySyncedData(closestveh, "locked") == true)
                    {
                        API.sendChatMessageToPlayer(player, "This vehicle is locked!");
                    }
                    else
                    {
                        if (action == "trunk")
                        {
                            API.setVehicleDoorState(closestveh, 5, true);
                        }
                        else if (action == "hood")
                        {
                            API.setVehicleDoorState(closestveh, 4, true);
                        }
                        else if (action == "door1")
                        {
                            API.setVehicleDoorState(closestveh, 0, true);
                        }
                        else if (action == "door2")
                        {
                            API.setVehicleDoorState(closestveh, 1, true);
                        }
                        else if (action == "door3")
                        {
                            API.setVehicleDoorState(closestveh, 2, true);
                        }
                        else if (action == "door4")
                        {
                            API.setVehicleDoorState(closestveh, 3, true);
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(player, "Part does not exist!");
                        }
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "No car found nearby.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "No car found nearby.");
            }
        }
    }

    //Flags given by GTA:N, but for some reason not included in the API.
    [Flags]
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    [Command("anim", GreedyArg = true)]
    public void animFunc(Client player, string action)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            if (!API.isPlayerInAnyVehicle(player))
            {
                //The holy grail of animations (Always use PH_L_HAND/PH_R_HAND as the game optimizes objects for actual hand usage automatically, learned that after wasting hours making custom positions)
                if (action == "clean")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "switch@franklin@cleaning_car", "001946_01_gc_fras_v2_ig_5_base");
                }
                else if (action == "help")
                {
                    API.triggerClientEvent(player, "anim_list");
                }
                else if (action == "clipboard1")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(-969349845, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_L_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_clipboard@male@idle_a", "idle_a");
                }
                else if (action == "clipboard2")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(-969349845, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_L_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_clipboard@male@idle_b", "idle_d");
                }
                else if (action == "phone1")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(94130617, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));

                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_mobile_film_shocking@female@idle_a", "idle_a");
                }
                else if (action == "phone2")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(94130617, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));

                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_mobile_film_shocking@male@idle_a", "idle_a");
                }
                else if (action == "phone3")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(94130617, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));

                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_mobile_film_shocking@female@idle_a", "idle_b");
                }
                else if (action == "checkbody1")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "amb@medic@standing@kneel@idle_a", "idle_a");
                }
                else if (action == "checkbody2")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "amb@medic@standing@tendtodead@idle_a", "idle_a");
                }
                else if (action == "leancar")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "switch@michael@sitting_on_car_bonnet", "sitting_on_car_bonnet_loop");
                }
                else if (action == "sit")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "switch@michael@sitting", "idle");
                }
                else if (action == "leanfoot")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "amb@world_human_leaning@male@wall@back@foot_up@idle_a", "idle_a");
                }
                else if (action == "lean")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), "amb@world_human_leaning@male@wall@back@hands_together@idle_a", "idle_a");
                }
                else if (action == "handsupknees")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "busted", "idle_2_hands_up");
                }
                else if (action == "handsup")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missfbi5ig_20b", "hands_up_scientist");
                }
                else if (action == "smoke1")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(175300549, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_aa_smoke@male@idle_a", "idle_a");
                }
                else if (action == "smoke2")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(175300549, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@female@smoke@idle_a", "idle_a");
                }
                else if (action == "coffee1")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(-163314598, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, -0.1), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_aa_coffee@idle_a", "idle_a");
                }
                else if (action == "coffee2")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(-163314598, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_R_Hand", new Vector3(0.0, 0.0, -0.1), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_drinking@coffee@male@idle_a", "idle_a");
                }
                else if (action == "guitar")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(-708789241, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_L_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_musician@guitar@male@idle_a", "idle_b");
                }
                else if (action == "drums")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                    API.setEntitySyncedData(player, "anim_obj", API.createObject(591916419, API.getEntityPosition(player), API.getEntityRotation(player)));
                    API.attachEntityToEntity(API.getEntitySyncedData(player, "anim_obj"), player, "PH_L_Hand", new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_musician@bongos@male@idle_a", "idle_a");
                }
                else if (action == "stop")
                {
                    API.stopPlayerAnimation(player);
                    NetHandle temp = new NetHandle();
                    if (API.getEntitySyncedData(player, "anim_obj") != null)
                        temp = API.getEntitySyncedData(player, "anim_obj");
                    API.deleteEntity(temp);
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "You cannot do that in a vehicle!");
            }
        }
    }

    [Command("skin", GreedyArg = true)]
    public void skinFunc(Client player, string action)
    {
        API.setPlayerSkin(player, API.pedNameToModel(action));
    }

    //Close car doors
    [Command("close", GreedyArg = true)]
    public void doorCloseFunc(Client player, string action)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            List<NetHandle> vehs = new List<NetHandle>();
            vehs = API.getAllVehicles();
            float smallestDist = 100.0f;
            NetHandle closestveh = new NetHandle();
            bool found = false;
            for (int i = 0; i < vehs.Count; i++)
            {
                float vr = vecdist(API.getEntityPosition(vehs[i]), API.getEntityPosition(player));
                if (vr < smallestDist)
                {
                    smallestDist = vr;
                    closestveh = vehs[i];
                    found = true;
                }
            }

            if (found)
            {
                if (smallestDist < 3.5f)
                {
                    //Can be done regardless if car is locked or not
                    if (action == "trunk")
                    {
                        API.setVehicleDoorState(closestveh, 5, false);
                    }
                    else if (action == "hood")
                    {
                        API.setVehicleDoorState(closestveh, 4, false);
                    }
                    else if (action == "door1")
                    {
                        API.setVehicleDoorState(closestveh, 0, false);
                    }
                    else if (action == "door2")
                    {
                        API.setVehicleDoorState(closestveh, 1, false);
                    }
                    else if (action == "door3")
                    {
                        API.setVehicleDoorState(closestveh, 2, false);
                    }
                    else if (action == "door4")
                    {
                        API.setVehicleDoorState(closestveh, 3, false);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "Part does not exist!");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "No car found nearby.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "No car found nearby.");
            }
        }
    }


    //Used for debugging
    [Command("spawncar")]
    public void spawnCar(Client player)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            API.sendChatMessageToPlayer(player, "Spawned car!");
            Vehicle hash = API.createVehicle(API.vehicleNameToModel("primo"), API.getEntityPosition(player), API.getEntityRotation(player), 0, 0);
            //API.setVehicleNumberPlate(hash, "TJM");
            VehicleData temp = new VehicleData(true);
            temp.vehicle_id = RandomIDVehiclePool[0];
            RandomIDVehiclePool.RemoveAt(0);
            temp.vehicle_hash = hash;
            temp.position = API.getEntityPosition(hash);
            temp.rotation = API.getEntityRotation(hash);

            temp.owner_name = PlayerDatabase[indx].player_fake_name;

            API.setEntitySyncedData(temp.vehicle_hash, "id", (int)temp.vehicle_id);
            API.setEntitySyncedData(temp.vehicle_hash, "owner", (string)temp.owner_name);
            API.setEntitySyncedData(temp.vehicle_hash, "plate", (string)temp.license_plate);
            API.setEntitySyncedData(temp.vehicle_hash, "indicator_right", false);
            API.setEntitySyncedData(temp.vehicle_hash, "indicator_left", false);
            API.setEntitySyncedData(temp.vehicle_hash, "locked", true);
            API.setVehicleNumberPlate(temp.vehicle_hash, (string)temp.license_plate);
            API.setVehicleEngineStatus(temp.vehicle_hash, false);
            API.setVehicleLocked(temp.vehicle_hash, true);
            PlayerDatabase[indx].vehicles.Add(temp);
            PlayerData plr = PlayerDatabase[indx];
            plr.vehicles_owned++;
            PlayerDatabase[indx] = plr;
        }
    }

    //Can be used for police factions (Not included in script)
    [Command("license", GreedyArg = true)]
    public void carinfoFunc(Client player, string license)
    {
        //API.triggerClientEvent(player, "vehicle_draw_text", temp.vehicle_hash, temp.vehicle_id, temp.owner_name, temp.license_plate);
        license = license.ToUpper(); //Get lowercase string for lazy people
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            List<NetHandle> vehs = new List<NetHandle>();
            vehs = API.getAllVehicles();
            bool found = false;
            for (int i = 0; i < vehs.Count; i++)
            {
                if (API.getEntitySyncedData(vehs[i], "plate") == license)
                {
                    //Display data about car
                    API.sendChatMessageToPlayer(player, "Vehicle is ~g~registered ~w~in database!");
                    var model = API.getEntityModel(vehs[i]);
                    string plr_nm = getOwnerNameByVehicleID(API.getEntitySyncedData(vehs[i], "id"));
                    int plr_indx = getPlayerIndexByFakeName(plr_nm);
                    int veh_indx = getVehicleIndexByOwnerName(plr_nm, API.getEntitySyncedData(vehs[i], "id"));
                    API.sendChatMessageToPlayer(player, "Model: ~b~" + API.getVehicleDisplayName((VehicleHash)model) + " ~w~-~b~|~w~- Color: ~b~" + PlayerDatabase[plr_indx].vehicles[veh_indx].color_name);
                    API.sendChatMessageToPlayer(player, "License Plate: ~b~" + license);
                    API.sendChatMessageToPlayer(player, "Owner: ~b~" + getVehicleOwnerByID(API.getEntitySyncedData(vehs[i], "id")));
                    found = true;
                }
            }
            if (found == false)
            {
                //BUSTED
                API.sendChatMessageToPlayer(player, "Vehicle is not ~r~registered ~w~in database!");
            }
        }
    }


    //Statistics about your character
    [Command("stats")]
    public void statsFunc(Client player)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            API.sendChatMessageToPlayer(player, "~b~|-------STATS-------|");
            API.sendChatMessageToPlayer(player, "ID: ~b~" + PlayerDatabase[indx].player_id);
            API.sendChatMessageToPlayer(player, "Full Name: ~b~" + PlayerDatabase[indx].player_fake_name + "   |   ~w~Faction: ~b~" + PlayerDatabase[indx].faction);
            API.sendChatMessageToPlayer(player, "Phone #: ~b~" + PlayerDatabase[indx].phone_number);
            API.sendChatMessageToPlayer(player, "Money (Bank): ~b~$" + PlayerDatabase[indx].money_in_bank + "  |  ~w~Money (Hand): ~b~$" + PlayerDatabase[indx].money_in_hand);
            API.sendChatMessageToPlayer(player, "Paycheck: ~b~$" + PlayerDatabase[indx].pay_check);
            API.sendChatMessageToPlayer(player, "Vehicle(s) Owned: ~b~" + PlayerDatabase[indx].vehicles_owned);
            API.sendChatMessageToPlayer(player, "~b~|-------------------|");
        }
    }

    [Command("login", "Usage: /login ~b~(password)", SensitiveInfo = true, GreedyArg = true)]
    public void loginFunc(Client player, string password)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            if (password == PlayerDatabase[indx].password)
            {
                API.sendChatMessageToPlayer(player, "Welcome, ~b~" + player.name + "~w~!");
                if (indx != -1)
                {
                    //Change player data and log him in
                    PlayerData plr_temp = PlayerDatabase[indx];
                    plr_temp.is_logged = true;
                    if (plr_temp.data_reset == false)
                    {
                        API.setEntityPosition(player, plr_temp.position);
                        API.setEntityRotation(player, plr_temp.rotation);
                        API.setEntityPositionFrozen(player, false);
                        API.setEntityTransparency(player, 255);
                        API.setEntityInvincible(player, false);
                        API.setEntityCollisionless(player, false);
                    }
                    else
                    {
                        plr_temp.data_reset = false;
                        API.setEntityPosition(player, new Vector3(-1034.600, -2733.600, 13.800));
                        API.setEntityPositionFrozen(player, false);
                        API.setEntityTransparency(player, 255);
                        API.setEntityInvincible(player, false);
                        API.setEntityCollisionless(player, false);
                    }
                    PlayerDatabase[indx] = plr_temp;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "Wrong credentials!");
            }
        }
    }

    [Command("register", "Usage: /register ~b~(password)", SensitiveInfo = true, GreedyArg = true)]
    public void registerFunc(Client player, string password)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            PlayerData plr_temp = PlayerDatabase[indx];
            plr_temp.password = password;
            plr_temp.is_registered = true;
            PlayerDatabase[indx] = plr_temp;
            API.sendChatMessageToPlayer(player, "You have been registered! Use /login ~b~(password) ~w~to login.");
        }
    }

    [Command("logout")]
    public void logoutFunc(Client player)
    {
        int indx = getPlayerIndexByRealName(player.name);
        if (indx != -1)
        {
            //Log user out, don't recycle ID yet.
            API.stopPlayerAnimation(player);
            NetHandle temp = new NetHandle();
            if (API.getEntitySyncedData(player, "anim_obj") != null)
                temp = API.getEntitySyncedData(player, "anim_obj");
            API.deleteEntity(temp);

            API.sendChatMessageToPlayer(player, "You have been logged out!");
            PlayerData plr_temp = PlayerDatabase[indx];
            plr_temp.position = API.getEntityPosition(player);
            plr_temp.rotation = API.getEntityRotation(player);
            plr_temp.is_logged = false;
            PlayerDatabase[indx] = plr_temp;
            Random rnd = new Random();
            int temprnd = rnd.Next(0, 5);
            API.setEntityPosition(player, loginscreen_locations[temprnd]);
            API.setEntityPositionFrozen(player, true);
            API.setEntityTransparency(player, 0);
            API.setEntityInvincible(player, true);
            API.setEntityCollisionless(player, true);
        }
    }
}