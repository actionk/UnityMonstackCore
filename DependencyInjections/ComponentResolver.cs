using UnityEngine;

namespace Plugins.UnityMonstackCore.DependencyInjections
{
    public class ComponentResolver<T> where T : Component
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