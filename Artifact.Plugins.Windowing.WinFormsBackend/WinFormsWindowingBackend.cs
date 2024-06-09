using Artifact.Plugins.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Artifact.Plugins.Windowing.WinFormsBackend
{
    public class WinFormsWindowingBackend : IWindowingBackend
    {
        public object WindowHandle => form.Handle;


        private Application _app;
        private Form form;

        public string Title { get; set; } = "";
        public string TitleSuffix { get; set; } = "";

        public void Close()
        {
            form.Dispose();
        }

        public void CreateWindow(string title, int width, int height, Application app)
        {
            _app = app;
            Title = title;

            form = new Form();
            form.Text = Title + " - Artifact Engine";

            form.MaximizeBox = false;
            form.MinimizeBox = false;

            form.Width = width;
            form.Height = height;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;

            form.Show();
        }

        public void PollEvents()
        {
            form.Text = Title + $" - Artifact Engine ({_app.FPS} FPS){TitleSuffix}";
            System.Windows.Forms.Application.DoEvents();
        }

        public bool ShouldClose()
        {
            return !form.Visible;
        }

        public bool IsKeyDown(Key key)
        {
            throw new NotImplementedException();
        }
    }
}
