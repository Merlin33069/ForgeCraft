using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
   
   public partial class World
    {


       public bool Israining = false;
       
       //public Weather()
       //    :base(0, 127, 0, "main", new Random().Next())
       //{
       //}

        public void SendLightning(int x, int y, int z, int EntityId, Player p)
        {
            byte[] bytes = new byte[17];
            util.EndianBitConverter.Big.GetBytes(EntityId).CopyTo(bytes, 0);
            util.EndianBitConverter.Big.GetBytes(true).CopyTo(bytes, 4);
            util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 5);
            util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 9);
            util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 13);
            p.SendRaw(0x47, bytes);
        }
        public void rain(bool on, Player p)
        {
            
           if (on)
           {
               
                byte[] bytes = new byte[1];
                byte thisin = 1;
                bytes[0] = thisin;
                p.SendRaw(0x46, bytes);
                Israining = true;
               // p.SendMessage("Weather is: " + Israining.ToString());
                return;

            }
            if(!on)
            {
                byte[] bytes = new byte[1];
                bytes[0] = 2;
                p.SendRaw(0x46, bytes);
                Israining = false;
                return;
               // p.SendMessage("Weather is: " + Israining.ToString());
            }
            //
            //{

            //    Israining = false;
            //}
            //else
            //{
            //    Israining = true;
            //}



        }
        public void rainTimer()
        { }
        public bool isRain()
        {
            if (Israining)
            {
                return true;
            }
            else
                return false;
        }
        public void setthemotherfinrainon()
        {
            try
            {
                Israining.Equals(true);
                Israining = true;
            }
            catch (Exception e)
            { Player.GlobalMessage(e.Message); }
        }

   
    }
}
