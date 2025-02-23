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

namespace ddsv.chemist
{
    internal class chemistmain
    {
        static Vector3 chemistlocation = new Vector3(-9.5923f, -672.1529f, 40.7235f);
        static Ped chemist;
        static Blip chemistblip;
        static bool chemistspawned = false;
        static bool wasnearchemist = false;

        static float getcurrentamountforsupply()
        {
            return 1;
        }

        private static void onTick(object sender, EventArgs e)
        {
            if (Game.Player.Character.Position.DistanceTo(chemistlocation) < 50f)
            {
                if (wasnearchemist == false)
                {
                    wasnearchemist = true;
                    if (Game.Player.Character.Position.DistanceTo(chemistlocation) < 10f)
                    {
                        chemistblip.ShowRoute = false;
                        if (Game.Player.Character.Position.DistanceTo(chemistlocation) < 2f)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Click ~INPUT_CONTEXT~ to buy the current supply of your chemist: ${getcurrentamountforsupply()}");
                        }
                    }
                }
            }
            else
            {
                if (wasnearchemist == true)
                {
                    wasnearchemist = false;
                    chemist.MarkAsNoLongerNeeded();
                    chemistblip.Delete();
                    chemistspawned = false;
                }
            }
        }

        public static void scheduleappointment()
        {
            if (chemistspawned == false)
            {
                chemistspawned = true;
                chemist = World.CreatePed(PedHash.MethMale01, chemistlocation);
                chemist.Heading = 71.8210f;
                chemistblip = World.CreateBlip(chemistlocation);
                chemistblip.Sprite = BlipSprite.Acid;
                chemistblip.Color = BlipColor.Blue;
                chemistblip.Name = "Chemist";
                chemistblip.ShowRoute = true;
                Notification.PostTicker("The location of your chemist has been marked on your map.", true);
            } 
            else
            {
                Notification.PostTicker("You already scheduled a appointment with your chemist.", true);
                return;
            }
        }
    }
}
