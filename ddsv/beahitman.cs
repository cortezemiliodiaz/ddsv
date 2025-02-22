using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;
using System.Drawing;
using System.Threading;
using iFruitAddon2;
using System.Drawing.Text;

namespace gta_ddsv1
{
    internal class beahitman : Script
    {


        public static bool play = false;
        public static Ped Target;

        public static Blip blip;
        public static Random rnd = new Random();

        public static bool hasmarkedonmap = false;
        public static int txtindex = 1;

        public static bool isanswered;

        public static Prop cellphone;

        public static void OnTick(object sender, EventArgs e)
        {
            if (play == true)
            {
                switch (txtindex)
                {
                    case 1:
                        GTA.UI.Screen.ShowHelpTextThisFrame("Click ~INPUT_CONTEXT~ to talk");
                        GTA.UI.Screen.ShowSubtitle("So you wanna ~r~kill~w~ someone huh?");
                        if (Game.IsControlJustPressed(GTA.Control.Context))
                        {
                            txtindex = 2;
                        }
                        break;
                    case 2:
                        GTA.UI.Screen.ShowHelpTextThisFrame("Click ~INPUT_CONTEXT~ to talk");
                        GTA.UI.Screen.ShowSubtitle("I marked your target on your map, after you ~r~kill~w~ them i will give you your ~g~money~w~.");
                        if (hasmarkedonmap == false)
                        {
                            Target = World.CreatePed(PedHash.Billionaire, Main.clientpositions[rnd.Next(Main.clientpositions.Length)]);
                            hasmarkedonmap = true;
                            blip = World.CreateBlip(Target.Position);
                            blip.Color = BlipColor.Red;
                            blip.Sprite = BlipSprite.BountyHit;
                            blip.ShowRoute = true;
                        }
                        if (Game.IsControlJustPressed(GTA.Control.Context))
                        {
                            Main.hitmanjob.Answered -= Main.starthitmanjob;
                            Main.hitmanjob.EndCall();
                            Main.iFruit.Close(100);
                            Model cellphonemodel = new Model("prop_npc_phone_02");
                            cellphonemodel.Request(10000);
                            while (!cellphonemodel.IsLoaded)
                            {
                                Script.Wait(10);
                            }
                            cellphone = World.CreateProp(cellphonemodel, Target.Position, false, false);
                            int boneindex = Function.Call<int>(Hash.GET_PED_BONE_INDEX, Target.Handle, 57005);
                            cellphone.AttachToBonePhysically(Target.Bones[boneindex], Vector3.Zero, Vector3.Zero, Vector3.Zero, 1f, false);
                            #region anim
                            Function.Call(Hash.REQUEST_ANIM_DICT, "cellphone@");
                            while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "cellphone@"))
                            {
                                Script.Wait(10);
                            }
                            Target.Task.PlayAnimation("cellphone@", "cellphone_call_listen_base", 8f, -1, AnimationFlags.Loop);
                            #endregion
                            txtindex = 3;
                        }
                        break;
                    case 3:
                        if (Target.IsDead == true)
                        {
                            Main.hitmanjob.Active = true;
                            Main.hitmanjob.Answered += answer;
                            Main.hitmanjob.Call();
                            //txtindex = -1;
                            txtindex = 4;
                        }
                        break;
                    case 4:
                        if (isanswered == true)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Click ~INPUT_CONTEXT~ to finish");
                            GTA.UI.Screen.ShowSubtitle("Hey, thank you for doing this. I will go ahead and transfer the ~g~money~w~ now.");
                            if (Game.IsControlJustPressed(GTA.Control.Context))
                            {

                                Game.Player.Money = Game.Player.Money + 1000;
                                hasmarkedonmap = false;
                                Main.hitmanjob.EndCall();
                                Main.iFruit.Close(100);
                                Main.hitmanjob.Active = true;
                                Main.hitmanjob.Answered -= answer;
                                Main.hitmanjob.Answered += Main.starthitmanjob;
                                blip.ShowRoute = false;
                                txtindex = 1;
                                blip.Delete();
                                Target.MarkAsNoLongerNeeded();
                                play = false;
                                isanswered = false;
                            }
                        }
                        break;
                }
            }
        }

        public static void answer(iFruitContact contact)
        {
            isanswered = true;
        }
    }
}
