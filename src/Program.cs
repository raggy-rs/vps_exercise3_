using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;

using ImGuiNET;

namespace MandelbrotGenerator
{
    using System.Diagnostics;
    using System.Reflection;

    class Program
    {
        private unsafe static void UpdateTexture(GL gl, Image<Rgba32> image)
        {
            image.ProcessPixelRows(accessor =>
            {
                // Color is pixel-agnostic, but it's implicitly convertible to the Rgba32 pixel type
                for (int y = 0; y < accessor.Height; y++)
                {
                    fixed (void* data = accessor.GetRowSpan(y))
                    {
                        //Loading the actual image.
                        gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                    }
                }
            });
        }

        unsafe static void Main(string[] args)
        {
            // Create a Silk.NET window
            using var window = Window.Create(WindowOptions.Default);

            // these must be initialized after we have a window (in Load)
            IInputContext inputContext = null;
            GL gl = null;
            ImGuiController controller = null;

            uint texture = 0;
            var settings = new Settings();

            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;

            var signal = new ManualResetEventSlim(false);
            var area = new Area();
            Image<Rgba32> image = new Image<Rgba32>(area.Width, area.Height);

            var generator = new SyncImageGenerator();

            // coordinates of selection rectangle for zooming in
            System.Numerics.Vector2 startPos = new(0, 0);
            System.Numerics.Vector2 endPos = new(0, 0);

            // color of the selection rectangle
            System.Numerics.Vector4 color = new(255, 255, 255, 255);

            // Our loading function
            window.Load += () =>
            {
                gl = window.CreateOpenGL();
                inputContext = window.CreateInput();
                controller = new ImGuiController(gl, window, inputContext);
                // var io = new ImGuiNET.ImGuiIO();
                var io = ImGuiNET.ImGui.GetIO();
                io.ConfigWindowsMoveFromTitleBarOnly = true;

                texture = gl.GenTexture();
                gl.ActiveTexture(TextureUnit.Texture0);
                gl.BindTexture(TextureTarget.Texture2D, texture);

                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)area.Width,
                    (uint)area.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                gl.GenerateMipmap(TextureTarget.Texture2D);
                gl.Enable(EnableCap.Blend);
                gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            };

            // Handle resizes
            window.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                gl.Viewport(s);
            };

            // The render function
            window.Render += delta =>
            {
                // Make sure ImGui is up-to-date
                controller.Update((float)delta);

                // This is where you'll do any rendering beneath the ImGui context
                // Here, we just have a blank screen.
                gl.ClearColor(System.Drawing.Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));
                gl.Clear((uint)ClearBufferMask.ColorBufferBit);

                // This is where you'll do all of your ImGUi rendering
                // Here, we're just showing the ImGui built-in demo window.
                ImGui.ShowMetricsWindow();

                ImGui.Begin("MandelBrot");
                if (ImGui.Button("Run")) {
                    area = new Area(); // reset zoom
                    image = generator.GenerateImage(area);
                    UpdateTexture(gl, image);
                }

                // Handle zoom

                // get current mouse coordinates corresponding to the zoom rectangle
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                    startPos = ImGui.GetMousePos();
                }
                if (ImGui.IsMouseDown(ImGuiMouseButton.Left)) {
                    endPos = ImGui.GetMousePos();
                }

                // compute the Mandelbrot window position
                var windowPos = ImGui.GetWindowPos();
                var xmin = Math.Min(startPos.X, endPos.X);
                var xmax = Math.Max(startPos.X, endPos.X);
                var ymin = Math.Min(startPos.Y, endPos.Y);
                var ymax = Math.Max(startPos.Y, endPos.Y);

                // make sure that our selection is limited to the boundary of the Mandelbrot window
                var pos1 = ImGui.GetCursorScreenPos();
                var pos2 = new System.Numerics.Vector2(pos1.X + area.Width, pos1.Y + area.Height);
                if (pos1.X <= xmin && pos1.Y <= ymin && pos2.X >= xmax && pos2.Y >= ymax)
                {
                    var dl = ImGui.GetForegroundDrawList();
                    dl.AddRect(startPos, endPos, ImGui.GetColorU32(color));

                    if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        var minReal = area.MinReal + area.PixelWidth  * (Math.Min(startPos.X, endPos.X) - pos1.X);
                        var minImg  = area.MinImg  + area.PixelHeight * (Math.Min(startPos.Y, endPos.Y) - pos1.Y);
                        var maxReal = area.MinReal + area.PixelWidth  * (Math.Max(startPos.X, endPos.X) - pos1.X);
                        var maxImg  = area.MinImg  + area.PixelHeight * (Math.Max(startPos.Y, endPos.Y) - pos1.Y);

                        // generate an image of the given area
                        area = new Area(minReal, minImg, maxReal, maxImg, area.Width, area.Height);
                        image = generator.GenerateImage(area);
                        UpdateTexture(gl, image);

                        startPos = new(0,0);
                        endPos = new(0,0);
                    }
                }

                ImGui.Image(new IntPtr(texture), new System.Numerics.Vector2(area.Width, area.Height));
                ImGui.End();

                // Make sure ImGui renders too!
                controller.Render();
            };

            // The closing function
            window.Closing += () =>
            {
                // Dispose our controller first
                controller?.Dispose();

                // Dispose the input context
                inputContext?.Dispose();

                // Unload OpenGL
                gl?.Dispose();
            };

            // Now that everything's defined, let's run this bad boy!
            window.Run();

            window.Dispose();
        }
    }
}