﻿using C3DE.Demo.Scripts;

namespace C3DE.Demo.Scenes
{
    public class GUIDemo : SimpleDemo
    {
        public GUIDemo() : base("User Interface") { }

        public override void Initialize()
        {
            base.Initialize();
            m_Camera.AddComponent<UIWidgetDemo>();
        }
    }
}
