using System.Media;

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
