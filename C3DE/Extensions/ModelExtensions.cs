﻿using C3DE.Components.Rendering;
using C3DE.Graphics.Primitives;
using C3DE.Graphics.Materials;
using C3DE;
using C3DE.Graphics;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class ModelExtensions
    {
        public static GameObject ToMeshRenderers(this Model model, Scene scene = null)
        {
            if (scene == null)
                scene = Scene.current;

            var boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            var gameObject = new GameObject("Model");
            scene.Add(gameObject);

            var materials = new Dictionary<string, StandardMaterial>();

            foreach (ModelMesh mesh in model.Meshes)
            {
                var meshPartIndex = 0;

                var parent = new GameObject(mesh.Name);
                scene.Add(parent);
                parent.Transform.Parent = gameObject.Transform;

                var matrix = boneTransforms[mesh.ParentBone.Index];
                Vector3 position;
                Quaternion rotation;
                Vector3 scale;

                matrix.Decompose(out scale, out rotation, out position);

                parent.Transform.LocalPosition = position;
                parent.Transform.LocalRotation = rotation.ToEuler();
                parent.Transform.LocalScale = scale;

                foreach (var part in mesh.MeshParts)
                {
                    var effect = (BasicEffect)part.Effect;
                    var material = TryGetMaterial(effect, materials);

                    if (material == null)
                    {
                        material = new StandardMaterial();
                        material.MainTexture = effect.Texture;
                        material.DiffuseColor = new Color(effect.DiffuseColor.X, effect.DiffuseColor.Y, effect.DiffuseColor.Z);
                        material.SpecularTexture = TextureFactory.CreateColor(new Color(effect.SpecularColor.X, effect.SpecularColor.Y, effect.SpecularColor.Z), 1, 1);
                        material.SpecularPower = effect.SpecularPower;
                        material.EmissiveColor = new Color(effect.EmissiveColor.X, effect.EmissiveColor.Y, effect.EmissiveColor.Z);

                        if (!string.IsNullOrEmpty(effect?.Texture?.Name))
                            materials.Add(effect.Texture.Name, material);
                    }

                    var child = new GameObject($"{mesh.Name}_{meshPartIndex}");
                    scene.Add(child);
                    var renderer = child.AddComponent<MeshRenderer>();
                    renderer.material = material;
                    renderer.CastShadow = true;
                    renderer.ReceiveShadow = true;

                    var geometry = new Mesh();
                    geometry.VertexBuffer = part.VertexBuffer;
                    geometry.IndexBuffer = part.IndexBuffer;

                    renderer.Mesh = geometry;

                    child.Transform.Parent = parent.Transform;
                }
            }

            Debug.Log(materials.Count);

            return gameObject;
        }

        private static StandardMaterial TryGetMaterial(BasicEffect effect, Dictionary<string, StandardMaterial> materials)
        {
            var name = effect?.Texture?.Name;
            var hasValidName = !string.IsNullOrEmpty(name);

            if (hasValidName && materials.ContainsKey(name))
                return materials[name];

            return null;
        }
    }
}
