﻿using C3DE.Components;
using C3DE.Components.Rendering;
using C3DE.UI;
using Microsoft.Xna.Framework;

namespace C3DE.Demo.Scripts
{
    public class ReflectionPlanarViewer : Behaviour
    {
        private Rectangle _rect;
        public PlanarReflection _planar;

        public override void Start()
        {
            _rect = new Rectangle(0, 0, 250, 150);
            _planar = GetComponent<PlanarReflection>();
        }

        public override void OnGUI(GUI gui)
        {
            if (_planar != null)
                gui.DrawTexture(_rect, _planar.ReflectionMap);
        }
    }
}
