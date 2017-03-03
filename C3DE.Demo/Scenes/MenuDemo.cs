﻿using C3DE.Demo.Scripts;
using C3DE.Prefabs;
using C3DE.Rendering;
using C3DE.UI;

namespace C3DE.Demo.Scenes
{
    public class MenuDemo : Scene
    {
        public MenuDemo() : base("Menu demo") { }

        public override void Initialize()
        {
            base.Initialize();

            GUI.Skin = DemoGame.CreateSkin(Application.Content, false);

            var camera = new CameraPrefab("cam");
            camera.AddComponent<MenuBehaviour>();
            camera.AddComponent<PointerCoordDebug>();
            Add(camera);

            Application.Engine.Renderer = new OculusRiftRenderer();
        }
    }
}
