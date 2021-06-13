using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.DependencyInjections
{
    public class ComponentLocator<T> where T : Component
    {
        private T m_instance;

        public ComponentLocator(bool locateImmediately = false)
        {
            if (locateImmediately)
                m_instance = ForceLocate();
        }

        public T Component
        {
            get
            {
                if (m_instance == null)
                    m_instance = ForceLocate();

                return m_instance;
            }
        }

        public void Locate()
        {
            if (m_instance == null)
                m_instance = ForceLocate();
        }

        private static T ForceLocate()
        {
            return Object.FindObjectOfType<T>();
        }
    }
}