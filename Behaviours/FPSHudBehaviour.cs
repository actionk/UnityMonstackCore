using System;
using UnityEngine;
using Text = TMPro.TMP_Text;

namespace Plugins.UnityMonstackCore.Behaviours
{
    public class FPSHudBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Text m_guiText = default;

        public float updateInterval = 0.5F;

        private float m_accum = 0;
        private int m_frames = 0;
        private float m_timeleft;

        void Start()
        {
            m_timeleft = updateInterval;
        }

        void Update()
        {
            m_timeleft -= Time.deltaTime;
            m_accum += Time.timeScale / Time.deltaTime;
            ++m_frames;

            // Interval ended - update GUI text and start new interval
            if (m_timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float fps = m_accum / m_frames;
                string format = String.Format("{0:F2} FPS", fps);
                m_guiText.text = format;

                if (fps < 30)
                {
                    m_guiText.color = Color.red;
                }
                else if (fps < 50)
                {
                    m_guiText.color = Color.yellow;
                }
                else
                {
                    m_guiText.color = Color.green;
                }

                m_timeleft = updateInterval;
                m_accum = 0.0F;
                m_frames = 0;
            }
        }
    }
}