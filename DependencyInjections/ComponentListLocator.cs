using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.DependencyInjections
{
    public class ComponentListLocator<T> where T : Component
    {
        private List<T> m_instance;

        public List<T> Components
        {
            get
            {
                if (m_instance == null)
                    m_instance = Object.FindObjectsOfType<T>().ToList();

                return m_instance;
            }
        }
    }
}