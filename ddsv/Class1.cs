using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;
using System.Drawing;
using LemonUI;
using LemonUI.Menus;
using System.Threading;
using iFruitAddon2;
using System.IO;

namespace gta_ddsv1
{
    public class Main : Script
    {

        bool isplayinganimation = false;

        public int index = 0;
        public bool ddspawned = false;
        public bool clientspawned = false;
        public Vector3 ddposition = new Vector3(115.2737f, -1949.512f, 22.6728f);
        public Ped dd;
        public int reputation;
        public static bool alreadystartedhitman;

        Thread editpricesinbackground = new Thread(new ThreadStart(editprices.edit));
        // my brain hurts
        public int weed;
        public int cocaine;
        public int heroin;
        public int acid;
        public int ecstacy;
        public int lsd;

        public static float weedprice = 10f;
        public static float cocaineprice = 17f;
        public static float heroinprice = 19f;
        public static float acidprice = 7f;
        public static float ecstacyprice = 9f;
        public static float lsdprice = 8f;

        public static float defaultweedprice = 10f;
        public static float defaultcocaineprice = 17f;
        public static float defaultheroinprice = 19f;
        public static float defaultacidprice = 7f;
        public static float defaultecstacyprice = 9f;
        public static float defaultlsdprice = 8f;

        private ScriptSettings config;

        public static Vector3[] clientpositions = { new Vector3(5.3613f, -1674.728f, 29.3069f), new Vector3(-133.7501f, -996.5969f, 27.2585f), new Vector3(39.3495f, -1020.944f, 29.4762f), new Vector3(94.7569f, -972.0383f, 29.3525f), new Vector3(203.6087f, -960.0268f, 30.0753f), new Vector3(790.3320f, -837.6572f, 25.9709f),
        new Vector3(337.3239f, -633.0912f, 29.2758f) };
        float[] clientyaw = { 39.2243f, 39.2244f, 78.5907f, -108.1053f, 100.6956f, 121.9895f, -74.1382f };
        Vector3[] deaddroppositions = { new Vector3(378.2689f, -682.9201f, 28.2885f), new Vector3(333.8959f, -678.5932f, 28.3221f), new Vector3(499.4009f, -725.4374f, 23.8775f), new Vector3(372.4997f, -869.2589f, 28.2932f), new Vector3(133.5368f, -994.0809f, 28.3606f), new Vector3(287.1237f, -1063.982f, 28.4216f) };

        ObjectPool menupool;
        NativeMenu menu, chemistMenu, prices;
        NativeItem reputationmi, inventory, schedulechemist;

        NativeItem weedpr, cocainepr, heroinpr, acidpr, ecstacypr, lsdpr;
        public static CustomiFruit iFruit = new CustomiFruit();
        public static iFruitContact hitmanjob = new iFruitContact("Become a H1tman")
        {
            DialTimeout = 4000,
            Active = true,
            Icon = ContactIcon.Skull,
        };
        public Main()
        {
            iFruit.Contacts.Add(hitmanjob);
            editpricesinbackground.Start();
            menupool = new ObjectPool();
            config = ScriptSettings.Load("scripts\\ddconfig.ini");
            Notification.Show($"Welcome to Cortez's Drug Dealing mod. Click {Keys.Z} to open Drug Dealing Menu.");
            Blip dd = World.CreateBlip(ddposition);
            dd.Sprite = BlipSprite.Weed;
            dd.Name = "Supplier";
            Tick += onTick;
            Tick += beahitman.OnTick;
            KeyDown += onKeyDown;
            reputation = config.GetValue("stats", "reputation", 0);
            Setup();
            hitmanjob.Answered += starthitmanjob;
        }

        private void onTick(object sender, EventArgs e)
        {
            iFruit.Update();
            menupool.Process();
            switch (index)
            {
                case 0:
                    #region ddspawner
                    if (Game.Player.Character.Position.DistanceTo(ddposition) < 50f && ddspawned == false)
                    {
                        dd = World.CreatePed(PedHash.MexGang01GMY, ddposition, 33f);
                        dd.Weapons.Give(WeaponHash.CombatPistol, 9999, false, false);
                        ddspawned = true;
                        World.DrawMarker(MarkerType.Cylinder, new Vector3(115.2737f, -1949.512f, 21.6728f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), Color.FromArgb(0, 255, 255, 0));
                    }
                    else if (ddspawned == true && Game.Player.Character.Position.DistanceTo(ddposition) > 50f)
                    {
                        dd.MarkAsNoLongerNeeded();
                        ddspawned = false;
                    }
                    #endregion
                    #region searchtrashcans
                    foreach (Vector3 trashcan in deaddroppositions)
                    {
                        if (Game.Player.Character.Position.DistanceTo(trashcan) < 2f)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Click ~INPUT_CONTEXT~ to search");
                            Prop prop = World.GetClosestProp(trashcan, 0.001f);
                            Function.Call(Hash.FREEZE_ENTITY_POSITION, prop.Handle, true);
                            if (Game.IsControlJustPressed(GTA.Control.Context))
                            {
                                Function.Call(Hash.REQUEST_ANIM_DICT, "amb@prop_human_bum_bin@base");
                                while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "amb@prop_human_bum_bin@base"))
                                {
                                    Script.Wait(50);
                                }
                                Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base", 300f, -300f, 5000, (AnimationFlags)1, 0);
                                isplayinganimation = true;
                                index = 10;
                            }
                        }
                    }
                    #endregion
                    if (Game.Player.Character.Position.DistanceTo(ddposition) < 3f)
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Click ~INPUT_CONTEXT~ to buy weed {Math.Round(weedprice)}.");
                        if (Game.IsControlJustPressed(GTA.Control.Context))
                        {
                            if (Game.Player.Money > Math.Round(weedprice))
                            {
                                boughtweed(1);
                            }
                        }
                    }
                    #region this makes wanna kill myself too much code
                    weedpr.Title = $"weed %{Math.Round(((weedprice - defaultweedprice) / defaultweedprice)) * 10}";
                    cocainepr.Title = $"cocaine %{Math.Round(((cocaineprice - defaultcocaineprice) / defaultcocaineprice) * 10)}";
                    heroinpr.Title = $"heroin %{Math.Round(((heroinprice - defaultheroinprice) / defaultheroinprice) * 10)}";
                    acidpr.Title = $"acid %{Math.Round(((acidprice - defaultacidprice) / defaultacidprice)) * 10}";
                    ecstacypr.Title = $"ecstacy %{Math.Round(((ecstacyprice - defaultecstacyprice) / defaultecstacyprice) * 10)}";
                    lsdpr.Title = $"lsd %{Math.Round(((lsdprice - defaultlsdprice) / defaultlsdprice) * 10)}";
                    #endregion
                    break;
                case 10:
                    if (isplayinganimation && !Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, Game.Player.Character.Handle, "amb@prop_human_bum_bin@base", "base", 3))
                    {
                        isplayinganimation = false;
                        //GTA.UI.Notification.Show("Animation Complete");
                        #region add da drug
                        if (Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 2) == Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 2))
                        {
                            int drug = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 5);
                            if (drug == 0)
                            {
                                weed++;
                            }
                            else if (drug == 1)
                            {
                                cocaine++;
                            }
                            else if (drug == 2)
                            {
                                heroin++;
                            }
                            else if (drug == 3)
                            {
                                acid++;
                            }
                            else if (drug == 4)
                            {
                                ecstacy++;
                            }
                            else if (drug == 5)
                            {
                                lsd++;
                            }
                        }
                        #endregion
                        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "MP_WAVE_COMPLETE", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
                        index = 0;
                    }
                    break;
            }
        }

        void boughtweed(int amount)
        {
            Game.Player.Money = Game.Player.Money - (int)Math.Round(weedprice) * amount;
            weed = weed + amount;
            config.SetValue<int>("stats", "weed", weed);
        }

        void onsell(int addreputation, int money)
        {
            Game.Player.Money = Game.Player.Money + money;
            reputation = reputation + addreputation;
            Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "MP_WAVE_COMPLETE", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        }

        public static void starthitmanjob(iFruitContact contact)
        {
            if (alreadystartedhitman == false)
            {
                alreadystartedhitman = true;
                hitmanjob.Active = false;
                beahitman.play = true;
            }
        }

        void showrep(object sender, EventArgs e)
        {
            Notification.Show($"Reputation: {reputation}");
        }

        void showinv(object sender, EventArgs e)
        {
            Notification.Show($"Inventory: ~g~ weed: {weed}~w~   cocaine: {cocaine}   ~y~heroin: {heroin}~w~");
        }

        void shedulewithchemist(object sender, EventArgs e)
        {
            Notification.Show("~g~test");
        }

        void Setup()
        {
            menu = new NativeMenu("DD Menu", "Drug Dealing Menu");
            chemistMenu = new NativeMenu("Chemist", "Chemist", "Control your chemist from here");
            prices = new NativeMenu("Price's", "Price's", "Check the price's of drugs here");
            menupool.Add(menu);
            menupool.Add(chemistMenu);
            menupool.Add(prices);

            NativeSubmenuItem chemistbutton = menu.AddSubMenu(chemistMenu);
            NativeSubmenuItem pricesbutton = menu.AddSubMenu(prices);

            //test = new NativeItem("test", "test");
            //test.Activated += ;
            //chemistMenu.Add(test);

            schedulechemist = new NativeItem("Schedule appointment with chemist", "Lets you schedule a appointment with your chemist");
            schedulechemist.Activated += shedulewithchemist;

            reputationmi = new NativeItem("Reputation", "Views your reputation");
            reputationmi.Activated += showrep;
            menu.Add(reputationmi);

            inventory = new NativeItem("Inventory", "Shows your inventory");
            inventory.Activated += showinv;
            menu.Add(inventory);

            #region prices
            //i wanna kill my self this took longer than it should have
            weedpr = new NativeItem($"%{((weedprice - defaultweedprice) / defaultweedprice) * 100}", "The price of weed");
            cocainepr = new NativeItem($"%{((cocaineprice - defaultcocaineprice) / defaultcocaineprice) * 100}", "The price of cocaine");
            heroinpr = new NativeItem($"%{((heroinprice - defaultheroinprice) / defaultheroinprice) * 100}", "The price of heroin");
            acidpr = new NativeItem($"%{((acidprice - defaultacidprice) / defaultacidprice) * 100}", "The price of acid");
            ecstacypr = new NativeItem($"%{((ecstacyprice - defaultecstacyprice) / defaultecstacyprice) * 100}", "The price of ecstacy");
            lsdpr = new NativeItem($"%{((lsdprice - defaultlsdprice) / defaultlsdprice) * 100}", "The price of lsd");
            prices.Add(weedpr);
            prices.Add(cocainepr);
            prices.Add(heroinpr);
            prices.Add(acidpr);
            prices.Add(ecstacypr);
            prices.Add(lsdpr);
            #endregion
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                menu.Visible = !menu.Visible;
            }
        }
    }
}
