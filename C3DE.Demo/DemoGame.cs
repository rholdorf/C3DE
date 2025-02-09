using C3DE.Demo.Scenes;
using C3DE.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Demo
{
    public static class DemoGame
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 480;
        public const int UIWidth = 1280;
        public const int UIHeight = 800;

        public static string[] BlueSkybox =
        {
            "Textures/Skybox/bluesky/px",
            "Textures/Skybox/bluesky/nx",
            "Textures/Skybox/bluesky/py",
            "Textures/Skybox/bluesky/ny",
            "Textures/Skybox/bluesky/pz",
            "Textures/Skybox/bluesky/nz"
        };

        public static string[] StarsSkybox =
        {
            "Textures/Skybox/starfield/px",
            "Textures/Skybox/starfield/nx",
            "Textures/Skybox/starfield/py",
            "Textures/Skybox/starfield/ny",
            "Textures/Skybox/starfield/pz",
            "Textures/Skybox/starfield/nz"
        };

        public static string[] NatureSkybox =
        {
            "Textures/Skybox/nature/sky_right",
            "Textures/Skybox/nature/sky_left",
            "Textures/Skybox/nature/sky_top",
            "Textures/Skybox/nature/sky_bottom",
            "Textures/Skybox/nature/sky_forward",
            "Textures/Skybox/nature/sky_backward"
        };

        public static string[] CloudSkybox =
        {
            "Textures/Skybox/clouds1/clouds1_east",
            "Textures/Skybox/clouds1/clouds1_west",
            "Textures/Skybox/clouds1/clouds1_up",
            "Textures/Skybox/clouds1/clouds1_down",
            "Textures/Skybox/clouds1/clouds1_north",
            "Textures/Skybox/clouds1/clouds1_south"
        };

        public static GUISkin CreateSkin(ContentManager content, bool customSkin = true)
        {
            GUISkin skin = new GUISkin("Font/Default");
            skin.LoadContent(Application.Content);

            if (customSkin)
            {
                skin.Box = content.Load<Texture2D>("Textures/UI/grey_panel");
                skin.Buttons[0] = content.Load<Texture2D>("Textures/UI/grey_button00");
                skin.Buttons[1] = content.Load<Texture2D>("Textures/UI/grey_button01");
                skin.Buttons[2] = content.Load<Texture2D>("Textures/UI/grey_button02");
                skin.Checkbox[0] = content.Load<Texture2D>("Textures/UI/grey_box");
                skin.Checkbox[1] = content.Load<Texture2D>("Textures/UI/grey_checkmarkWhite");
                skin.Checkbox[2] = content.Load<Texture2D>("Textures/UI/grey_checkmarkGrey");
                skin.Sliders[0] = content.Load<Texture2D>("Textures/UI/grey_button02");
                skin.Sliders[1] = content.Load<Texture2D>("Textures/UI/grey_button00");
                skin.TextColor = Color.Black;
            }

            return skin;
        }

        public static void InitializeGame()
        {
            Application.SceneManager.Add(new MenuDemo(), true);
            Application.SceneManager.Add(new PBRDemo());
            Application.SceneManager.Add(new HeightmapDemo());
            Application.SceneManager.Add(new ProceduralTerrainWater());
            Application.SceneManager.Add(new ProceduralTerrainLava());
            Application.SceneManager.Add(new LightingDemo());
            Application.SceneManager.Add(new PhysicsDemo());
            Application.SceneManager.Add(new SponzaDemo());
            Application.SceneManager.Add(new GUIDemo());
            Application.SceneManager.Add(new DeferredDemo());
            Application.SceneManager.Add(new LightPrePassDemo());
            Application.SceneManager.LoadLevel(0);
            Screen.SetVirtualResolution(UIWidth, UIHeight, true);
        }

#if !ANDROID && !NETFX_CORE

        // Entry point.
        static void Main(string[] args)
        {
            using (var game = new Engine("C3DE Game Engine", ScreenWidth, ScreenHeight))
            {
                InitializeGame();
                game.Run();
            }
        }

#endif
    }
}
