﻿using C3DE.Components.Controllers;
using C3DE.Components.Lights;
using C3DE.Demo.Scripts;
using C3DE.Materials;
using C3DE.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Demo.Scenes
{
    public class HalloweenDemo : Scene
    {
        public HalloweenDemo() : base("Post Process Demo") { }

        public override void Initialize()
        {
            base.Initialize();

            // Skybox
            RenderSettings.Skybox.Generate(Application.GraphicsDevice, Application.Content, DemoGame.StarsSkybox);

            // Camera
            var camera = new CameraPrefab("camera");
            camera.Transform.Translate(0, 25, 0);
            camera.AddComponent<DemoBehaviour>();
            camera.AddComponent<PostProcessSwitcher>();
            Add(camera);

            var orbitController = camera.AddComponent<OrbitController>();

            // Light
            var lightPrefab = new LightPrefab("light", LightType.Directional);
            lightPrefab.Light.Direction = new Vector3(0, 1, -1);
            lightPrefab.Light.ShadowGenerator.SetShadowMapSize(Application.GraphicsDevice, 1024);
            lightPrefab.EnableShadows = true;
            lightPrefab.Light.Intensity = 0.5f;
            lightPrefab.Light.FallOf = 5.0f;
            Add(lightPrefab);

            // Terrain
            var terrainMaterial = new StandardMaterial(scene);
            terrainMaterial.MainTexture = Application.Content.Load<Texture2D>("Textures/Terrain/Rock");
            terrainMaterial.Shininess = 50;
            terrainMaterial.Tiling = new Vector2(8);

            var terrain = new TerrainPrefab("terrain");
            terrain.Renderer.Geometry.Size = new Vector3(2);
            terrain.Renderer.ReceiveShadow = true;
            terrain.Randomize(4, 15, 0.086, 0.25, true);
            terrain.Renderer.Material = terrainMaterial;
            terrain.Transform.Translate(-terrain.Width >> 1, 0, -terrain.Depth / 2);
            Add(terrain);

            // Lava
            var lavaMaterial = new LavaMaterial(this);
            lavaMaterial.MainTexture = Application.Content.Load<Texture2D>("Textures/lava_texture");
            lavaMaterial.NormalMap = Application.Content.Load<Texture2D>("Textures/lava_bump");

            var lava = new WaterPrefab("water");
            lava.Renderer.Material = lavaMaterial;
            lava.Renderer.ReceiveShadow = true;
            lava.Renderer.Geometry.Size = new Vector3(terrain.Width * 0.5f);
            lava.Renderer.Geometry.Generate();
            Add(lava);

            var jack = new ModelPrefab("Jack");
            jack.Transform.Rotate(-MathHelper.PiOver2, MathHelper.Pi, 0);
            jack.Transform.Translate(0, 35, 0);
            jack.Transform.LocalScale = new Vector3(4);
            jack.LoadModel("Models/Jack/JackOLantern");
            var jackMaterial = new StandardMaterial(this);
            jackMaterial.EmissiveColor = new Color(0.2f, 0.005f, 0);
            jackMaterial.MainTexture = Application.Content.Load<Texture2D>("Models/Jack/PumpkinColor");
            jack.Renderer.MainMaterial = jackMaterial;
            Add(jack);

            orbitController.LookAt(jack.Transform);
            orbitController.Distance = 150.0f;
        }
    }
}
