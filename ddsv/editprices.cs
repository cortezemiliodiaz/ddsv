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

namespace gta_ddsv1
{
    internal class editprices
    {
        static float maxweed = 12f;
        static float maxcocaine = 20f;
        static float maxheroin = 22f;
        static float maxacid = 10f;
        static float maxecstacy = 14f;
        static float maxlsd = 11f;

        static float minweed = 5f;
        static float mincocaine = 13f;
        static float minheroin = 15f;
        static float minacid = 3f;
        static float minecstacy = 6f;
        static float minlsd = 4f;
        public static float getnewprice(float min, float max) { return Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, min, max); }
        public static void edit()
        {
            while (true)
            {
                //Function.Call(Hash.WAIT, 20000);
                Main.weedprice = getnewprice(maxweed, minweed);
                Main.cocaineprice = getnewprice(maxcocaine, mincocaine);
                Main.heroinprice = getnewprice(maxheroin, minheroin);
                Main.acidprice = getnewprice(maxacid, minacid);
                Main.ecstacyprice = getnewprice(minecstacy, maxecstacy);
                Main.lsdprice = getnewprice(minlsd, maxlsd);
                Thread.Sleep(300000);
            }
        }
    }
}
