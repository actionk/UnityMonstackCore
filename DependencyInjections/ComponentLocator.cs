using UnityEngine;

namespace Plugins.UnityMonstackCore.DependencyInjections
{
    public class ComponentLocator<T> where T : Component
    {
        private T m_instance;

        public T Component
        {
            get
            {
                if (m_instance == null)
                    m_instance = Object.FindObjectOfType<T>();

                return m_instance;
            }
        }
    }
}