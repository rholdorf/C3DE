﻿using C3DE.Graphics;
using C3DE.Graphics.PostProcessing;
using C3DE.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace C3DE.Components.Lighting
{
    public enum LightType
    {
        Ambient = 0, Directional, Point, Spot
    }

    [DataContract]
    public class Light : Component
    {
        internal protected Matrix _viewMatrix;
        internal protected Matrix _projectionMatrix;
        internal protected ShadowGenerator _shadowGenerator;
        internal protected Vector3 _color = Color.White.ToVector3();
        private Effect _deferredAmbientEffect;
        private Effect _deferredDirLightEffect;
        private Effect _deferredPointLightEffect;
        private Effect _lPPPointLightEffect;
        private Effect _lPPDirLightEffect;
        private QuadRenderer _quadRenderer;
        private SphereMesh _sphereMesh;
        private BoundingSphere _boundingSphere;

        public Matrix View => _viewMatrix;

        public Matrix Projection => _projectionMatrix;

        public Vector3 Direction
        {
            get
            {
                var position = _transform.Position;
                var rotation = _transform.Rotation;
                var matrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
                return position + Vector3.Transform(Vector3.Forward, matrix);
            }
        }

        public BoundingSphere BoundingSphere => _boundingSphere;

        [DataMember]
        public bool ShadowEnabled
        {
            get => _shadowGenerator.Enabled;
            set { _shadowGenerator.Enabled = value; }
        }

        [DataMember]
        public ShadowGenerator ShadowGenerator
        {
            get => _shadowGenerator;
            protected set { _shadowGenerator = value; }
        }

        /// <summary>
        /// The color of the light.
        /// </summary>
        [DataMember]
        public Color Color
        {
            get => new Color(_color);
            set { _color = value.ToVector3(); }
        }

        /// <summary>
        /// The intensity of the light.
        /// </summary>
        [DataMember]
        public float Intensity { get; set; } = 1.0f;

        /// <summary>
        /// The maximum distance of emission.
        /// </summary>
        [DataMember]
        public float Radius { get; set; } = 25;

        [DataMember]
        public float FallOf { get; set; } = 5.0f;

        /// <summary>
        /// The type of the light.
        /// </summary>
        [DataMember]
        public LightType TypeLight { get; set; } = LightType.Directional;

        /// <summary>
        /// The angle used by the Spot light.
        /// </summary>
        [DataMember]
        public float Angle { get; set; } = MathHelper.PiOver4;

        public Light()
            : base()
        {
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, 1, 1000);
            _viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Zero, Vector3.Up);
            _shadowGenerator = new ShadowGenerator();
        }

        public override void Start()
        {
            base.Start();

            _shadowGenerator.Initialize();

            _quadRenderer = new QuadRenderer(Application.GraphicsDevice);

            if (_transform != null)
                _boundingSphere = new BoundingSphere(_transform.Position, Radius);

            var content = Application.Content;
            _deferredAmbientEffect = content.Load<Effect>("Shaders/Deferred/AmbientLight");
            _deferredDirLightEffect = content.Load<Effect>("Shaders/Deferred/DirectionalLight");
            _deferredPointLightEffect = content.Load<Effect>("Shaders/Deferred/PointLight");
            _lPPDirLightEffect = content.Load<Effect>("Shaders/LPP/DirectionalLight");
            _lPPPointLightEffect = content.Load<Effect>("Shaders/LPP/PointLight");
            _sphereMesh = new SphereMesh(1, 8);
            _sphereMesh.Build();
        }

        public override void Update()
        {
            base.Update();

            if (!_gameObject.IsStatic)
            {
                _boundingSphere.Radius = Radius;
                _boundingSphere.Center = _transform.Position;
            }
        }

        // Need to be changed quickly !
        public void Update(ref BoundingSphere sphere)
        {
            Vector3 dir = sphere.Center - _gameObject.Transform.LocalPosition;
            dir.Normalize();

            _viewMatrix = Matrix.CreateLookAt(_transform.LocalPosition, sphere.Center, Vector3.Up);
            float size = sphere.Radius;

            float dist = Vector3.Distance(_transform.LocalPosition, sphere.Center);
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(-size, size, size, -size, dist - sphere.Radius, dist + sphere.Radius * 2);
        }

        public void RenderLPP(RenderTarget2D normal, RenderTarget2D depth, Camera camera)
        {
            var graphics = Application.GraphicsDevice;
            var previousRS = graphics.RasterizerState;
            var viewProjection = camera._viewMatrix * camera._projectionMatrix;
            var invViewProjection = Matrix.Invert(viewProjection);
            var viewport = new Vector2(Screen.Width, Screen.Height);

            if (TypeLight == LightType.Ambient)
            {
                _deferredAmbientEffect.Parameters["Color"].SetValue(_color);
                _deferredAmbientEffect.CurrentTechnique.Passes[0].Apply();
                _quadRenderer.RenderFullscreenQuad();
            }
            else if (TypeLight == LightType.Directional)
            {
                _lPPDirLightEffect.Parameters["NormalTexture"].SetValue(normal);
                _lPPDirLightEffect.Parameters["DepthTexture"].SetValue(depth);
                _lPPDirLightEffect.Parameters["InvViewProjection"].SetValue(invViewProjection);
                _lPPDirLightEffect.Parameters["WorldViewProjection"].SetValue(_transform._worldMatrix * viewProjection);
                _lPPDirLightEffect.Parameters["LightColor"].SetValue(_color);
                _lPPDirLightEffect.Parameters["LightPosition"].SetValue(Transform.Position);
                _lPPDirLightEffect.Parameters["LightIntensity"].SetValue(Intensity);
                _lPPDirLightEffect.CurrentTechnique.Passes[0].Apply();
                _quadRenderer.RenderFullscreenQuad();
            }
            else
            {
                _lPPPointLightEffect.Parameters["CameraPosition"].SetValue(camera._transform.Position);
                _lPPPointLightEffect.Parameters["NormalTexture"].SetValue(normal);
                _lPPPointLightEffect.Parameters["DepthTexture"].SetValue(depth);
                _lPPPointLightEffect.Parameters["InvViewProjection"].SetValue(invViewProjection);

                var worldViewProjection = (Matrix.CreateScale(Radius) * _transform._worldMatrix) * viewProjection;
                _lPPPointLightEffect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);
                _lPPPointLightEffect.Parameters["LightColor"].SetValue(_color);
                _lPPPointLightEffect.Parameters["LightAttenuation"].SetValue(FallOf);
                _lPPPointLightEffect.Parameters["LightPosition"].SetValue(Transform.Position);
                _lPPPointLightEffect.Parameters["LightRange"].SetValue(Radius);
                _lPPPointLightEffect.Parameters["LightIntensity"].SetValue(Intensity);

                var inside = Vector3.Distance(camera._transform.Position, _transform.Position) < (Radius * 1.25f);
                graphics.RasterizerState = inside ? RasterizerState.CullClockwise : RasterizerState.CullCounterClockwise;

                _lPPPointLightEffect.CurrentTechnique.Passes[0].Apply();

                graphics.SetVertexBuffer(_sphereMesh.VertexBuffer);
                graphics.Indices = _sphereMesh.IndexBuffer;
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _sphereMesh.IndexBuffer.IndexCount / 3);

                graphics.RasterizerState = previousRS;
            }
        }

        public void RenderDeferred(RenderTarget2D colorMap, RenderTarget2D normalMap, RenderTarget2D depthMap, Camera camera)
        {
            var graphics = Application.GraphicsDevice;
            var invertViewProjection = Matrix.Invert(camera._viewMatrix * camera._projectionMatrix);

            if (TypeLight == LightType.Ambient)
            {
                _deferredAmbientEffect.Parameters["Color"].SetValue(_color);
                _deferredAmbientEffect.CurrentTechnique.Passes[0].Apply();
                _quadRenderer.RenderFullscreenQuad();
            }
            else if (TypeLight == LightType.Directional)
            {
                _deferredDirLightEffect.Parameters["ColorMap"].SetValue(colorMap);
                _deferredDirLightEffect.Parameters["NormalMap"].SetValue(normalMap);
                _deferredDirLightEffect.Parameters["DepthMap"].SetValue(depthMap);
                _deferredDirLightEffect.Parameters["Color"].SetValue(_color);
                _deferredDirLightEffect.Parameters["Intensity"].SetValue(Intensity);
                _deferredDirLightEffect.Parameters["CameraPosition"].SetValue(camera._transform.Position);
                _deferredDirLightEffect.Parameters["InvertViewProjection"].SetValue(invertViewProjection);
                _deferredDirLightEffect.Parameters["LightPosition"].SetValue(_transform.LocalPosition);
                _deferredDirLightEffect.Parameters["World"].SetValue(_transform._worldMatrix);
                _deferredDirLightEffect.CurrentTechnique.Passes[0].Apply();
                _quadRenderer.RenderFullscreenQuad();
            }
            else
            {
                var previousRS = graphics.RasterizerState;
                var sphereWorldMatrix = Matrix.CreateScale(Radius) * Matrix.CreateTranslation(_transform.Position);

                _deferredPointLightEffect.Parameters["ColorMap"].SetValue(colorMap);
                _deferredPointLightEffect.Parameters["NormalMap"].SetValue(normalMap);
                _deferredPointLightEffect.Parameters["DepthMap"].SetValue(depthMap);
                _deferredPointLightEffect.Parameters["World"].SetValue(sphereWorldMatrix);
                _deferredPointLightEffect.Parameters["LightPosition"].SetValue(_transform.Position);
                _deferredPointLightEffect.Parameters["Color"].SetValue(_color);
                _deferredPointLightEffect.Parameters["Radius"].SetValue(Radius);
                _deferredPointLightEffect.Parameters["Intensity"].SetValue(Intensity);
                _deferredPointLightEffect.Parameters["View"].SetValue(camera._viewMatrix);
                _deferredPointLightEffect.Parameters["Projection"].SetValue(camera._projectionMatrix);
                _deferredPointLightEffect.Parameters["InvertViewProjection"].SetValue(invertViewProjection);
                _deferredPointLightEffect.Parameters["CameraPosition"].SetValue(camera._transform.Position);

                var inside = Vector3.Distance(camera._transform.Position, _transform.Position) < (Radius * 1.25f);
                graphics.RasterizerState = inside ? RasterizerState.CullClockwise : RasterizerState.CullCounterClockwise;

                _deferredPointLightEffect.CurrentTechnique.Passes[0].Apply();

                graphics.SetVertexBuffer(_sphereMesh.VertexBuffer);
                graphics.Indices = _sphereMesh.IndexBuffer;
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _sphereMesh.IndexBuffer.IndexCount / 3);

                graphics.RasterizerState = previousRS;
            }
        }

        public override void Dispose()
        {
            _shadowGenerator.Dispose();
        }

        public override int CompareTo(object obj)
        {
            var light = obj as Light;

            if (light == null)
                return -1;

            if (TypeLight == light.TypeLight)
                return 1;
            else
                return 0;
        }
    }
}
