﻿using C3DE.Graphics.Materials.Shaders;
using C3DE.Graphics.Rendering;
using C3DE.Graphics.Shaders.Forward;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Graphics.Materials
{
    public class StandardWaterMaterial : Material
    {
        public Texture2D NormalMap { get; set; }
        public Texture2D SpecularTexture { get; set; }
        public TextureCube ReflectionTexture { get; set; }

        public float SpecularPower { get; set; } = 250.0f;
        public float Speed = 0.5f;
        public float ReflectionIntensity { get; set; } = 0.35f;
        public float Alpha { get; set; } = 0.65f;

        public StandardWaterMaterial()
            : base()
        {
            _hasAlpha = true;
        }

        protected override void SetupShaderMaterial(BaseRenderer renderer)
        {
            if (renderer is ForwardRenderer)
                _shaderMaterial = new ForwardStandardWater(this);
            else if (renderer is DeferredRenderer)
                _shaderMaterial = new DeferredStandardWater(this);

            _shaderMaterial.LoadEffect(Application.Content);
        }
    }
}
