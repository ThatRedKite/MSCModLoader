﻿#if !Mini
using Newtonsoft.Json;
#endif
using System;
using System.IO;
using UnityEngine.EventSystems;

namespace MSCLoader
{

#pragma warning disable CS1591
    //resize console UI by mouse
    public class ConsoleUIResizer : MonoBehaviour, IDragHandler
    {
        private RectTransform canvasRectTransform;
        private RectTransform panelRectTransform;
        private RectTransform m_consoleContainer;
        private bool clampedToRight;
        private bool clampedToTop;

        public GameObject consoleContainer;
        public Texture2D cursor;
        public bool Xresizer = false;
        class ConsoleSizeSave
        {
            public int v = 1;
            public float[] consoleSize = new float[2];
        }
        void Start()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasRectTransform = canvas.transform as RectTransform;
                panelRectTransform = transform as RectTransform;
            }
            m_consoleContainer = consoleContainer.GetComponent<RectTransform>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            ClampToBorder();
            if (Xresizer)
            {
                if (clampedToRight && eventData.delta.x > 0)
                    return;
                if (m_consoleContainer.sizeDelta.x < 300f)
                {
                    m_consoleContainer.sizeDelta = new Vector2(300f, m_consoleContainer.sizeDelta.y);
                    return;
                }
                m_consoleContainer.sizeDelta = new Vector2(m_consoleContainer.sizeDelta.x + eventData.delta.x, m_consoleContainer.sizeDelta.y);
            }
            else
            {
                if (clampedToTop && eventData.delta.y > 0)
                    return;
                if (m_consoleContainer.sizeDelta.y < 100f)
                {
                    m_consoleContainer.sizeDelta = new Vector2(m_consoleContainer.sizeDelta.x, 100f);
                    return;
                }
                m_consoleContainer.sizeDelta = new Vector2(m_consoleContainer.sizeDelta.x, m_consoleContainer.sizeDelta.y + eventData.delta.y);
            }
        }
        public void OnMouseEnter()
        {
            Cursor.SetCursor(cursor, new Vector2(16, 16), CursorMode.Auto);
        }

        public void OnMouseExit()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        public void OnDisable()
        {
            OnMouseExit();
        }
        public void LoadConsoleSize()
        {
#if !Mini
            if (!Xresizer) return;
            Start();
            string path = ModLoader.GetModSettingsFolder(new ModConsole());
            if (File.Exists(Path.Combine(path, "consoleSize.data")))
            {
                try
                {
                    ConsoleSizeSave css = JsonConvert.DeserializeObject<ConsoleSizeSave>(File.ReadAllText(Path.Combine(path, "consoleSize.data")));
                    if (css.v != 2)
                    {
                        throw new Exception("Console size reset, due to new changes.");
                    }
                    m_consoleContainer.sizeDelta = new Vector2(css.consoleSize[0], css.consoleSize[1]);
                }
                catch (Exception e)
                {
                    if (ModLoader.devMode)
                        ModConsole.Error(e.ToString());
                    System.Console.WriteLine(e);
                    File.Delete(Path.Combine(path, "consoleSize.data"));
                }
            }
#endif
        }
        public void SaveConsoleSize()
        {
            #if !Mini
            string path = ModLoader.GetModSettingsFolder(new ModConsole());
            if (Xresizer)
            {
                ConsoleSizeSave css = new ConsoleSizeSave()
                {
                    v = 2,
                    consoleSize = new float[] { m_consoleContainer.sizeDelta.x, m_consoleContainer.sizeDelta.y },
                };
                string serializedData = JsonConvert.SerializeObject(css, Formatting.Indented);
                File.WriteAllText(Path.Combine(path, "consoleSize.data"), serializedData);
            }
#endif
        }
        private void ClampToBorder()
        {
            Vector3[] canvasCorners = new Vector3[4];
            Vector3[] panelRectCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);
            panelRectTransform.GetWorldCorners(panelRectCorners);

            if (panelRectCorners[2].x > canvasCorners[2].x - 5)
            {
                if (!clampedToRight)
                {
                    clampedToRight = true;
                }
            }
            else if (clampedToRight)
            {
                clampedToRight = false;
            }

            if (panelRectCorners[2].y > canvasCorners[2].y - 5)
            {

                if (!clampedToTop)
                {
                    clampedToTop = true;
                }
            }
            else if (clampedToTop)
            {
                clampedToTop = false;
            }
        }
#pragma warning restore CS1591
    }
}
