using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model
{
    public static class SoundEffect
    {
        private static SoundPlayer ErrorSound = new(@"D:\Программирование\CSharp\WPF\Sounds\error-sound.wav");
        private static SoundPlayer ShootSound = new SoundPlayer(@"D:\Программирование\CSharp\WPF\Sounds\shoot1.wav");
        public static void PlayErrorSound()
        {
            ErrorSound.Play();
        }
        public static void PlayShootSound()
        {
            ShootSound.Play();
        }
    }
}
