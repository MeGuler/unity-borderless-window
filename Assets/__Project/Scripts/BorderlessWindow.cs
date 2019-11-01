using UnityEngine;
using Window.Structures;

namespace Window
{
    public class BorderlessWindow : Window
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Maximize();
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            GUI.color = Color.red;
            GUI.Label(new UnityEngine.Rect(50, 50, 150, 50), "Window Handle: " + HandledWindow.Handle);
        }
    }
}